FROM mcr.microsoft.com/dotnet/sdk:8.0

# This image intentionally does NOT run `dotnet restore` during build.
# NightlyBilling depends on private packages (e.g. VaxCare.*), so CI should provide a NuGet.config at runtime.

WORKDIR /repo

RUN mkdir -p /app/TestResults

RUN printf '%s\n' \
  '#!/usr/bin/env bash' \
  'set -euo pipefail' \
  '' \
  '# Expect the repo to be mounted at /repo' \
  'if [ ! -f "/repo/NightlyBilling/NightlyBillingUnitTests/NightlyBillingUnitTests.csproj" ]; then' \
  '  echo "Expected NightlyBilling project at /repo/NightlyBilling but it was not found."' \
  '  exit 2' \
  'fi' \
  '' \
  'dotnet test -c Release /repo/NightlyBilling/NightlyBillingUnitTests/NightlyBillingUnitTests.csproj \' \
  '  --logger "trx;LogFileName=TestResults.trx" \' \
  '  --results-directory /app/TestResults \' \
  '  "$@"' \
  > /usr/local/bin/run-nightlybilling-tests.sh \
  && chmod +x /usr/local/bin/run-nightlybilling-tests.sh

ENTRYPOINT ["/usr/local/bin/run-nightlybilling-tests.sh"]

