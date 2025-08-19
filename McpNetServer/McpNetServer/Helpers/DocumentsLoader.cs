using System.Text.RegularExpressions;
using McpNetServer.Models;

namespace McpNetServer.Helpers;

public static class DocumentsLoader
{
    public static IEnumerable<string> LoadDocuments(string path)
    {
        if (!Directory.Exists(path))
        {
            throw new DirectoryNotFoundException($"Directory not found: {path}");
        }

        var markdownFiles = Directory.GetFiles(path, "*.md", SearchOption.AllDirectories);
        
        foreach (var filePath in markdownFiles)
        {
            yield return File.ReadAllText(filePath);
        }
    }

    public static IEnumerable<(string Content, DocumentDetails Details)> LoadDocumentsWithDetails(string path)
    {
        if (!Directory.Exists(path))
        {
            throw new DirectoryNotFoundException($"Directory not found: {path}");
        }

        var markdownFiles = Directory.GetFiles(path, "*.md", SearchOption.AllDirectories);
        
        foreach (var filePath in markdownFiles)
        {
            var content = File.ReadAllText(filePath);
            var details = ParseDocumentDetails(content, Path.GetFileNameWithoutExtension(filePath));
            yield return (content, details);
        }
    }

    public static DocumentDetails ParseDocumentDetails(string content, string fallbackTitle = "")
    {
        var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        
        string title = fallbackTitle;
        DateTime date = DateTime.Now;
        List<string> keywords = new();

        // Parse the metadata section
        bool inMetadata = false;
        string createdLine = "";
        string keywordsLine = "";

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();

            // Look for **Created:** pattern
            if (line.StartsWith("**Created:**"))
            {
                createdLine = line;
                inMetadata = true;
                continue;
            }

            // Look for **Keywords:** pattern
            if (line.StartsWith("**Keywords:**"))
            {
                keywordsLine = line;
                continue;
            }

            // Look for the main title (first # header after metadata)
            if (line.StartsWith("# ") && inMetadata && string.IsNullOrEmpty(title.Trim()))
            {
                title = line.Substring(2).Trim();
                break;
            }

            // Stop parsing metadata when we hit the --- separator
            if (line == "---" && inMetadata)
            {
                break;
            }
        }

        // Parse creation date
        if (!string.IsNullOrEmpty(createdLine))
        {
            date = ParseCreatedDate(createdLine);
        }

        // Parse keywords
        if (!string.IsNullOrEmpty(keywordsLine))
        {
            keywords = ParseKeywords(keywordsLine);
        }

        // If no title found in content, try to extract from first header
        if (string.IsNullOrEmpty(title.Trim()) || title == fallbackTitle)
        {
            title = ExtractTitleFromContent(lines) ?? fallbackTitle;
        }

        return new DocumentDetails
        {
            Title = title,
            Date = date,
            Keywords = keywords.AsReadOnly()
        };
    }

    private static DateTime ParseCreatedDate(string createdLine)
    {
        // Extract date from "**Created:** March 15, 2024" format
        var dateMatch = Regex.Match(createdLine, @"\*\*Created:\*\*\s*(.+)");
        if (dateMatch.Success)
        {
            var dateString = dateMatch.Groups[1].Value.Trim();
            if (DateTime.TryParse(dateString, out DateTime parsedDate))
            {
                return parsedDate;
            }
        }
        return DateTime.Now;
    }

    private static List<string> ParseKeywords(string keywordsLine)
    {
        // Extract keywords from "**Keywords:** keyword1, keyword2, keyword3" format
        var keywordsMatch = Regex.Match(keywordsLine, @"\*\*Keywords:\*\*\s*(.+)");
        if (keywordsMatch.Success)
        {
            var keywordsString = keywordsMatch.Groups[1].Value.Trim();
            return keywordsString
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(k => k.Trim())
                .Where(k => !string.IsNullOrEmpty(k))
                .ToList();
        }
        return new List<string>();
    }

    private static string? ExtractTitleFromContent(string[] lines)
    {
        // Look for the first main header (# Title)
        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            if (trimmedLine.StartsWith("# ") && trimmedLine.Length > 2)
            {
                return trimmedLine.Substring(2).Trim();
            }
        }
        return null;
    }

    // Method to load only DocumentDetails without content
    public static IEnumerable<DocumentDetails> LoadDocumentDetails(string path)
    {
        if (!Directory.Exists(path))
        {
            throw new DirectoryNotFoundException($"Directory not found: {path}");
        }

        var markdownFiles = Directory.GetFiles(path, "*.md", SearchOption.AllDirectories);
        
        foreach (var filePath in markdownFiles)
        {
            var content = File.ReadAllText(filePath);
            var details = ParseDocumentDetails(content, Path.GetFileNameWithoutExtension(filePath));
            yield return details;
        }
    }

    // Alternative method that returns structured data instead of just content
    public static IEnumerable<DocumentInfo> LoadDocumentsAsInfo(string path)
    {
        foreach (var (content, details) in LoadDocumentsWithDetails(path))
        {
            yield return new DocumentInfo
            {
                Content = content,
                Details = details
            };
        }
    }
}
