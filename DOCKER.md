# Running Tests in Docker

This project includes a Dockerfile to run tests in a containerized environment.

## Prerequisites

- Docker Desktop installed and running
- Docker daemon must be running

## Quick Start

### Option 1: Using the script
```bash
./run-tests-docker.sh
```

### Option 2: Using Docker directly
```bash
# Build the image
docker build -t vaxcare-tests .

# Run the tests
docker run --rm vaxcare-tests
```

### Option 3: Using Docker Compose
```bash
docker-compose up --build
```

## What the Dockerfile does

1. **Base Image**: Uses .NET 8.0 SDK
2. **Chrome Installation**: Installs Google Chrome and all required dependencies
3. **Project Setup**: Copies and restores all project dependencies
4. **Build**: Builds the solution in Release mode
5. **Configuration**: Automatically enables headless mode for Docker
6. **Test Execution**: Runs the GoogleSearchTest

## Test Reports

Test reports are automatically generated in TRX format after each test run. The reports are saved to the `TestResults` directory.

### Running Tests with Report Generation

**Option 1: Using the script**
```bash
./run-tests-with-report.sh
```

**Option 2: Using Docker directly**
```bash
# Create TestResults directory
mkdir -p TestResults

# Run tests with volume mount for reports
docker run --rm -v "$(pwd)/TestResults:/app/TestResults" vaxcare-tests
```

**Option 3: Using Docker Compose**
```bash
docker-compose up --build
```

The test report will be available at: `./TestResults/TestResults.trx`

### Viewing Test Reports

The TRX file can be opened in:
- **Visual Studio**: Open the `.trx` file directly
- **Azure DevOps**: Upload to test results
- **ReportGenerator**: Convert to HTML using [ReportGenerator](https://github.com/danielpalme/ReportGenerator)

To convert TRX to HTML:
```bash
# Install ReportGenerator (if not already installed)
dotnet tool install -g dotnet-reportgenerator-globaltool

# Convert TRX to HTML
reportgenerator -reports:TestResults/TestResults.trx -targetdir:TestResults/html -reporttypes:Html
```

## Running Specific Tests

To run a different test, override the command when running:

```bash
docker run --rm -v "$(pwd)/TestResults:/app/TestResults" vaxcare-tests --filter "FullyQualifiedName~YourTestName"
```

## Troubleshooting

### Docker daemon not running
If you see `Cannot connect to the Docker daemon`, make sure Docker Desktop is running.

### Chrome/ChromeDriver issues
The Dockerfile automatically:
- Installs the latest Chrome
- Uses Selenium 4.x's selenium-manager to download the correct ChromeDriver
- Configures Chrome with Docker-compatible arguments (--no-sandbox, --disable-dev-shm-usage)

### Headless mode
Tests run in headless mode by default in Docker. This is configured automatically in the Dockerfile.


