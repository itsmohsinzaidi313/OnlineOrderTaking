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
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

ENV TZ=Asia/Karachi

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        curl \
        ca-certificates \
        tzdata \
    && ln -snf /usr/share/zoneinfo/$TZ /etc/localtime \
    && echo $TZ > /etc/timezone \
    && rm -rf /var/lib/apt/lists/*

RUN useradd -m -d /app -s /usr/sbin/nologin appuser

WORKDIR /app
COPY --from=build /app/out .
RUN chown -R appuser:appuser /app

USER appuser

ENTRYPOINT ["dotnet", "GatewayService.dll"]

HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1