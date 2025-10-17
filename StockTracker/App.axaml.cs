using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StockTracker.Application.Services;
using StockTracker.Domain.Repositories;
using StockTracker.Domain.Services;
using StockTracker.Infrastructure.Repositories;
using StockTracker.Infrastructure.Services;
using StockTracker.ViewModels;
using StockTracker.Views;
using System;
using System.IO;
using System.Net.Http;

namespace StockTracker
{
    public partial class App : Avalonia.Application
    {
        private IHost? _host;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Build configuration
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                // Build host with dependency injection
                _host = Host.CreateDefaultBuilder()
                    .ConfigureServices((context, services) =>
                    {
                        // Configuration
                        services.AddSingleton<IConfiguration>(configuration);

                        // HTTP Client
                        services.AddHttpClient();

                        // Repositories
                        var dataDirectory = configuration["Application:DataDirectory"] ?? "Data";
                        services.AddSingleton<IStockRepository>(provider => 
                            new FileStockRepository(dataDirectory));

                        // Services
                        services.AddScoped<IStockPriceService>(provider =>
                        {
                            var httpClient = provider.GetRequiredService<IHttpClientFactory>().CreateClient();
                            var apiKey = configuration["AlphaVantage:ApiKey"];
                            
                            if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_API_KEY_HERE")
                            {
                                throw new InvalidOperationException(
                                    "Alpha Vantage API key is not configured. Please set your API key in appsettings.json");
                            }
                            
                            return new AlphaVantageStockPriceService(httpClient, apiKey);
                        });

                        services.AddScoped<IStockManagementService, StockManagementService>();

                        // ViewModels
                        services.AddTransient<MainWindowViewModel>();

                        // Views
                        services.AddTransient<MainWindow>();
                    })
                    .Build();

                // Start the host
                _host.Start();

                // Create and show main window
                var mainWindowViewModel = _host.Services.GetRequiredService<MainWindowViewModel>();
                var mainWindow = _host.Services.GetRequiredService<MainWindow>();
                mainWindow.DataContext = mainWindowViewModel;

                desktop.MainWindow = mainWindow;

                // Load stocks when the window is shown
                mainWindow.Loaded += async (sender, e) =>
                {
                    await mainWindowViewModel.LoadStocksCommand.ExecuteAsync(null);
                };

                desktop.ShutdownRequested += OnShutdownRequested;
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void OnShutdownRequested(object? sender, ShutdownRequestedEventArgs e)
        {
            _host?.StopAsync().GetAwaiter().GetResult();
            _host?.Dispose();
        }
    }
}
