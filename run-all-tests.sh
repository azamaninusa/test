#!/bin/bash

# Script to run ALL tests in Docker and automatically open the test report
# Usage: ./run-all-tests.sh

set -e

# Colors for output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${BLUE}========================================${NC}"
echo -e "${BLUE}  VaxCare Test Runner - All Tests${NC}"
echo -e "${BLUE}========================================${NC}"
echo ""

# Create TestResults directory if it doesn't exist
mkdir -p TestResults
mkdir -p TestResults/html

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo -e "${YELLOW}Error: Docker daemon is not running.${NC}"
    echo "Please start Docker Desktop and try again."
    exit 1
fi

echo -e "${BLUE}[1/4]${NC} Checking Docker image..."
if ! docker images vaxcare-tests | grep -q vaxcare-tests; then
    echo -e "${YELLOW}Docker image not found. Building...${NC}"
    docker build -t vaxcare-tests . > /dev/null 2>&1
    echo -e "${GREEN}✓ Docker image built successfully${NC}"
else
    echo -e "${GREEN}✓ Docker image found${NC}"
fi

echo ""
echo -e "${BLUE}[2/4]${NC} Running ALL tests in Docker..."
echo ""

# Run ALL tests (no filter) with volume mount for reports
if docker run --rm \
    -v "$(pwd)/TestResults:/app/TestResults" \
    vaxcare-tests \
    dotnet test --logger "trx;LogFileName=TestResults.trx" --results-directory /app/TestResults 2>&1 | tee /tmp/test_output.log; then
    
    echo ""
    echo -e "${GREEN}✓ Tests completed successfully${NC}"
else
    echo ""
    echo -e "${YELLOW}⚠ Tests completed with warnings or failures${NC}"
fi

echo ""
echo -e "${BLUE}[3/4]${NC} Generating HTML report..."

# Check if TRX file exists
if [ ! -f "TestResults/TestResults.trx" ]; then
    echo -e "${YELLOW}Warning: TestResults.trx not found${NC}"
    echo "Tests may not have generated a report file."
    exit 1
fi

# Generate HTML report
if python3 generate-html-report.py > /dev/null 2>&1; then
    echo -e "${GREEN}✓ HTML report generated${NC}"
else
    echo -e "${YELLOW}Warning: Could not generate HTML report${NC}"
    echo "You can still view the TRX file: TestResults/TestResults.trx"
    exit 1
fi

echo ""
echo -e "${BLUE}[4/4]${NC} Opening test report..."

# Open the HTML report
if [ -f "TestResults/html/TestReport.html" ]; then
    open "TestResults/html/TestReport.html"
    echo -e "${GREEN}✓ Report opened in browser${NC}"
    echo ""
    echo -e "${GREEN}========================================${NC}"
    echo -e "${GREEN}  Test Run Complete!${NC}"
    echo -e "${GREEN}========================================${NC}"
    echo ""
    echo "Report location: $(pwd)/TestResults/html/TestReport.html"
    echo "TRX file: $(pwd)/TestResults/TestResults.trx"
else
    echo -e "${YELLOW}Warning: HTML report not found${NC}"
    echo "TRX file available at: TestResults/TestResults.trx"
fi

