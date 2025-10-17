using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StockTracker.Domain.Entities;

namespace StockTracker.Domain.Repositories
{
    public interface IStockRepository
    {
        Task<Stock> GetBySymbolAsync(string symbol);
        Task<IEnumerable<Stock>> GetAllAsync();
        Task SaveAsync(Stock stock);
        Task DeleteAsync(string symbol);
        Task<bool> ExistsAsync(string symbol);
    }
}
