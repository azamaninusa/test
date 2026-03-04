#!/usr/bin/env bash
set -euo pipefail

cd /app

echo "========================================="
echo "  Running tests..."
echo "========================================="

args=("$@")
if [ "${#args[@]}" -eq 0 ] && [ -n "${TEST_FILTER:-}" ]; then
  args=(--filter "$TEST_FILTER")
fi

TEST_EXIT_CODE=0
set +e
dotnet test -c Release --verbosity normal --logger "trx;LogFileName=TestResults.trx" --results-directory /app/TestResults "${args[@]}"
TEST_EXIT_CODE=$?
set -e

echo ""
echo "========================================="
echo "  Generating HTML report..."
echo "========================================="

if [ -f "/app/TestResults/TestResults.trx" ]; then
  python3 generate-html-report.py || echo "Warning: Failed to generate HTML report"

  if [ -f "/app/TestResults/html/TestReport.html" ]; then
    echo ""
    echo "HTML report generated successfully"
    echo "Location: /app/TestResults/html/TestReport.html"
    echo ""

    cat > /app/TestResults/open-report.sh <<'OPEN'
#!/usr/bin/env bash
set -euo pipefail

report_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
report_path="${report_dir}/html/TestReport.html"

if [[ "${OSTYPE:-}" == linux-gnu* ]]; then
  xdg-open "$report_path" >/dev/null 2>&1 || true
elif [[ "${OSTYPE:-}" == darwin* ]]; then
  open "$report_path" || true
elif [[ "${OSTYPE:-}" == msys || "${OSTYPE:-}" == cygwin ]]; then
  start "$report_path" || true
else
  echo "Please open the report manually: $report_path"
fi
OPEN
    chmod +x /app/TestResults/open-report.sh
  else
    echo "Warning: HTML report file not found after generation"
  fi
else
  echo "Warning: TestResults.trx not found, skipping HTML report generation"
fi

exit "$TEST_EXIT_CODE"

