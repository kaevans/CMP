resource "azurerm_storage_account" "default" {
  name                     = "stcmpfunctions"
  resource_group_name      = azurerm_resource_group.default.name
  location                 = azurerm_resource_group.default.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}