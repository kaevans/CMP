#!/bin/bash

RESOURCE_GROUP_NAME=CMP
STORAGE_ACCOUNT_NAME=stcmptfstate
CONTAINER_NAME=tfstate
LOCATION=southcentralus

# Create resource group
az group create --name $RESOURCE_GROUP_NAME --location $LOCATION

# Create storage account
az storage account create --resource-group $RESOURCE_GROUP_NAME --name $STORAGE_ACCOUNT_NAME --sku Standard_LRS --encryption-services blob

# Create blob container
az storage container create --name $CONTAINER_NAME --account-name $STORAGE_ACCOUNT_NAME