using System;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using FitLogs.Foods;

namespace FitLogs.ExternalServices.OpenFoodFacts;

public class OpenFoodFactsClient : IOpenFoodFactsClient
{
    private const int MaxAttempts = 2;
    private readonly HttpClient _httpClient;
    private readonly OpenFoodFactsCircuitBreaker _circuitBreaker;

    public OpenFoodFactsClient(HttpClient httpClient, OpenFoodFactsCircuitBreaker circuitBreaker)
    {
        _httpClient = httpClient;
        _circuitBreaker = circuitBreaker;
    }

    public async Task<OpenFoodFactsProductResult?> GetByBarcodeAsync(string barcode)
    {
        if (!_circuitBreaker.TryEnter())
        {
            throw new OpenFoodFactsUpstreamException(
                OpenFoodFactsFailureKind.Unavailable,
                "Open Food Facts circuit is temporarily open.");
        }

        for (var attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            try
            {
                using var response = await _httpClient.GetAsync(
                    $"api/v2/product/{Uri.EscapeDataString(barcode)}.json");

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    _circuitBreaker.RecordSuccess();
                    return null;
                }

                if (!response.IsSuccessStatusCode)
                {
                    if (IsTransient(response.StatusCode) && attempt < MaxAttempts)
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(100 * attempt));
                        continue;
                    }

                    _circuitBreaker.RecordTransientFailure();
                    throw new OpenFoodFactsUpstreamException(
                        OpenFoodFactsFailureKind.Unavailable,
                        $"Open Food Facts returned HTTP {(int)response.StatusCode}.");
                }

                var payload = await response.Content.ReadFromJsonAsync<OpenFoodFactsResponse>();
                if (payload == null)
                {
                    throw InvalidData("The Open Food Facts response was empty.");
                }

                if (payload.Status != 1 || payload.Product == null)
                {
                    _circuitBreaker.RecordSuccess();
                    return null;
                }

                var product = payload.Product;
                if (string.IsNullOrWhiteSpace(product.ProductName))
                {
                    throw InvalidData("The Open Food Facts product has no name.");
                }

                ValidateNutriments(product.Nutriments);
                var name = product.ProductName.Trim();
                if (name.Length > FoodProductConsts.MaxNameLength)
                {
                    throw InvalidData("The Open Food Facts product name is too long.");
                }

                var result = new OpenFoodFactsProductResult
                {
                    Barcode = barcode,
                    Name = name,
                    Brand = Normalize(product.Brands, FoodProductConsts.MaxBrandLength),
                    ImageUrl = Normalize(product.ImageUrl, FoodProductConsts.MaxImageUrlLength),
                    ServingSize = Normalize(product.ServingSize, FoodProductConsts.MaxServingSizeLength),
                    CaloriesPer100g = product.Nutriments?.EnergyKcal100g,
                    ProteinPer100g = product.Nutriments?.Proteins100g,
                    CarbPer100g = product.Nutriments?.Carbohydrates100g,
                    FatPer100g = product.Nutriments?.Fat100g
                };
                result.DataQuality = result.CaloriesPer100g.HasValue &&
                                     result.ProteinPer100g.HasValue &&
                                     result.CarbPer100g.HasValue &&
                                     result.FatPer100g.HasValue
                    ? FoodProductDataQuality.Complete
                    : FoodProductDataQuality.Partial;

                _circuitBreaker.RecordSuccess();
                return result;
            }
            catch (OpenFoodFactsUpstreamException exception)
            {
                // A malformed payload means the host responded; only transient transport failures affect the circuit.
                if (exception.Kind == OpenFoodFactsFailureKind.InvalidData)
                {
                    _circuitBreaker.RecordSuccess();
                }

                throw;
            }
            catch (TaskCanceledException exception)
            {
                if (attempt < MaxAttempts)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100 * attempt));
                    continue;
                }

                _circuitBreaker.RecordTransientFailure();
                throw new OpenFoodFactsUpstreamException(
                    OpenFoodFactsFailureKind.Timeout,
                    "Open Food Facts timed out.",
                    exception);
            }
            catch (HttpRequestException exception)
            {
                if (attempt < MaxAttempts)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100 * attempt));
                    continue;
                }

                _circuitBreaker.RecordTransientFailure();
                throw new OpenFoodFactsUpstreamException(
                    OpenFoodFactsFailureKind.Unavailable,
                    "Open Food Facts was unavailable.",
                    exception);
            }
            catch (JsonException exception)
            {
                throw new OpenFoodFactsUpstreamException(
                    OpenFoodFactsFailureKind.InvalidData,
                    "Open Food Facts returned invalid JSON.",
                    exception);
            }
            catch (NotSupportedException exception)
            {
                throw new OpenFoodFactsUpstreamException(
                    OpenFoodFactsFailureKind.InvalidData,
                    "Open Food Facts returned an unsupported payload.",
                    exception);
            }
        }

        throw new OpenFoodFactsUpstreamException(
            OpenFoodFactsFailureKind.Unavailable,
            "Open Food Facts request failed.");
    }

    private static bool IsTransient(HttpStatusCode statusCode) =>
        statusCode == HttpStatusCode.RequestTimeout ||
        (int)statusCode == 429 ||
        (int)statusCode >= 500;

    private static void ValidateNutriments(OpenFoodFactsNutriments? nutriments)
    {
        if (nutriments == null)
        {
            return;
        }

        if (nutriments.EnergyKcal100g < 0 || nutriments.Proteins100g < 0 ||
            nutriments.Carbohydrates100g < 0 || nutriments.Fat100g < 0)
        {
            throw InvalidData("Open Food Facts returned a negative nutrient value.");
        }
    }

    private static string? Normalize(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = value.Trim();
        return normalized.Length <= maxLength
            ? normalized
            : throw InvalidData("Open Food Facts returned an overlong text field.");
    }

    private static OpenFoodFactsUpstreamException InvalidData(string message) =>
        new(OpenFoodFactsFailureKind.InvalidData, message);
}
