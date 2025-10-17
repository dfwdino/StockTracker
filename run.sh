#!/bin/bash

# Stock Tracker Application Launcher
# This script helps you run the Stock Tracker application

echo "Stock Tracker Application"
echo "========================"

# Check if .NET 8.0 is installed
if ! command -v dotnet &> /dev/null; then
    echo "Error: .NET 8.0 SDK is not installed."
    echo "Please install .NET 8.0 SDK from: https://dotnet.microsoft.com/download/dotnet/8.0"
    exit 1
fi

# Check .NET version
DOTNET_VERSION=$(dotnet --version | cut -d. -f1)
if [ "$DOTNET_VERSION" -lt "8" ]; then
    echo "Error: .NET 8.0 or higher is required. Current version: $(dotnet --version)"
    exit 1
fi

# Check if API key is configured
if grep -q "YOUR_API_KEY_HERE" StockTracker/appsettings.json; then
    echo "Warning: Alpha Vantage API key is not configured!"
    echo "Please edit StockTracker/appsettings.json and replace 'YOUR_API_KEY_HERE' with your actual API key."
    echo "Get a free API key at: https://www.alphavantage.co/support/#api-key"
    echo ""
    read -p "Do you want to continue anyway? (y/N): " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        exit 1
    fi
fi

# Build the application
echo "Building application..."
cd StockTracker
if ! dotnet build --configuration Release; then
    echo "Error: Build failed!"
    exit 1
fi

echo ""
echo "Starting Stock Tracker..."
echo "Press Ctrl+C to stop the application"
echo ""

# Run the application
dotnet run --configuration Release
