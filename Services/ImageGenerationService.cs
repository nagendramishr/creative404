using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Creative404.Services;

public class ImageGenerationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ImageGenerationService> _logger;

    public ImageGenerationService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<ImageGenerationService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ImageGenerationResult> GenerateImageAsync(McpInspectionResult mcpResult)
    {
        var result = new ImageGenerationResult();

        var apiKey = _configuration["ImageGeneration:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            result.ErrorMessage = "Image generation API key is not configured. Set ImageGeneration:ApiKey in appsettings.json or environment variables.";
            return result;
        }

        try
        {
            var prompt = BuildPrompt(mcpResult);
            _logger.LogInformation("Generating 404 image with prompt: {Prompt}", prompt);

            var client = _httpClientFactory.CreateClient("ImageGenClient");
            var endpoint = _configuration["ImageGeneration:Endpoint"]
                           ?? "https://api.openai.com/v1/images/generations";
            var model = _configuration["ImageGeneration:Model"] ?? "dall-e-3";
            var size = _configuration["ImageGeneration:Size"] ?? "1024x1024";

            var requestBody = new
            {
                model,
                prompt,
                n = 1,
                size,
                response_format = "url"
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Content = content;
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var imageResponse = JsonSerializer.Deserialize<OpenAiImageResponse>(responseBody, JsonOptions);

            if (imageResponse?.Data is { Count: > 0 })
            {
                result.ImageUrl = imageResponse.Data[0].Url;
                result.RevisedPrompt = imageResponse.Data[0].RevisedPrompt;
                result.Success = true;
            }
            else
            {
                result.ErrorMessage = "No image was returned by the API.";
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error generating image");
            result.ErrorMessage = $"Image generation failed: {ex.Message}";
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning(ex, "Image generation request timed out");
            result.ErrorMessage = "Image generation timed out.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error generating image");
            result.ErrorMessage = $"Image generation error: {ex.Message}";
        }

        return result;
    }

    internal static string BuildPrompt(McpInspectionResult mcpResult)
    {
        var toolSummary = string.Join(", ",
            mcpResult.Tools.Select(t =>
                string.IsNullOrEmpty(t.Description) ? t.Name : $"{t.Name} ({t.Description})"));

        return $"Create a whimsical, colorful, and creative illustration for a 404 'Page Not Found' error page. "
               + $"The theme should be inspired by an MCP server called '{mcpResult.ServerName}' "
               + $"which provides these tools: {toolSummary}. "
               + "The image should humorously depict the concept of a missing page "
               + "in a way that relates to what the server does. "
               + "Use a fun, cartoonish art style with vibrant colors. "
               + "Include the text '404' prominently in the image. "
               + "Do not include any other text.";
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
}
