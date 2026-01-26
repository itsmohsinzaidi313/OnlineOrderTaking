# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

# Use a root-level workdir for multi-project build
WORKDIR /src

# Copy only csproj files first to leverage Docker layer caching
# IMPORTANT: This assumes the build context is the repo root
COPY GatewayService/GatewayService.csproj GatewayService/
COPY PointofSaleModels/PointofSaleModels.csproj PointofSaleModels/

# Restore dependencies
RUN dotnet restore GatewayService/GatewayService.csproj

# Copy the remaining source
COPY GatewayService/ GatewayService/
COPY PointofSaleModels/ PointofSaleModels/

# Publish the service
RUN dotnet publish GatewayService/GatewayService.csproj -c Release -o /app/out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Install curl for healthcheck and create non-root user for security
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl ca-certificates

RUN rm -rf /var/lib/apt/lists/*
RUN adduser --disabled-password --home /app --gecos '' appuser
RUN chown -R appuser /app
USER appuser

WORKDIR /app
RUN rm -rf /app/out/appsettings.*.json
COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "GatewayService.dll"]

# Healthcheck to ensure the service is running
HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1
