# Stage 1: Get Chrome from Docker image
FROM selenium/node-chrome:latest AS chrome-source

# Stage 2: Use .NET 8.0 SDK as base image
FROM mcr.microsoft.com/dotnet/sdk:8.0

# Install common tools including Python for HTML report generation
# IF YOU WANT TO DECREASE IMAGE'S SIZE, REMOVE THE UNNECESSARY TOOLS FROM THIS COMMAND
# Use a backslash (\), if you want to break up a command to more than one line.
# The command "rm -rf /var/lib/apt/lists/*" removes redundant files from a given layer

RUN apt-get update \
    && apt-get -y install curl \
    && apt-get -y install wget \
    && apt-get -y install python3 \
    && rm -rf /var/lib/apt/lists/*

# Install packages required by Chrome
RUN apt-get update && apt-get install -y \
    lsb-release \
    libgtk-3-0 \
    libappindicator3-1 \
    xdg-utils \
    libxss1 \
    libnss3 \
    libnspr4 \
    libasound2 \
    libappindicator1 \
    fonts-liberation \
    libpango1.0-0 \
    libpangoxft-1.0-0 \
    libv4l-0 \
    libv4lconvert0 \
    libgl1-mesa-dri \
    libgl1-mesa-glx \
    libpulse0 \
    fonts-symbola \
    --no-install-recommends \
    && rm -rf /var/lib/apt/lists/*

# Copy Chrome and ChromeDriver from the Chrome Docker image
# Selenium images have Chrome in /usr/bin/google-chrome
COPY --from=chrome-source /usr/bin/google-chrome /usr/bin/google-chrome
COPY --from=chrome-source /usr/bin/chromedriver /usr/bin/chromedriver

# Copy Chrome dependencies and libraries
COPY --from=chrome-source /opt/google/chrome /opt/google/chrome

# Copy required shared libraries that Chrome depends on from the Chrome image
COPY --from=chrome-source /usr/lib/x86_64-linux-gnu/libgbm.so.1 /usr/lib/x86_64-linux-gnu/libgbm.so.1
COPY --from=chrome-source /usr/lib/x86_64-linux-gnu/libgbm.so.1.0.0 /usr/lib/x86_64-linux-gnu/libgbm.so.1.0.0

# Make Chrome and ChromeDriver executable and update library cache
RUN chmod +x /usr/bin/google-chrome /usr/bin/chromedriver && \
    ldconfig || true

# Add chrome user
RUN groupadd -r chrome && useradd -r -g chrome -G audio,video chrome \
    && mkdir -p /home/chrome/Downloads && chown -R chrome:chrome /home/chrome

# Set working directory
WORKDIR /app

# Copy solution file
COPY *.sln ./

# Copy project files (for better layer caching)
COPY VaxCare.ApiClient/*.csproj ./VaxCare.ApiClient/
COPY VaxCare.Core/*.csproj ./VaxCare.Core/
COPY VaxCare.Data/*.csproj ./VaxCare.Data/
COPY VaxCare.Pages/*.csproj ./VaxCare.Pages/
COPY VaxCare.Tests/*.csproj ./VaxCare.Tests/
COPY VaxCare.Tests/appsettings.json ./VaxCare.Tests/
COPY VaxCare.UnitTests/*.csproj ./VaxCare.UnitTests/

# Copy all source files
COPY VaxCare.ApiClient/ ./VaxCare.ApiClient/
COPY VaxCare.Core/ ./VaxCare.Core/
COPY VaxCare.Data/ ./VaxCare.Data/
COPY VaxCare.Pages/ ./VaxCare.Pages/
COPY VaxCare.Tests/ ./VaxCare.Tests/
COPY VaxCare.UnitTests/ ./VaxCare.UnitTests/

# Copy HTML report generation script
COPY generate-html-report.py ./

# Update appsettings.json to enable headless mode for Docker
RUN sed -i 's/"Headless": false/"Headless": true/' VaxCare.Tests/appsettings.json

# Restore and build the solution
RUN dotnet restore && dotnet build -c Release

# Set Chrome environment variables (Chrome from Docker image)
ENV CHROME_BIN=/usr/bin/google-chrome
ENV CHROME_PATH=/usr/bin/google-chrome
ENV CHROMEDRIVER_PATH=/usr/bin/chromedriver
ENV DISPLAY=:99

# Create reports directory and HTML subdirectory
RUN mkdir -p /app/TestResults/html

# Create wrapper script to run tests and generate HTML report
RUN echo '#!/bin/bash\n\
set -e\n\
\n\
# Ensure we are in the correct directory\n\
cd /app\n\
\n\
# Run tests with TRX report generation\n\
echo "========================================="\n\
echo "  Running tests..."\n\
echo "========================================="\n\
dotnet test -c Release --verbosity normal --logger "trx;LogFileName=TestResults.trx" --results-directory /app/TestResults "$@"\n\
\n\
# Generate HTML report from TRX file\n\
echo ""\n\
echo "========================================="\n\
echo "  Generating HTML report..."\n\
echo "========================================="\n\
if [ -f "/app/TestResults/TestResults.trx" ]; then\n\
    cd /app\n\
    python3 generate-html-report.py\n\
    if [ -f "/app/TestResults/html/TestReport.html" ]; then\n\
        echo ""\n\
        echo "✓ HTML report generated successfully!"\n\
        echo "  Location: /app/TestResults/html/TestReport.html"\n\
        echo ""\n\
        echo "========================================="\n\
        echo "  Test Run Complete!"\n\
        echo "========================================="\n\
        echo ""\n\
        # Create a script in TestResults that host can execute to open the report\n\
        echo "#!/bin/bash\n\
# Auto-generated script to open test report\n\
if [[ \"$OSTYPE\" == \"linux-gnu\"* ]]; then\n\
    xdg-open \"$(dirname \"$0\")/html/TestReport.html\" 2>/dev/null\n\
elif [[ \"$OSTYPE\" == \"darwin\"* ]]; then\n\
    open \"$(dirname \"$0\")/html/TestReport.html\"\n\
elif [[ \"$OSTYPE\" == \"msys\" || \"$OSTYPE\" == \"cygwin\" ]]; then\n\
    start \"$(dirname \"$0\")/html/TestReport.html\"\n\
else\n\
    echo \"Please open the report manually: $(dirname \"$0\")/html/TestReport.html\"\n\
fi\n\
" > /app/TestResults/open-report.sh\n\
        chmod +x /app/TestResults/open-report.sh\n\
        echo "To open the report, run from host:"\n\
        echo "  ./TestResults/open-report.sh"\n\
        echo "  (or macOS: open TestResults/html/TestReport.html)"\n\
        echo "  (or Linux: xdg-open TestResults/html/TestReport.html)"\n\
        echo ""\n\
    else\n\
        echo "Warning: HTML report file not found after generation"\n\
    fi\n\
else\n\
    echo "Warning: TestResults.trx not found, skipping HTML report generation"\n\
fi\n\
' > /app/run-tests-and-report.sh && chmod +x /app/run-tests-and-report.sh

# Selenium 4.x will automatically use selenium-manager to download the correct ChromeDriver
# Set entrypoint to run tests and generate HTML report
ENTRYPOINT ["/app/run-tests-and-report.sh"]
CMD ["--filter", "FullyQualifiedName~GoogleSearchTest"]
