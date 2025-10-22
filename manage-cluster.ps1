# Patroni Cluster Management Script

param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("start", "stop", "status", "reset", "init-db", "logs", "test")]
    [string]$Action
)

function Show-Status {
    Write-Host "=== Container Status ===" -ForegroundColor Green
    docker-compose ps
    
    Write-Host "`n=== Patroni Cluster Status ===" -ForegroundColor Green
    try {
        docker exec patroni1 patronictl -c /etc/patroni/patroni.yml list
    }
    catch {
        Write-Host "Could not get Patroni status. Cluster may still be starting..." -ForegroundColor Yellow
    }
    
    Write-Host "`n=== HAProxy Stats ===" -ForegroundColor Green
    Write-Host "HAProxy stats available at: http://localhost:8404" -ForegroundColor Cyan
    
    Write-Host "`n=== Service Endpoints ===" -ForegroundColor Green
    Write-Host "Microservice API: http://localhost:8080" -ForegroundColor Cyan
    Write-Host "PostgreSQL (via HAProxy): localhost:5432" -ForegroundColor Cyan
}

function Start-Services {
    Write-Host "Starting Patroni cluster and services..." -ForegroundColor Green
    docker-compose up -d
    
    Write-Host "Waiting for services to start..." -ForegroundColor Yellow
    Start-Sleep -Seconds 30
    
    Show-Status
}

function Stop-Services {
    Write-Host "Stopping all services..." -ForegroundColor Yellow
    docker-compose down
}

function Reset-Cluster {
    Write-Host "WARNING: This will destroy all data!" -ForegroundColor Red
    $confirmation = Read-Host "Are you sure? (yes/no)"
    
    if ($confirmation -eq "yes") {
        Write-Host "Resetting cluster..." -ForegroundColor Yellow
        docker-compose down -v
        
        # Remove patroni volumes
        $volumes = docker volume ls -q | Where-Object { $_ -like "*patroni*" -or $_ -like "*etcd*" }
        if ($volumes) {
            docker volume rm $volumes
        }
        
        Write-Host "Cluster reset complete. Run 'start' to create a fresh cluster." -ForegroundColor Green
    } else {
        Write-Host "Reset cancelled." -ForegroundColor Green
    }
}

function Initialize-Database {
    Write-Host "Initializing database..." -ForegroundColor Green
    
    # Wait for cluster to be ready
    $ready = $false
    $attempts = 0
    
    while (-not $ready -and $attempts -lt 30) {
        try {
            $result = docker exec patroni1 patronictl -c /etc/patroni/patroni.yml list 2>$null
            if ($result -match "Leader") {
                $ready = $true
            }
        }
        catch {
            # Continue waiting
        }
        
        if (-not $ready) {
            Write-Host "Waiting for Patroni cluster to be ready... ($attempts/30)" -ForegroundColor Yellow
            Start-Sleep -Seconds 5
            $attempts++
        }
    }
    
    if ($ready) {
        Write-Host "Cluster is ready. Initializing database..." -ForegroundColor Green
        
        # Create database and tables
        $initScript = @"
CREATE DATABASE mydb;
\c mydb;
CREATE TABLE IF NOT EXISTS products (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    price DECIMAL(10, 2) NOT NULL,
    description TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
"@
        
        $initScript | docker exec -i patroni1 psql -U postgres
        Write-Host "Database initialized successfully!" -ForegroundColor Green
    } else {
        Write-Host "Cluster is not ready. Please try again later." -ForegroundColor Red
    }
}

function Show-Logs {
    Write-Host "=== Recent Logs ===" -ForegroundColor Green
    Write-Host "Choose a service to view logs:" -ForegroundColor Cyan
    Write-Host "1. Patroni1"
    Write-Host "2. Patroni2" 
    Write-Host "3. Patroni3"
    Write-Host "4. HAProxy"
    Write-Host "5. Microservice"
    Write-Host "6. All services"
    
    $choice = Read-Host "Enter choice (1-6)"
    
    switch ($choice) {
        "1" { docker logs --tail=50 patroni1 }
        "2" { docker logs --tail=50 patroni2 }
        "3" { docker logs --tail=50 patroni3 }
        "4" { docker logs --tail=50 haproxy }
        "5" { docker logs --tail=50 microservice }
        "6" { 
            Write-Host "=== Patroni1 Logs ===" -ForegroundColor Yellow
            docker logs --tail=20 patroni1
            Write-Host "`n=== HAProxy Logs ===" -ForegroundColor Yellow  
            docker logs --tail=20 haproxy
            Write-Host "`n=== Microservice Logs ===" -ForegroundColor Yellow
            docker logs --tail=20 microservice
        }
        default { Write-Host "Invalid choice" -ForegroundColor Red }
    }
}

function Test-Services {
    Write-Host "Testing services..." -ForegroundColor Green
    
    # Test microservice
    Write-Host "`nTesting microservice endpoint..." -ForegroundColor Cyan
    try {
        $response = Invoke-RestMethod -Uri "http://localhost:8080" -TimeoutSec 10
        Write-Host "✅ Microservice: $response" -ForegroundColor Green
    }
    catch {
        Write-Host "❌ Microservice: Not responding" -ForegroundColor Red
    }
    
    # Test database connection through HAProxy
    Write-Host "`nTesting database connection..." -ForegroundColor Cyan
    try {
        $result = docker exec patroni1 psql -h haproxy -U postgres -d postgres -c "SELECT 1;" 2>$null
        if ($result -match "1") {
            Write-Host "✅ Database: Connection successful" -ForegroundColor Green
        } else {
            Write-Host "❌ Database: Connection failed" -ForegroundColor Red
        }
    }
    catch {
        Write-Host "❌ Database: Connection failed" -ForegroundColor Red
    }
    
    # Test products API
    Write-Host "`nTesting products API..." -ForegroundColor Cyan
    try {
        $products = Invoke-RestMethod -Uri "http://localhost:8080/products" -TimeoutSec 10
        Write-Host "✅ Products API: $($products.Count) products found" -ForegroundColor Green
    }
    catch {
        Write-Host "❌ Products API: Not responding" -ForegroundColor Red
    }
}

# Main script logic
switch ($Action) {
    "start" { Start-Services }
    "stop" { Stop-Services }
    "status" { Show-Status }
    "reset" { Reset-Cluster }
    "init-db" { Initialize-Database }
    "logs" { Show-Logs }
    "test" { Test-Services }
}