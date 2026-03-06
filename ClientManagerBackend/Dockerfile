# 1. Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files for restore
COPY BC.ClientManagerAPI/BC.ClientManagerAPI.csproj BC.ClientManagerAPI/
COPY BC.ClientManager.BL/BC.ClientManager.BL.csproj BC.ClientManager.BL/
COPY BC.Persistence/BC.Persistence.csproj BC.Persistence/

# Restore dependencies
RUN dotnet restore BC.ClientManagerAPI/BC.ClientManagerAPI.csproj

# Copy all source code
COPY . .

# Publish the API
WORKDIR /src/BC.ClientManagerAPI
RUN dotnet publish BC.ClientManagerAPI.csproj -c Release -o /app/publish /p:UseAppHost=false

# 2. Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Install SQL Server tools (for SQL Server 2019/2022)
RUN apt-get update && \
    apt-get install -y curl gnupg2 && \
    curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add - && \
    curl https://packages.microsoft.com/config/debian/11/prod.list > /etc/apt/sources.list.d/mssql-release.list && \
    apt-get update && \
    ACCEPT_EULA=Y apt-get install -y msodbcsql18 mssql-tools18 && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

# Add SQL tools to PATH (note: tools18 for newer versions)
ENV PATH="$PATH:/opt/mssql-tools18/bin"

# Copy published app
COPY --from=build /app/publish .

# Expose API port
EXPOSE 44345

# Environment variables
ENV ASPNETCORE_URLS=http://+:44345 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_USE_POLLING_FILE_WATCHER=true

# Copy wait-for-sql script
COPY wait-for-sql.sh /wait-for-sql.sh
RUN chmod +x /wait-for-sql.sh

# Start API only after SQL Server is ready
ENTRYPOINT ["/wait-for-sql.sh", "dotnet", "BC.ClientManagerAPI.dll"]