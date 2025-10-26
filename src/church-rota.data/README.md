# church-rota.data SQL Database Project

This project contains the database model for the ChurchRota application. It defines the initial schema (People, Roles, Schedules).

How to use

Option A — Publish raw SQL files using sqlcmd (quick, no dependencies)

1. Make sure SQL Server is reachable from where you run the command (host or container).
2. Run the create script(s) or the model files in order. Example using `sqlcmd`:

```bash
sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Passw0rd' -i scripts/sql/create-database.sql
sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Passw0rd' -i src/church-rota.data/Model/People.sql
sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Passw0rd' -i src/church-rota.data/Model/Roles.sql
sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Passw0rd' -i src/church-rota.data/Model/Schedules.sql
```

Option B — Build & publish a DACPAC (recommended for CI/CD)

- Building the `.sqlproj` produces a `.dacpac` which can be deployed with `SqlPackage` or `sqlpackage`.
- Note: building a `sqlproj` in CI may require the MSBuild targets for SQL projects. Many CI images already include the required SDKs, or you can use a Microsoft-provided image.

Example publish with SqlPackage (after you have the .dacpac):

```bash
# build the sqlproj (requires MSBuild support for SQL projects)
dotnet build src/church-rota.data/church-rota.data.sqlproj -c Release

# publish with sqlpackage
sqlpackage /Action:Publish /SourceFile:bin/Release/church-rota.data.dacpac /TargetServerName:localhost,1433 /TargetUser:sa /TargetPassword:YourStrong@Passw0rd /TargetDatabaseName:ChurchRota
```

CI suggestion

- In CI pipelines, prefer producing a DACPAC and deploying it with `SqlPackage` to get idempotent deployments. Alternatively, run the individual .sql files via `sqlcmd` in order.

Troubleshooting

- If the SQL Server process fails on start, check the container logs for SA password policy errors or missing ACCEPT_EULA.
- If the connection fails from inside a devcontainer, use `host.docker.internal` or run both services in the same Docker network.
