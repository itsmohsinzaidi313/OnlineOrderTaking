# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy everything and restore
COPY . .
RUN dotnet publish GetMenuService.csproj -c Release -o out

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

# Health check - use the actual health endpoint
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD curl -fsS http://localhost:8080/api/health || exit 1

ENTRYPOINT ["dotnet", "GetMenuService.dll"]
