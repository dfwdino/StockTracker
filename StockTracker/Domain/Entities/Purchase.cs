using System;

namespace StockTracker.Domain.Entities
{
    public class Purchase
    {
        public decimal PricePerShare { get; private set; }
        public decimal Quantity { get; private set; }
        public string PurchaseDate { get; private set; }
        public decimal TotalCost => PricePerShare * Quantity;

        public Purchase(decimal pricePerShare, decimal quantity, string purchaseDate)
        {
            if (pricePerShare <= 0)
                throw new ArgumentException("Price per share must be greater than zero", nameof(pricePerShare));
            
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

            PricePerShare = pricePerShare;
            Quantity = quantity;
            PurchaseDate = purchaseDate;
        }
    }
}
