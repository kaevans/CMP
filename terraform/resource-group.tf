resource "azurerm_resource_group" "default" {
  name     = "CMP"
  location = var.location
}