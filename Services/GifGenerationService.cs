using Creative404.Models;
using SkiaSharp;

namespace Creative404.Services;

public class GifGenerationService
{
    private readonly ILogger<GifGenerationService> _logger;

    // Theme definitions - single source of truth
    private static readonly List<ThemeInfo> Themes = new()
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
                
                // Generate a single frame since GIF animation encoding is not yet implemented
                var bitmap = GenerateFrame(
                    request.Width,
                    request.Height,
                    0,
                    1,
                    theme,
                    mcpInfo);

                // Encode as PNG
                var imageBytes = EncodeAsPng(bitmap);

                // Clean up
                bitmap.Dispose();

                return imageBytes;
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

        var progress = totalFrames > 1 ? (float)frameIndex / totalFrames : 0.5f;

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

    private byte[] EncodeAsPng(SKBitmap bitmap)
    {
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        using var stream = new MemoryStream();
        data.SaveTo(stream);
        return stream.ToArray();
    }

    private ThemeInfo GetTheme(string themeId)
    {
        return Themes.FirstOrDefault(t => t.Id == themeId) ?? Themes[0];
    }

    public List<ThemeInfo> GetAvailableThemes()
    {
        return new List<ThemeInfo>(Themes);
    }
}
