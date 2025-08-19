using System.ComponentModel;
using McpNetServer.Helpers;
using McpNetServer.Models;
using ModelContextProtocol.Server;

namespace McpNetServer.Tools;

[McpServerToolType]
public static class McpDetailsTool
{
    [McpServerTool(Name = "GetDocumentDetails")]
    [Description("Gets a document details for the MCP server. Optionally takes in one or more keywords to search.")]
    public static IEnumerable<DocumentDetails> GetDocumentDetails(
        List<string> keywords)
    {
        // Path might need to be adjusted in other machines
        var documentDetails = DocumentsLoader.LoadDocumentsWithDetails(@".\Documents");
        return documentDetails.Select(d => d.Details);
    }
}