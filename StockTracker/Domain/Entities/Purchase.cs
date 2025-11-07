namespace StockTracker.Domain.Entities
{
    public class Purchase
    {
        public decimal PricePerShare { get; private set; }
        public decimal Quantity { get; private set; }
        public string PurchaseDate { get; private set; }
        public decimal TotalCost => PricePerShare * Quantity;
        public bool IsDividend { get; private set; } = false;

        public Purchase(decimal pricePerShare, decimal quantity, string purchaseDate, bool isdividend = false)
        {
            if (pricePerShare <= 0)
                throw new ArgumentException("Price per share must be greater than zero", nameof(pricePerShare));

            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

            PricePerShare = pricePerShare;
            Quantity = quantity;
            PurchaseDate = purchaseDate;
            IsDividend = isdividend;
        }
    }
}
