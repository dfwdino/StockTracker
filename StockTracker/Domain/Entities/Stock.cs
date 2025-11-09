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

        public void AddPurchase(decimal price, decimal quantity, string purchaseDate, bool isdividend = false)
        {
            var purchase = new Purchase(price, quantity, purchaseDate, isdividend);
            Purchases.Add(purchase);
        }

        public void UpdateCurrentPrice(decimal currentPrice)
        {
            CurrentPrice = currentPrice;
            LastUpdated = DateTime.UtcNow;
        }

        public decimal? GetMaxBought()
        {
            return Purchases.Where(p => p.IsDividend == false).Max(p => (decimal?)p.PricePerShare);
        }

        public decimal? GetMinBought()
        {
            return Purchases.Where(p => p.IsDividend == false).Min(p => (decimal?)p.PricePerShare);
        }

        public string GetMaxMinDiv()
        {
            return string.Concat(Purchases.Where(p => p.IsDividend == true).Max(p => (decimal?)p.PricePerShare).ToString(), "-", Purchases.Where(p => p.IsDividend == true).Min(p => (decimal?)p.PricePerShare).ToString());
        }


        public Purchase? GetLatestPurchase()
        {
            return Purchases.OrderByDescending(p => p.PurchaseDate).FirstOrDefault();
        }


        public decimal? GetTotalBought()
        {
            return Purchases.Where(p => p.IsDividend == false).Sum(p => p.TotalCost);
        }


        public decimal? GetLatestDividendTotal()
        {
            return Purchases.Where(p => p.IsDividend == true).Sum(p => p.TotalCost);
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
