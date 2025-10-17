using System;
using System.Threading.Tasks;

namespace StockTracker.Domain.Services
{
    public interface IStockPriceService
    {
        Task<decimal> GetCurrentPriceAsync(string symbol);
    }
}
