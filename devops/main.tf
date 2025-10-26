# Provider configuration
terraform {
  required_version = ">= 1.0"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
  }
}

provider "azurerm" {
  features {}
}

# Variables
variable "resource_group_name" {
  description = "Name of the resource group"
  type        = string
  default     = "rg-church-rota"
}

variable "location" {
  description = "Azure region for resources"
  type        = string
  default     = "UK South"
}

variable "environment" {
  description = "Environment name (dev, staging, prod)"
  type        = string
  default     = "dev"
}

variable "sql_admin_username" {
  description = "SQL Server administrator username"
  type        = string
  default     = "sqladmin"
}

variable "sql_admin_password" {
  description = "SQL Server administrator password"
  type        = string
  sensitive   = true
}

# Resource Group
resource "azurerm_resource_group" "main" {
  name     = var.resource_group_name
  location = var.location

  tags = {
    Environment = var.environment
    Project     = "ChurchRota"
    ManagedBy   = "Terraform"
  }
}

# App Service Plan
resource "azurerm_service_plan" "main" {
  name                = "asp-church-rota-${var.environment}"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  os_type             = "Linux"
  sku_name            = "B1"

  tags = {
    Environment = var.environment
    Project     = "ChurchRota"
  }
}

# App Service
resource "azurerm_linux_web_app" "main" {
  name                = "app-church-rota-${var.environment}"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  service_plan_id     = azurerm_service_plan.main.id

  site_config {
    always_on = false
    
    application_stack {
      dotnet_version = "9.0"
    }

    cors {
      allowed_origins = ["*"]
    }
  }

  app_settings = {
    "ASPNETCORE_ENVIRONMENT" = var.environment
    "ConnectionStrings__DefaultConnection" = "Server=tcp:${azurerm_mssql_server.main.fully_qualified_domain_name},1433;Initial Catalog=${azurerm_mssql_database.main.name};Persist Security Info=False;User ID=${var.sql_admin_username};Password=${var.sql_admin_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }

  https_only = true

  tags = {
    Environment = var.environment
    Project     = "ChurchRota"
  }
}

# SQL Server
resource "azurerm_mssql_server" "main" {
  name                         = "sql-church-rota-${var.environment}"
  resource_group_name          = azurerm_resource_group.main.name
  location                     = azurerm_resource_group.main.location
  version                      = "12.0"
  administrator_login          = var.sql_admin_username
  administrator_login_password = var.sql_admin_password
  minimum_tls_version          = "1.2"

  tags = {
    Environment = var.environment
    Project     = "ChurchRota"
  }
}

# SQL Database
resource "azurerm_mssql_database" "main" {
  name           = "sqldb-church-rota-${var.environment}"
  server_id      = azurerm_mssql_server.main.id
  collation      = "SQL_Latin1_General_CP1_CI_AS"
  license_type   = "LicenseIncluded"
  sku_name       = "Basic"
  zone_redundant = false

  tags = {
    Environment = var.environment
    Project     = "ChurchRota"
  }
}

# Firewall rule to allow Azure services
resource "azurerm_mssql_firewall_rule" "allow_azure_services" {
  name             = "AllowAzureServices"
  server_id        = azurerm_mssql_server.main.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "0.0.0.0"
}

# Optional: Firewall rule to allow access from your IP (uncomment and set your IP)
# resource "azurerm_mssql_firewall_rule" "allow_my_ip" {
#   name             = "AllowMyIP"
#   server_id        = azurerm_mssql_server.main.id
#   start_ip_address = "YOUR_IP_ADDRESS"
#   end_ip_address   = "YOUR_IP_ADDRESS"
# }

# Outputs
output "resource_group_name" {
  value       = azurerm_resource_group.main.name
  description = "Name of the resource group"
}

output "app_service_name" {
  value       = azurerm_linux_web_app.main.name
  description = "Name of the App Service"
}

output "app_service_url" {
  value       = "https://${azurerm_linux_web_app.main.default_hostname}"
  description = "URL of the App Service"
}

output "sql_server_fqdn" {
  value       = azurerm_mssql_server.main.fully_qualified_domain_name
  description = "Fully qualified domain name of the SQL Server"
}

output "sql_database_name" {
  value       = azurerm_mssql_database.main.name
  description = "Name of the SQL Database"
}
