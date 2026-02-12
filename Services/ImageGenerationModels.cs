using System.Text.Json.Serialization;

namespace Creative404.Services;

public class ImageGenerationResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ImageUrl { get; set; }
    public string? RevisedPrompt { get; set; }
}

public class OpenAiImageResponse
{
    [JsonPropertyName("created")]
    public long Created { get; set; }

    [JsonPropertyName("data")]
    public List<OpenAiImageData> Data { get; set; } = new();
}

public class OpenAiImageData
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("revised_prompt")]
    public string? RevisedPrompt { get; set; }
}
