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
import urllib.parse

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
def find_screenshots_for_test(test_name_or_description):
    """Find screenshots matching the test name or description"""
    screenshots = []
    if not os.path.exists(screenshots_dir):
        print(f"Debug: Screenshots directory not found: {screenshots_dir}", file=sys.stderr)
        return screenshots
    
    # Get all PNG files in screenshots directory
    all_png_files = glob.glob(os.path.join(screenshots_dir, '*.png'))
    print(f"Debug: Found {len(all_png_files)} PNG files in {screenshots_dir}", file=sys.stderr)
    
    if not all_png_files:
        return screenshots
    
    # Normalize the test name/description for matching
    # Screenshot filenames now use underscores instead of spaces
    def normalize_for_match(text):
        """Normalize text for matching by removing special chars, converting to lowercase, and using underscores"""
        if not text:
            return ""
        # Convert to lowercase
        normalized = text.lower()
        # Replace spaces and underscores with a single underscore
        normalized = re.sub(r'[\s_]+', '_', normalized)
        # Remove quotes, apostrophes, and other special chars (keep alphanumeric and underscores)
        normalized = re.sub(r"[^\w_]", "", normalized)
        # Collapse multiple underscores
        normalized = re.sub(r"_+", "_", normalized)
        # Remove leading/trailing underscores
        normalized = normalized.strip('_')
        return normalized
    
    normalized_search = normalize_for_match(test_name_or_description)
    print(f"Debug: Searching for screenshots matching: '{test_name_or_description}' (normalized: '{normalized_search}')", file=sys.stderr)
    
    # Try multiple matching strategies
    matches = []
    
    # Strategy 1: Direct match with test description (screenshot name often matches test description)
    # Screenshot format: "Search_for_pen_on_Google_20260120_054934.png" (with underscores)
    # Extract the base name (before timestamp) and normalize
    for png_file in all_png_files:
        filename = os.path.basename(png_file)
        print(f"Debug: Checking screenshot file: '{filename}'", file=sys.stderr)
        
        # Remove extension and timestamp pattern (_YYYYMMDD_HHMMSS)
        base_name = re.sub(r'_\d{8}_\d{6}\.png$', '', filename)
        base_name = re.sub(r'\.png$', '', base_name)  # Fallback if no timestamp
        
        normalized_base = normalize_for_match(base_name)
        print(f"Debug:   Base name: '{base_name}' -> normalized: '{normalized_base}'", file=sys.stderr)
        
        # Check if normalized search text matches normalized base name
        if normalized_search and normalized_base:
            # Strategy 1a: Exact match (after normalization)
            if normalized_search == normalized_base:
                matches.append((png_file, 100))  # High score for exact match
                print(f"Debug:   ✓ Exact match!", file=sys.stderr)
                continue
            
            # Strategy 1b: Check if search text is contained in base name or vice versa
            if normalized_search in normalized_base or normalized_base in normalized_search:
                matches.append((png_file, 50))  # Medium score for substring match
                print(f"Debug:   ✓ Substring match!", file=sys.stderr)
                continue
            
            # Strategy 1c: Check if significant words match (split by underscores)
            search_words = set(normalized_search.split('_'))
            base_words = set(normalized_base.split('_'))
            
            # Remove common stop words that don't help matching
            stop_words = {'for', 'on', 'the', 'a', 'an', 'and', 'or', 'but', 'in', 'at', 'to', 'of'}
            search_words = search_words - stop_words
            base_words = base_words - stop_words
            
            # If at least 2 words match (or 1 word if search is short), consider it a match
            common_words = search_words.intersection(base_words)
            if len(common_words) >= min(2, len(search_words)) and len(search_words) > 0:
                matches.append((png_file, len(common_words)))
                print(f"Debug:   ✓ Word match! (common words: {common_words})", file=sys.stderr)
    
    # Strategy 2: If no matches, try matching with method name from test name
    if not matches and '.' in test_name_or_description:
        method_name = test_name_or_description.split('.')[-1]
        normalized_method = normalize_for_match(method_name)
        print(f"Debug: Trying method name match: '{method_name}' (normalized: '{normalized_method}')", file=sys.stderr)
        for png_file in all_png_files:
            filename = os.path.basename(png_file)
            base_name = re.sub(r'_\d{8}_\d{6}\.png$', '', filename)
            base_name = re.sub(r'\.png$', '', base_name)
            normalized_base = normalize_for_match(base_name)
            
            if normalized_method in normalized_base or normalized_base in normalized_method:
                matches.append((png_file, 1))
                print(f"Debug: Matched screenshot '{filename}' using method name", file=sys.stderr)
    
    # Strategy 3: If still no matches, try to match any screenshot (fallback - show all screenshots for failed tests)
    if not matches:
        print(f"Debug: No matches found, using fallback: showing all screenshots", file=sys.stderr)
        # Sort by modification time (newest first) and take the most recent one
        all_png_files.sort(key=os.path.getmtime, reverse=True)
        if all_png_files:
            matches.append((all_png_files[0], 0))  # Low score for fallback
            print(f"Debug: Using most recent screenshot: '{os.path.basename(all_png_files[0])}'", file=sys.stderr)
    
    # Sort by match quality (number of common words) and then by modification time
    matches.sort(key=lambda x: (-x[1], os.path.getmtime(x[0])), reverse=True)
    
    # Get unique file paths (remove duplicates)
    seen_files = set()
    unique_matches = []
    for file_path, score in matches:
        if file_path not in seen_files:
            seen_files.add(file_path)
            unique_matches.append(file_path)
    
    print(f"Debug: Found {len(unique_matches)} matching screenshots", file=sys.stderr)
    
    # Copy screenshots to HTML report directory and return relative paths
    for screenshot_path in unique_matches:
        screenshot_filename = os.path.basename(screenshot_path)
        dest_path = os.path.join(html_screenshots_dir, screenshot_filename)
        try:
            shutil.copy2(screenshot_path, dest_path)
            # Return relative path from HTML report
            screenshots.append(os.path.join('screenshots', screenshot_filename))
            print(f"Debug: Copied screenshot to HTML report: {screenshot_filename}", file=sys.stderr)
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
                # Ensure forward slashes and URL-encode the path properly
                safe_path = screenshot_rel_path.replace('\\', '/')
                # URL encode the path to handle any special characters
                url_encoded_path = urllib.parse.quote(safe_path, safe='/')
                escaped_name = html.escape(screenshot_name)
                
                # Use absolute path from HTML file location
                # The HTML file is in TestResults/html/, screenshots are in TestResults/html/screenshots/
                # So relative path should be "screenshots/filename.png"
                if not safe_path.startswith('screenshots/'):
                    # Extract just the filename if full path is provided
                    safe_path = os.path.join('screenshots', screenshot_name)
                    url_encoded_path = urllib.parse.quote(safe_path, safe='/')
                
                screenshots_html += f'''<div class="screenshot-container">
                        <div class="screenshot-title">Screenshot: {escaped_name}</div>
                        <a href="{url_encoded_path}" target="_blank" class="screenshot-link">
                            <img src="{url_encoded_path}" alt="Screenshot: {escaped_name}" class="screenshot-img" style="max-width: 800px; display: block; margin-top: 10px;" onerror="this.style.display='none'; this.parentElement.innerHTML='<div style=\\'color: red;\\'>Failed to load image: {escaped_name}</div>';" />
                        </a>
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
