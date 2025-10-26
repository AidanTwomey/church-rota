#!/bin/bash

# Set environment variables
SA_PASSWORD="YourStrong@Passw0rd"
SERVER="host.docker.internal"
PORT=1433
DATABASE="ChurchRota"

# Wait for SQL Server to be ready
echo "Waiting for SQL Server to be ready..."
for i in {1..60}; do
    if /opt/mssql-tools/bin/sqlcmd -S $SERVER,$PORT -U sa -P $SA_PASSWORD -Q "SELECT 1" &>/dev/null; then
        echo "SQL Server is ready!"
        break
    fi
    echo "Waiting... ($i/60)"
    sleep 1
done

if [ $i -eq 60 ]; then
    echo "Error: SQL Server did not become ready in time"
    exit 1
fi

# Create the database and schema
echo "Creating database and schema..."
/opt/mssql-tools/bin/sqlcmd \
    -S $SERVER,$PORT \
    -U sa \
    -P $SA_PASSWORD \
    -i /workspace/scripts/sql/create-database.sql

if [ $? -eq 0 ]; then
    echo "Database creation successful!"
    echo "Connection string: Server=$SERVER,$PORT;Database=$DATABASE;User Id=sa;Password=$SA_PASSWORD;TrustServerCertificate=True"
else
    echo "Error: Database creation failed"
    exit 1
fi