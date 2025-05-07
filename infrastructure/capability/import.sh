#!/bin/bash

# Use subscription ID from pipeline variable
SUBSCRIPTION_ID=$(SUBSCRIPTION_ID)

# Import resource group
echo "Importing resource group..."
terraform import azurerm_resource_group.wedding_api_capability_rg /subscriptions/$SUBSCRIPTION_ID/resourceGroups/wedding-api-capability-rg

# Import storage account
echo "Importing storage account..."
terraform import azurerm_storage_account.terraform_state /subscriptions/$SUBSCRIPTION_ID/resourceGroups/wedding-api-capability-rg/providers/Microsoft.Storage/storageAccounts/weddingapistate

# Import storage container
echo "Importing storage container..."
terraform import azurerm_storage_container.terraform_state https://weddingapistate.blob.core.windows.net/tfstate 