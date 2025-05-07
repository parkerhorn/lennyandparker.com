variable "location" {
  description = "The Azure region where resources will be created"
  type        = string
  default     = "eastus"
}

variable "service_principal_id" {
  description = "The ID of the service principal that will be used to deploy the infrastructure"
  type        = string
}

variable "subscription_id" {
  description = "The ID of the Azure subscription"
  type        = string
}

variable "tenant_id" {
  description = "The ID of the Azure tenant"
  type        = string
} 