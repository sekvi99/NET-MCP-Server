using McpNetServer.Models;

namespace McpNetServer.Helpers;

public static class DocumentDetailsExtensions
{
    public static List<DocumentDetails> FilterByKeywords(
        this IEnumerable<DocumentDetails> documentDetails,
        List<string>? keywords
    ) =>
        documentDetails
            .Where(x => keywords?
                .All(filterTag => x.Keywords?
                    .Any(t => t.Equals(filterTag, StringComparison.InvariantCultureIgnoreCase)) == true) == true)
            .Select(x => new DocumentDetails
            {
                Title = x.Title,
                Keywords = x.Keywords ?? []
            }).ToList();
}