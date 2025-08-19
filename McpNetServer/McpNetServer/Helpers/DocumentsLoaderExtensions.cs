using McpNetServer.Models;

namespace McpNetServer.Helpers;

public static class DocumentsLoaderExtensions
{
    public static IEnumerable<DocumentDetails> LoadDocumentDetails(this string path)
    {
        return DocumentsLoader.LoadDocumentsWithDetails(path).Select(x => x.Details);
    }

    public static IEnumerable<string> FilterByKeyword(this IEnumerable<(string Content, DocumentDetails Details)> documents, string keyword)
    {
        return documents
            .Where(doc => doc.Details.Keywords.Any(k => k.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
            .Select(doc => doc.Content);
    }

    public static IEnumerable<(string Content, DocumentDetails Details)> FilterByDateRange(
        this IEnumerable<(string Content, DocumentDetails Details)> documents, 
        DateTime startDate, 
        DateTime endDate)
    {
        return documents.Where(doc => doc.Details.Date >= startDate && doc.Details.Date <= endDate);
    }
}