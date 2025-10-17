using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using StockTracker.Domain.Services;

namespace StockTracker.Infrastructure.Services
{
    public class AlphaVantageStockPriceService : IStockPriceService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly JsonSerializerOptions _jsonOptions;

        public AlphaVantageStockPriceService(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<decimal> GetCurrentPriceAsync(string symbol)
        {
            try
            {
                var url = $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={symbol}&apikey={_apiKey}";
                
                var response = await _httpClient.GetStringAsync(url);
                var jsonDocument = JsonDocument.Parse(response);

                // Check for error messages in the response
                if (jsonDocument.RootElement.TryGetProperty("Error Message", out var errorMessage))
                {
                    throw new Exception($"Alpha Vantage API Error: {errorMessage.GetString()}");
                }

                if (jsonDocument.RootElement.TryGetProperty("Note", out var note))
                {
                    throw new Exception($"Alpha Vantage API Rate Limit: {note.GetString()}");
                }

                if (jsonDocument.RootElement.TryGetProperty("Global Quote", out var globalQuote))
                {
                    if (globalQuote.TryGetProperty("05. price", out var priceElement))
                    {
                        if (decimal.TryParse(priceElement.GetString(), out var price))
                        {
                            return price;
                        }
                    }
                }

                throw new Exception($"Unable to parse stock price for symbol: {symbol}");
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Network error while fetching stock price for {symbol}: {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                throw new Exception($"Invalid JSON response for {symbol}: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching stock price for {symbol}: {ex.Message}", ex);
            }
        }
    }
}
