using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StockTracker.Domain.Entities;

namespace StockTracker.Domain.Services
{
    public interface IStockManagementService
    {
        Task<IEnumerable<Stock>> GetAllStocksAsync();
        Task<Stock> GetStockAsync(string symbol);
        Task AddPurchaseAsync(string symbol, decimal pricePerShare, decimal quantity, string purchaseDate);
        Task UpdateStockPriceAsync(string symbol);
        Task DeleteStockAsync(string symbol);
        Task<bool> StockExistsAsync(string symbol);
    }
}
