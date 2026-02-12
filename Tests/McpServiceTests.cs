using Creative404.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Creative404.Tests;

public class McpServiceTests
{
    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
    private readonly Mock<ILogger<McpService>> _mockLogger;
    private readonly McpService _mcpService;

    public McpServiceTests()
    {
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockLogger = new Mock<ILogger<McpService>>();
        _mcpService = new McpService(_mockHttpClientFactory.Object, _mockLogger.Object);
    }

    [Fact]
    public void ValidateMcpUrl_ValidHttpUrl_ReturnsTrue()
    {
        // Arrange
        var url = "http://localhost:3001";

        // Act
        var result = _mcpService.ValidateMcpUrl(url);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ValidateMcpUrl_ValidHttpsUrl_ReturnsTrue()
    {
        // Arrange
        var url = "https://example.com/mcp";

        // Act
        var result = _mcpService.ValidateMcpUrl(url);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ValidateMcpUrl_EmptyString_ReturnsFalse()
    {
        // Arrange
        var url = "";

        // Act
        var result = _mcpService.ValidateMcpUrl(url);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateMcpUrl_CommandString_ReturnsTrue()
    {
        // Arrange
        var url = "npx mcp-server";

        // Act
        var result = _mcpService.ValidateMcpUrl(url);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ExtractServerName_HttpUrl_ReturnsHostname()
    {
        // Arrange
        var url = "http://localhost:3001";

        // Act
        var result = _mcpService.ExtractServerName(url);

        // Assert
        Assert.Equal("localhost", result);
    }
}
