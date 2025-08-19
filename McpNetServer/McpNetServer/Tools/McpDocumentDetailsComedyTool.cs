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
        IMcpServer myServer,
        List<string>? keywords,
        CancellationToken cancellationToken)
    {
        var documentDetails = DocumentsLoader.LoadDocumentsWithDetails(@".\Documents")
            .Select(d => d.Details);
        
        var filteredDetails = documentDetails.FilterByKeywords(keywords);

        foreach (var detail in filteredDetails)
        {
            var llmResponse = $"{await myServer.AsSamplingChatClient().GetResponseAsync((ChatMessage[])
            [
                new(ChatRole.User,
                    $"Change this document title to be funny: \"{detail.Title}\"." +
                    $"Do not include any other text in the response - just the title itself."),
            ], cancellationToken: cancellationToken)}";

            detail.Title = llmResponse;
        }
        
        return filteredDetails;
    }
}