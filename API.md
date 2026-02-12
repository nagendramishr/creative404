# API Documentation

## Overview

The Creative404 API provides endpoints for generating custom 404 images based on MCP (Model Context Protocol) server content.

**Base URL**: `https://your-domain.com/api`

## Authentication

Currently, the API does not require authentication. In production, consider implementing:
- API keys
- OAuth 2.0
- JWT tokens

## Rate Limiting

Consider implementing rate limiting in production to prevent abuse:
- Recommended: 10 requests per minute per IP
- 100 requests per hour per IP

## Endpoints

### 1. Generate 404 Image

Generate a custom 404 image based on MCP server content.

**Endpoint**: `POST /api/gif/generate`

**Request Headers**:
```
Content-Type: application/json
```

**Request Body**:
```json
{
  "mcpServerUrl": "string",  // Required: MCP server URL or command
  "theme": "string",          // Optional: Theme ID (default: "default")
  "width": number,            // Optional: Image width in pixels (default: 600, min: 200, max: 1200)
  "height": number            // Optional: Image height in pixels (default: 400, min: 150, max: 800)
}
```

**Example Request**:
```bash
curl -X POST https://your-domain.com/api/gif/generate \
  -H "Content-Type: application/json" \
  -d '{
    "mcpServerUrl": "http://localhost:3001",
    "theme": "ocean",
    "width": 800,
    "height": 600
  }' \
  --output 404.png
```

**Success Response** (200 OK):
- **Content-Type**: `image/png`
- **Body**: Binary PNG image data

**Error Responses**:

- **400 Bad Request**:
  ```json
  {
    "error": "MCP server URL is required"
  }
  ```
  ```json
  {
    "error": "Invalid MCP server URL"
  }
  ```

- **500 Internal Server Error**:
  ```json
  {
    "error": "Failed to connect to MCP server",
    "details": "Connection timeout"
  }
  ```
  ```json
  {
    "error": "Failed to generate GIF",
    "details": "Image generation error"
  }
  ```

---

### 2. Preview MCP Server

Preview MCP server information without generating an image.

**Endpoint**: `POST /api/gif/preview`

**Request Headers**:
```
Content-Type: application/json
```

**Request Body**:
```json
{
  "mcpServerUrl": "string"  // Required: MCP server URL or command
}
```

**Example Request**:
```bash
curl -X POST https://your-domain.com/api/gif/preview \
  -H "Content-Type: application/json" \
  -d '{
    "mcpServerUrl": "http://localhost:3001"
  }'
```

**Success Response** (200 OK):
```json
{
  "success": true,
  "data": {
    "serverName": "localhost",
    "toolCount": 3,
    "resourceCount": 2,
    "tools": [
      "search",
      "analyze",
      "process"
    ],
    "resources": [
      "resource://docs",
      "resource://data"
    ],
    "connected": true,
    "errorMessage": null
  }
}
```

**Error Responses**:

- **400 Bad Request**:
  ```json
  {
    "error": "MCP server URL is required"
  }
  ```
  ```json
  {
    "error": "Invalid MCP server URL"
  }
  ```

- **500 Internal Server Error**:
  ```json
  {
    "error": "Failed to query MCP server",
    "details": "Connection refused"
  }
  ```

---

### 3. Get Available Themes

Retrieve a list of all available themes for image generation.

**Endpoint**: `GET /api/gif/themes`

**Request Headers**: None required

**Example Request**:
```bash
curl https://your-domain.com/api/gif/themes
```

**Success Response** (200 OK):
```json
{
  "themes": [
    {
      "id": "default",
      "name": "Default",
      "description": "Dark blue theme",
      "backgroundColors": ["#1a1a2e", "#16213e"],
      "textColor": "#eeeeee",
      "accentColor": "#0f3460"
    },
    {
      "id": "ocean",
      "name": "Ocean",
      "description": "Cool blue waves",
      "backgroundColors": ["#0077be", "#005f8c"],
      "textColor": "#ffffff",
      "accentColor": "#00d4ff"
    },
    {
      "id": "sunset",
      "name": "Sunset",
      "description": "Warm orange glow",
      "backgroundColors": ["#ff6b6b", "#ee5a6f"],
      "textColor": "#ffffff",
      "accentColor": "#ffd93d"
    },
    {
      "id": "forest",
      "name": "Forest",
      "description": "Natural green",
      "backgroundColors": ["#2d4a3e", "#1f3329"],
      "textColor": "#e8f5e9",
      "accentColor": "#66bb6a"
    }
  ]
}
```

---

## Data Models

### GifGenerationRequest

```typescript
interface GifGenerationRequest {
  mcpServerUrl: string;    // MCP server URL or command
  theme?: string;          // Theme ID (default, ocean, sunset, forest)
  width?: number;          // Width in pixels (200-1200)
  height?: number;         // Height in pixels (150-800)
}
```

### McpServerInfo

```typescript
interface McpServerInfo {
  serverName: string;      // Extracted server name
  toolCount: number;       // Number of available tools
  resourceCount: number;   // Number of available resources
  tools: string[];         // List of tool names
  resources: string[];     // List of resource URIs
  connected: boolean;      // Connection status
  errorMessage?: string;   // Error message if connection failed
}
```

### ThemeInfo

```typescript
interface ThemeInfo {
  id: string;                 // Theme identifier
  name: string;               // Display name
  description: string;        // Theme description
  backgroundColors: string[]; // Gradient colors (hex)
  textColor: string;          // Text color (hex)
  accentColor: string;        // Accent color (hex)
}
```

---

## Error Handling

All endpoints follow a consistent error response format:

```json
{
  "error": "Error message",
  "details": "Additional error details (optional)"
}
```

### HTTP Status Codes

- `200 OK`: Successful request
- `400 Bad Request`: Invalid request parameters
- `500 Internal Server Error`: Server-side error

---

## Usage Examples

### JavaScript/TypeScript

```typescript
// Generate 404 image
async function generate404Image(mcpUrl: string, theme: string = 'default') {
  const response = await fetch('https://your-domain.com/api/gif/generate', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({
      mcpServerUrl: mcpUrl,
      theme: theme,
      width: 600,
      height: 400
    })
  });

  if (response.ok) {
    const blob = await response.blob();
    const url = URL.createObjectURL(blob);
    // Use the URL to display or download the image
    return url;
  } else {
    const error = await response.json();
    throw new Error(error.error);
  }
}

// Preview MCP server
async function previewMcpServer(mcpUrl: string) {
  const response = await fetch('https://your-domain.com/api/gif/preview', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({
      mcpServerUrl: mcpUrl
    })
  });

  const data = await response.json();
  return data.data;
}

// Get themes
async function getThemes() {
  const response = await fetch('https://your-domain.com/api/gif/themes');
  const data = await response.json();
  return data.themes;
}
```

### Python

```python
import requests

# Generate 404 image
def generate_404_image(mcp_url, theme='default'):
    response = requests.post(
        'https://your-domain.com/api/gif/generate',
        json={
            'mcpServerUrl': mcp_url,
            'theme': theme,
            'width': 600,
            'height': 400
        }
    )
    
    if response.status_code == 200:
        with open('404.png', 'wb') as f:
            f.write(response.content)
        return '404.png'
    else:
        raise Exception(response.json()['error'])

# Preview MCP server
def preview_mcp_server(mcp_url):
    response = requests.post(
        'https://your-domain.com/api/gif/preview',
        json={'mcpServerUrl': mcp_url}
    )
    
    data = response.json()
    return data['data']

# Get themes
def get_themes():
    response = requests.get('https://your-domain.com/api/gif/themes')
    data = response.json()
    return data['themes']
```

### C# / .NET

```csharp
using System.Net.Http;
using System.Text;
using System.Text.Json;

// Generate 404 image
public async Task<byte[]> Generate404Image(string mcpUrl, string theme = "default")
{
    using var client = new HttpClient();
    
    var request = new
    {
        mcpServerUrl = mcpUrl,
        theme = theme,
        width = 600,
        height = 400
    };
    
    var content = new StringContent(
        JsonSerializer.Serialize(request),
        Encoding.UTF8,
        "application/json"
    );
    
    var response = await client.PostAsync(
        "https://your-domain.com/api/gif/generate",
        content
    );
    
    response.EnsureSuccessStatusCode();
    return await response.Content.ReadAsByteArrayAsync();
}

// Preview MCP server
public async Task<McpServerInfo> PreviewMcpServer(string mcpUrl)
{
    using var client = new HttpClient();
    
    var request = new { mcpServerUrl = mcpUrl };
    var content = new StringContent(
        JsonSerializer.Serialize(request),
        Encoding.UTF8,
        "application/json"
    );
    
    var response = await client.PostAsync(
        "https://your-domain.com/api/gif/preview",
        content
    );
    
    var json = await response.Content.ReadAsStringAsync();
    var result = JsonSerializer.Deserialize<PreviewResponse>(json);
    return result.Data;
}
```

---

## Best Practices

1. **Caching**: Cache MCP server responses to reduce load
2. **Validation**: Always validate MCP URLs before requests
3. **Error Handling**: Implement proper error handling
4. **Timeouts**: Set appropriate timeouts for MCP connections
5. **Rate Limiting**: Respect rate limits to avoid throttling
6. **HTTPS**: Always use HTTPS in production
7. **Logging**: Log errors for debugging

---

## Versioning

Current API Version: **v1**

Future versions will be available at `/api/v2/...`

---

## Support

For API questions or issues:
- GitHub Issues: https://github.com/nagendramishr/creative404/issues
- Documentation: https://github.com/nagendramishr/creative404
