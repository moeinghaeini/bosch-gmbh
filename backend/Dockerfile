# Use the official .NET 8 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the official .NET 8 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["IndustrialAutomation.API/IndustrialAutomation.API.csproj", "IndustrialAutomation.API/"]
COPY ["IndustrialAutomation.Core/IndustrialAutomation.Core.csproj", "IndustrialAutomation.Core/"]
COPY ["IndustrialAutomation.Infrastructure/IndustrialAutomation.Infrastructure.csproj", "IndustrialAutomation.Infrastructure/"]
COPY ["IndustrialAutomation.Tests/IndustrialAutomation.Tests.csproj", "IndustrialAutomation.Tests/"]

# Restore dependencies
RUN dotnet restore "IndustrialAutomation.API/IndustrialAutomation.API.csproj"

# Copy all source code
COPY . .

# Build the application
WORKDIR "/src/IndustrialAutomation.API"
RUN dotnet build "IndustrialAutomation.API.csproj" -c Release --no-restore

# Publish the application
FROM build AS publish
WORKDIR "/src/IndustrialAutomation.API"
RUN dotnet publish "IndustrialAutomation.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published application
COPY --from=publish /app/publish .

# Create logs directory
RUN mkdir -p /app/logs

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=5 \
  CMD curl -f http://localhost/health || exit 1

# Start the application
ENTRYPOINT ["dotnet", "IndustrialAutomation.API.dll"]
