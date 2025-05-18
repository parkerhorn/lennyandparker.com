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


resource "random_password" "sql_admin_password" {
  length           = 32
  special          = true
  override_special = "!@#$%"
  min_lower        = 1
  min_upper        = 1
  min_numeric      = 1
  min_special      = 1
}

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

# Add firewall rule to allow Azure services
resource "azurerm_mssql_firewall_rule" "allow_azure_services" {
  name             = "AllowAzureServices"
  server_id        = azurerm_mssql_server.wedding_sql_server.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "0.0.0.0"
}

resource "azurerm_key_vault" "wedding_api_kv" {
  name                        = "lennyandparkerweddingkv"
  location                    = data.azurerm_resource_group.wedding_api_capability_rg.location
  resource_group_name         = data.azurerm_resource_group.wedding_api_capability_rg.name
  enabled_for_disk_encryption = true
  tenant_id                   = data.azurerm_client_config.current.tenant_id
  soft_delete_retention_days  = 7
  purge_protection_enabled    = false
  sku_name                   = "standard"
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
      "List",
      "Set",
      "Delete",
      "Recover",
      "Backup",
      "Restore",
      "Purge"
    ]
  }

  access_policy {
    tenant_id = data.azurerm_client_config.current.tenant_id
    object_id = var.admin_object_id

    secret_permissions = [
      "Get",
      "List",
      "Set",
      "Delete",
      "Recover",
      "Backup",
      "Restore",
      "Purge"
    ]
    
    key_permissions = [
      "Get",
      "List",
      "Create",
      "Delete"
    ]
    
    certificate_permissions = [
      "Get",
      "List",
      "Create",
      "Delete"
    ]
  }
}

output "sql_admin_password" {
  value     = random_password.sql_admin_password.result
  sensitive = true
}