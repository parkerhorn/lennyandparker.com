# lennyandparker.com

Full-stack wedding platform showcasing enterprise .NET 8 microservice architecture, modern SvelteKit frontend, and automated Azure DevOps deployment pipelines.

## Architecture

**Backend** - .NET 8 Minimal APIs with Repository Pattern, Unit of Work, and Entity Framework Core  
**Frontend** - SvelteKit with TypeScript and TailwindCSS  
**Infrastructure** - Terraform-managed Azure microservices with automated CI/CD

## Key Technical Features

### Backend
- **Repository Pattern** with generic async repositories
- **Unit of Work Pattern** for transaction management  
- **Dependency Injection** with scoped service lifetimes
- **Entity Framework Core** migrations and SQL Server integration
- **Application Insights** telemetry and Azure Monitor alerts
- **Fuzzy string matching** for intelligent guest search (90%+ similarity)

### Frontend
- **Svelte 5 Runes** for reactive state management with `$state` and `$bindable`
- **SvelteKit API Routes** acting as middleware proxy to .NET backend
- **Component Library** with Bits UI and custom TailwindCSS components
- **Type-safe Development** with TypeScript and JSDoc
- **Modern CSS** with TailwindCSS 4.0, custom design tokens, and responsive utilities
- **Progressive Enhancement** with JavaScript-optional forms and navigation

### Infrastructure
- **Infrastructure as Code** with Terraform
- **Multi-environment deployments** via Azure DevOps pipelines

## Technology Stack

**Backend**: .NET 8, Entity Framework Core, SQL Server, Application Insights, FuzzySharp  
**Frontend**: SvelteKit 2.0, TypeScript, TailwindCSS, Bits UI  
**Infrastructure**: Azure App Service, Azure SQL Database, Azure Key Vault, Terraform, Azure DevOps

## Azure Microservices

- **API Service** - .NET 8 minimal APIs with health checks and telemetry
- **Database Service** - Azure SQL with auto-pause and managed backups  
- **Storage Service** - Azure Key Vault for configuration and secrets
- **Monitoring Service** - Application Insights with custom metrics and alerts

## DevOps Implementation

Fully automated deployment pipeline with environment-specific configurations:
- **Infrastructure Pipeline** - Terraform provisioning and database migrations
- **Application Pipelines** - Automated build, test, and deployment
- **Environment Management** - Dev/prod isolation with Azure Key Vault integration