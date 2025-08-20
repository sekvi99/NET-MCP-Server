using System.ComponentModel;
using McpNetServer.Helpers;
using McpNetServer.Models;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Server;

namespace McpNetServer.Tools;

[McpServerToolType]
public static class McpDocumentDetailsComedyTool
{
    [McpServerTool(Name = "GetComedyDocumentDetails")]
    [Description(
        "Gets a list of documents, where the details have a comedy slant. Optionally takes in one or more keywords.")]
public static async Task<IEnumerable<DocumentDetails>> GetComedyDocumentDetailsAsync(
    IMcpServer mcpServer,
    List<string>? keywords = null,
    CancellationToken cancellationToken = default)
{
    if (mcpServer == null)
        throw new ArgumentNullException(nameof(mcpServer));

    try
    {
        const string documentsPath = @".\Documents";
        
        var documentDetails = DocumentsLoader
            .LoadDocumentsWithDetails(documentsPath)
            .FilterByKeyword(keywords)
            .Select(document => document.Item2)
            .ToList();

        if (!documentDetails.Any())
            return documentDetails;

        var chatClient = mcpServer.AsSamplingChatClient();
        
        // Process documents concurrently for better performance
        var tasks = documentDetails.Select(async documentDetail =>
        {
            try
            {
                var prompt = CreateComedyTitlePrompt(documentDetail.Title);
                var messages = new ChatMessage[] { new(ChatRole.User, prompt) };
                
                var response = await chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);
                documentDetail.Title = response?.ToString()?.Trim() ?? documentDetail.Title;
            }
            catch (Exception ex)
            {
                // Log individual document processing errors but continue with others
                Console.WriteLine($"Failed to process document '{documentDetail.Title}': {ex.Message}");
                // Keep original title on error
            }
        });

        await Task.WhenAll(tasks);
        return documentDetails;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error in GetComedyDocumentDetailsAsync: {ex.Message}");
        return Enumerable.Empty<DocumentDetails>();
    }
}

private static string CreateComedyTitlePrompt(string originalTitle)
{
    return $"Change this podcast title to be funny: \"{originalTitle}\". " +
           "Do not include any other text in the response - just the title itself.";
}
}