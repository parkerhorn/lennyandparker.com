variable "environment" {
  description = "Environment name"
  type        = string
}

variable "location" {
  description = "Azure region for resources"
  type        = string
  default     = "centralus"
}

variable "app_settings" {
  description = "Application settings for the web app"
  type        = map(string)
  default     = {}
}

variable "service_principal_id" {
  description = "The ID of the service principal that will be used to deploy the infrastructure"
  type        = string
}

variable "admin_object_id" {
  description = "The Object ID of the admin account to grant Key Vault access"
  type        = string
}

variable "alert_emails" {
  description = "List of email addresses to receive RSVP alerts"
  type        = list(string)
  default     = []
} 