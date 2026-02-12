# Deployment Guide

This guide provides detailed instructions for deploying the Creative404 application to various hosting environments.

## Table of Contents

1. [Azure App Service Deployment](#azure-app-service-deployment)
2. [Docker Deployment](#docker-deployment)
3. [IIS Deployment](#iis-deployment)
4. [Linux Server Deployment](#linux-server-deployment)
5. [Environment Variables](#environment-variables)
6. [Troubleshooting](#troubleshooting)

## Azure App Service Deployment

### Prerequisites
- Azure account with an active subscription
- Azure CLI or Azure Portal access

### Option 1: GitHub Actions (Recommended)

1. **Create Azure App Service**
   ```bash
   az webapp create --resource-group myResourceGroup \
     --plan myAppServicePlan \
     --name creative404 \
     --runtime "DOTNET|10.0"
   ```

2. **Get Publish Profile**
   ```bash
   az webapp deployment list-publishing-profiles \
     --name creative404 \
     --resource-group myResourceGroup \
     --xml
   ```

3. **Add GitHub Secret**
   - Go to your GitHub repository
   - Navigate to Settings > Secrets and variables > Actions
   - Add a new secret named `AZURE_WEBAPP_PUBLISH_PROFILE`
   - Paste the publish profile XML

4. **Update Workflow**
   - Edit `.github/workflows/azure-deploy.yml`
   - Set `AZURE_WEBAPP_NAME` to your app name

5. **Deploy**
   - Push to `main` branch or trigger workflow manually
   - Monitor deployment in GitHub Actions tab

### Option 2: Azure Portal

1. **Create App Service**
   - Go to Azure Portal
   - Create new App Service
   - Select .NET 10 runtime
   - Choose Windows or Linux

2. **Deploy from GitHub**
   - In App Service, go to Deployment Center
   - Select GitHub as source
   - Authorize and select repository
   - Configure build and deploy

3. **Configure App**
   - Set environment variables in Configuration
   - Enable Application Insights (optional)
   - Configure custom domain (optional)

## Docker Deployment

### Create Dockerfile

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Creative404.csproj", "."]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Creative404.dll"]
```

### Build and Run

```bash
# Build image
docker build -t creative404 .

# Run container
docker run -d -p 8080:80 --name creative404-app creative404

# Access application
# http://localhost:8080
```

### Docker Compose

```yaml
version: '3.8'

services:
  web:
    build: .
    ports:
      - "8080:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
    restart: unless-stopped
```

## IIS Deployment

### Prerequisites
- Windows Server with IIS installed
- .NET 10 Hosting Bundle
- URL Rewrite Module (optional)

### Steps

1. **Install .NET 10 Hosting Bundle**
   - Download from [Microsoft](https://dotnet.microsoft.com/download/dotnet/10.0)
   - Run installer
   - Restart IIS: `iisreset`

2. **Publish Application**
   ```bash
   dotnet publish -c Release -o C:\inetpub\creative404
   ```

3. **Create IIS Site**
   - Open IIS Manager
   - Right-click Sites > Add Website
   - Site name: Creative404
   - Physical path: C:\inetpub\creative404
   - Binding: Port 80 (or your choice)

4. **Configure Application Pool**
   - Set .NET CLR version to "No Managed Code"
   - Set identity to ApplicationPoolIdentity

5. **Set Permissions**
   - Grant IIS_IUSRS read/execute permissions
   - Grant write permissions to logs folder

## Linux Server Deployment

### Prerequisites
- Ubuntu 20.04+ or similar
- .NET 10 Runtime
- Nginx or Apache

### Using Systemd Service

1. **Install .NET 10**
   ```bash
   wget https://dot.net/v1/dotnet-install.sh
   chmod +x dotnet-install.sh
   ./dotnet-install.sh --version 10.0
   ```

2. **Publish and Copy**
   ```bash
   dotnet publish -c Release -o /var/www/creative404
   ```

3. **Create Service**
   ```bash
   sudo nano /etc/systemd/system/creative404.service
   ```

   ```ini
   [Unit]
   Description=Creative404 Application

   [Service]
   WorkingDirectory=/var/www/creative404
   ExecStart=/usr/bin/dotnet /var/www/creative404/Creative404.dll
   Restart=always
   RestartSec=10
   KillSignal=SIGINT
   SyslogIdentifier=creative404
   User=www-data
   Environment=ASPNETCORE_ENVIRONMENT=Production
   Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

   [Install]
   WantedBy=multi-user.target
   ```

4. **Start Service**
   ```bash
   sudo systemctl enable creative404
   sudo systemctl start creative404
   sudo systemctl status creative404
   ```

5. **Configure Nginx**
   ```bash
   sudo nano /etc/nginx/sites-available/creative404
   ```

   ```nginx
   server {
       listen 80;
       server_name your-domain.com;

       location / {
           proxy_pass http://localhost:5000;
           proxy_http_version 1.1;
           proxy_set_header Upgrade $http_upgrade;
           proxy_set_header Connection keep-alive;
           proxy_set_header Host $host;
           proxy_cache_bypass $http_upgrade;
           proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
           proxy_set_header X-Forwarded-Proto $scheme;
       }
   }
   ```

6. **Enable Site**
   ```bash
   sudo ln -s /etc/nginx/sites-available/creative404 /etc/nginx/sites-enabled/
   sudo nginx -t
   sudo systemctl restart nginx
   ```

## Environment Variables

### Production Settings

```bash
# Azure App Service
ASPNETCORE_ENVIRONMENT=Production
WEBSITE_RUN_FROM_PACKAGE=1

# Custom Settings (optional)
MAX_GIF_WIDTH=1200
MAX_GIF_HEIGHT=800
DEFAULT_GIF_WIDTH=600
DEFAULT_GIF_HEIGHT=400
```

### Setting Environment Variables

**Azure:**
```bash
az webapp config appsettings set --name creative404 \
  --resource-group myResourceGroup \
  --settings ASPNETCORE_ENVIRONMENT=Production
```

**Docker:**
```bash
docker run -e ASPNETCORE_ENVIRONMENT=Production creative404
```

**Linux:**
Add to systemd service file or `/etc/environment`

## Troubleshooting

### Application Won't Start

**Check .NET Runtime**
```bash
dotnet --list-runtimes
```

**Check Logs**
- Azure: App Service Logs in Portal
- Linux: `sudo journalctl -u creative404 -f`
- Windows: Event Viewer > Application logs

### 502 Bad Gateway

- Verify application is running
- Check port binding (5000/5001)
- Verify reverse proxy configuration
- Check firewall rules

### SkiaSharp Errors

**Linux:** Install required dependencies
```bash
sudo apt-get install -y libfontconfig1
```

**Docker:** Use appropriate base image
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0
RUN apt-get update && apt-get install -y libfontconfig1
```

### High Memory Usage

- Monitor with Application Insights
- Implement caching for MCP requests
- Limit concurrent GIF generations
- Use connection pooling

## Performance Optimization

### Production Checklist

- [ ] Enable response compression
- [ ] Configure caching headers
- [ ] Use CDN for static assets
- [ ] Enable Application Insights
- [ ] Set up health checks
- [ ] Configure auto-scaling
- [ ] Implement rate limiting
- [ ] Enable HTTPS/TLS

### Scaling Considerations

- **Vertical Scaling**: Increase CPU/memory of host
- **Horizontal Scaling**: Add multiple instances with load balancer
- **Database**: Consider caching MCP responses
- **CDN**: Serve generated images from CDN

## Monitoring

### Azure Application Insights

Add to `Program.cs`:
```csharp
builder.Services.AddApplicationInsightsTelemetry();
```

### Health Checks

```csharp
builder.Services.AddHealthChecks();
app.MapHealthChecks("/health");
```

### Logging

Monitor logs in production:
```bash
# Azure
az webapp log tail --name creative404 --resource-group myResourceGroup

# Linux
sudo journalctl -u creative404 -f

# Docker
docker logs -f creative404-app
```

## Security Best Practices

1. **Use HTTPS**: Always enable SSL/TLS in production
2. **Environment Variables**: Never commit secrets
3. **Rate Limiting**: Implement to prevent abuse
4. **Input Validation**: Validate all MCP URLs
5. **CORS**: Configure appropriately for your domain
6. **Updates**: Keep .NET and dependencies updated
7. **Monitoring**: Enable security alerts

## Support

For deployment issues:
- Check [GitHub Issues](https://github.com/nagendramishr/creative404/issues)
- Review [.NET Deployment Docs](https://docs.microsoft.com/aspnet/core/host-and-deploy/)
- Contact: Repository maintainers
