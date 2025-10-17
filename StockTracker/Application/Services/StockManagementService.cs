using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StockTracker.Domain.Entities;
using StockTracker.Domain.Repositories;
using StockTracker.Domain.Services;

namespace StockTracker.Application.Services
{
    public class StockManagementService : IStockManagementService
    {
        private readonly IStockRepository _stockRepository;
        private readonly IStockPriceService _stockPriceService;

        public StockManagementService(IStockRepository stockRepository, IStockPriceService stockPriceService)
        {
            _stockRepository = stockRepository ?? throw new ArgumentNullException(nameof(stockRepository));
            _stockPriceService = stockPriceService ?? throw new ArgumentNullException(nameof(stockPriceService));
        }

        public async Task<IEnumerable<Stock>> GetAllStocksAsync()
        {
            return await _stockRepository.GetAllAsync();
        }

        public async Task<Stock> GetStockAsync(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("Symbol cannot be null or empty", nameof(symbol));

            return await _stockRepository.GetBySymbolAsync(symbol.ToUpperInvariant());
        }

        public async Task AddPurchaseAsync(string symbol, decimal pricePerShare, decimal quantity, string purchaseDate)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("Symbol cannot be null or empty", nameof(symbol));

            if (pricePerShare <= 0)
                throw new ArgumentException("Price per share must be greater than zero", nameof(pricePerShare));

            //if (quantity <= 0)
              //  throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

            var stock = await _stockRepository.GetBySymbolAsync(symbol.ToUpperInvariant());
            stock.AddPurchase(pricePerShare, quantity, purchaseDate);
            await _stockRepository.SaveAsync(stock);
        }

        public async Task UpdateStockPriceAsync(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("Symbol cannot be null or empty", nameof(symbol));

            try
            {
                var currentPrice = await _stockPriceService.GetCurrentPriceAsync(symbol.ToUpperInvariant());
                var stock = await _stockRepository.GetBySymbolAsync(symbol.ToUpperInvariant());
                stock.UpdateCurrentPrice(currentPrice);
                await _stockRepository.SaveAsync(stock);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update stock price for {symbol}: {ex.Message}", ex);
            }
        }

        public async Task DeleteStockAsync(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("Symbol cannot be null or empty", nameof(symbol));

            await _stockRepository.DeleteAsync(symbol.ToUpperInvariant());
        }

        public async Task<bool> StockExistsAsync(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                return false;

            return await _stockRepository.ExistsAsync(symbol.ToUpperInvariant());
        }
    }
}
