# Creative 404 Generator

A Blazor Server application built with .NET 10 that generates unique and creative 404 error pages.

## Features

- Generate random creative 404 error messages
- Beautiful gradient backgrounds with random colors
- Interactive Blazor Server application
- Responsive design
- Real-time updates without page refresh
- Easy to customize

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later

## Installation

1. Clone the repository:
```bash
git clone https://github.com/nagendramishr/creative404.git
cd creative404
```

2. Restore dependencies:
```bash
dotnet restore
```

## Usage

### Run the Application

```bash
dotnet run
```

Then open your browser to `https://localhost:5001` or `http://localhost:5000`

### Build for Production

```bash
dotnet build --configuration Release
```

### Publish the Application

```bash
dotnet publish --configuration Release --output ./publish
```

## Project Structure

```
creative404/
├── Components/
│   ├── Layout/            # Layout components
│   │   ├── MainLayout.razor
│   │   ├── NavMenu.razor
│   │   └── ReconnectModal.razor
│   ├── Pages/             # Blazor pages
│   │   ├── Home.razor     # Main 404 generator page
│   │   ├── Home.razor.css # Page-specific styles
│   │   ├── NotFound.razor # Example 404 page
│   │   ├── NotFound.razor.css
│   │   └── Error.razor
│   ├── App.razor          # Root component
│   ├── Routes.razor       # Routing configuration
│   └── _Imports.razor     # Global using directives
├── Properties/
│   └── launchSettings.json
├── wwwroot/               # Static files
│   ├── app.css           # Global styles
│   └── favicon.png
├── tests/                 # Test files
├── docs/                  # Documentation
├── examples/              # Example implementations
├── config/                # Configuration files
├── Program.cs            # Application entry point
├── appsettings.json      # Configuration settings
├── Creative404.csproj    # Project file
├── README.md             # This file
└── LICENSE               # License information
```

## Technology Stack

- **.NET 10**: Latest .NET framework
- **Blazor Server**: Interactive web UI framework
- **C#**: Primary programming language
- **Razor Components**: Component-based UI development

## Customization

### Adding New Messages

Edit the `messages` array in `Components/Pages/Home.razor`:

```csharp
private readonly string[] messages = new[]
{
    "Your custom message here",
    // ... more messages
};
```

### Changing Colors

Modify the `colors` array in `Components/Pages/Home.razor`:

```csharp
private readonly string[] colors = new[]
{
    "#YourColorHex",
    // ... more colors
};
```

### Customizing Styles

- Global styles: Edit `wwwroot/app.css`
- Page-specific styles: Edit `Components/Pages/Home.razor.css`

## Development

### Watch Mode

For development with automatic reloading:

```bash
dotnet watch run
```

### Hot Reload

.NET 10 includes hot reload support. Changes to C# code and Razor files will be automatically applied.

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- Built with .NET 10 and Blazor
- Inspired by creative 404 pages across the web
- Uses modern C# and Razor syntax
