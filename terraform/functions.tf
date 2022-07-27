

resource "azurerm_linux_function_app" "default" {
  name                = "func-cmp"
  location            = azurerm_resource_group.default.location
  resource_group_name = azurerm_resource_group.default.name

  storage_account_name = azurerm_storage_account.default.name
  service_plan_id      = azurerm_service_plan.default.id

  site_config {}

}