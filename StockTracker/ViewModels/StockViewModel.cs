using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StockTracker.Domain.Entities;
using StockTracker.Domain.Services;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

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

        // persisted UI flag
        [ObservableProperty]
        private bool isMinimized;

        // Computed helper for XAML (no converter needed)
        public bool IsExpanded => !IsMinimized;

        // Persisted/minimized formatted values (show these in the minimized bar)
        public string MinimizedTotalInvestmentFormatted =>
            (_stock.MinimizedTotalInvestment != 0m ? _stock.MinimizedTotalInvestment : TotalInvestment)
            .ToString("C2", CultureInfo.CurrentCulture);

        public string MinimizedCurrentPriceFormatted =>
            (_stock.MinimizedCurrentPrice != 0m ? _stock.MinimizedCurrentPrice : CurrentPrice)
            .ToString("C2", CultureInfo.CurrentCulture);

        public StockViewModel(Stock stock, IStockManagementService stockManagementService)
        {
            _stock = stock ?? throw new ArgumentNullException(nameof(stock));
            _stockManagementService = stockManagementService ?? throw new ArgumentNullException(nameof(stockManagementService));

            // initialize ViewModel state from domain model
            IsMinimized = _stock.IsMinimized;

            Purchases = new ObservableCollection<PurchaseViewModel>(
                stock.Purchases.Select(p => new PurchaseViewModel(p)).OrderByDescending(p => p.PurchaseDate)
            );
        }

        // Notify IsExpanded when IsMinimized changes
        partial void OnIsMinimizedChanged(bool value)
        {
            OnPropertyChanged(nameof(IsExpanded));
            OnPropertyChanged(nameof(MinimizedTotalInvestmentFormatted));
            OnPropertyChanged(nameof(MinimizedCurrentPriceFormatted));
        }

        [RelayCommand]
        public async Task ToggleMinimizedAsync()
        {
            try
            {
                IsMinimized = !IsMinimized;
                _stock.IsMinimized = IsMinimized;

                // capture the current summary into the persisted fields so the minimized bar shows stable values when loaded
                _stock.MinimizedTotalInvestment = _stock.GetTotalInvestment();
                _stock.MinimizedCurrentPrice = _stock.CurrentPrice;

                await _stockManagementService.SaveStockAsync(_stock);

                // raise formatted properties
                OnPropertyChanged(nameof(MinimizedTotalInvestmentFormatted));
                OnPropertyChanged(nameof(MinimizedCurrentPriceFormatted));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving minimized state for {Symbol}: {ex.Message}");
            }
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

                // keep ViewModel state in sync with domain
                IsMinimized = _stock.IsMinimized;

                // update minimized formatted values
                OnPropertyChanged(nameof(MinimizedTotalInvestmentFormatted));
                OnPropertyChanged(nameof(MinimizedCurrentPriceFormatted));
            }
            catch (Exception ex)
            {
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

            // Keep UI flag in sync
            IsMinimized = updatedStock.IsMinimized;

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
            OnPropertyChanged(nameof(MinimizedTotalInvestmentFormatted));
            OnPropertyChanged(nameof(MinimizedCurrentPriceFormatted));
        }
    }
}
