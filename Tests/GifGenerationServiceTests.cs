using Creative404.Models;
using Creative404.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Creative404.Tests;

public class GifGenerationServiceTests
{
    private readonly Mock<ILogger<GifGenerationService>> _mockLogger;
    private readonly GifGenerationService _gifService;

    public GifGenerationServiceTests()
    {
        _mockLogger = new Mock<ILogger<GifGenerationService>>();
        _gifService = new GifGenerationService(_mockLogger.Object);
    }

    [Fact]
    public void GetAvailableThemes_ReturnsExpectedCount()
    {
        // Act
        var themes = _gifService.GetAvailableThemes();

        // Assert
        Assert.Equal(4, themes.Count);
    }

    [Fact]
    public void GetAvailableThemes_ContainsDefaultTheme()
    {
        // Act
        var themes = _gifService.GetAvailableThemes();

        // Assert
        Assert.Contains(themes, t => t.Id == "default");
    }

    [Fact]
    public async Task GenerateGifAsync_ValidRequest_ReturnsBytes()
    {
        // Arrange
        var request = new GifGenerationRequest
        {
            McpServerUrl = "http://localhost:3001",
            Theme = "default",
            Width = 600,
            Height = 400
        };

        var mcpInfo = new McpServerInfo
        {
            ServerName = "Test Server",
            Connected = true,
            ToolCount = 2,
            ResourceCount = 1
        };

        // Act
        var result = await _gifService.GenerateGifAsync(request, mcpInfo);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }
}
