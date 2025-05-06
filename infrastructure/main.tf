terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
  }
  backend "azurerm" {
    resource_group_name  = "wedding-api-terraform-state"
    storage_account_name = "weddingapistate"
    container_name      = "tfstate"
    key                 = "terraform.tfstate"
  }
}

provider "azurerm" {
  features {}
}

variable "app_settings" {
  description = "Application settings for the web app"
  type = map(string)
  default = {
    "ASPNETCORE_ENVIRONMENT" = "Development"
    "EnableSwagger" = "true"
    "MaxAttendeesPerInvitation" = "5"
  }
}

# Locals
locals {
  environment = var.environment
  location    = var.location
  tags = {
    Environment = var.environment
    Project     = "WeddingAPI"
    ManagedBy   = "Terraform"
  }
}

# Resource Group
resource "azurerm_resource_group" "main" {
  name     = "wedding-api-${local.environment}"
  location = local.location
  tags     = local.tags
}

# App Service Plan
resource "azurerm_service_plan" "main" {
  name                = "wedding-api-plan-${local.environment}"
  resource_group_name = azurerm_resource_group.main.name
  location           = azurerm_resource_group.main.location
  os_type            = "Windows"
  sku_name           = var.app_service_plan_sku
  tags               = local.tags
}

# SQL Server
resource "azurerm_mssql_server" "main" {
  name                         = "wedding-api-sql-${local.environment}"
  resource_group_name          = azurerm_resource_group.main.name
  location                     = azurerm_resource_group.main.location
  version                      = "12.0"
  administrator_login          = "weddingadmin"
  administrator_login_password = var.sql_server_admin_password
  tags                         = local.tags

  azuread_administrator {
    login_username = "AzureAD Admin"
    object_id      = "00000000-0000-0000-0000-000000000000" # Replace with your Azure AD admin object ID
  }
}

# SQL Database
resource "azurerm_mssql_database" "main" {
  name           = "WeddingAPI"
  server_id      = azurerm_mssql_server.main.id
  collation      = "SQL_Latin1_General_CP1_CI_AS"
  license_type   = "LicenseIncluded"
  max_size_gb    = 2
  sku_name       = "Basic"
  tags           = local.tags
}

# SQL Firewall Rule
resource "azurerm_mssql_firewall_rule" "main" {
  name             = "AllowAzureServices"
  server_id        = azurerm_mssql_server.main.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "0.0.0.0"
}

# Web App
resource "azurerm_windows_web_app" "main" {
  name                = "wedding-api-${local.environment}"
  resource_group_name = azurerm_resource_group.main.name
  location           = azurerm_resource_group.main.location
  service_plan_id    = azurerm_service_plan.main.id
  tags               = local.tags

  site_config {
    application_stack {
      dotnet_version = "v8.0"
    }
    always_on = true
  }

  app_settings = merge(var.app_settings, {
    "ConnectionStrings__DefaultConnection" = "Server=${azurerm_mssql_server.main.fully_qualified_domain_name};Database=WeddingAPI;User Id=${azurerm_mssql_server.main.administrator_login};Password=${var.sql_server_admin_password};TrustServerCertificate=True"
  })
} 