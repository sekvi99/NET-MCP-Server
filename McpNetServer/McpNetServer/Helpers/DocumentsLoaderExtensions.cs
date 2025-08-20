using McpNetServer.Models;

namespace McpNetServer.Helpers;

public static class DocumentsLoaderExtensions
{
    public static IEnumerable<DocumentDetails> LoadDocumentDetails(this string path)
    {
        return DocumentsLoader.LoadDocumentsWithDetails(path).Select(x => x.Details);
    }

    public static IEnumerable<(string, DocumentDetails)> FilterByKeyword(this IEnumerable<(string Content, DocumentDetails Details)> documents, List<string> keywords)
    {
        if (keywords.Count == 0) 
            return documents;
        
        return documents
            .Where(doc => doc.Details.Keywords.Any(k => 
                keywords.Any(keyword => k.Contains(keyword, StringComparison.OrdinalIgnoreCase))));
    }

    public static IEnumerable<(string Content, DocumentDetails Details)> FilterByDateRange(
        this IEnumerable<(string Content, DocumentDetails Details)> documents, 
        DateTime startDate, 
        DateTime endDate)
    {
        return documents.Where(doc => doc.Details.Date >= startDate && doc.Details.Date <= endDate);
    }
}