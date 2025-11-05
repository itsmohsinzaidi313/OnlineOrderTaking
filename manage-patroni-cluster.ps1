#!/usr/bin/env pwsh

# Patroni Cluster Management Script

param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("start", "stop", "restart", "status", "logs", "failover", "switchover", "cleanup")]
    [string]$Action,
    
    [string]$Node = "",
    [string]$Target = ""
)

function Show-Help {
    Write-Host "Patroni Cluster Management Script"
    Write-Host "Usage: .\manage-patroni-cluster.ps1 -Action <action> [-Node <node>] [-Target <target>]"
    Write-Host ""
    Write-Host "Actions:"
    Write-Host "  start      - Start the Patroni cluster"
    Write-Host "  stop       - Stop the Patroni cluster"
    Write-Host "  restart    - Restart the Patroni cluster"
    Write-Host "  status     - Show cluster status"
    Write-Host "  logs       - Show logs for specific node or all nodes"
    Write-Host "  failover   - Trigger a failover"
    Write-Host "  switchover - Perform a switchover to specific node"
    Write-Host "  cleanup    - Remove all volumes and reset cluster"
    Write-Host ""
    Write-Host "Examples:"
    Write-Host "  .\manage-patroni-cluster.ps1 -Action start"
    Write-Host "  .\manage-patroni-cluster.ps1 -Action status"
    Write-Host "  .\manage-patroni-cluster.ps1 -Action logs -Node patroni1"
    Write-Host "  .\manage-patroni-cluster.ps1 -Action switchover -Target patroni2"
}

function Start-Cluster {
    Write-Host "Starting Patroni cluster..." -ForegroundColor Green
    docker-compose up -d etcd
    Start-Sleep -Seconds 10
    docker-compose up -d patroni1 patroni2
    Start-Sleep -Seconds 20
    docker-compose up -d haproxy microservice
    Write-Host "Cluster started. Use 'status' action to check cluster health." -ForegroundColor Green
}

function Stop-Cluster {
    Write-Host "Stopping Patroni cluster..." -ForegroundColor Yellow
    docker-compose down
    Write-Host "Cluster stopped." -ForegroundColor Yellow
}

function Restart-Cluster {
    Write-Host "Restarting Patroni cluster..." -ForegroundColor Blue
    Stop-Cluster
    Start-Sleep -Seconds 5
    Start-Cluster
}

function Show-Status {
    Write-Host "Patroni Cluster Status:" -ForegroundColor Cyan
    Write-Host "======================" -ForegroundColor Cyan
    
    # Check if containers are running
    Write-Host "`nContainer Status:" -ForegroundColor Yellow
    docker-compose ps
    
    # Check etcd health
    Write-Host "`nETCD Health:" -ForegroundColor Yellow
    try {
        docker exec etcd etcdctl endpoint health 2>$null
    } catch {
        Write-Host "etcd not accessible" -ForegroundColor Red
    }
    
    # Check Patroni cluster status
    Write-Host "`nPatroni Cluster Status:" -ForegroundColor Yellow
    try {
        $patroniStatus = docker exec patroni1 patronictl -c /etc/patroni.yml list 2>$null
        if ($patroniStatus) {
            Write-Host $patroniStatus
        } else {
            Write-Host "Patroni cluster not ready yet" -ForegroundColor Red
        }
    } catch {
        Write-Host "Unable to get Patroni status" -ForegroundColor Red
    }
    
    # Check HAProxy stats
    Write-Host "`nHAProxy Status:" -ForegroundColor Yellow
    Write-Host "HAProxy stats available at: http://localhost:8404"
    
    # Show connection info
    Write-Host "`nConnection Information:" -ForegroundColor Yellow
    Write-Host "Primary connection: localhost:5433"
    Write-Host "Patroni1 direct: localhost:5433"
    Write-Host "Patroni2 direct: localhost:5434"
    Write-Host "Patroni1 REST API: http://localhost:8008"
    Write-Host "Patroni2 REST API: http://localhost:8009"
    Write-Host "etcd API: localhost:2389"
}

function Show-Logs {
    if ($Node) {
        Write-Host "Showing logs for $Node..." -ForegroundColor Cyan
        docker-compose logs -f $Node
    } else {
        Write-Host "Showing logs for all services..." -ForegroundColor Cyan
        docker-compose logs -f
    }
}

function Trigger-Failover {
    Write-Host "Triggering failover..." -ForegroundColor Red
    try {
        docker exec patroni1 patronictl -c /etc/patroni.yml failover --force
        Write-Host "Failover triggered successfully" -ForegroundColor Green
    } catch {
        Write-Host "Failed to trigger failover: $_" -ForegroundColor Red
    }
}

function Perform-Switchover {
    if (-not $Target) {
        Write-Host "Target node required for switchover. Use -Target parameter." -ForegroundColor Red
        return
    }
    
    Write-Host "Performing switchover to $Target..." -ForegroundColor Blue
    try {
        docker exec patroni1 patronictl -c /etc/patroni.yml switchover --master $Target --force
        Write-Host "Switchover completed successfully" -ForegroundColor Green
    } catch {
        Write-Host "Failed to perform switchover: $_" -ForegroundColor Red
    }
}

function Cleanup-Cluster {
    Write-Host "WARNING: This will remove all data and reset the cluster!" -ForegroundColor Red
    $confirm = Read-Host "Are you sure? (yes/no)"
    
    if ($confirm -eq "yes") {
        Write-Host "Stopping and removing containers..." -ForegroundColor Yellow
        docker-compose down -v --remove-orphans
        
        Write-Host "Removing volumes..." -ForegroundColor Yellow
        docker volume prune -f
        
        Write-Host "Cleanup completed." -ForegroundColor Green
    } else {
        Write-Host "Cleanup cancelled." -ForegroundColor Yellow
    }
}

# Main script logic
switch ($Action.ToLower()) {
    "start" { Start-Cluster }
    "stop" { Stop-Cluster }
    "restart" { Restart-Cluster }
    "status" { Show-Status }
    "logs" { Show-Logs }
    "failover" { Trigger-Failover }
    "switchover" { Perform-Switchover }
    "cleanup" { Cleanup-Cluster }
    default { Show-Help }
}