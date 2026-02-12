using Creative404.Models;
using SkiaSharp;

namespace Creative404.Services;

public class GifGenerationService
{
    private readonly ILogger<GifGenerationService> _logger;

    public GifGenerationService(ILogger<GifGenerationService> logger)
    {
        _logger = logger;
    }

    public async Task<byte[]> GenerateGifAsync(GifGenerationRequest request, McpServerInfo mcpInfo)
    {
        return await Task.Run(() =>
        {
            try
            {
                var theme = GetTheme(request.Theme);
                var frames = new List<SKBitmap>();
                var numFrames = 30;

                // Generate frames
                for (int frame = 0; frame < numFrames; frame++)
                {
                    var bitmap = GenerateFrame(
                        request.Width,
                        request.Height,
                        frame,
                        numFrames,
                        theme,
                        mcpInfo);
                    frames.Add(bitmap);
                }

                // Encode as GIF
                var gifBytes = EncodeAsGif(frames, request.Width, request.Height);

                // Clean up
                foreach (var frame in frames)
                {
                    frame.Dispose();
                }

                return gifBytes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating GIF");
                throw;
            }
        });
    }

    private SKBitmap GenerateFrame(int width, int height, int frameIndex, int totalFrames, ThemeInfo theme, McpServerInfo mcpInfo)
    {
        var bitmap = new SKBitmap(width, height);
        using var canvas = new SKCanvas(bitmap);

        var progress = (float)frameIndex / totalFrames;

        // Draw background gradient
        var gradient = SKShader.CreateLinearGradient(
            new SKPoint(0, 0),
            new SKPoint(width, height),
            new[] { SKColor.Parse(theme.BackgroundColors[0]), SKColor.Parse(theme.BackgroundColors[1]) },
            SKShaderTileMode.Clamp);

        using var bgPaint = new SKPaint { Shader = gradient };
        canvas.DrawRect(0, 0, width, height, bgPaint);

        // Draw animated accent circles
        var accentColor = SKColor.Parse(theme.AccentColor);
        accentColor = accentColor.WithAlpha(51); // 20% opacity

        using var circlePaint = new SKPaint
        {
            Color = accentColor,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        for (int i = 0; i < 3; i++)
        {
            var angle = (progress + (float)i / 3) * MathF.PI * 2;
            var x = width / 2 + MathF.Cos(angle) * 100;
            var y = height / 2 + MathF.Sin(angle) * 80;
            var radius = 20 + MathF.Sin(angle * 2) * 10;
            canvas.DrawCircle(x, y, radius, circlePaint);
        }

        // Draw main text with bounce animation
        var bounce = MathF.Sin(progress * MathF.PI * 2) * 10;
        using var font = new SKFont
        {
            Size = 48,
            Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold)
        };
        using var textPaint = new SKPaint
        {
            Color = SKColor.Parse(theme.TextColor),
            IsAntialias = true
        };

        canvas.DrawText("404", width / 2, height / 2 - 50 + bounce, SKTextAlign.Center, font, textPaint);

        // Draw subtitle
        font.Size = 24;
        font.Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Normal);
        canvas.DrawText("Page Not Found", width / 2, height / 2 + 10, SKTextAlign.Center, font, textPaint);

        // Draw MCP content hint
        if (mcpInfo.Connected && !string.IsNullOrEmpty(mcpInfo.ServerName))
        {
            font.Size = 16;
            textPaint.Color = SKColor.Parse(theme.AccentColor);
            var hint = $"Powered by {mcpInfo.ServerName}";
            if (hint.Length > 50)
                hint = hint.Substring(0, 47) + "...";
            canvas.DrawText(hint, width / 2, height / 2 + 60, SKTextAlign.Center, font, textPaint);
        }

        return bitmap;
    }

    private byte[] EncodeAsGif(List<SKBitmap> frames, int width, int height)
    {
        // For simplicity, we'll encode as individual PNG frames and create a simple animated structure
        // In production, you'd use a proper GIF encoder library
        using var stream = new MemoryStream();
        
        // For now, just encode the first frame as PNG to demonstrate
        // A proper GIF encoder would be needed for actual animation
        using var image = SKImage.FromBitmap(frames[0]);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        data.SaveTo(stream);
        
        return stream.ToArray();
    }

    private ThemeInfo GetTheme(string themeId)
    {
        var themes = new Dictionary<string, ThemeInfo>
        {
            ["default"] = new ThemeInfo
            {
                Id = "default",
                Name = "Default",
                Description = "Dark blue theme",
                BackgroundColors = new[] { "#1a1a2e", "#16213e" },
                TextColor = "#eeeeee",
                AccentColor = "#0f3460"
            },
            ["ocean"] = new ThemeInfo
            {
                Id = "ocean",
                Name = "Ocean",
                Description = "Cool blue waves",
                BackgroundColors = new[] { "#0077be", "#005f8c" },
                TextColor = "#ffffff",
                AccentColor = "#00d4ff"
            },
            ["sunset"] = new ThemeInfo
            {
                Id = "sunset",
                Name = "Sunset",
                Description = "Warm orange glow",
                BackgroundColors = new[] { "#ff6b6b", "#ee5a6f" },
                TextColor = "#ffffff",
                AccentColor = "#ffd93d"
            },
            ["forest"] = new ThemeInfo
            {
                Id = "forest",
                Name = "Forest",
                Description = "Natural green",
                BackgroundColors = new[] { "#2d4a3e", "#1f3329" },
                TextColor = "#e8f5e9",
                AccentColor = "#66bb6a"
            }
        };

        return themes.ContainsKey(themeId) ? themes[themeId] : themes["default"];
    }

    public List<ThemeInfo> GetAvailableThemes()
    {
        return new List<ThemeInfo>
        {
            new ThemeInfo
            {
                Id = "default",
                Name = "Default",
                Description = "Dark blue theme",
                BackgroundColors = new[] { "#1a1a2e", "#16213e" },
                TextColor = "#eeeeee",
                AccentColor = "#0f3460"
            },
            new ThemeInfo
            {
                Id = "ocean",
                Name = "Ocean",
                Description = "Cool blue waves",
                BackgroundColors = new[] { "#0077be", "#005f8c" },
                TextColor = "#ffffff",
                AccentColor = "#00d4ff"
            },
            new ThemeInfo
            {
                Id = "sunset",
                Name = "Sunset",
                Description = "Warm orange glow",
                BackgroundColors = new[] { "#ff6b6b", "#ee5a6f" },
                TextColor = "#ffffff",
                AccentColor = "#ffd93d"
            },
            new ThemeInfo
            {
                Id = "forest",
                Name = "Forest",
                Description = "Natural green",
                BackgroundColors = new[] { "#2d4a3e", "#1f3329" },
                TextColor = "#e8f5e9",
                AccentColor = "#66bb6a"
            }
        };
    }
}
