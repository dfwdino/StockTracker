#!/bin/bash

# Stock Tracker Setup Script
# This script helps you set up the Stock Tracker application

echo "Stock Tracker Setup"
echo "=================="

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

echo "✓ .NET $(dotnet --version) is installed"

# Restore packages
echo "Restoring NuGet packages..."
cd StockTracker
if ! dotnet restore; then
    echo "Error: Failed to restore packages"
    exit 1
fi

echo "✓ Packages restored successfully"

# Build the application
echo "Building application..."
if ! dotnet build; then
    echo "Error: Build failed"
    exit 1
fi

echo "✓ Application built successfully"

# Check API key configuration
echo ""
echo "Checking API key configuration..."
if grep -q "YOUR_API_KEY_HERE" appsettings.json; then
    echo "⚠️  Alpha Vantage API key is not configured"
    echo ""
    echo "To configure your API key:"
    echo "1. Get a free API key at: https://www.alphavantage.co/support/#api-key"
    echo "2. Edit appsettings.json"
    echo "3. Replace 'YOUR_API_KEY_HERE' with your actual API key"
    echo ""
    read -p "Do you want to configure the API key now? (y/N): " -n 1 -r
    echo
    if [[ $REPLY =~ ^[Yy]$ ]]; then
        read -p "Enter your Alpha Vantage API key: " API_KEY
        if [ ! -z "$API_KEY" ]; then
            sed -i "s/YOUR_API_KEY_HERE/$API_KEY/g" appsettings.json
            echo "✓ API key configured successfully"
        else
            echo "⚠️  No API key entered. You can configure it later in appsettings.json"
        fi
    fi
else
    echo "✓ API key is configured"
fi

echo ""
echo "Setup complete! You can now run the application with:"
echo "  ./run.sh"
echo ""
echo "Or manually with:"
echo "  cd StockTracker && dotnet run"
