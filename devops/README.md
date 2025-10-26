# Church Rota - Azure Infrastructure

This directory contains Terraform configuration to provision Azure infrastructure for the Church Rota application.

## Resources Created

- **Resource Group**: Container for all resources
- **App Service Plan**: Linux-based B1 tier (Basic)
- **App Service**: .NET 9.0 web application
- **SQL Server**: Azure SQL Server (v12.0)
- **SQL Database**: Basic tier database
- **Firewall Rules**: Allow Azure services to access SQL Server

## Prerequisites

1. [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli) installed
2. [Terraform](https://www.terraform.io/downloads.html) >= 1.0 installed
3. Azure subscription

## Setup

1. Login to Azure:
   ```bash
   az login
   ```

2. Set your subscription (if you have multiple):
   ```bash
   az account set --subscription "Your Subscription Name"
   ```

3. Create a `terraform.tfvars` file from the example:
   ```bash
   cp terraform.tfvars.example terraform.tfvars
   ```

4. Edit `terraform.tfvars` and set your values, especially the SQL admin password

## Usage

### Initialize Terraform
```bash
cd devops
terraform init
```

### Plan the deployment
```bash
terraform plan
```

### Apply the configuration
```bash
terraform apply
```

### Destroy resources (when needed)
```bash
terraform destroy
```

## Setting SQL Admin Password Securely

Instead of storing the password in `terraform.tfvars`, you can:

### Option 1: Environment Variable
```bash
export TF_VAR_sql_admin_password="YourSecurePassword123!"
terraform apply
```

### Option 2: Command Line
```bash
terraform apply -var="sql_admin_password=YourSecurePassword123!"
```

### Option 3: Azure Key Vault (recommended for production)
Store the password in Azure Key Vault and reference it in your Terraform code.

## Outputs

After applying, Terraform will output:
- Resource group name
- App Service name and URL
- SQL Server FQDN
- SQL Database name

## Cost Considerations

The default configuration uses:
- **App Service Plan**: B1 (Basic) - ~£40/month
- **SQL Database**: Basic tier - ~£4/month

Total estimated cost: ~£44/month

For production, consider upgrading to:
- App Service Plan: S1 (Standard) or P1V2 (Premium)
- SQL Database: S0 (Standard) or higher

## Customization

Edit `main.tf` to customize:
- SKU tiers (change `sku_name` values)
- Azure region (change `location` variable)
- Environment name (dev, staging, prod)
- Resource naming conventions

## Next Steps

After provisioning:
1. Deploy your application to the App Service
2. Run EF migrations against the Azure SQL Database
3. Configure custom domains and SSL certificates (optional)
4. Set up Application Insights for monitoring (optional)
