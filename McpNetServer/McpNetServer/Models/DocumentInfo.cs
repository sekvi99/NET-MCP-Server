namespace McpNetServer.Models;

public sealed record DocumentInfo
{
    public string Content { get; init; } = string.Empty;
    public DocumentDetails Details { get; init; } = null!;
}