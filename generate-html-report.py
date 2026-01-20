#!/usr/bin/env python3
"""
Generate HTML report from TRX file (Cross-platform Python script)
Works on both Windows and Linux/Mac
"""

import xml.etree.ElementTree as ET
import html
import sys
import os
import glob
import shutil
import re

# Paths
trx_file = 'TestResults/TestResults.trx'
html_dir = 'TestResults/html'
screenshots_dir = 'TestResults/screenshots'
html_file = os.path.join(html_dir, 'TestReport.html')

if not os.path.exists(trx_file):
    print(f"Error: TestResults.trx not found at {trx_file}", file=sys.stderr)
    print("Please run tests first to generate the TRX file.", file=sys.stderr)
    sys.exit(1)

os.makedirs(html_dir, exist_ok=True)

# Create screenshots subdirectory in HTML report directory
html_screenshots_dir = os.path.join(html_dir, 'screenshots')
os.makedirs(html_screenshots_dir, exist_ok=True)

# Function to find screenshots for a test
def find_screenshots_for_test(test_name):
    """Find screenshots matching the test name"""
    screenshots = []
    if not os.path.exists(screenshots_dir):
        return screenshots
    
    # Extract method name from fully qualified test name (e.g., "VaxCare.Tests.GoogleSearchTest.SearchForPenOnGoogle" -> "SearchForPenOnGoogle")
    method_name = test_name.split('.')[-1] if '.' in test_name else test_name
    
    # Screenshot naming format: {TestName}_{YYYYMMDD}_{HHMMSS}.png
    # Try multiple patterns to find screenshots
    all_matches = []
    
    # Try to match with method name
    pattern = os.path.join(screenshots_dir, f'*{method_name}*.png')
    all_matches.extend(glob.glob(pattern))
    
    # Try to match with test name words (screenshot name might have spaces)
    words = method_name.replace('_', ' ').split()
    if words:
        for word in words[:3]:  # Try first 3 words
            pattern = os.path.join(screenshots_dir, f'*{word}*.png')
            all_matches.extend(glob.glob(pattern))
    
    # Try to match with full test name parts
    test_parts = test_name.split('.')
    if len(test_parts) >= 2:
        class_name = test_parts[-2]  # Class name (e.g., "GoogleSearchTest")
        pattern = os.path.join(screenshots_dir, f'*{class_name}*.png')
        all_matches.extend(glob.glob(pattern))
    
    # Remove duplicates and sort by modification time (newest first)
    matches = list(set(all_matches))
    matches.sort(key=os.path.getmtime, reverse=True)
    
    # Copy screenshots to HTML report directory and return relative paths
    for screenshot_path in matches:
        screenshot_filename = os.path.basename(screenshot_path)
        dest_path = os.path.join(html_screenshots_dir, screenshot_filename)
        try:
            shutil.copy2(screenshot_path, dest_path)
            # Return relative path from HTML report
            screenshots.append(os.path.join('screenshots', screenshot_filename))
        except Exception as e:
            print(f"Warning: Could not copy screenshot {screenshot_path}: {e}", file=sys.stderr)
    
    return screenshots

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
        .screenshots {{ margin-top: 15px; padding-top: 15px; border-top: 1px solid #ddd; }}
        .screenshot-container {{ margin: 10px 0; }}
        .screenshot-title {{ font-weight: bold; color: #666; margin-bottom: 5px; }}
        .screenshot-img {{ max-width: 100%; height: auto; border: 1px solid #ddd; border-radius: 4px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); cursor: pointer; }}
        .screenshot-img:hover {{ box-shadow: 0 4px 8px rgba(0,0,0,0.2); }}
        .screenshot-link {{ display: inline-block; margin-right: 10px; margin-bottom: 10px; color: #2196F3; text-decoration: none; }}
        .screenshot-link:hover {{ text-decoration: underline; }}
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
    
    # Find screenshots for failed tests
    screenshots_html = ''
    if outcome == 'Failed':
        # Try to extract test description from output (e.g., "Starting Test: Search for 'pen' on Google")
        test_description = ''
        if output and 'Starting Test:' in output:
            match = re.search(r'Starting Test:\s*(.+)', output)
            if match:
                test_description = match.group(1).strip()
        
        # Try matching with test description first (more accurate), then with test name
        screenshots = find_screenshots_for_test(test_description) if test_description else find_screenshots_for_test(test_name)
        if screenshots:
            screenshots_html = '<div class="screenshots"><div class="screenshot-title">Screenshots:</div>'
            for screenshot_rel_path in screenshots:
                screenshot_name = os.path.basename(screenshot_rel_path)
                escaped_path = html.escape(screenshot_rel_path)
                escaped_name = html.escape(screenshot_name)
                screenshots_html += f'''<div class="screenshot-container">
                        <a href="{escaped_path}" target="_blank" class="screenshot-link">
                            <img src="{escaped_path}" alt="Screenshot: {escaped_name}" class="screenshot-img" style="max-width: 800px;" />
                        </a>
                        <div style="font-size: 11px; color: #666; margin-top: 5px;">{escaped_name}</div>
                    </div>'''
            screenshots_html += '</div>'
        else:
            screenshots_html = '<div class="screenshots" style="color: #999; font-style: italic;">No screenshots available for this test failure.</div>'
    
    html_content += f"""
        <div class="test" style="border-left-color: {border_color};">
            <div class="test-name">{html.escape(test_name)}</div>
            <div class="test-info"><strong>Status:</strong> <span class="{outcome_class}">{outcome}</span></div>
            <div class="test-info"><strong>Duration:</strong> {duration}</div>
            <div class="test-info"><strong>Start Time:</strong> {start_time}</div>
            <div class="output">{html.escape(output)}</div>
            {screenshots_html}
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
