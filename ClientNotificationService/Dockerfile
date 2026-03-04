# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

# Use a root-level workdir for multi-project build
WORKDIR /src

# Copy only csproj files first to leverage Docker layer caching
# IMPORTANT: This assumes the build context is the repo root
COPY ClientNotificationService/ClientNotificationService.csproj ClientNotificationService/
COPY PointofSaleModels/PointofSaleModels.csproj PointofSaleModels/

# Restore dependencies
RUN dotnet restore ClientNotificationService/ClientNotificationService.csproj

# Copy the remaining source
COPY ClientNotificationService/ ClientNotificationService/
COPY PointofSaleModels/ PointofSaleModels/

# Publish the service
RUN dotnet publish ClientNotificationService/ClientNotificationService.csproj -c Release -o /app/out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl ca-certificates \
    && rm -rf /var/lib/apt/lists/*

RUN useradd -m -d /app -s /usr/sbin/nologin appuser

WORKDIR /app
COPY --from=build /app/out .
RUN chown -R appuser:appuser /app

USER appuser

ENTRYPOINT ["dotnet", "ClientNotificationService.dll"]

HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1
