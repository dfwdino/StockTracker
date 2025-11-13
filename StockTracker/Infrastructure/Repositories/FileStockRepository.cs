using StockTracker.Domain.Entities;
using StockTracker.Domain.Repositories;
using System.Text.Json;
using System.Linq;

namespace StockTracker.Infrastructure.Repositories
{
    public class FileStockRepository : IStockRepository
    {
        private readonly string _dataDirectory;
        private readonly JsonSerializerOptions _jsonOptions;

        public FileStockRepository(string dataDirectory = "Data")
        {
            _dataDirectory = dataDirectory;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            if (!Directory.Exists(_dataDirectory))
            {
                Directory.CreateDirectory(_dataDirectory);
            }
        }

        public async Task<Stock> GetBySymbolAsync(string symbol)
        {
            var filePath = GetFilePath(symbol);

            if (!File.Exists(filePath))
            {
                return new Stock(symbol);
            }

            try
            {
                var json = await File.ReadAllTextAsync(filePath);
                var stockData = JsonSerializer.Deserialize<StockData>(json, _jsonOptions);

                if (stockData == null)
                {
                    return new Stock(symbol);
                }

                var stock = new Stock(symbol);

                // restore persisted values
                stock.UpdateCurrentPrice(stockData.CurrentPrice);
                stock.SetLastUpdated(stockData.LastUpdated);

                stock.IsMinimized = stockData.IsMinimized;
                stock.MinimizedTotalInvestment = stockData.MinimizedTotalInvestment;
                stock.MinimizedCurrentPrice = stockData.MinimizedCurrentPrice;

                foreach (var purchaseData in stockData.Purchases)
                {
                    stock.AddPurchase(
                        purchaseData.PricePerShare,
                        purchaseData.Quantity,
                        purchaseData.PurchaseDate,
                        purchaseData.IsDividend
                    );
                }

                return stock;
            }
            catch (Exception)
            {
                // If file is corrupted, return a new stock instance
                return new Stock(symbol);
            }
        }

        public async Task<IEnumerable<Stock>> GetAllAsync()
        {
            var stocks = new List<Stock>();

            if (!Directory.Exists(_dataDirectory))
            {
                return stocks;
            }

            var files = Directory.GetFiles(_dataDirectory, "*.txt");

            foreach (var file in files)
            {
                var symbol = Path.GetFileNameWithoutExtension(file);
                var stock = await GetBySymbolAsync(symbol);
                stocks.Add(stock);
            }

            return stocks;
        }

        public async Task SaveAsync(Stock stock)
        {
            if (stock == null) throw new ArgumentNullException(nameof(stock));

            var filePath = GetFilePath(stock.Symbol);

            var stockData = new StockData
            {
                Symbol = stock.Symbol,
                CurrentPrice = stock.CurrentPrice,
                LastUpdated = stock.LastUpdated,
                IsMinimized = stock.IsMinimized,
                MinimizedTotalInvestment = stock.MinimizedTotalInvestment,
                MinimizedCurrentPrice = stock.MinimizedCurrentPrice,
                Purchases = stock.Purchases.OrderBy(mm => mm.IsDividend).Select(p => new PurchaseData
                {
                    PricePerShare = p.PricePerShare,
                    Quantity = p.Quantity,
                    PurchaseDate = p.PurchaseDate,
                    IsDividend = p.IsDividend
                }).ToList()
            };

            var json = JsonSerializer.Serialize(stockData, _jsonOptions);
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task DeleteAsync(string symbol)
        {
            var filePath = GetFilePath(symbol);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            await Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(string symbol)
        {
            var filePath = GetFilePath(symbol);
            var exists = File.Exists(filePath);
            return await Task.FromResult(exists);
        }

        private string GetFilePath(string symbol)
        {
            return Path.Combine(_dataDirectory, $"{symbol.ToLowerInvariant()}.txt");
        }

        // Data transfer objects for serialization
        private class StockData
        {
            public string Symbol { get; set; } = string.Empty;
            public decimal CurrentPrice { get; set; }
            public DateTime LastUpdated { get; set; }
            public bool IsMinimized { get; set; } = false;
            public decimal MinimizedTotalInvestment { get; set; } = 0m;
            public decimal MinimizedCurrentPrice { get; set; } = 0m;
            public List<PurchaseData> Purchases { get; set; } = new();
        }

        private class PurchaseData
        {
            public decimal PricePerShare { get; set; }
            public decimal Quantity { get; set; }
            public string? PurchaseDate { get; set; }
            public bool IsDividend { get; set; } = false;
        }
    }
}
