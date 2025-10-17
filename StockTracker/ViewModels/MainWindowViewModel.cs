using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StockTracker.Domain.Entities;
using StockTracker.Domain.Services;

namespace StockTracker.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly IStockManagementService _stockManagementService;

        [ObservableProperty]
        private ObservableCollection<StockViewModel> stocks = new();

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string statusMessage = "Ready";

        [ObservableProperty]
        private string newSymbol = string.Empty;

        [ObservableProperty]
        private decimal newPrice;

        [ObservableProperty]
        private decimal newQuantity;// = 1;

        [ObservableProperty]
        private string newPurchaseDate = DateTime.Today.ToString("yyyy-MM-dd");

        public MainWindowViewModel(IStockManagementService stockManagementService)
        {
            _stockManagementService = stockManagementService ?? throw new ArgumentNullException(nameof(stockManagementService));
        }

        [RelayCommand]
        private async Task LoadStocksAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Loading stocks...";

                var stocks = await _stockManagementService.GetAllStocksAsync();
                Stocks.Clear();

                foreach (var stock in stocks)
                {
                    var stockViewModel = new StockViewModel(stock, _stockManagementService);
                    Stocks.Add(stockViewModel);
                }

                StatusMessage = $"Loaded {Stocks.Count} stocks";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading stocks: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task AddPurchaseAsync()
        {
            if (string.IsNullOrWhiteSpace(NewSymbol))
            {
                StatusMessage = "Please enter a stock symbol";
                return;
            }

            try
            {
                IsLoading = true;
                StatusMessage = "Adding purchase...";

                await _stockManagementService.AddPurchaseAsync(NewSymbol, NewPrice, NewQuantity, NewPurchaseDate);
                
                // Refresh the stocks list
                await LoadStocksAsync();
                
                // Reset form
                NewSymbol = string.Empty;
                //NewPrice = 0;
                //NewQuantity = 0;
                NewPurchaseDate = DateTime.Today.ToString("yyyy-MM-dd");

                StatusMessage = "Purchase added successfully";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error adding purchase: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task RefreshAllPricesAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Refreshing stock prices...";

                var tasks = Stocks.Select(async stock => await stock.RefreshPriceAsync());
                await Task.WhenAll(tasks);

                StatusMessage = "All prices refreshed";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error refreshing prices: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task DeleteStockAsync(StockViewModel stockViewModel)
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Deleting stock...";

                await _stockManagementService.DeleteStockAsync(stockViewModel.Symbol);
                Stocks.Remove(stockViewModel);

                StatusMessage = $"Stock {stockViewModel.Symbol} deleted";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error deleting stock: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
