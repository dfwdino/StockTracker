using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StockTracker.Domain.Entities;
using StockTracker.Domain.Services;
using System.Collections.ObjectModel;
using System.Globalization;

namespace StockTracker.ViewModels
{
    public partial class StockViewModel : ObservableObject
    {
        private readonly IStockManagementService _stockManagementService;
        private Stock _stock;

        public string Symbol => _stock.Symbol;
        public decimal CurrentPrice => _stock.CurrentPrice;
        public DateTime LastUpdated => _stock.LastUpdated;
        public decimal TotalInvestment => _stock.GetTotalInvestment();
        public string TotalBought => _stock.GetTotalBought()?.ToString("C2", CultureInfo.CurrentCulture);
        public decimal TotalShares => _stock.GetTotalShares();
        public decimal CurrentValue => _stock.GetCurrentValue();
        public decimal TotalGainLoss => _stock.GetTotalGainLoss();
        public decimal TotalGainLossPercentage => _stock.GetTotalGainLossPercentage();
        public decimal LatestPurchaseGainLoss => _stock.GetLatestPurchaseGainLoss();
        public decimal LatestPurchaseGainLossPercentage => _stock.GetLatestPurchaseGainLossPercentage();

        public string LatestDivTotalFormatted => _stock.GetLatestDividendTotal()?.ToString("C2", CultureInfo.CurrentCulture);

        public ObservableCollection<PurchaseViewModel> Purchases { get; }

        public string CurrentPriceFormatted => CurrentPrice.ToString("C2", CultureInfo.CurrentCulture);
        public string TotalInvestmentFormatted => TotalInvestment.ToString("C2", CultureInfo.CurrentCulture);
        public string CurrentValueFormatted => CurrentValue.ToString("C2", CultureInfo.CurrentCulture);
        public string TotalGainLossFormatted => TotalGainLoss.ToString("C2", CultureInfo.CurrentCulture);
        public string TotalGainLossPercentageFormatted => $"{TotalGainLossPercentage:F2}%";
        public string LatestPurchaseGainLossFormatted => LatestPurchaseGainLoss.ToString("C2", CultureInfo.CurrentCulture);
        public string LatestPurchaseGainLossPercentageFormatted => $"{LatestPurchaseGainLossPercentage:F2}%";
        public string LastUpdatedFormatted => LastUpdated.ToString("yyyy-MM-dd HH:mm:ss UTC");


        public string BoughtMax => _stock.GetMaxBought()?.ToString("C2", CultureInfo.CurrentCulture);
        public string BoughtMin => _stock.GetMinBought()?.ToString("C2", CultureInfo.CurrentCulture);
        public string DivMaxMin => _stock.GetMaxMinDiv();

        public string TotalGainLossColor => TotalGainLoss >= 0 ? "Green" : "Red";
        public string LatestPurchaseGainLossColor => LatestPurchaseGainLoss >= 0 ? "Green" : "Red";

        [ObservableProperty]
        private bool isRefreshing;

        public StockViewModel(Stock stock, IStockManagementService stockManagementService)
        {
            _stock = stock ?? throw new ArgumentNullException(nameof(stock));
            _stockManagementService = stockManagementService ?? throw new ArgumentNullException(nameof(stockManagementService));

            Purchases = new ObservableCollection<PurchaseViewModel>(
                stock.Purchases.Select(p => new PurchaseViewModel(p)).OrderByDescending(p => p.PurchaseDate)


            );
        }

        [RelayCommand]
        public async Task RefreshPriceAsync()
        {
            try
            {
                IsRefreshing = true;
                await _stockManagementService.UpdateStockPriceAsync(_stock.Symbol);

                // Reload the stock data
                _stock = await _stockManagementService.GetStockAsync(_stock.Symbol);

                // Notify property changes
                OnPropertyChanged(nameof(CurrentPrice));
                OnPropertyChanged(nameof(LastUpdated));
                OnPropertyChanged(nameof(CurrentValue));
                OnPropertyChanged(nameof(TotalGainLoss));
                OnPropertyChanged(nameof(TotalGainLossPercentage));
                OnPropertyChanged(nameof(LatestPurchaseGainLoss));
                OnPropertyChanged(nameof(LatestPurchaseGainLossPercentage));
                OnPropertyChanged(nameof(CurrentPriceFormatted));
                OnPropertyChanged(nameof(CurrentValueFormatted));
                OnPropertyChanged(nameof(TotalGainLossFormatted));
                OnPropertyChanged(nameof(TotalGainLossPercentageFormatted));
                OnPropertyChanged(nameof(LatestPurchaseGainLossFormatted));
                OnPropertyChanged(nameof(LatestPurchaseGainLossPercentageFormatted));
                OnPropertyChanged(nameof(LastUpdatedFormatted));
                OnPropertyChanged(nameof(TotalGainLossColor));
                OnPropertyChanged(nameof(LatestPurchaseGainLossColor));
            }
            catch (Exception ex)
            {
                // Handle error - you might want to show this in the UI
                System.Diagnostics.Debug.WriteLine($"Error refreshing price for {Symbol}: {ex.Message}");
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        public void UpdateStock(Stock updatedStock)
        {
            _stock = updatedStock;

            // Update purchases collection
            Purchases.Clear();
            foreach (var purchase in updatedStock.Purchases.OrderByDescending(p => p.PurchaseDate))
            {
                Purchases.Add(new PurchaseViewModel(purchase));
            }

            // Notify all property changes
            OnPropertyChanged(nameof(CurrentPrice));
            OnPropertyChanged(nameof(LastUpdated));
            OnPropertyChanged(nameof(TotalInvestment));
            OnPropertyChanged(nameof(TotalShares));
            OnPropertyChanged(nameof(CurrentValue));
            OnPropertyChanged(nameof(TotalGainLoss));
            OnPropertyChanged(nameof(TotalGainLossPercentage));
            OnPropertyChanged(nameof(LatestPurchaseGainLoss));
            OnPropertyChanged(nameof(LatestPurchaseGainLossPercentage));
            OnPropertyChanged(nameof(CurrentPriceFormatted));
            OnPropertyChanged(nameof(TotalInvestmentFormatted));
            OnPropertyChanged(nameof(CurrentValueFormatted));
            OnPropertyChanged(nameof(TotalGainLossFormatted));
            OnPropertyChanged(nameof(TotalGainLossPercentageFormatted));
            OnPropertyChanged(nameof(LatestPurchaseGainLossFormatted));
            OnPropertyChanged(nameof(LatestPurchaseGainLossPercentageFormatted));
            OnPropertyChanged(nameof(LastUpdatedFormatted));
            OnPropertyChanged(nameof(TotalGainLossColor));
            OnPropertyChanged(nameof(LatestPurchaseGainLossColor));
        }
    }
}
