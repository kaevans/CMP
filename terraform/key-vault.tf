resource "azurerm_key_vault" "default" {
  name                = "cmpdeploy"
  resource_group_name = azurerm_resource_group.default.name
  location            = azurerm_resource_group.default.location

  enabled_for_disk_encryption = true
  tenant_id                   = data.azurerm_client_config.current.tenant_id

  sku_name = "standard"

}