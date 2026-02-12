namespace Creative404.Models;

public class GifGenerationRequest
{
    public string McpServerUrl { get; set; } = string.Empty;
    public string Theme { get; set; } = "default";
    public int Width { get; set; } = 600;
    public int Height { get; set; } = 400;
}
