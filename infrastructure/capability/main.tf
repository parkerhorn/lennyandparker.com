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
    container_name       = "tfstate"
    key                  = "capability.tfstate"
  }
}

provider "azurerm" {
  features {}
}

data "azurerm_client_config" "current" {}

# Locals
locals {
  location = var.location
  tags = {
    Project     = "WeddingAPI"
    ManagedBy   = "Terraform"
    Type        = "Capability"
    Environment = "Capability"
  }
}

data "azurerm_resource_group" "wedding_api_capability_rg" {
  name = "wedding-api-capability-rg"
}

# App Service Plan
resource "azurerm_service_plan" "wedding_api_asp" {
  name                = "wedding-api-asp"
  resource_group_name = data.azurerm_resource_group.wedding_api_capability_rg.name
  location            = data.azurerm_resource_group.wedding_api_capability_rg.location
  os_type            = "Linux"
  sku_name           = "F1"
  tags               = local.tags
}

resource "random_password" "sql_admin_password" {
  length           = 32
  special          = true
  override_special = "!@#$%"
  min_lower        = 1
  min_upper        = 1
  min_numeric      = 1
  min_special      = 1
}

# SQL Server - Using Basic tier
resource "azurerm_mssql_server" "wedding_sql_server" {
  name                         = "wedding-api-sql-server"
  resource_group_name          = data.azurerm_resource_group.wedding_api_capability_rg.name
  location                     = data.azurerm_resource_group.wedding_api_capability_rg.location
  version                      = "12.0"
  administrator_login          = "weddingadmin"
  administrator_login_password = random_password.sql_admin_password.result
  minimum_tls_version         = "1.2"
  tags                        = local.tags
}

# SQL Database - Using Basic tier (5 DTU)
resource "azurerm_mssql_database" "wedding_db" {
  name           = "wedding-api-db"
  server_id      = azurerm_mssql_server.wedding_sql_server.id
  sku_name       = "Basic"
  max_size_gb    = 2
  tags           = local.tags
}

# Key Vault - Using standard tier (free tier not available)
resource "azurerm_key_vault" "wedding_api_kv" {
  name                        = "wedding-api-kv"
  location                    = data.azurerm_resource_group.wedding_api_capability_rg.location
  resource_group_name         = data.azurerm_resource_group.wedding_api_capability_rg.name
  enabled_for_disk_encryption = true
  tenant_id                   = data.azurerm_client_config.current.tenant_id
  soft_delete_retention_days  = 7
  purge_protection_enabled    = false
  sku_name                   = "standard"  # Only standard available
  tags                       = local.tags

  access_policy {
    tenant_id = data.azurerm_client_config.current.tenant_id
    object_id = data.azurerm_client_config.current.object_id

    secret_permissions = [
      "Get",
      "List",
      "Set",
      "Delete"
    ]
  }

  access_policy {
    tenant_id = data.azurerm_client_config.current.tenant_id
    object_id = var.service_principal_id

    secret_permissions = [
      "Get",
      "List"
    ]
  }
}

resource "azurerm_key_vault_secret" "sql_admin_password" {
  name         = "sql-admin-password"
  value        = random_password.sql_admin_password.result
  key_vault_id = azurerm_key_vault.wedding_api_kv.id
}

# App Configuration - Using free tier
resource "azurerm_app_configuration" "wedding_api_app_config" {
  name                = "wedding-api-app-config"
  resource_group_name = data.azurerm_resource_group.wedding_api_capability_rg.name
  location            = data.azurerm_resource_group.wedding_api_capability_rg.location
  sku                = "free"
  tags               = local.tags
} 