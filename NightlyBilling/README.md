# NightlyBilling

NightlyBilling is a small .NET 8 set of utilities/tests for validating VaxCare “Nightly Billing” behavior against the DataEntry/RealMed SQL data and (for some tests) generated 837 claim content.

## What’s in this folder

- **`NightlyBillingTestRunner/`**: CLI runner that:
  - Executes `[DataEntry].[Billing].[GatherBillableClaims]`
  - Loads per-claim data from SQL
  - Validates each claim using `GatherBillableClaimsValidator` (FluentValidation)
- **`NightlyBillingData/`**: Dapper-based SQL query layer + EDI parsing helpers used by the runner/tests.
- **`NightlyBillingValidator/`**: Validators (FluentValidation) for claim data and EDI 837 scenarios / filing rules.
- **`NightlyBillingConfigManager/`**: Central place for resolving connection strings by environment (`qa`, `stg`/`staging`).
- **`NightlyBillingUnitTests/`**: NUnit integration-style tests that validate extracted 837 claim data / filing rules.

## Prerequisites

- **.NET SDK**: .NET 8+
- **Database access**: ability to connect to the target SQL Server(s) referenced by your connection strings

## Configuration (connection strings)

Connection strings are read via `System.Configuration.ConfigurationManager` and must be present under:

- If you are in the **parent repo root** (where `NightlyBilling/` is a subfolder):
  - `NightlyBilling/NightlyBillingTestRunner/App.config`
  - `NightlyBilling/NightlyBillingUnitTests/testhost.dll.config`
- If you are in the **NightlyBilling repo root** (where `NightlyBillingTestRunner/` is directly under the current folder):
  - `NightlyBillingTestRunner/App.config`
  - `NightlyBillingUnitTests/testhost.dll.config`

Both configs should define the same named connection strings:

- `QA`
- `STG`

## Run the CLI test runner

The runner requires an environment flag:

- **`qa`** → uses connection string `QA`
- **`stg`** or **`staging`** → uses connection string `STG`
- Any other value → prints a warning and defaults to **QA**

If your current directory is the **parent repo root**:

```bash
dotnet run --project NightlyBilling/NightlyBillingTestRunner -- -e qa
```

If your current directory is the **NightlyBilling repo root**:

```bash
dotnet run --project NightlyBillingTestRunner -- -e qa
```

You can also use the long option name:

```bash
dotnet run --project NightlyBilling/NightlyBillingTestRunner -- --Environment stg
```

## Run the unit tests

If your current directory is the **parent repo root**:

```bash
dotnet test NightlyBilling/NightlyBillingUnitTests/NightlyBillingUnitTests.csproj
```

If your current directory is the **NightlyBilling repo root**:

```bash
dotnet test NightlyBillingUnitTests/NightlyBillingUnitTests.csproj
```

## Run tests in Docker (PowerShell / Windows)

From the **parent repo root** (the folder that contains `NightlyBilling\`):

### Build the image

```powershell
docker build -t nightlybilling-tests:local -f .\NightlyBilling\Dockerfile .
```

### Run the container (writes TRX to `NightlyBilling\TestResults`)

```powershell
New-Item -ItemType Directory -Force -Path .\NightlyBilling\TestResults | Out-Null

docker run --rm `
  -v "${PWD}\NightlyBilling\TestResults:/app/TestResults" `
  -v "${PWD}:/repo" `
  nightlybilling-tests:local
```

### Run with a test filter (optional)

```powershell
docker run --rm `
  -v "${PWD}\NightlyBilling\TestResults:/app/TestResults" `
  -v "${PWD}:/repo" `
  nightlybilling-tests:local `
  --filter "FullyQualifiedName~NightlyBillingUnitTests.GatherBillableClaimsValidatorTests"
```

### NuGet / Azure Artifacts (self-hosted runners)

If your `NightlyBilling` projects restore from **Azure Artifacts** (private packages), make sure the container can see a working `NuGet.Config`.

Common approach on self-hosted runners: mount the runner’s user-level config:

```powershell
docker run --rm `
  -v "${PWD}\NightlyBilling\TestResults:/app/TestResults" `
  -v "${PWD}:/repo" `
  -v "$HOME\.nuget\NuGet\NuGet.Config:/root/.nuget/NuGet/NuGet.Config:ro" `
  nightlybilling-tests:local
```

### Fixing `PartialChain` / SSL errors when downloading from `nuget.org`

If you see errors like `The remote certificate is invalid ... PartialChain`, your network is likely doing TLS inspection and the container doesn’t trust your company CA.

Export your company root CA to a file (example: `corp-root.crt`) and mount it:

```powershell
docker run --rm `
  -v "${PWD}\NightlyBilling\TestResults:/app/TestResults" `
  -v "${PWD}:/repo" `
  -v "${PWD}\corp-root.crt:/tmp/extra-ca.crt:ro" `
  -e "EXTRA_CA_CERT_PATH=/tmp/extra-ca.crt" `
  nightlybilling-tests:local
```

Notes:

- `EDI837IntegrationTests` currently uses `stg` in its setup (`ConfigManager.GetConnectionString("stg")`). If you need to run against QA, update that value (or refactor it to read from an environment variable).

## Troubleshooting

- **Connection string not found / empty**: ensure `QA` and `STG` entries exist in the `.config` files listed above.
- **Auth failures**: the sample configs use Integrated Security; update connection strings as needed for your environment.
- **Environment value rejected**: only `qa`, `stg`, and `staging` are recognized (others default to QA).