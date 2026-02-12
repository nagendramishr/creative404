using System.Net.Http.Headers;
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

            // Ensure the URL path ends with a trailing slash to avoid 307 redirects
            // that can cause headers (including Accept) to be dropped
            serverUrl = NormalizeUrl(serverUrl);

            var client = _httpClientFactory.CreateClient("McpClient");
            string? sessionId = null;

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

            var (initResponse, newSessionId) = await SendJsonRpcAsync<InitializeResult>(client, serverUrl, initRequest, sessionId);
            sessionId = newSessionId ?? sessionId;

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

            var (toolsResponse, _) = await SendJsonRpcAsync<ToolsListResult>(client, serverUrl, toolsRequest, sessionId);
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

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static async Task<(JsonRpcResponse<T>?, string?)> SendJsonRpcAsync<T>(
        HttpClient client, string url, JsonRpcRequest request, string? sessionId)
    {
        var json = JsonSerializer.Serialize(request, JsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
        httpRequest.Content = content;

        // MCP StreamableHTTP requires Accept header with both application/json and text/event-stream
        httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/event-stream"));

        if (sessionId != null)
        {
            httpRequest.Headers.Add("Mcp-Session-Id", sessionId);
        }

        var response = await client.SendAsync(httpRequest);

        // Handle 307/308 redirects manually to preserve method, body, and headers
        if (response.StatusCode is System.Net.HttpStatusCode.TemporaryRedirect
            or System.Net.HttpStatusCode.PermanentRedirect)
        {
            var redirectUrl = response.Headers.Location;
            if (redirectUrl != null)
            {
                var redirectUri = redirectUrl.IsAbsoluteUri
                    ? redirectUrl
                    : new Uri(new Uri(url), redirectUrl);

                var redirectContent = new StringContent(json, Encoding.UTF8, "application/json");
                using var redirectRequest = new HttpRequestMessage(HttpMethod.Post, redirectUri);
                redirectRequest.Content = redirectContent;
                redirectRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                redirectRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/event-stream"));

                if (sessionId != null)
                {
                    redirectRequest.Headers.Add("Mcp-Session-Id", sessionId);
                }

                response = await client.SendAsync(redirectRequest);
            }
        }

        response.EnsureSuccessStatusCode();

        // Extract session ID from response headers
        string? returnedSessionId = null;
        if (response.Headers.TryGetValues("Mcp-Session-Id", out var sessionValues))
        {
            returnedSessionId = sessionValues.FirstOrDefault();
        }

        var responseBody = await response.Content.ReadAsStringAsync();
        var contentType = response.Content.Headers.ContentType?.MediaType;

        // Handle SSE (text/event-stream) responses from StreamableHTTP servers
        if (string.Equals(contentType, "text/event-stream", StringComparison.OrdinalIgnoreCase))
        {
            var jsonPayload = ExtractJsonFromSse(responseBody);
            if (jsonPayload != null)
            {
                return (JsonSerializer.Deserialize<JsonRpcResponse<T>>(jsonPayload, JsonOptions), returnedSessionId);
            }
            return (null, returnedSessionId);
        }

        return (JsonSerializer.Deserialize<JsonRpcResponse<T>>(responseBody, JsonOptions), returnedSessionId);
    }

    /// <summary>
    /// Ensures the URL path ends with a trailing slash to prevent 307 redirects
    /// that can cause headers to be dropped by HTTP clients.
    /// </summary>
    internal static string NormalizeUrl(string url)
    {
        if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            if (!uri.AbsolutePath.EndsWith('/'))
            {
                var builder = new UriBuilder(uri);
                builder.Path += "/";
                return builder.Uri.ToString();
            }
        }
        return url;
    }

    /// <summary>
    /// Extracts the first JSON-RPC message from an SSE stream.
    /// SSE format: lines prefixed with "data: " followed by JSON content.
    /// </summary>
    internal static string? ExtractJsonFromSse(string sseBody)
    {
        foreach (var line in sseBody.Split(["\r\n", "\n"], StringSplitOptions.None))
        {
            var trimmed = line.Trim();
            if (trimmed.StartsWith("data:", StringComparison.Ordinal))
            {
                var data = trimmed["data:".Length..].Trim();
                if (data.Length > 0 && data[0] == '{')
                {
                    return data;
                }
            }
        }
        return null;
    }
}
