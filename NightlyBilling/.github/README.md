## Note about GitHub Actions location

GitHub Actions workflows only run from the repository root at:

- `.github/workflows/*.yml`

This `NightlyBilling/.github/` folder exists only to keep module-related CI templates/config close to the NightlyBilling code for easier discovery.

The active workflow for this module is:

- `.github/workflows/nb-docker-tests.yml`

