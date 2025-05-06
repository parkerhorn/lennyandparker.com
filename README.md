# .NET 8 Minimal API for Azure

This is a minimal API project built with .NET 8, ready for Azure deployment. The project uses Linux-based infrastructure for optimal performance and cost efficiency.

## Features

- .NET 8 Minimal API setup
- Linux-based Azure App Service
- Swagger/OpenAPI support
- Health check endpoint
- Azure SQL Database (Basic tier)
- Azure DevOps CI/CD pipeline
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

The project is configured for deployment to Azure using Terraform and Azure DevOps pipelines:

1. Infrastructure is managed by Terraform:
   - Linux App Service Plan
   - Linux Web App
   - Azure SQL Database (Basic tier)
   - Resource Group and supporting resources

2. CI/CD Pipeline:
   - Builds on Ubuntu agents
   - Automated testing
   - Deployment to dev/prod environments
   - Uses dotnet-isolated runtime

3. Environment Configuration:
   - Connection strings managed through Azure App Service
   - Environment-specific settings
   - Secure credential management

## Pipeline Setup

To deploy the application:

1. Create a new pipeline in Azure DevOps using the `azure-pipelines.yml` file
2. Create a service connection named "Azure Subscription" with access to your Azure resources
3. Run the pipeline

The pipeline will automatically:
- Create required Azure resources for Terraform state
- Set up dev and prod environments
- Create and secure necessary variables
- Deploy infrastructure
- Build and deploy the application

## API Endpoints

- GET / - Welcome message
- GET /health - Health check endpoint
- GET /swagger - Swagger UI (in development)
- RSVP endpoints under /api/rsvp

## Development

To add new endpoints, modify the `Program.cs` file. The project uses minimal API syntax for defining endpoints.

## Infrastructure

The project uses Terraform for infrastructure management. Key components:

- Linux App Service Plan (B1 tier)
- Linux Web App with dotnet-isolated runtime
- Azure SQL Database (Basic tier)
- Azure DevOps pipeline with Ubuntu agents

To deploy infrastructure:
```bash
cd infrastructure
terraform init
terraform plan
terraform apply
``` 