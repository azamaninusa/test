# Stage 1: Get Chrome from Docker image
FROM selenium/node-chrome:latest AS chrome-source

# Stage 2: Use .NET 8.0 SDK as base image
FROM mcr.microsoft.com/dotnet/sdk:8.0

# Install common tools
# IF YOU WANT TO DECREASE IMAGE'S SIZE, REMOVE THE UNNECESSARY TOOLS FROM THIS COMMAND
# Use a backslash (\), if you want to break up a command to more than one line.
# The command "rm -rf /var/lib/apt/lists/*" removes redundant files from a given layer

RUN apt-get update \
    && apt-get -y install curl \
    && apt-get -y install wget \
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

# Update appsettings.json to enable headless mode for Docker
RUN sed -i 's/"Headless": false/"Headless": true/' VaxCare.Tests/appsettings.json

# Restore and build the solution
RUN dotnet restore && dotnet build -c Release

# Set Chrome environment variables (Chrome from Docker image)
ENV CHROME_BIN=/usr/bin/google-chrome
ENV CHROME_PATH=/usr/bin/google-chrome
ENV CHROMEDRIVER_PATH=/usr/bin/chromedriver
ENV DISPLAY=:99

# Create reports directory
RUN mkdir -p /app/TestResults

# Selenium 4.x will automatically use selenium-manager to download the correct ChromeDriver
# Set entrypoint and default command for running tests with report generation
ENTRYPOINT ["dotnet", "test", "-c", "Release", "--verbosity", "normal", "--logger", "trx;LogFileName=TestResults.trx", "--results-directory", "/app/TestResults"]
CMD ["--filter", "FullyQualifiedName~GoogleSearchTest"]
