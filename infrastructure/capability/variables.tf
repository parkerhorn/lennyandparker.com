variable "location" {
  description = "The Azure region where resources will be created"
  type        = string
  default     = "eastus"
}

variable "service_principal_id" {
  description = "The ID of the service principal that will be used to deploy the infrastructure"
  type        = string
} 