# Church Rota Application

This workspace includes a devcontainer configuration that uses the .NET 9 SDK image.

## Getting Started

### Development Environment Setup
1. In VS Code, open the Command Palette and choose "Dev Containers: Open Folder in Container..."
2. The repository will be mounted into /workspace inside the container.
3. The C# extension (ms-dotnettools.csharp) is preinstalled.

### Database Setup
1. Make sure Docker is installed and running on your system
2. Navigate to the workspace root:
   ```bash
   cd /workspace
   ```
3. Make the database script executable (if not already):
   ```bash
   chmod +x ./scripts/run-sqlserver.sh
   ```
4. Run the SQL Server container:
   ```bash
   ./scripts/run-sqlserver.sh
   ```
5. The script will:
   - Start a SQL Server 2022 container
   - Configure the necessary environment
   - Display the connection string when ready

Default database connection details:
- Server: localhost,1433
- Database: ChurchRota
- User: sa
- Password: YourStrong@Passw0rd
- Container Name: church-rota-sqlserver

```
sqlcmd -S localhost,1433 -U SA -P 'YourStrong@Passw0rd' \
  -Q "CREATE DATABASE ChurchRota"
```

Note: For production environments, make sure to change the default password in the script.

## Technical Notes
- The devcontainer uses the image `mcr.microsoft.com/dotnet/sdk:9.0`
- After the container is created the `postCreateCommand` runs `dotnet --info` to verify the SDK
- SQL Server runs in a separate Docker container for development
