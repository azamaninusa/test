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

- `NightlyBilling/NightlyBillingTestRunner/App.config`
- `NightlyBilling/NightlyBillingUnitTests/testhost.dll.config`

Both configs should define the same named connection strings:

- `QA`
- `STG`

## Run the CLI test runner

The runner requires an environment flag:

- **`qa`** → uses connection string `QA`
- **`stg`** or **`staging`** → uses connection string `STG`
- Any other value → prints a warning and defaults to **QA**

From repo root:

```bash
dotnet run --project NightlyBilling/NightlyBillingTestRunner -- -e qa
```

You can also use the long option name:

```bash
dotnet run --project NightlyBilling/NightlyBillingTestRunner -- --Environment stg
```

## Run the unit tests

From repo root:

```bash
dotnet test NightlyBilling/NightlyBillingUnitTests/NightlyBillingUnitTests.csproj
```

Notes:

- `EDI837IntegrationTests` currently uses `stg` in its setup (`ConfigManager.GetConnectionString("stg")`). If you need to run against QA, update that value (or refactor it to read from an environment variable).

## Troubleshooting

- **Connection string not found / empty**: ensure `QA` and `STG` entries exist in the `.config` files listed above.
- **Auth failures**: the sample configs use Integrated Security; update connection strings as needed for your environment.
- **Environment value rejected**: only `qa`, `stg`, and `staging` are recognized (others default to QA).