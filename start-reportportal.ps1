#!/usr/bin/env powershell

<#
.SYNOPSIS
    ReportPortal Docker Automation Script
.DESCRIPTION
    Manage ReportPortal Docker containers with simple commands
.PARAMETER Start
    Start ReportPortal services
.PARAMETER Stop
    Stop ReportPortal services
.PARAMETER Logs
    View live logs from all services
.PARAMETER Restart
    Restart ReportPortal services
.PARAMETER Clean
    Remove all containers and volumes (WARNING: Deletes all data)
.PARAMETER Status
    Show status of all services
.EXAMPLE
    .\start-reportportal.ps1 -Start
    .\start-reportportal.ps1 -Logs
    .\start-reportportal.ps1 -Stop
#>

param(
    [switch]$Start,
    [switch]$Stop,
    [switch]$Logs,
    [switch]$Restart,
    [switch]$Clean,
    [switch]$Status,
    [switch]$Help
)

# Configuration
$projectPath = Split-Path -Parent $MyInvocation.MyCommandPath
$composeFile = Join-Path $projectPath "docker-compose.yml"

# Colors for output
$colors = @{
    Green  = 'Green'
    Yellow = 'Yellow'
    Cyan   = 'Cyan'
    Red    = 'Red'
}

function Write-Header {
    param([string]$Message)
    Write-Host "`n" -NoNewline
    Write-Host "=" * 60 -ForegroundColor $colors.Cyan
    Write-Host $Message -ForegroundColor $colors.Cyan
    Write-Host "=" * 60 -ForegroundColor $colors.Cyan
}

function Write-Success {
    param([string]$Message)
    Write-Host $Message -ForegroundColor $colors.Green
}

function Write-Warning {
    param([string]$Message)
    Write-Host $Message -ForegroundColor $colors.Yellow
}

function Write-Error {
    param([string]$Message)
    Write-Host $Message -ForegroundColor $colors.Red
}

function Test-Docker {
    Write-Host "Checking Docker installation..." -ForegroundColor $colors.Cyan
    
    if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
        Write-Error "Docker is not installed. Please install Docker Desktop from https://www.docker.com/products/docker-desktop"
        exit 1
    }
    
    if (-not (Get-Command docker-compose -ErrorAction SilentlyContinue)) {
        Write-Error "Docker Compose is not installed."
        exit 1
    }
    
    Write-Success "✓ Docker is installed"
}

function Start-ReportPortal {
    Write-Header "Starting ReportPortal Services"
    
    Test-Docker
    
    if (-not (Test-Path $composeFile)) {
        Write-Error "docker-compose.yml not found at $composeFile"
        exit 1
    }
    
    Write-Host "Starting services from: $composeFile`n" -ForegroundColor $colors.Cyan
    
    & docker-compose -f $composeFile up -d
    
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Failed to start services"
        exit 1
    }
    
    Write-Host "Waiting for services to start (30 seconds)..." -ForegroundColor $colors.Yellow
    Start-Sleep -Seconds 30
    
    Write-Host "`nChecking service status..." -ForegroundColor $colors.Cyan
    & docker-compose -f $composeFile ps
    
    Write-Success "`n✓ ReportPortal started successfully!"
    Write-Host "`nAccess ReportPortal at: http://localhost:8081" -ForegroundColor $colors.Green
    Write-Host "Default login:" -ForegroundColor $colors.Green
    Write-Host "  Email: superadmin@reportportal.io" -ForegroundColor $colors.Green
    Write-Host "  Password: erebus" -ForegroundColor $colors.Green
    
    Write-Host "`nOther services:" -ForegroundColor $colors.Green
    Write-Host "  API: http://localhost:8080" -ForegroundColor $colors.Green
    Write-Host "  RabbitMQ Management: http://localhost:15672" -ForegroundColor $colors.Green
    Write-Host "  MinIO: http://localhost:9001" -ForegroundColor $colors.Green
    
    Write-Host "`nStop services with: .\start-reportportal.ps1 -Stop" -ForegroundColor $colors.Yellow
}

function Stop-ReportPortal {
    Write-Header "Stopping ReportPortal Services"
    
    Test-Docker
    
    Write-Host "Stopping services..." -ForegroundColor $colors.Yellow
    & docker-compose -f $composeFile down
    
    if ($LASTEXITCODE -eq 0) {
        Write-Success "✓ ReportPortal stopped successfully"
        Write-Host "Data is preserved. Start again with: .\start-reportportal.ps1 -Start" -ForegroundColor $colors.Yellow
    }
    else {
        Write-Error "Failed to stop services"
        exit 1
    }
}

function Show-Logs {
    Write-Header "ReportPortal Logs (Live)"
    Write-Host "Press Ctrl+C to exit" -ForegroundColor $colors.Yellow
    
    Test-Docker
    & docker-compose -f $composeFile logs -f
}

function Restart-ReportPortal {
    Write-Header "Restarting ReportPortal Services"
    
    Test-Docker
    
    Write-Host "Restarting services..." -ForegroundColor $colors.Yellow
    & docker-compose -f $composeFile restart
    
    Write-Host "Waiting for services to restart (15 seconds)..." -ForegroundColor $colors.Yellow
    Start-Sleep -Seconds 15
    
    Write-Host "`nChecking service status..." -ForegroundColor $colors.Cyan
    & docker-compose -f $composeFile ps
    
    Write-Success "`n✓ ReportPortal restarted successfully!"
}

function Clean-ReportPortal {
    Write-Header "⚠️  WARNING: Complete Cleanup"
    Write-Warning "`nThis will DELETE all containers and volumes."
    Write-Warning "All ReportPortal data will be lost!"
    
    $confirm = Read-Host "`nType 'yes' to confirm cleanup"
    
    if ($confirm -ne "yes") {
        Write-Host "Cleanup cancelled" -ForegroundColor $colors.Yellow
        return
    }
    
    Test-Docker
    
    Write-Host "Removing all containers and volumes..." -ForegroundColor $colors.Red
    & docker-compose -f $composeFile down -v
    
    if ($LASTEXITCODE -eq 0) {
        Write-Success "✓ Cleanup completed"
        Write-Host "All ReportPortal data has been deleted" -ForegroundColor $colors.Yellow
    }
    else {
        Write-Error "Cleanup failed"
        exit 1
    }
}

function Show-Status {
    Write-Header "ReportPortal Service Status"
    
    Test-Docker
    & docker-compose -f $composeFile ps
    
    Write-Host "`nService Information:" -ForegroundColor $colors.Green
    Write-Host "  ReportPortal UI: http://localhost:8081" -ForegroundColor $colors.Cyan
    Write-Host "  ReportPortal API: http://localhost:8080" -ForegroundColor $colors.Cyan
    Write-Host "  RabbitMQ Management: http://localhost:15672" -ForegroundColor $colors.Cyan
    Write-Host "  MinIO: http://localhost:9001" -ForegroundColor $colors.Cyan
    Write-Host "  PostgreSQL: localhost:5432" -ForegroundColor $colors.Cyan
    Write-Host "  MongoDB: localhost:27017" -ForegroundColor $colors.Cyan
}

function Show-Help {
    Write-Header "ReportPortal Docker Manager"
    
    Write-Host @"
USAGE:
    .\start-reportportal.ps1 [OPTIONS]

OPTIONS:
    -Start      Start ReportPortal services (default)
    -Stop       Stop ReportPortal services
    -Restart    Restart ReportPortal services
    -Logs       View live logs from all services
    -Status     Show service status
    -Clean      Remove all containers and data (WARNING!)
    -Help       Show this help message

EXAMPLES:
    Start services:
        .\start-reportportal.ps1 -Start

    View logs:
        .\start-reportportal.ps1 -Logs

    Stop services:
        .\start-reportportal.ps1 -Stop

    Check status:
        .\start-reportportal.ps1 -Status

QUICK START:
    1. Start services:
        .\start-reportportal.ps1 -Start

    2. Open in browser:
        http://localhost:8081

    3. Login with:
        Email: superadmin@reportportal.io
        Password: erebus

    4. Create project "selenium-tests"

    5. Get API token from profile

    6. Update reportportal.json with token

    7. Run tests:
        dotnet test

SERVICES:
    - ReportPortal UI:    http://localhost:8081
    - ReportPortal API:   http://localhost:8080
    - RabbitMQ Manager:   http://localhost:15672
    - MinIO Console:      http://localhost:9001
    - PostgreSQL:         localhost:5432
    - MongoDB:            localhost:27017

TROUBLESHOOTING:
    Check logs:
        .\start-reportportal.ps1 -Logs

    Restart services:
        .\start-reportportal.ps1 -Restart

    Full reset (delete data):
        .\start-reportportal.ps1 -Clean

"@ -ForegroundColor $colors.Cyan
}

# Main execution
$hasAction = $Start -or $Stop -or $Logs -or $Restart -or $Clean -or $Status -or $Help

if (-not $hasAction) {
    # Default action: Start
    Start-ReportPortal
}
elseif ($Help) {
    Show-Help
}
elseif ($Start) {
    Start-ReportPortal
}
elseif ($Stop) {
    Stop-ReportPortal
}
elseif ($Restart) {
    Restart-ReportPortal
}
elseif ($Logs) {
    Show-Logs
}
elseif ($Clean) {
    Clean-ReportPortal
}
elseif ($Status) {
    Show-Status
}
