# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Use a root-level workdir for multi-project build
WORKDIR /src

# Copy only csproj files first to leverage Docker layer caching
# IMPORTANT: This assumes the build context is the repo root
COPY JwtTokenService/JwtTokenService.csproj JwtTokenService/
COPY PointofSaleModels/PointofSaleModels.csproj PointofSaleModels/

# Restore dependencies
RUN dotnet restore JwtTokenService/JwtTokenService.csproj

# Copy the remaining source
COPY JwtTokenService/ JwtTokenService/
COPY PointofSaleModels/ PointofSaleModels/

# Publish the service
RUN dotnet publish JwtTokenService/JwtTokenService.csproj -c Release -o /app/out

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

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "JwtTokenService.dll"]
