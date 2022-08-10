resource "azurerm_linux_web_app" "default" {
  name                = "web-cmp"
  resource_group_name = azurerm_resource_group.default.name
  location            = azurerm_service_plan.default.location
  service_plan_id     = azurerm_service_plan.default.id

  https_only = true

  site_config {}
}