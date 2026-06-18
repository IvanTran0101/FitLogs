using System.Net.Http.Json;
using FitLogs.Foods;

namespace FitLogs.ExternalServices.OpenFoodFacts;

public class OpenFoodFactsClient : IOpenFoodFactsClient
{
    private readonly HttpClient _httpClient;
    public OpenFoodFactsClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<OpenFoodFactsProductResult?> GetByBarcodeAsync(string barcode)
    {
        var response = await _httpClient.GetFromJsonAsync<OpenFoodFactsResponse>(
            $"api/v2/product/{barcode}.json");
        if (response?.Status != 1 || response.Product == null)
        {
            return null;
        }
        var product = response.Product;
        if (string.IsNullOrWhiteSpace(product.ProductName))
        {
            return null;
        }

        return new OpenFoodFactsProductResult

        {

            Barcode = barcode,

            Name = product.ProductName.Trim(),

            Brand = Normalize(product.Brands),

            ImageUrl = Normalize(product.ImageUrl),

            ServingSize = Normalize(product.ServingSize),

            CaloriesPer100g = product.Nutriments?.EnergyKcal100g ?? 0,

            ProteinPer100g = product.Nutriments?.Proteins100g,

            CarbPer100g = product.Nutriments?.Carbohydrates100g,

            FatPer100g = product.Nutriments?.Fat100g

        };
            
    }
    
    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}