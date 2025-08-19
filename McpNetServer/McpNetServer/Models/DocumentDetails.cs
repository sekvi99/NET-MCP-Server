namespace McpNetServer.Models;

public sealed record DocumentDetails
{
    public string Title { get; set; } = null!;
    public DateTime Date { get; set; }
    public IReadOnlyList<string> Keywords { get; set; } = null!;
}