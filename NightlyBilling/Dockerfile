FROM mcr.microsoft.com/dotnet/sdk:8.0

# This image intentionally does NOT run `dotnet restore` during build.
# NightlyBilling depends on private packages (e.g. VaxCare.*), so CI should provide a NuGet.config at runtime.

WORKDIR /repo

RUN apt-get update \
  && apt-get install -y --no-install-recommends ca-certificates \
  && rm -rf /var/lib/apt/lists/*

RUN mkdir -p /app/TestResults

RUN printf '%s\n' \
  '#!/usr/bin/env bash' \
  'set -euo pipefail' \
  '' \
  '# Optional: trust a company TLS root CA inside the container.' \
  '# This is needed on networks that MITM/inspect TLS and causes NuGet "PartialChain" errors.' \
  'EXTRA_CA_CERT_PATH="${EXTRA_CA_CERT_PATH:-/tmp/extra-ca.crt}"' \
  'if [ -f "$EXTRA_CA_CERT_PATH" ]; then' \
  '  echo "Importing extra CA cert from $EXTRA_CA_CERT_PATH"' \
  '  cp "$EXTRA_CA_CERT_PATH" /usr/local/share/ca-certificates/extra-ca.crt' \
  '  update-ca-certificates' \
  'fi' \
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

