using System;
using System.Collections.Generic;
using System.Linq;

namespace StockTracker.Domain.Entities
{
    public class Stock
    {
        public string Symbol { get; private set; }
        public List<Purchase> Purchases { get; private set; }
        public decimal CurrentPrice { get; private set; }
        public DateTime LastUpdated { get; private set; }

        public Stock(string symbol)
        {
            Symbol = symbol?.ToUpperInvariant() ?? throw new ArgumentNullException(nameof(symbol));
            Purchases = new List<Purchase>();
        }

        public void AddPurchase(decimal price, decimal quantity, string purchaseDate)
        {
            var purchase = new Purchase(price, quantity, purchaseDate);
            Purchases.Add(purchase);
        }

        public void UpdateCurrentPrice(decimal currentPrice)
        {
            CurrentPrice = currentPrice;
            LastUpdated = DateTime.UtcNow;
        }

        public Purchase? GetLatestPurchase()
        {
            return Purchases.OrderByDescending(p => p.PurchaseDate).FirstOrDefault();
        }

        public decimal GetTotalInvestment()
        {
            return Purchases.Sum(p => p.TotalCost);
        }

        public decimal GetTotalShares()
        {
            return Purchases.Sum(p => p.Quantity);
        }

        public decimal GetCurrentValue()
        {
            return GetTotalShares() * CurrentPrice;
        }

        public decimal GetTotalGainLoss()
        {
            return GetCurrentValue() - GetTotalInvestment();
        }

        public decimal GetTotalGainLossPercentage()
        {
            var investment = GetTotalInvestment();
            if (investment == 0) return 0;
            return (GetTotalGainLoss() / investment) * 100;
        }

        public decimal GetLatestPurchaseGainLoss()
        {
            var latestPurchase = GetLatestPurchase();
            if (latestPurchase == null) return 0;
            return (CurrentPrice - latestPurchase.PricePerShare) * latestPurchase.Quantity;
        }

        public decimal GetLatestPurchaseGainLossPercentage()
        {
            var latestPurchase = GetLatestPurchase();
            if (latestPurchase == null) return 0;
            return ((CurrentPrice - latestPurchase.PricePerShare) / latestPurchase.PricePerShare) * 100;
        }
    }
}
