variable "environment" {
  description = "Environment name"
  type        = string
}

variable "location" {
  description = "Azure region for resources"
  type        = string
  default     = "eastus"
}

variable "app_settings" {
  description = "Application settings for the web app"
  type        = map(string)
  default     = {}
} 