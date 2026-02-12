using System.Text;
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

    public async Task<McpInspectionResult> InspectServerAsync(string serverUrl)
    {
        var result = new McpInspectionResult();

        try
        {
            if (!Uri.TryCreate(serverUrl, UriKind.Absolute, out var uri)
                || (uri.Scheme != "http" && uri.Scheme != "https"))
            {
                result.ErrorMessage = "Invalid URL. Please enter a valid HTTP or HTTPS URL.";
                return result;
            }

            var client = _httpClientFactory.CreateClient("McpClient");
            client.Timeout = TimeSpan.FromSeconds(10);

            // Step 1: Initialize the MCP connection
            var initRequest = new JsonRpcRequest
            {
                Id = 1,
                Method = "initialize",
                Params = new
                {
                    protocolVersion = "2024-11-05",
                    capabilities = new { },
                    clientInfo = new { name = "Creative404", version = "1.0.0" }
                }
            };

            var initResponse = await SendJsonRpcAsync<InitializeResult>(client, serverUrl, initRequest);
            if (initResponse?.Result == null)
            {
                result.ErrorMessage = initResponse?.Error?.Message ?? "Failed to initialize MCP connection. The server may not support the MCP protocol.";
                return result;
            }

            result.ServerName = initResponse.Result.ServerInfo?.Name ?? "Unknown";
            result.ServerVersion = initResponse.Result.ServerInfo?.Version ?? "Unknown";

            // Step 2: List available tools
            var toolsRequest = new JsonRpcRequest
            {
                Id = 2,
                Method = "tools/list",
                Params = new { }
            };

            var toolsResponse = await SendJsonRpcAsync<ToolsListResult>(client, serverUrl, toolsRequest);
            if (toolsResponse?.Result?.Tools != null)
            {
                result.Tools = toolsResponse.Result.Tools;
            }

            result.Success = true;
        }
        catch (TaskCanceledException)
        {
            result.ErrorMessage = "Connection timed out. The server did not respond in time.";
        }
        catch (HttpRequestException ex)
        {
            result.ErrorMessage = $"Could not connect to the server: {ex.Message}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inspecting MCP server at {Url}", serverUrl);
            result.ErrorMessage = $"An error occurred: {ex.Message}";
        }

        return result;
    }

    private static async Task<JsonRpcResponse<T>?> SendJsonRpcAsync<T>(HttpClient client, string url, JsonRpcRequest request)
    {
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(url, content);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<JsonRpcResponse<T>>(responseJson);
    }
}
