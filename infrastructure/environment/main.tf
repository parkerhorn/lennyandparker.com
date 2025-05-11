terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
  }
  backend "azurerm" {
    resource_group_name  = "wedding-api-capability-rg"
    storage_account_name = "weddingapistate"
    container_name      = "tfstate"
    key                 = "environment-${var.environment}.tfstate"
  }
}

provider "azurerm" {
  features {}
}

# Data sources to reference capability resources
data "azurerm_resource_group" "wedding_api_capability_rg" {
  name = "wedding-api-capability-rg"
}

data "azurerm_service_plan" "wedding_api_asp" {
  name                = "wedding-api-asp"
  resource_group_name = data.azurerm_resource_group.wedding_api_capability_rg.name
}

data "azurerm_mssql_server" "wedding_sql_server" {
  name                = "wedding-api-sql-server"
  resource_group_name = data.azurerm_resource_group.wedding_api_capability_rg.name
}

data "azurerm_app_configuration" "wedding_config" {
  name                = "wedding-api-app-config"
  resource_group_name = data.azurerm_resource_group.wedding_api_capability_rg.name
}

# Get Key Vault
data "azurerm_key_vault" "wedding_api_kv" {
  name                = "wedding-api-kv"
  resource_group_name = data.azurerm_resource_group.wedding_api_capability_rg.name
}

# Get the admin password from Key Vault
data "azurerm_key_vault_secret" "sql_admin_password" {
  name         = "sql-admin-password"
  key_vault_id = data.azurerm_key_vault.wedding_api_kv.id
}

# Locals
locals {
  environment = var.environment
  location    = var.location
  tags = {
    Environment = var.environment
    Project     = "WeddingAPI"
    ManagedBy   = "Terraform"
    Type        = "Environment"
  }
}

# Resource Group
resource "azurerm_resource_group" "wedding_api_env_rg" {
  name     = "wedding-api-${local.environment}-rg"
  location = local.location
  tags     = local.tags
}

# SQL Database
resource "azurerm_mssql_database" "wedding_api_db" {
  name           = "WeddingApi-${local.environment}"
  server_id      = data.azurerm_mssql_server.wedding_sql_server.id
  collation      = "SQL_Latin1_General_CP1_CI_AS"
  license_type   = "LicenseIncluded"
  max_size_gb    = 2
  sku_name       = "Basic"
  tags           = local.tags
}

# Web App
resource "azurerm_linux_web_app" "wedding_api" {
  name                = "wedding-api-${local.environment}"
  resource_group_name = azurerm_resource_group.wedding_api_env_rg.name
  location           = azurerm_resource_group.wedding_api_env_rg.location
  service_plan_id    = data.azurerm_service_plan.wedding_api_asp.id
  tags               = local.tags

  site_config {
    application_stack {
      dotnet_version = "8.0"
    }
    always_on = true
  }

  connection_string {
    name  = "DefaultConnection"
    type  = "SQLAzure"
    value = "Server=tcp:${data.azurerm_mssql_server.wedding_sql_server.fully_qualified_domain_name},1433;Initial Catalog=${azurerm_mssql_database.wedding_api_db.name};Persist Security Info=False;User ID=${data.azurerm_mssql_server.wedding_sql_server.administrator_login};Password=${data.azurerm_key_vault_secret.sql_admin_password.value};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }

  app_settings = {
    "AppConfiguration__ConnectionString" = data.azurerm_app_configuration.wedding_config.primary_read_key[0].connection_string
    "ASPNETCORE_ENVIRONMENT" = local.environment == "dev" ? "Development" : "Production"
    "WEBSITE_RUN_FROM_PACKAGE" = "1"
  }
} 