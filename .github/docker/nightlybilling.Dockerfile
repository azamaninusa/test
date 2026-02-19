## NightlyBilling test image (no HTML/TRX report generation)
# syntax=docker/dockerfile:1.7
FROM mcr.microsoft.com/dotnet/sdk:8.0

RUN apt-get update \
  && apt-get install -y --no-install-recommends ca-certificates \
  && rm -rf /var/lib/apt/lists/*

WORKDIR /src

# Default NuGet config (sources only). CI can override securely via BuildKit secret `nuget_config`.
COPY NuGet.config ./NuGet.config
RUN mkdir -p /root/.nuget/NuGet && cp ./NuGet.config /root/.nuget/NuGet/NuGet.Config

# Copy project files first (better layer caching)
COPY NightlyBillingConfigManager/*.csproj NightlyBillingConfigManager/
COPY NightlyBillingData/*.csproj NightlyBillingData/
COPY NightlyBillingValidator/*.csproj NightlyBillingValidator/
COPY NightlyBillingTestRunner/*.csproj NightlyBillingTestRunner/
COPY NightlyBillingUnitTests/*.csproj NightlyBillingUnitTests/

# Restore with optional secret:
# - `nuget_config`: authenticated NuGet.Config (Azure Artifacts) to avoid baking creds into layers
RUN --mount=type=secret,id=nuget_config,dst=/root/.nuget/NuGet/NuGet.Config,required=false \
    dotnet restore NightlyBillingUnitTests/NightlyBillingUnitTests.csproj

# Copy full source after restore
COPY . .

RUN dotnet build -c Release NightlyBillingUnitTests/NightlyBillingUnitTests.csproj --no-restore

RUN printf '%s\n' \
  '#!/usr/bin/env bash' \
  'set -euo pipefail' \
  '' \
  'dotnet test -c Release NightlyBillingUnitTests/NightlyBillingUnitTests.csproj \' \
  '  --no-build --no-restore \' \
  '  "$@"' \
  > /usr/local/bin/run-nightlybilling-tests.sh \
  && chmod +x /usr/local/bin/run-nightlybilling-tests.sh

ENTRYPOINT ["/usr/local/bin/run-nightlybilling-tests.sh"]
CMD ["--filter", "FullyQualifiedName~NightlyBillingUnitTests.GatherBillableClaimsValidatorTests"]

