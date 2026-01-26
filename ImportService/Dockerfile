# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Use a root-level workdir for multi-project build
WORKDIR /src

# Copy only csproj files first to leverage Docker layer caching
# IMPORTANT: This assumes the build context is the repo root
COPY ImportService/ImportService.csproj ImportService/
COPY PointofSaleModels/PointofSaleModels.csproj PointofSaleModels/

# Restore dependencies
RUN dotnet restore ImportService/ImportService.csproj

# Copy the remaining source
COPY ImportService/ ImportService/
COPY PointofSaleModels/ PointofSaleModels/

# Publish the service
RUN dotnet publish ImportService/ImportService.csproj -c Release -o /app/out -v minimal

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
COPY --from=build /app/out .

# Health check - use the actual health endpoint
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD curl -fsS http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "ImportService.dll"]
