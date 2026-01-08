# VaxCare QA Automation Framework

A comprehensive .NET 8.0 test automation framework for VaxCare applications, supporting both UI automation with Selenium WebDriver and API testing with integrated Okta authentication.

## 📋 Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Prerequisites](#prerequisites)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Writing Tests](#writing-tests)
- [Running Tests](#running-tests)
  - [Run All Tests](#run-all-tests)
  - [Run Specific Test Project](#run-specific-test-project)
  - [Run Specific Test Class](#run-specific-test-class)
  - [Run Tests with Verbose Output](#run-tests-with-verbose-output)
  - [Run Tests in Visual Studio](#run-tests-in-visual-studio)
  - [Run Tests in Docker](#run-tests-in-docker)
- [Features](#features)
- [Technologies](#technologies)

## 🎯 Overview

This framework provides a robust foundation for automated testing of VaxCare applications, including:
- **UI Testing**: Selenium WebDriver-based automation for Portal and DataEntry applications
- **API Testing**: RESTful API client with Okta OAuth2 authentication
- **Database Testing**: Entity Framework Core integration for database validation
- **Page Object Model**: Maintainable page object pattern implementation
- **Comprehensive Logging**: Serilog integration with test output and console logging
- **Test Reporting**: Screenshot capture and Teams notifications for test results

## 🏗️ Architecture

The solution follows a modular architecture with clear separation of concerns:

```
VaxCare.QaAutomation/
├── VaxCare.Core/          # Core framework components
├── VaxCare.Pages/         # Page Object Model implementations
├── VaxCare.ApiClient/     # API client and authentication
├── VaxCare.Data/          # Database access layer
├── VaxCare.Tests/         # Integration tests
└── VaxCare.UnitTests/     # Unit tests
```

### Key Components

- **VaxCare.Core**: Base classes, WebDriver management, logging, helpers, and test fixtures
- **VaxCare.Pages**: Page objects for Portal, DataEntry, and Login pages
- **VaxCare.ApiClient**: HTTP client with Okta token generation and PKCE flow
- **VaxCare.Data**: Entity Framework contexts and repositories for multiple databases
- **VaxCare.Tests**: Integration tests for Portal and DataEntry workflows
- **VaxCare.UnitTests**: Unit tests for API client and WebDriver extensions

## 📦 Prerequisites

- **.NET 8.0 SDK** or later
- **Visual Studio 2022** (17.14+) or **Visual Studio Code** with C# extension
- **Chrome/Edge/Firefox** browser installed (for UI tests)
- **SQL Server** access (for database tests)
- **Okta credentials** (for API authentication)

## 📁 Project Structure

### VaxCare.Core
Core framework functionality including:
- `BaseTest.cs`: Abstract base class for all tests with WebDriver lifecycle management
- `BasePage.cs`: Base class for page objects
- `WebDriver/`: WebDriver builder and extension methods
- `Logger/`: Serilog configuration and test output integration
- `Helpers/`: Screenshot capture and Teams notification helpers
- `Entities/`: Test data entities (patients, users)
- `TestFixtures/`: XUnit fixtures for database and test collection setup

### VaxCare.Pages
Page Object Model implementations:
- `Portal/PortalPage.cs`: Portal application page objects
- `DataEntry/InsuranceMappingPage.cs`: DataEntry page objects
- `Login/`: Login page objects for Portal and DataEntry

### VaxCare.ApiClient
API client with authentication:
- `Client.cs`: HTTP client wrapper
- `Api.cs`: API request handling
- `TokenGenerator/`: Okta OAuth2 token generation with PKCE

### VaxCare.Data
Database access layer:
- Multiple Entity Framework contexts (Sales, DataEntry, Risk, HealthSystems, Reporting)
- Repository pattern implementations
- Entity models and DTOs

### VaxCare.Tests
Integration test suites:
- `Portal/`: Portal application tests (scheduling, patient verification)
- `DataEntry/`: DataEntry regression tests

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd Vaxcare.QaAutomation
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Configure Settings

Update `VaxCare.Tests/appsettings.json` with your environment-specific settings:

```json
{
  "OktaConfiguration": {
    "OktaDomain": "https://identitystg.vaxcare.com",
    "OktaClientId": "your-client-id",
    "Username": "your-username",
    "Password": "your-password"
  },
  "WebDriverSettings": {
    "BrowserType": "Chrome",
    "Headless": false
  },
  "ConnectionStrings": {
    "Sales": "your-connection-string",
    "DataEntry": "your-connection-string"
  }
}
```

### 4. Build the Solution

```bash
dotnet build
```

## ⚙️ Configuration

### WebDriver Settings

Configure browser type and headless mode in `appsettings.json`:

```json
"WebDriverSettings": {
  "BrowserType": "Chrome",  // Chrome, Edge, or Firefox
  "Headless": false
}
```

### Logging Configuration

Serilog is configured via `appsettings.json`:

```json
"Serilog": {
  "MinimumLevel": "Information",
  "WriteTo": [
    { "Name": "Console" },
    { "Name": "TestOutputSink" }
  ]
}
```

### Database Connections

Configure connection strings for each database context:

```json
"ConnectionStrings": {
  "Sales": "Server=...; Database=Sales; ...",
  "DataEntry": "Server=...; Database=DataEntry; ...",
  "Risk": "Server=...; Database=Risk; ...",
  "HealthSystems": "Server=...; Database=HealthSystems; ...",
  "Reporting": "Server=...; Database=Reporting; ..."
}
```

## ✍️ Writing Tests

### Creating a Test Class

Inherit from `BaseTest` and use the provided helpers:

```csharp
using VaxCare.Core;
using Xunit;
using Xunit.Abstractions;

public class MyTest : BaseTest
{
    public MyTest(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public async Task MyTestScenario()
    {
        await RunTestAsync("My Test Name", async () =>
        {
            // Your test logic here
            var loginPage = Page<PortalLogin>();
            await loginPage.LoginAsync("username", "password");
            
            var portalPage = await PageAsync<PortalPage>();
            // Continue with test steps
        });
    }
}
```

### Using Page Objects

```csharp
// Synchronous page instantiation
var page = Page<PortalPage>();

// Asynchronous page instantiation (with initialization)
var page = await PageAsync<PortalPage>();
```

### Using the API Client

```csharp
var apiClient = Services.GetRequiredService<IApiClient>();
var response = await apiClient.GetAsync("/api/endpoint");
```

### Database Access

```csharp
var repository = Services.GetRequiredService<IDataEntryRepository>();
var data = await repository.GetDataAsync();
```

## 🏃 Running Tests

### Run All Tests

```bash
dotnet test
```

### Run Specific Test Project

```bash
dotnet test VaxCare.Tests/VaxCare.Tests.csproj
```

### Run Specific Test Class

```bash
dotnet test --filter "FullyQualifiedName~ScheduleMedDPatient"
```

### Run Tests with Verbose Output

```bash
dotnet test --logger "console;verbosity=detailed"
```

### Run Tests in Visual Studio

1. Open the solution in Visual Studio
2. Build the solution (Ctrl+Shift+B)
3. Open Test Explorer (Test → Test Explorer)
4. Run tests individually or in groups

### Run Tests in Docker

The project includes Docker support for running tests in a containerized environment. This ensures consistent test execution across different machines.

#### Quick Start - Run Tests and View Report

The easiest way to run tests in Docker and automatically view the report:

```bash
./run-tests-and-open-report.sh
```

This script will:
1. Build the Docker image (if needed)
2. Run the tests in Docker
3. Generate an HTML test report
4. Open the report in your browser

#### Run Specific Tests

To run a specific test or test filter:

```bash
./run-tests-and-open-report.sh "FullyQualifiedName~YourTestName"
```

#### Manual Docker Commands

**Option 1: Using Docker directly**
```bash
# Build the image
docker build -t vaxcare-tests .

# Run tests with report generation
mkdir -p TestResults
docker run --rm -v "$(pwd)/TestResults:/app/TestResults" vaxcare-tests
```

**Option 2: Using Docker Compose**
```bash
docker-compose up --build
```

**Option 3: Using helper scripts**
```bash
# Run tests with report
./run-tests-with-report.sh

# Generate HTML report from existing TRX file
./generate-html-report.sh
```

#### Test Reports

After running tests, reports are generated in the `TestResults` directory:

- **TRX File**: `TestResults/TestResults.trx` - Visual Studio Test Results format
- **HTML Report**: `TestResults/html/TestReport.html` - Human-readable HTML report

The HTML report includes:
- Test summary (total, passed, failed, executed)
- Individual test results with status and duration
- Full console output and logs
- Test execution timestamps

To view the report:
```bash
# Automatically opens in browser
./generate-html-report.sh

# Or open manually
open TestResults/html/TestReport.html
```

For more details, see [DOCKER.md](DOCKER.md).

## ✨ Features

### WebDriver Management
- Automatic browser lifecycle management
- Support for Chrome, Edge, and Firefox
- Headless mode support
- Customizable browser arguments

### Logging
- Serilog integration with multiple sinks
- Test output integration
- Structured logging with context enrichment
- Machine name and thread ID tracking

### Error Handling
- Automatic screenshot capture on test failure
- Teams notifications for test results
- Comprehensive exception logging
- Test context information in error messages

### Test Data Management
- Predefined test patient entities
- Fake data generation with Bogus
- Database fixtures for test isolation
- Entity models for common test scenarios

### API Testing
- Okta OAuth2 authentication with PKCE
- Token generation and management
- HTTP client with retry policies (Polly)
- Request/response logging

### Database Testing
- Multiple database context support
- Repository pattern for data access
- Entity Framework Core integration
- Test fixtures for database setup/teardown

## 🛠️ Technologies

- **.NET 8.0**: Target framework
- **Selenium WebDriver 4.38.0**: Browser automation
- **xUnit 2.9.3**: Testing framework
- **Serilog 4.3.0**: Structured logging
- **Entity Framework Core 9.0.11**: ORM for database access
- **Microsoft.Extensions**: Dependency injection and configuration
- **Polly 8.6.4**: Resilience and retry policies
- **Bogus 35.6.5**: Fake data generation
- **Shouldly 4.3.0**: Assertion library

## 📝 Notes

- Ensure WebDriver executables (ChromeDriver, GeckoDriver, etc.) are in PATH or configured appropriately
- Database connection strings require appropriate permissions
- Okta credentials must be valid for the configured environment
- Screenshots are saved automatically on test failures
- Teams webhook URL should be configured in `BaseTest.cs` for notifications

## 🤝 Contributing

When adding new tests or features:
1. Follow the existing project structure
2. Use the Page Object Model pattern for UI tests
3. Inherit from `BaseTest` for all test classes
4. Use `RunTestAsync` wrapper for test execution
5. Add appropriate logging and error handling
6. Update this README if adding significant features
