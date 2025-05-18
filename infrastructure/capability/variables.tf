variable "location" {
  description = "The Azure region where resources will be created"
  type        = string
  default     = "centralus"
}

variable "service_principal_id" {
  description = "The ID of the service principal that will be used to deploy the infrastructure"
  type        = string
}

variable "admin_object_id" {
  description = "The Object ID of the admin account to grant Key Vault access"
  type        = string
} 