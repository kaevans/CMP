resource "azurerm_resource_group" "tfstate" {
  name     = "CMP-tfstate"
  location = var.location
}