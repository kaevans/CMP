terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "3.15.0"
    }
  }
}

provider "azurerm" {
  # Configuration options
  features {

  }
}

data "azurerm_client_config" "current" {}

resource "azurerm_resource_group" "default" {
  name     = "CMP"
  location = "South Central US"
}

