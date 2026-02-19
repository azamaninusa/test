## NightlyBilling test image (no HTML/TRX report generation)
# syntax=docker/dockerfile:1.7

FROM mcr.microsoft.com/dotnet/sdk:8.0

RUN apt-get update \
  && apt-get install -y --no-install-recommends ca-certificates \
  && rm -rf /var/lib/apt/lists/*

WORKDIR /src

# Default NuGet config (sources only). CI can override securely via BuildKit secret `nuget_config`.
COPY NightlyBilling/NuGet.config ./NightlyBilling/NuGet.config
RUN mkdir -p /root/.nuget/NuGet && cp ./NightlyBilling/NuGet.config /root/.nuget/NuGet/NuGet.Config

# Copy project files first (better layer caching)
COPY NightlyBilling/NightlyBillingConfigManager/*.csproj NightlyBilling/NightlyBillingConfigManager/
COPY NightlyBilling/NightlyBillingData/*.csproj NightlyBilling/NightlyBillingData/
COPY NightlyBilling/NightlyBillingValidator/*.csproj NightlyBilling/NightlyBillingValidator/
COPY NightlyBilling/NightlyBillingTestRunner/*.csproj NightlyBilling/NightlyBillingTestRunner/
COPY NightlyBilling/NightlyBillingUnitTests/*.csproj NightlyBilling/NightlyBillingUnitTests/

# Restore with optional secrets:
# - `nuget_config`: authenticated NuGet.Config (Azure Artifacts) to avoid baking creds into layers
RUN --mount=type=secret,id=nuget_config,dst=/root/.nuget/NuGet/NuGet.Config,required=false \
    dotnet restore NightlyBilling/NightlyBillingUnitTests/NightlyBillingUnitTests.csproj

# Copy full source after restore
COPY NightlyBilling/ NightlyBilling/

RUN dotnet build -c Release NightlyBilling/NightlyBillingUnitTests/NightlyBillingUnitTests.csproj --no-restore

# Wrapper script similar to the main Dockerfile, but without report generation.
RUN printf '%s\n' \
  '#!/usr/bin/env bash' \
  'set -euo pipefail' \
  '' \
  'dotnet test -c Release NightlyBilling/NightlyBillingUnitTests/NightlyBillingUnitTests.csproj \' \
  '  --no-build --no-restore \' \
  '  "$@"' \
  > /usr/local/bin/run-nightlybilling-tests.sh \
  && chmod +x /usr/local/bin/run-nightlybilling-tests.sh

ENTRYPOINT ["/usr/local/bin/run-nightlybilling-tests.sh"]
CMD ["--filter", "FullyQualifiedName~NightlyBillingUnitTests.GatherBillableClaimsValidatorTests"]

