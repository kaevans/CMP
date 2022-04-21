# Cloud Messaging Project (CMP)

This project demonstrates how to build a solution similar to the [Azure Quickstart Templates](https://azure.microsoft.com/en-us/resources/templates/) site
using native Azure technologies. 

The CMP.Functions project demonstrates how to build an Azure Function using C# that reads all repositories 
from an Azure DevOps project, reads the items from each repository to find files named `metadata.json`, 
and stores metadata about each repo in an Azure Cosmos Database. 

The CMP.Web project then points to the Azure Cosmos Database to provide a view of the data similar to 
what is shown on the [Azure Quickstart Templates](https://azure.microsoft.com/en-us/resources/templates/) site.

The code is done as a quick proof of concept to demonstrate an approach for a company to publish a list
of approved deployment templates and how to surface them to users.

The project leverages dependency injection with .NET 6.0.

- CMP.Core - models and interfaces used in multiple projects
- CMP.Deployment - sample project for ARM template deployment
- CMP.Functions - Azure Function project
- CMP.Functions.Integration.Tests - MSTest project for integration testing
- CMP.Functions.Tests - Unit tests for the CMP.Functions project
- CMP.Functions.Tests.Core - core code shared across multiple testing projects
- CMP.Infrastructure - middleware infrastructure for IOptions, repository and factory services
- CMP.Infrastructure.Tests.Unit - Unit tests for the CMP.Infrastructure project
- CMP.Web - Web project to visualize the contents of the Cosmos Database.
