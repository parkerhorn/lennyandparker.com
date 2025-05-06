# .NET 8 Minimal API for Azure

This is a minimal API project built with .NET 8, ready for Azure deployment.

## Features

- .NET 8 Minimal API setup
- Swagger/OpenAPI support
- Health check endpoint
- Azure-ready configuration
- HTTPS redirection

## Getting Started

1. Ensure you have .NET 8 SDK installed
2. Clone this repository
3. Run the following commands:

```bash
dotnet restore
dotnet run
```

The API will be available at:
- https://localhost:5001
- http://localhost:5000

## Azure Deployment

To deploy to Azure:

1. Create an Azure App Service
2. Configure the following settings in Azure:
   - Set the .NET version to 8.0
   - Enable HTTPS
   - Configure application settings if needed

3. Deploy using Azure CLI or Visual Studio

## API Endpoints

- GET / - Welcome message
- GET /health - Health check endpoint
- GET /swagger - Swagger UI (in development)

## Development

To add new endpoints, modify the `Program.cs` file. The project uses minimal API syntax for defining endpoints. 