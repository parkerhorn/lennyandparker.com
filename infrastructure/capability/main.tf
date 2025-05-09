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

# Resource Group
resource "azurerm_resource_group" "wedding_api_capability_rg" {
  name     = "wedding-api-capability-rg"
  location = var.location
  tags     = local.tags

  lifecycle {
    prevent_destroy = true
  }
}

# Storage Account for Terraform State
resource "azurerm_storage_account" "terraform_state" {
  name                     = "weddingapistate"
  resource_group_name      = azurerm_resource_group.wedding_api_capability_rg.name
  location                 = azurerm_resource_group.wedding_api_capability_rg.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
  min_tls_version         = "TLS1_2"
  
  blob_properties {
    delete_retention_policy {
      days = 7
    }
    versioning_enabled = true
    change_feed_enabled = true
    restore_policy {
      days = 7
    }
    container_delete_retention_policy {
      days = 7
    }
  }

  tags = merge(local.tags, {
    Purpose = "TerraformState"
  })

  lifecycle {
    prevent_destroy = true
  }
}

# Blob Container for Terraform State
resource "azurerm_storage_container" "terraform_state" {
  name                  = "tfstate"
  storage_account_name  = azurerm_storage_account.terraform_state.name
  container_access_type = "private"

  lifecycle {
    prevent_destroy = true
  }
}

# App Service Plan
resource "azurerm_service_plan" "wedding_api_asp" {
  name                = "wedding-api-asp"
  resource_group_name = azurerm_resource_group.wedding_api_capability_rg.name
  location            = azurerm_resource_group.wedding_api_capability_rg.location
  os_type            = "Linux"
  sku_name           = "B1"
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

# SQL Server
resource "azurerm_mssql_server" "wedding_sql_server" {
  name                         = "wedding-api-sql-server"
  resource_group_name          = azurerm_resource_group.wedding_api_capability_rg.name
  location                     = azurerm_resource_group.wedding_api_capability_rg.location
  version                      = "12.0"
  administrator_login          = "weddingadmin"
  administrator_login_password = random_password.sql_admin_password.result
  tags                         = local.tags
}

# Key Vault
resource "azurerm_key_vault" "wedding_api_kv" {
  name                        = "wedding-api-kv"
  location                    = azurerm_resource_group.wedding_api_capability_rg.location
  resource_group_name         = azurerm_resource_group.wedding_api_capability_rg.name
  enabled_for_disk_encryption = true
  tenant_id                   = data.azurerm_client_config.current.tenant_id
  soft_delete_retention_days  = 7
  purge_protection_enabled    = false
  sku_name                   = "standard"

  # Access policy for current user
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

  # Access policy for service principal
  access_policy {
    tenant_id = data.azurerm_client_config.current.tenant_id
    object_id = var.service_principal_id

    secret_permissions = [
      "Get",
      "List"
    ]
  }

  tags = local.tags
}

resource "azurerm_key_vault_secret" "sql_admin_password" {
  name         = "sql-admin-password"
  value        = random_password.sql_admin_password.result
  key_vault_id = azurerm_key_vault.wedding_api_kv.id
}

# App Configuration
resource "azurerm_app_configuration" "wedding_api_app_config" {
  name                = "wedding-api-app-config"
  resource_group_name = azurerm_resource_group.wedding_api_capability_rg.name
  location            = azurerm_resource_group.wedding_api_capability_rg.location
  sku                 = "free"
  tags                = local.tags
} 