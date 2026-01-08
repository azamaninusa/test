# PowerShell script to run tests in Docker (Windows version)
# Usage: .\run-tests-windows.ps1 [test-filter]

param(
    [string]$TestFilter = "FullyQualifiedName~GoogleSearchTest"
)

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  VaxCare Test Runner (Windows)" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Create TestResults directory if it doesn't exist
if (-not (Test-Path "TestResults")) {
    New-Item -ItemType Directory -Path "TestResults" | Out-Null
}
if (-not (Test-Path "TestResults\html")) {
    New-Item -ItemType Directory -Path "TestResults\html" | Out-Null
}

# Check if Docker is running
try {
    docker info | Out-Null
} catch {
    Write-Host "Error: Docker daemon is not running." -ForegroundColor Yellow
    Write-Host "Please start Docker Desktop and try again."
    exit 1
}

Write-Host "[1/4] Checking Docker image..." -ForegroundColor Cyan
$imageExists = docker images vaxcare-tests-windows | Select-String "vaxcare-tests-windows"
if (-not $imageExists) {
    Write-Host "Docker image not found. Building..." -ForegroundColor Yellow
    docker build -f Dockerfile.windows -t vaxcare-tests-windows .
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Error: Docker build failed" -ForegroundColor Red
        exit 1
    }
    Write-Host "✓ Docker image built successfully" -ForegroundColor Green
} else {
    Write-Host "✓ Docker image found" -ForegroundColor Green
}

Write-Host ""
Write-Host "[2/4] Running tests in Docker..." -ForegroundColor Cyan
Write-Host "   Filter: $TestFilter"
Write-Host ""

# Get current directory for volume mount
$currentDir = (Get-Location).Path

# Run tests with volume mount for reports
docker run --rm `
    -v "${currentDir}\TestResults:C:\app\TestResults" `
    vaxcare-tests-windows `
    --filter $TestFilter

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "✓ Tests completed successfully" -ForegroundColor Green
} else {
    Write-Host ""
    Write-Host "⚠ Tests completed with warnings or failures" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "[3/4] Generating HTML report..." -ForegroundColor Cyan

# Check if TRX file exists
if (-not (Test-Path "TestResults\TestResults.trx")) {
    Write-Host "Warning: TestResults.trx not found" -ForegroundColor Yellow
    Write-Host "Tests may not have generated a report file."
    exit 1
}

# Generate HTML report using Python (cross-platform script)
if (Get-Command python -ErrorAction SilentlyContinue) {
    python generate-html-report.py
} elseif (Get-Command python3 -ErrorAction SilentlyContinue) {
    python3 generate-html-report.py
} else {
    Write-Host "Warning: Python not found. Cannot generate HTML report." -ForegroundColor Yellow
    Write-Host "You can still view the TRX file: TestResults\TestResults.trx"
}

if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ HTML report generated" -ForegroundColor Green
} else {
    Write-Host "Warning: Could not generate HTML report" -ForegroundColor Yellow
    Write-Host "You can still view the TRX file: TestResults\TestResults.trx"
}

Write-Host ""
Write-Host "[4/4] Opening test report..." -ForegroundColor Cyan

# Open the HTML report
if (Test-Path "TestResults\html\TestReport.html") {
    Start-Process "TestResults\html\TestReport.html"
    Write-Host "✓ Report opened in browser" -ForegroundColor Green
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "  Test Run Complete!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Report location: $currentDir\TestResults\html\TestReport.html"
    Write-Host "TRX file: $currentDir\TestResults\TestResults.trx"
} else {
    Write-Host "Warning: HTML report not found" -ForegroundColor Yellow
    Write-Host "TRX file available at: TestResults\TestResults.trx"
}

