using Creative404.Models;
using System.Text.Json;

namespace Creative404.Services;

public class McpService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<McpService> _logger;

    public McpService(IHttpClientFactory httpClientFactory, ILogger<McpService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<McpServerInfo> QueryMcpServerAsync(string serverUrl)
    {
        var serverInfo = new McpServerInfo
        {
            ServerName = ExtractServerName(serverUrl)
        };

        try
        {
            if (string.IsNullOrWhiteSpace(serverUrl))
            {
                throw new ArgumentException("Server URL cannot be empty");
            }

            // For HTTP/HTTPS servers, attempt to connect
            if (serverUrl.StartsWith("http://") || serverUrl.StartsWith("https://"))
            {
                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(10);

                try
                {
                    // Try to fetch server capabilities (this is a simplified MCP interaction)
                    // In a real implementation, this would use the MCP protocol
                    var response = await client.GetAsync(serverUrl);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        serverInfo.Connected = true;
                        // Mock data for demonstration
                        serverInfo.Tools = new List<string> { "search", "analyze", "process" };
                        serverInfo.Resources = new List<string> { "resource://docs", "resource://data" };
                        serverInfo.ToolCount = serverInfo.Tools.Count;
                        serverInfo.ResourceCount = serverInfo.Resources.Count;
                    }
                    else
                    {
                        serverInfo.Connected = false;
                        serverInfo.ErrorMessage = $"Server returned status code: {response.StatusCode}";
                    }
                }
                catch (HttpRequestException ex)
                {
                    serverInfo.Connected = false;
                    serverInfo.ErrorMessage = $"Connection failed: {ex.Message}";
                    _logger.LogWarning(ex, "Failed to connect to MCP server: {Url}", serverUrl);
                }
                catch (TaskCanceledException)
                {
                    serverInfo.Connected = false;
                    serverInfo.ErrorMessage = "Connection timeout";
                    _logger.LogWarning("Connection timeout for MCP server: {Url}", serverUrl);
                }
            }
            else
            {
                // For command-based servers, return mock data
                serverInfo.Connected = true;
                serverInfo.Tools = new List<string> { "custom-tool" };
                serverInfo.Resources = new List<string> { "resource://local" };
                serverInfo.ToolCount = serverInfo.Tools.Count;
                serverInfo.ResourceCount = serverInfo.Resources.Count;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error querying MCP server: {Url}", serverUrl);
            serverInfo.Connected = false;
            serverInfo.ErrorMessage = ex.Message;
        }

        return serverInfo;
    }

    public bool ValidateMcpUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        // Check for HTTP(S) URLs
        if (url.StartsWith("http://") || url.StartsWith("https://"))
        {
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }

        // Allow command-based servers
        return url.Length > 0 && url.Length < 500;
    }

    public string ExtractServerName(string url)
    {
        try
        {
            if (url.StartsWith("http://") || url.StartsWith("https://"))
            {
                var uri = new Uri(url);
                return uri.Host;
            }

            // Extract from command
            var parts = url.Split(new[] { ' ', '/' }, StringSplitOptions.RemoveEmptyEntries);
            return parts.LastOrDefault() ?? "Unknown Server";
        }
        catch
        {
            return "MCP Server";
        }
    }
}
