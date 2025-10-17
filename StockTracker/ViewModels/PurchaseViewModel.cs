using System;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using StockTracker.Domain.Entities;

namespace StockTracker.ViewModels
{
    public partial class PurchaseViewModel : ObservableObject
    {
        private readonly Purchase _purchase;

        public decimal PricePerShare => _purchase.PricePerShare;
        public decimal Quantity => _purchase.Quantity;
        public DateTime PurchaseDate => Convert.ToDateTime(_purchase.PurchaseDate);
        public decimal TotalCost => _purchase.TotalCost;

        public string PricePerShareFormatted => PricePerShare.ToString("C2", CultureInfo.InvariantCulture);
        public string QuantityFormatted => Quantity.ToString("N2");
        public string TotalCostFormatted => TotalCost.ToString("C2", CultureInfo.InvariantCulture);
        public string PurchaseDateFormatted => PurchaseDate.ToString("yyyy-MM-dd");

        public PurchaseViewModel(Purchase purchase)
        {
            _purchase = purchase ?? throw new ArgumentNullException(nameof(purchase));
        }
    }
}
