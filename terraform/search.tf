resource "azurerm_search_service" "default" {
  name                = "srch-cmp"
  resource_group_name = azurerm_resource_group.default.name
  location            = azurerm_resource_group.default.location
  sku                 = "basic"

  replica_count   = 1
  partition_count = 1

}