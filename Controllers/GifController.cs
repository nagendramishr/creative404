using Creative404.Models;
using Creative404.Services;
using Microsoft.AspNetCore.Mvc;

namespace Creative404.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GifController : ControllerBase
{
    private readonly GifGenerationService _gifService;
    private readonly McpService _mcpService;
    private readonly ILogger<GifController> _logger;

    public GifController(
        GifGenerationService gifService,
        McpService mcpService,
        ILogger<GifController> logger)
    {
        _gifService = gifService;
        _mcpService = mcpService;
        _logger = logger;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateGif([FromBody] GifGenerationRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.McpServerUrl))
            {
                return BadRequest(new { error = "MCP server URL is required" });
            }

            if (!_mcpService.ValidateMcpUrl(request.McpServerUrl))
            {
                return BadRequest(new { error = "Invalid MCP server URL" });
            }

            // Query MCP server
            var mcpInfo = await _mcpService.QueryMcpServerAsync(request.McpServerUrl);

            if (!mcpInfo.Connected)
            {
                return StatusCode(500, new { 
                    error = "Failed to connect to MCP server",
                    details = mcpInfo.ErrorMessage
                });
            }

            // Generate GIF
            var gifBytes = await _gifService.GenerateGifAsync(request, mcpInfo);

            return File(gifBytes, "image/png", "404-custom.png");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating GIF");
            return StatusCode(500, new { error = "Failed to generate GIF", details = ex.Message });
        }
    }

    [HttpPost("preview")]
    public async Task<IActionResult> PreviewMcpServer([FromBody] PreviewRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.McpServerUrl))
            {
                return BadRequest(new { error = "MCP server URL is required" });
            }

            if (!_mcpService.ValidateMcpUrl(request.McpServerUrl))
            {
                return BadRequest(new { error = "Invalid MCP server URL" });
            }

            var mcpInfo = await _mcpService.QueryMcpServerAsync(request.McpServerUrl);

            return Ok(new { success = true, data = mcpInfo });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error previewing MCP server");
            return StatusCode(500, new { error = "Failed to query MCP server", details = ex.Message });
        }
    }

    [HttpGet("themes")]
    public IActionResult GetThemes()
    {
        var themes = _gifService.GetAvailableThemes();
        return Ok(new { themes });
    }
}

public class PreviewRequest
{
    public string McpServerUrl { get; set; } = string.Empty;
}
