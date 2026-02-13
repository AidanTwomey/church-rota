# Provider configuration
terraform {
  required_version = ">= 1.0"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
  }
  
  # Add backend configuration for state file
  backend "azurerm" {
    resource_group_name  = "rg-terraform-state"
    storage_account_name = "sttfstatechurchrota"
    container_name       = "tfstate"
    key                  = "church-rota.tfstate"
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
  default     = "West Europe"
}

variable "environment" {
  description = "Environment name (dev, staging, prod)"
  type        = string
  default     = "dev"
}

variable "storage_account_name" {
  description = "Name of the storage account (must be globally unique, 3-24 lowercase letters/numbers)"
  type        = string
  default     = "stchurchrota"
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

# Storage Account
resource "azurerm_storage_account" "main" {
  name                     = var.storage_account_name
  resource_group_name      = azurerm_resource_group.main.name
  location                 = azurerm_resource_group.main.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
  account_kind             = "StorageV2"
  
  min_tls_version           = "TLS1_2"
  https_traffic_only_enabled = true

  tags = {
    Environment = var.environment
    Project     = "ChurchRota"
  }
}

# Storage Tables for Rota Data
resource "azurerm_storage_table" "schedules" {
  name                 = "Schedules"
  storage_account_name = azurerm_storage_account.main.name
}

resource "azurerm_storage_table" "people" {
  name                 = "People"
  storage_account_name = azurerm_storage_account.main.name
}

resource "azurerm_storage_table" "availability" {
  name                 = "Availability"
  storage_account_name = azurerm_storage_account.main.name
}

# App Service Plan for Functions (Consumption/Serverless)
resource "azurerm_service_plan" "function" {
  name                = "asp-church-rota-function-${var.environment}"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  os_type             = "Linux"
  sku_name            = "Y1"  # Consumption plan (serverless/pay-per-use)

  tags = {
    Environment = var.environment
    Project     = "ChurchRota"
  }
}

# Azure Function App
resource "azurerm_linux_function_app" "swap" {
  name                       = "func-church-rota-swap-${var.environment}"
  resource_group_name        = azurerm_resource_group.main.name
  location                   = azurerm_resource_group.main.location
  service_plan_id            = azurerm_service_plan.function.id
  storage_account_name       = azurerm_storage_account.main.name
  storage_account_access_key = azurerm_storage_account.main.primary_access_key

  site_config {
    application_stack {
      dotnet_version              = "8.0"
      use_dotnet_isolated_runtime = true
    }

    cors {
      allowed_origins = ["*"]  # Update with your domain in production
    }
  }

  app_settings = {
    "FUNCTIONS_WORKER_RUNTIME"       = "dotnet-isolated"
    "AzureWebJobsStorage"            = azurerm_storage_account.main.primary_connection_string
    "WEBSITE_CONTENTAZUREFILECONNECTIONSTRING" = azurerm_storage_account.main.primary_connection_string
    "WEBSITE_CONTENTSHARE"           = "func-church-rota-swap-${var.environment}"
    "TableStorageConnectionString"   = azurerm_storage_account.main.primary_connection_string
    "ASPNETCORE_ENVIRONMENT"         = var.environment
  }

  https_only = true

  tags = {
    Environment = var.environment
    Project     = "ChurchRota"
  }
}

# Outputs
output "resource_group_name" {
  value       = azurerm_resource_group.main.name
  description = "Name of the resource group"
}

output "storage_account_name" {
  value       = azurerm_storage_account.main.name
  description = "Name of the storage account"
}

output "storage_account_primary_access_key" {
  value       = azurerm_storage_account.main.primary_access_key
  description = "Primary access key for the storage account"
  sensitive   = true
}

output "storage_account_primary_connection_string" {
  value       = azurerm_storage_account.main.primary_connection_string
  description = "Primary connection string for the storage account"
  sensitive   = true
}

output "function_app_name" {
  value       = azurerm_linux_function_app.swap.name
  description = "Name of the Function App"
}

output "function_app_url" {
  value       = "https://${azurerm_linux_function_app.swap.default_hostname}"
  description = "URL of the Function App"
}

output "function_app_id" {
  value       = azurerm_linux_function_app.swap.id
  description = "ID of the Function App"
}
