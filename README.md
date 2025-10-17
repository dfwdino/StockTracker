# Stock Tracker

A Linux desktop application for tracking stock investments built with C# and Avalonia UI, following Domain-Driven Design (DDD) architecture principles.

## Features

- **Stock Management**: Add and track multiple stocks with purchase history
- **Real-time Pricing**: Get current stock prices from Alpha Vantage API
- **Performance Tracking**: View total gain/loss and latest purchase performance
- **File-based Storage**: Each stock is stored in its own text file (e.g., `tgt.txt` for Target)
- **Modern GUI**: Clean, responsive interface built with Avalonia UI
- **Linux Compatible**: Designed specifically for Linux desktop environments

## Architecture

The application follows Domain-Driven Design (DDD) principles with clear separation of concerns:

- **Domain Layer**: Contains entities (`Stock`, `Purchase`) and repository interfaces
- **Application Layer**: Contains application services and business logic
- **Infrastructure Layer**: Contains repository implementations and external service integrations
- **Presentation Layer**: Contains ViewModels and Views for the GUI

## Prerequisites

- .NET 8.0 SDK
- Alpha Vantage API key (free at https://www.alphavantage.co/support/#api-key)

## Setup

### Quick Setup (Recommended)

1. **Run the setup script**
   ```bash
   cd /home/shane/StockTracker
   ./setup.sh
   ```

2. **Run the application**
   ```bash
   ./run.sh
   ```

### Manual Setup

1. **Prerequisites**
   - Install .NET 8.0 SDK from [Microsoft's website](https://dotnet.microsoft.com/download/dotnet/8.0)
   - Get a free Alpha Vantage API key from [Alpha Vantage](https://www.alphavantage.co/support/#api-key)

2. **Configure API Key**
   - Open `StockTracker/appsettings.json`
   - Replace `YSW8NM3HQE03IFHS2` with your Alpha Vantage API key:
   ```json
   {
     "AlphaVantage": {
       "ApiKey": "SW8NM3HQE03IFHS2"
     }
   }
   ```

3. **Build and Run**
   ```bash
   cd StockTracker
   dotnet restore
   dotnet build
   dotnet run
   ```

## Usage

1. **Add a Stock Purchase**:
   - Enter stock symbol (e.g., AAPL, MSFT, GOOGL)
   - Enter the price you paid per share
   - Enter the quantity purchased
   - Select the purchase date
   - Click "Add Purchase"

2. **View Performance**:
   - The app automatically loads all your stocks
   - View total investment, current value, and gain/loss
   - See performance for your latest purchase
   - View complete purchase history

3. **Refresh Prices**:
   - Click "Refresh Price" on individual stocks
   - Or click "Refresh All" to update all stock prices

4. **Delete Stocks**:
   - Click "Delete Stock" to remove a stock and all its data

## Data Storage

Each stock is stored in its own text file in the `Data` directory:
- File format: `{symbol}.txt` (e.g., `aapl.txt`, `tgt.txt`)
- Format: JSON with purchase history and current price information
- Location: `StockTracker/Data/` directory

## API Usage

The application uses the Alpha Vantage API for stock prices:
- **Endpoint**: Global Quote (GLOBAL_QUOTE)
- **Rate Limits**: 5 calls per minute, 500 calls per day (free tier)
- **Data**: Real-time stock prices for the last trading day

## Project Structure

```
StockTracker/
├── Domain/
│   ├── Entities/          # Stock and Purchase entities
│   ├── Repositories/      # Repository interfaces
│   └── Services/          # Domain service interfaces
├── Application/
│   └── Services/          # Application services
├── Infrastructure/
│   ├── Repositories/      # File-based repository implementations
│   └── Services/          # External API service implementations
├── ViewModels/            # MVVM ViewModels
├── Views/                 # Avalonia UI Views
└── Data/                  # Stock data files (created at runtime)
```

## Dependencies

- **Avalonia UI**: Cross-platform UI framework
- **Microsoft.Extensions**: Dependency injection and configuration
- **CommunityToolkit.Mvvm**: MVVM toolkit with source generators
- **Newtonsoft.Json**: JSON serialization
- **System.Text.Json**: Modern JSON serialization

## Scripts

The project includes helpful scripts:

- **`setup.sh`**: Automated setup script that checks dependencies, restores packages, builds the application, and helps configure the API key
- **`run.sh`**: Application launcher that checks dependencies, builds the app, and runs it with proper error handling

Both scripts are executable and provide helpful feedback during setup and execution.

## Troubleshooting

1. **API Key Issues**:
   - Ensure your Alpha Vantage API key is correctly set in `appsettings.json`
   - Check that you haven't exceeded the API rate limits (5 calls/minute, 500/day for free tier)

2. **Build Issues**:
   - Ensure you have .NET 8.0 SDK installed
   - Run `dotnet restore` to restore packages
   - Use the `setup.sh` script for automated setup

3. **Data Issues**:
   - Check that the `Data` directory has write permissions
   - Verify stock symbols are valid (use standard ticker symbols like AAPL, MSFT, GOOGL)

4. **Runtime Issues**:
   - The application requires a GUI environment (X11 or Wayland)
   - Ensure you have proper graphics drivers installed for your Linux distribution

## License

This project is for educational and personal use. Please respect Alpha Vantage API terms of service.
