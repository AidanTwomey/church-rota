#!/bin/bash

# Set environment variables
SA_PASSWORD="YourStrong@Passw0rd"
PORT=1433
CONTAINER_NAME="church-rota-sqlserver"

# Check if the container already exists
if [ "$(docker ps -aq -f name=^/${CONTAINER_NAME}$)" ]; then
    echo "Container already exists. Stopping and removing..."
    docker stop ${CONTAINER_NAME}
    docker rm ${CONTAINER_NAME}
fi

# Run SQL Server container
echo "Starting SQL Server container..."
docker run -e "ACCEPT_EULA=Y" \
    -e "MSSQL_SA_PASSWORD=${SA_PASSWORD}" \
    -p ${PORT}:1433 \
    --name ${CONTAINER_NAME} \
    -h ${CONTAINER_NAME} \
    -d mcr.microsoft.com/mssql/server:2022-latest

# Wait for SQL Server to start
echo "Waiting for SQL Server to start..."
sleep 10

# Check if container is running
if [ "$(docker ps -q -f name=^/${CONTAINER_NAME}$)" ]; then
    echo "SQL Server is running!"
    echo "Connection string: Server=localhost,${PORT};Database=ChurchRota;User Id=sa;Password=${SA_PASSWORD};TrustServerCertificate=True"
else
    echo "Failed to start SQL Server container"
    exit 1
fi