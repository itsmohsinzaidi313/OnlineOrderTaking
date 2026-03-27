# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

# Use a root-level workdir for multi-project build
WORKDIR /src

# Copy only csproj files first to leverage Docker layer caching
# IMPORTANT: This assumes the build context is the repo root
COPY GeneralSeoDataService/GeneralSeoDataService.csproj GeneralSeoDataService/
COPY PointofSaleModels/PointofSaleModels.csproj PointofSaleModels/

# Restore dependencies
RUN dotnet restore GeneralSeoDataService/GeneralSeoDataService.csproj

# Copy the remaining source
COPY GeneralSeoDataService/ GeneralSeoDataService/
COPY PointofSaleModels/ PointofSaleModels/

# Publish the service
RUN dotnet publish GeneralSeoDataService/GeneralSeoDataService.csproj -c Release -o /app/out -v minimal

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

ENTRYPOINT ["dotnet", "GeneralSeoDataService.dll"]

HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1
