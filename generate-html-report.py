#!/usr/bin/env python3
"""
Generate HTML report from TRX file (Cross-platform Python script)
Works on both Windows and Linux/Mac
"""

import xml.etree.ElementTree as ET
import html
import sys
import os

# Paths
trx_file = 'TestResults/TestResults.trx'
html_dir = 'TestResults/html'
html_file = os.path.join(html_dir, 'TestReport.html')

if not os.path.exists(trx_file):
    print(f"Error: TestResults.trx not found at {trx_file}", file=sys.stderr)
    print("Please run tests first to generate the TRX file.", file=sys.stderr)
    sys.exit(1)

os.makedirs(html_dir, exist_ok=True)

try:
    tree = ET.parse(trx_file)
    root = tree.getroot()
except Exception as e:
    print(f"Error parsing TRX file: {e}", file=sys.stderr)
    sys.exit(1)

# Extract test results
ns = {'ns': 'http://microsoft.com/schemas/VisualStudio/TeamTest/2010'}
results = root.findall('.//ns:UnitTestResult', ns)

# Get counters from ResultSummary for accurate counts
counters = root.find('.//ns:ResultSummary/ns:Counters', ns)
if counters is not None:
    total = int(counters.get('total', len(results)))
    executed = int(counters.get('executed', len(results)))
    passed = int(counters.get('passed', 0))
    failed = int(counters.get('failed', 0))
    error = int(counters.get('error', 0))
    inconclusive = int(counters.get('inconclusive', 0))
else:
    # Fallback to counting results if counters not found
    total = len(results)
    passed = sum(1 for r in results if r.get('outcome') == 'Passed')
    failed = sum(1 for r in results if r.get('outcome') == 'Failed')
    error = 0
    inconclusive = 0
    executed = total

# Define error and inconclusive sections before using them in f-string
error_section = f'<div class="summary-item"><strong>Errors:</strong> <span class="failed">{error}</span></div>' if error > 0 else ''
inconclusive_section = f'<div class="summary-item"><strong>Inconclusive:</strong> {inconclusive}</div>' if inconclusive > 0 else ''

html_content = f"""<!DOCTYPE html>
<html>
<head>
    <title>Test Results Report</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 20px; background: #f5f5f5; }}
        .container {{ max-width: 1200px; margin: 0 auto; background: white; padding: 20px; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }}
        h1 {{ color: #333; border-bottom: 3px solid #4CAF50; padding-bottom: 10px; }}
        .test {{ margin: 20px 0; padding: 15px; border-left: 4px solid #4CAF50; background: #f9f9f9; }}
        .test-name {{ font-size: 18px; font-weight: bold; color: #333; margin-bottom: 10px; }}
        .test-info {{ color: #666; margin: 5px 0; }}
        .passed {{ color: #4CAF50; font-weight: bold; }}
        .failed {{ color: #f44336; font-weight: bold; }}
        .output {{ background: #f0f0f0; padding: 10px; border-radius: 4px; margin-top: 10px; font-family: monospace; font-size: 12px; white-space: pre-wrap; }}
        .summary {{ background: #e8f5e9; padding: 15px; border-radius: 4px; margin-bottom: 20px; }}
        .summary-item {{ margin: 5px 0; }}
    </style>
</head>
<body>
    <div class="container">
        <h1>Test Results Report</h1>
        <div class="summary">
            <div class="summary-item"><strong>Total Tests:</strong> {total}</div>
            <div class="summary-item"><strong>Executed:</strong> {executed}</div>
            <div class="summary-item"><strong>Passed:</strong> <span class="passed">{passed}</span></div>
            <div class="summary-item"><strong>Failed:</strong> <span class="failed">{failed}</span></div>
            {error_section}
            {inconclusive_section}
        </div>
"""

for result in results:
    test_name = result.get('testName', 'Unknown')
    outcome = result.get('outcome', 'Unknown')
    duration = result.get('duration', 'N/A')
    start_time = result.get('startTime', 'N/A')
    
    # Determine border color based on outcome
    border_color = '#4CAF50' if outcome == 'Passed' else '#f44336' if outcome == 'Failed' else '#ff9800'
    
    output_elem = result.find('.//ns:StdOut', ns)
    output = output_elem.text if output_elem is not None and output_elem.text else 'No output'
    
    outcome_class = 'passed' if outcome == 'Passed' else 'failed'
    
    html_content += f"""
        <div class="test" style="border-left-color: {border_color};">
            <div class="test-name">{html.escape(test_name)}</div>
            <div class="test-info"><strong>Status:</strong> <span class="{outcome_class}">{outcome}</span></div>
            <div class="test-info"><strong>Duration:</strong> {duration}</div>
            <div class="test-info"><strong>Start Time:</strong> {start_time}</div>
            <div class="output">{html.escape(output)}</div>
        </div>
    """

html_content += """
    </div>
</body>
</html>
"""

# Write HTML file
with open(html_file, 'w', encoding='utf-8') as f:
    f.write(html_content)

print(f"HTML report generated successfully!")
print(f"Total: {total}, Executed: {executed}, Passed: {passed}, Failed: {failed}")
print(f"Location: {html_file}")

