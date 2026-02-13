#!/usr/bin/env bash
set -euo pipefail

# Keep previous behaviour
echo "Running post-create tasks: dotnet --info and ensure mssql-tools installed"

# Show dotnet info
if command -v dotnet >/dev/null 2>&1; then
  dotnet --info || true
else
  echo "dotnet not found in container"
fi

# Install Azure Functions Core Tools if not already installed
if command -v func >/dev/null 2>&1; then
  echo "Azure Functions Core Tools already installed"
  func --version
else
  echo "Installing Azure Functions Core Tools..."
  
  # Install Node.js and npm if not present (required for func tools)
  if ! command -v npm >/dev/null 2>&1; then
    echo "Installing Node.js and npm..."
    apt-get update -y
    apt-get install -y curl
    curl -fsSL https://deb.nodesource.com/setup_20.x | bash -
    apt-get install -y nodejs
  fi
  
  # Install Azure Functions Core Tools via npm
  npm install -g azure-functions-core-tools@4 --unsafe-perm true
  
  if command -v func >/dev/null 2>&1; then
    echo "Azure Functions Core Tools installed successfully:"
    func --version
  else
    echo "Warning: func not found after installation"
  fi
fi

# Install msodbcsql and mssql-tools (sqlcmd) if not already installed
if command -v sqlcmd >/dev/null 2>&1; then
  echo "sqlcmd already installed"
  exit 0
fi

echo "Installing mssql-tools (sqlcmd) and dependencies..."

# Detect distro and pick packages.microsoft.com config
OS_ID="$(. /etc/os-release && echo "$ID")"
OS_VER_ID="$(. /etc/os-release && echo "$VERSION_ID")"

# Use Debian 12 (bookworm) config for the microsoft repo when available
if [ "$OS_ID" = "debian" ] && [[ "$OS_VER_ID" =~ ^12 ]]; then
  MS_SRC_URL="https://packages.microsoft.com/config/debian/12/prod.list"
elif [ "$OS_ID" = "ubuntu" ]; then
  # map ubuntu versions to supported prod.list paths; fallback to 22.04
  MS_SRC_URL="https://packages.microsoft.com/config/ubuntu/22.04/prod.list"
else
  # fallback
  MS_SRC_URL="https://packages.microsoft.com/config/ubuntu/22.04/prod.list"
fi

apt-get update -y
apt-get install -y curl apt-transport-https ca-certificates gnupg

# Add Microsoft package signing key and repo
curl -sSL https://packages.microsoft.com/keys/microsoft.asc | apt-key add -
curl -sSL "$MS_SRC_URL" | tee /etc/apt/sources.list.d/mssql-release.list > /dev/null
apt-get update -y

# Install the ODBC driver and mssql-tools
ACCEPT_EULA=Y DEBIAN_FRONTEND=noninteractive apt-get install -y msodbcsql18 mssql-tools

# Add sqlcmd to PATH for all users
echo 'export PATH="$PATH:/opt/mssql-tools/bin"' > /etc/profile.d/mssql-tools.sh
chmod +x /etc/profile.d/mssql-tools.sh

# Show sqlcmd version
if command -v sqlcmd >/dev/null 2>&1; then
  echo "sqlcmd installed:" $(/opt/mssql-tools/bin/sqlcmd -? 2>&1 | head -n 1)
else
  echo "Warning: sqlcmd not found after installation"
fi

exit 0
