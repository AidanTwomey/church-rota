# GitHub Actions Setup for Azure Terraform Deployment

This guide explains how to configure GitHub Actions to deploy Azure infrastructure using Terraform.

## Prerequisites

1. Azure subscription
2. GitHub repository with Terraform code in `/devops` directory

## Setup Instructions

### 1. Create Azure Service Principal

Run these commands in Azure CLI:

```bash
# Login to Azure
az login

# Set your subscription (if you have multiple)
az account set --subscription "Your Subscription Name"

# Get your subscription ID
SUBSCRIPTION_ID=$(az account show --query id -o tsv)

# Create a service principal with Contributor role
az ad sp create-for-rbac \
  --name "github-actions-church-rota" \
  --role contributor \
  --scopes /subscriptions/$SUBSCRIPTION_ID \
  --sdk-auth
```

The output will look like this:
```json
{
  "clientId": "00000000-0000-0000-0000-000000000000",
  "clientSecret": "YOUR_SECRET",
  "subscriptionId": "00000000-0000-0000-0000-000000000000",
  "tenantId": "00000000-0000-0000-0000-000000000000",
  "activeDirectoryEndpointUrl": "https://login.microsoftonline.com",
  "resourceManagerEndpointUrl": "https://management.azure.com/",
  "activeDirectoryGraphResourceId": "https://graph.windows.net/",
  "sqlManagementEndpointUrl": "https://management.core.windows.net:8443/",
  "galleryEndpointUrl": "https://gallery.azure.com/",
  "managementEndpointUrl": "https://management.core.windows.net/"
}
```

**IMPORTANT:** Save this entire JSON output - you'll need it in the next step.

### 2. Configure GitHub Secrets

Go to your GitHub repository:
1. Click **Settings** → **Secrets and variables** → **Actions**
2. Click **New repository secret**

Create these secrets:

#### AZURE_CREDENTIALS
- **Name**: `AZURE_CREDENTIALS`
- **Value**: Paste the entire JSON output from the service principal creation

#### SQL_ADMIN_PASSWORD
- **Name**: `SQL_ADMIN_PASSWORD`
- **Value**: Your SQL Server admin password (e.g., `YourSecure@Password123`)

### 3. Optional: Configure Environment Protection Rules

For production safety:
1. Go to **Settings** → **Environments**
2. Click **New environment** and name it `production`
3. Add protection rules:
   - ✅ Required reviewers (add team members)
   - ✅ Wait timer (e.g., 5 minutes)
   - ✅ Deployment branches (only allow main/master)

## Workflow Triggers

The workflow runs in these scenarios:

### 1. Automatic Plan on Push
- **Trigger**: Push to `master` or `main` branch
- **Action**: Runs `terraform plan`
- **Use case**: Review infrastructure changes before manual apply

### 2. Manual Actions
- **Trigger**: Manual workflow dispatch from GitHub Actions tab
- **Actions available**:
  - **Plan**: Preview changes without applying
  - **Apply**: Deploy infrastructure changes
  - **Destroy**: Tear down all infrastructure

### How to Trigger Manual Actions:
1. Go to **Actions** tab in GitHub
2. Click **Deploy Azure Infrastructure** workflow
3. Click **Run workflow**
4. Select action (plan/apply/destroy)
5. Click **Run workflow**

## Terraform State Management

⚠️ **IMPORTANT**: This workflow uses local state. For production, you should use remote state storage.

### Configure Remote State (Recommended)

1. Create an Azure Storage Account for Terraform state:

```bash
# Variables
RESOURCE_GROUP_NAME="rg-terraform-state"
STORAGE_ACCOUNT_NAME="sttfstate$(cat /dev/urandom | tr -dc 'a-z0-9' | fold -w 8 | head -n 1)"
CONTAINER_NAME="tfstate"
LOCATION="UK South"

# Create resource group
az group create --name $RESOURCE_GROUP_NAME --location "$LOCATION"

# Create storage account
az storage account create \
  --resource-group $RESOURCE_GROUP_NAME \
  --name $STORAGE_ACCOUNT_NAME \
  --sku Standard_LRS \
  --encryption-services blob

# Get storage account key
ACCOUNT_KEY=$(az storage account keys list --resource-group $RESOURCE_GROUP_NAME --account-name $STORAGE_ACCOUNT_NAME --query '[0].value' -o tsv)

# Create blob container
az storage container create \
  --name $CONTAINER_NAME \
  --account-name $STORAGE_ACCOUNT_NAME \
  --account-key $ACCOUNT_KEY

echo "Storage account name: $STORAGE_ACCOUNT_NAME"
```

2. Update `/devops/main.tf` backend configuration:

```hcl
terraform {
  required_version = ">= 1.0"
  
  backend "azurerm" {
    resource_group_name  = "rg-terraform-state"
    storage_account_name = "YOUR_STORAGE_ACCOUNT_NAME"
    container_name       = "tfstate"
    key                  = "church-rota.tfstate"
  }
  
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
  }
}
```

3. Add these GitHub secrets:
   - `ARM_ACCESS_KEY`: Storage account access key

## Workflow Features

- ✅ Terraform format validation
- ✅ Automatic initialization
- ✅ Validation before plan/apply
- ✅ Plan-only execution on push
- ✅ Manual apply/destroy with approval
- ✅ Pull request comments with plan output
- ✅ Secure credential handling

## Security Best Practices

1. **Never commit secrets** to the repository
2. **Use environment protection rules** for production
3. **Require PR reviews** before merging infrastructure changes
4. **Enable branch protection** on master/main
5. **Rotate service principal credentials** regularly
6. **Use Azure Key Vault** for sensitive values in production

## Troubleshooting

### Authentication Failed
- Verify `AZURE_CREDENTIALS` secret is correct
- Check service principal has Contributor role
- Ensure subscription is active

### Terraform Init Failed
- Check Terraform version compatibility
- Verify backend configuration (if using remote state)

### Plan/Apply Failed
- Review Terraform error messages in workflow logs
- Check Azure resource quotas
- Verify SQL password meets complexity requirements

## Example: Full Deployment Flow

1. **Make changes** to `/devops/main.tf`
2. **Commit and push** to a feature branch
3. **Create pull request** → GitHub runs `terraform plan`
4. **Review plan** in PR comments
5. **Merge to master** → GitHub runs `terraform plan` again
6. **Manually trigger apply**:
   - Go to Actions → Deploy Azure Infrastructure
   - Run workflow → Select "apply"
   - (If protected) Approve the deployment
7. **Infrastructure deployed** ✅

## Cost Management

Monitor your Azure costs:
```bash
# View current costs
az consumption usage list --start-date 2025-10-01 --end-date 2025-10-31
```

Remember to destroy dev/test environments when not in use:
- Run workflow with "destroy" action
- Or run locally: `cd devops && terraform destroy`
