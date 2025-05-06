variable "environment" {
  description = "Environment (dev or prod)"
  type        = string
  validation {
    condition     = contains(["dev", "prod"], var.environment)
    error_message = "Environment must be either 'dev' or 'prod'."
  }
}

variable "location" {
  description = "Azure region for resources"
  type        = string
  default     = "eastus"
}

variable "app_service_plan_sku" {
  description = "SKU for App Service Plan"
  type        = string
  default     = "B1"
}

variable "sql_server_admin_password" {
  description = "SQL Server admin password"
  type        = string
  sensitive   = true
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