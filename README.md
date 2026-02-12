# Creative 404

A .NET 10 Blazor web application that generates custom animated 404 pages powered by MCP (Model Context Protocol) servers.

## Features

- 🎨 **Dynamic GIF Generation**: Create custom 404 images based on MCP server content
- 🔌 **MCP Integration**: Connect to any MCP server via HTTP/HTTPS
- 🎭 **Multiple Themes**: Choose from 4 beautiful pre-designed themes (Default, Ocean, Sunset, Forest)
- 📐 **Customizable Dimensions**: Generate images from 200x150 to 1200x800 pixels
- 💾 **Easy Download**: Download generated images instantly
- 📋 **Embed Code**: Get ready-to-use HTML embed code
- 🚀 **Azure Ready**: Configured for deployment to Azure App Service

## Technology Stack

- **.NET 10** - Latest ASP.NET Core framework
- **Blazor Server** - Interactive web UI with C#
- **SkiaSharp** - High-performance image rendering
- **Bootstrap 5** - Responsive design
- **xUnit** - Unit testing framework

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Any modern web browser
- (Optional) Azure account for deployment

### Local Development

1. **Clone the repository**
   ```bash
   git clone https://github.com/nagendramishr/creative404.git
   cd creative404
   ```

2. **Build the project**
   ```bash
   dotnet build
   ```

3. **Run the application**
   ```bash
   dotnet run
   ```

4. **Open your browser**
   Navigate to `http://localhost:5153`

### Running Tests

```bash
cd Tests
dotnet test
```

## How to Use

1. **Enter MCP Server URL**: Provide the URL or command for your MCP server
   - HTTP/HTTPS URL: `http://localhost:3001`
   - Command: `npx mcp-server`

2. **Preview MCP Info** (Optional): Click "Preview MCP Info" to verify the connection

3. **Choose Settings**:
   - Select a theme (Default, Ocean, Sunset, Forest)
   - Set dimensions (width and height in pixels)

4. **Generate**: Click "Generate 404 GIF" to create your custom image

5. **Download & Use**: Download the image or copy the embed code for your website

## API Endpoints

### POST `/api/gif/generate`
Generate a 404 image based on MCP server content.

**Request Body:**
```json
{
  "mcpServerUrl": "http://localhost:3001",
  "theme": "default",
  "width": 600,
  "height": 400
}
```

**Response:** PNG image file

### POST `/api/gif/preview`
Preview MCP server information without generating an image.

**Request Body:**
```json
{
  "mcpServerUrl": "http://localhost:3001"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "serverName": "localhost",
    "toolCount": 2,
    "resourceCount": 1,
    "tools": ["search", "analyze"],
    "resources": ["resource://docs"],
    "connected": true
  }
}
```

### GET `/api/gif/themes`
Get list of available themes.

**Response:**
```json
{
  "themes": [
    {
      "id": "default",
      "name": "Default",
      "description": "Dark blue theme"
    },
    ...
  ]
}
```

## Deployment

### Azure App Service

1. **Create an Azure App Service** (Windows or Linux)
   - Runtime: .NET 10
   - Operating System: Windows or Linux

2. **Configure Deployment**
   - Add your Azure publish profile as a GitHub secret named `AZURE_WEBAPP_PUBLISH_PROFILE`
   - Update `AZURE_WEBAPP_NAME` in `.github/workflows/azure-deploy.yml`

3. **Deploy**
   - Push to `main` branch, or
   - Trigger the workflow manually from GitHub Actions

### Manual Deployment

1. **Publish the application**
   ```bash
   dotnet publish -c Release -o ./publish
   ```

2. **Deploy to your web server**
   - Copy the contents of `./publish` to your server
   - Configure IIS or reverse proxy (nginx/Apache)

## Configuration

Application settings can be configured in `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

## Project Structure

```
Creative404/
├── Components/              # Blazor components
│   ├── Layout/             # Layout components
│   └── Pages/              # Page components
├── Controllers/            # API controllers
├── Models/                 # Data models
├── Services/               # Business logic
│   ├── GifGenerationService.cs
│   └── McpService.cs
├── Tests/                  # Unit tests
├── wwwroot/                # Static files
├── Program.cs              # Application entry point
└── Creative404.csproj      # Project file
```

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the ISC License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Built with [.NET 10](https://dotnet.microsoft.com/)
- Image processing powered by [SkiaSharp](https://github.com/mono/SkiaSharp)
- UI components from [Bootstrap](https://getbootstrap.com/)
- Supports [Model Context Protocol](https://modelcontextprotocol.io/)

