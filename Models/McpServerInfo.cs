namespace Creative404.Models;

public class McpServerInfo
{
    public string ServerName { get; set; } = string.Empty;
    public int ToolCount { get; set; }
    public int ResourceCount { get; set; }
    public List<string> Tools { get; set; } = new();
    public List<string> Resources { get; set; } = new();
    public bool Connected { get; set; }
    public string? ErrorMessage { get; set; }
}
