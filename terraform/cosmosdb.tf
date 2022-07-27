resource "azurerm_cosmosdb_account" "default" {
  name                = "cmpcosmosdb"
  resource_group_name = azurerm_resource_group.default.name
  location            = azurerm_resource_group.default.location

  offer_type = "Standard"
  kind       = "GlobalDocumentDB"

  enable_automatic_failover = true

  capabilities {
    name = "EnableServerless"
  }

  consistency_policy {
    consistency_level       = "Eventual"
    max_interval_in_seconds = 5
    max_staleness_prefix    = 100000
  }

  geo_location {
    location          = azurerm_resource_group.default.location
    failover_priority = 0
  }
}

resource "azurerm_cosmosdb_sql_database" "default" {
  name                = "cmp"
  account_name        = azurerm_cosmosdb_account.default.name
  resource_group_name = azurerm_cosmosdb_account.default.resource_group_name
}

resource "azurerm_cosmosdb_sql_container" "default" {
  name                = "DeploymentTemplates"
  account_name        = azurerm_cosmosdb_account.default.name
  resource_group_name = azurerm_cosmosdb_account.default.resource_group_name
  database_name       = azurerm_cosmosdb_sql_database.default.name

  partition_key_path = "/id"

  indexing_policy {

    indexing_mode = "consistent"

    included_path {
      path = "/*"
    }
  }
}