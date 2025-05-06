# Wedding API Infrastructure

This directory contains the Terraform configuration for the Wedding API infrastructure. The infrastructure is split into two main components:

## Pre-deployment Checklist

Before deploying to Azure, ensure:

1. **Azure Authentication**:
   - Log into the [Azure Portal](https://portal.azure.com)
   - Verify you have the correct subscription selected in the top-right corner
   - Ensure you have Owner or Contributor permissions on the subscription

2. **Register Required Resource Providers**:
   - Go to your subscription in the Azure Portal
   - Click on "Resource providers" in the left menu
   - Search for and register:
     - Microsoft.Web
     - Microsoft.Sql
     - Microsoft.KeyVault
     - Microsoft.AppConfiguration
     - Microsoft.Storage
   - Click "Register" for each provider
   - Wait for registration to complete (can take a few minutes)

3. **Create Service Principal**:
   ```bash
   # Create service principal
   az ad sp create-for-rbac --name "wedding-api-deploy" --role Contributor --scopes /subscriptions/<subscription-id>
   
   # Note the appId from the output - you'll need it for deployment
   ```

## Capability Infrastructure (`capability/`)

Shared resources used across all environments:

- **Resource Group**: `wedding-api-capability-rg`
- **Storage Account**: `wedding-api-state-storage` (for Terraform state)
- **App Service Plan**: `wedding-api-asp` (Linux, B1)
- **SQL Server**: `wedding-api-sql-server`
- **Key Vault**: `wedding-api-kv`
- **App Configuration**: `wedding-api-app-config`

## Environment Infrastructure (`environment/`)

Environment-specific resources:

- **Resource Group**: `wedding-api-{env}-rg`
- **SQL Database**: `WeddingApi-{env}`
- **Web App**: `wedding-api-{env}`

## Naming Conventions

- Resource Groups: `wedding-api-{env}-rg`
- Storage Accounts: `wedding-api-{purpose}`
- SQL Servers: `wedding-api-sql-server`
- SQL Databases: `WeddingApi-{env}`
- App Service Plans: `wedding-api-asp`
- Web Apps: `wedding-api-{env}`
- Key Vaults: `wedding-api-kv`
- App Configuration: `wedding-api-app-config`

## Security

- SQL Server admin password is generated using `random_password` and stored in Key Vault
- Key Vault is used for storing sensitive information
- Storage Account uses TLS 1.2
- Blob Container is private
- SQL Server connection strings are managed through Key Vault

## Deployment

1. Deploy capability infrastructure first:
   ```bash
   cd capability
   terraform init
   terraform apply -var="service_principal_id=<appId-from-previous-step>"
   ```

2. Deploy environment infrastructure:
   ```bash
   cd environment
   terraform init
   terraform apply -var-file=dev.tfvars  # or prod.tfvars
   ```

## State Management

- Capability state: `capability.tfstate`
- Environment state: `environment-{env}.tfstate`
- State is stored in Azure Storage Account
- Each environment has its own state file 