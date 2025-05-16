terraform {
  required_version = ">= 1.6.0"
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

data "azurerm_client_config" "current" {
}

data "azurerm_resource_group" "wedding_api_capability_rg" {
  name = "wedding-api-capability-rg"
}

data "azurerm_mssql_server" "wedding_sql_server" {
  name                = "wedding-api-sql-server"
  resource_group_name = data.azurerm_resource_group.wedding_api_capability_rg.name
}

data "azurerm_key_vault" "wedding_api_kv" {
  name                = "lennyandparkerweddingkv"
  resource_group_name = data.azurerm_resource_group.wedding_api_capability_rg.name
}

data "azurerm_key_vault_secret" "sql_admin_password" {
  name         = "sql-server-admin-password"
  key_vault_id = data.azurerm_key_vault.wedding_api_kv.id
}

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

resource "azurerm_resource_group" "wedding_api_env_rg" {
  name     = "wedding-api-${local.environment}-rg"
  location = local.location
  tags     = local.tags
}

resource "azurerm_service_plan" "wedding_api_asp" {
  name                = "wedding-api-${local.environment}-asp"
  resource_group_name = azurerm_resource_group.wedding_api_env_rg.name
  location            = azurerm_resource_group.wedding_api_env_rg.location
  os_type             = "Linux"
  sku_name            = "F1"
  tags                = local.tags
}

resource "azurerm_mssql_database" "wedding_api_db" {
  name            = "WeddingApi-${local.environment}"
  server_id       = data.azurerm_mssql_server.wedding_sql_server.id
  collation       = "SQL_Latin1_General_CP1_CI_AS"
  max_size_gb     = 2
  sku_name        = "GP_S_Gen5_1"
  min_capacity    = 0.5
  auto_pause_delay_in_minutes = 60
  tags            = local.tags
}

resource "azurerm_key_vault_secret" "database_connection_string" {
  name         = "WeddingApiDbConnectionString${local.environment}"
  value        = "Server=tcp:${data.azurerm_mssql_server.wedding_sql_server.fully_qualified_domain_name},1433;Initial Catalog=${azurerm_mssql_database.wedding_api_db.name};Persist Security Info=False;User ID=${data.azurerm_mssql_server.wedding_sql_server.administrator_login};Password=${data.azurerm_key_vault_secret.sql_admin_password.value};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  key_vault_id = data.azurerm_key_vault.wedding_api_kv.id
  tags         = local.tags

  depends_on = [
    data.azurerm_key_vault_secret.sql_admin_password,
    azurerm_mssql_database.wedding_api_db
  ]
}

resource "azurerm_linux_web_app" "wedding_api" {
  name                = "wedding-api-${local.environment}"
  resource_group_name = azurerm_resource_group.wedding_api_env_rg.name
  location            = azurerm_resource_group.wedding_api_env_rg.location
  service_plan_id     = azurerm_service_plan.wedding_api_asp.id
  tags                = local.tags

  identity {
    type = "SystemAssigned"
  }

  site_config {
    application_stack {
      dotnet_version = "8.0"
    }
    always_on = false
  }

  app_settings = {
    "ASPNETCORE_ENVIRONMENT"                 = local.environment == "dev" ? "Development" : "Production",
    "WEBSITE_RUN_FROM_PACKAGE"               = "1",
    "ConnectionStrings__DefaultConnection"   = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.database_connection_string.id})"
  }

  depends_on = [
    azurerm_key_vault_secret.database_connection_string
  ]
} 