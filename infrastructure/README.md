# Wedding API Infrastructure

This directory contains the Terraform configuration for the Wedding API infrastructure. The infrastructure is split into two main components:

## Initial Setup (One-time Manual Steps)

Before running any pipelines, you need to manually create the capability infrastructure first:

1. **Create Resource Group**:
   ```bash
   az group create --name wedding-api-capability-rg --location eastus
   ```

2. **Create Storage Account**:
   ```bash
   az storage account create \
     --name weddingapistate \
     --resource-group wedding-api-capability-rg \
     --location eastus \
     --sku Standard_LRS \
     --encryption-services blob
   ```

3. **Create Blob Container**:
   ```bash
   az storage container create \
     --name tfstate \
     --account-name weddingapistate
   ```

4. **Deploy Capability Infrastructure**:
   ```bash
   cd capability
   terraform init
   terraform apply -var="service_principal_id=<appId-from-previous-step>"
   ```

After these steps are complete, you can use the pipelines for subsequent deployments.

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

4. **Azure DevOps Setup**:
   - Go to your Azure DevOps project
   - Click on "Project Settings" (bottom left)
   - Click on "Extensions" under "General"
   - Click "Browse Marketplace"
   - Search for "Terraform"
   - Install "Terraform" by Microsoft DevLabs
   - Wait for the extension to be installed

5. **Create Service Connection**:
   - Go to your Azure DevOps project
   - Click on "Project Settings" (bottom left)
   - Click on "Service connections" under "Pipelines"
   - Click "Create service connection"
   - Select "Azure Resource Manager"
   - Select "Service principal (manual)"
   - Fill in the details:
     - Environment: Azure Cloud
     - Scope Level: Subscription
     - Subscription: Select your subscription
     - Service Principal ID: Use the appId from step 3
     - Service Principal Key: Use the password from step 3
     - Tenant ID: Use the tenant from step 3
     - Service Connection Name: "Azure Subscription"
   - Click "Grant access permission to all pipelines"
   - Click "Save"

6. **Request Pipeline Parallelism**:
   - Go to https://aka.ms/azpipelines-parallelism-request
   - Fill out the form with:
     - Organization name
     - Project name
     - Email address
     - Reason for request (e.g., "Infrastructure deployment")
   - Submit the form
   - Wait for approval (usually takes 1-2 business days)
   - Once approved, your pipelines will be able to run

## Capability Infrastructure (`capability/`)

Shared resources used across all environments:

- **Resource Group**: `wedding-api-capability-rg`
- **Storage Account**: `weddingapistate` (for Terraform state)
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
- Storage Accounts: `weddingapistate`
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