resource "azurerm_service_plan" "default" {
  name                = "plan-cmp"
  location            = azurerm_resource_group.default.location
  resource_group_name = azurerm_resource_group.default.name
  os_type             = "Linux"
  sku_name            = "S1"
}