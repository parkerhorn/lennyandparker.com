#!/bin/bash

# Get values from command line arguments
SUBSCRIPTION_ID=$1
TENANT_ID=$2
ACCESS_TOKEN=$3

# Debug logging
echo "Debug: Arguments received:"
echo "Debug: SUBSCRIPTION_ID: $SUBSCRIPTION_ID"
echo "Debug: TENANT_ID: $TENANT_ID"
echo "Debug: ACCESS_TOKEN: ***"

# Set environment variables for Terraform
export ARM_ACCESS_TOKEN=$ACCESS_TOKEN
export ARM_TENANT_ID=$TENANT_ID
export ARM_SUBSCRIPTION_ID=$SUBSCRIPTION_ID
export ARM_USE_OIDC=true
export ARM_USE_CLI=false

# Debug logging
echo "Debug: Environment variables set:"
env | grep ARM_

# Import resource group
echo "Importing resource group..."
terraform import azurerm_resource_group.wedding_api_capability_rg /subscriptions/$SUBSCRIPTION_ID/resourceGroups/wedding-api-capability-rg

# Import storage account
echo "Importing storage account..."
terraform import azurerm_storage_account.terraform_state /subscriptions/$SUBSCRIPTION_ID/resourceGroups/wedding-api-capability-rg/providers/Microsoft.Storage/storageAccounts/weddingapistate

# Import storage container
echo "Importing storage container..."
terraform import azurerm_storage_container.terraform_state https://weddingapistate.blob.core.windows.net/tfstate 