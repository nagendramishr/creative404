namespace Creative404.Models;

public class ThemeInfo
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string[] BackgroundColors { get; set; } = Array.Empty<string>();
    public string TextColor { get; set; } = string.Empty;
    public string AccentColor { get; set; } = string.Empty;
}
