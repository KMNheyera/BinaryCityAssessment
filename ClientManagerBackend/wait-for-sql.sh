#!/bin/bash
set -e

# First argument is the command to run after SQL is ready
cmd="$@"

echo "Waiting for SQL Server at sqlserver-db:1433..."

# Maximum number of retries
max_retries=30
counter=0

# Function to check SQL Server readiness
check_sql() {
  /opt/mssql-tools18/bin/sqlcmd -S sqlserver-db -U sa -P Interit_123 -Q "SELECT 1" -C -N > /dev/null 2>&1
  return $?
}

# Retry until SQL Server responds
until check_sql; do
  counter=$((counter + 1))
  if [ $counter -ge $max_retries ]; then
    echo "SQL Server not ready after $max_retries attempts. Exiting."
    exit 1
  fi
  echo "SQL Server not ready yet (attempt $counter/$max_retries), retrying in 2s..."
  sleep 2
done

echo "SQL Server is ready. Starting API..."
exec $cmd