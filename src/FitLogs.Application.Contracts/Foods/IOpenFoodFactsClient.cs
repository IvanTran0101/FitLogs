using System.Threading.Tasks;

namespace FitLogs.Foods;

public interface IOpenFoodFactsClient
{
    Task<OpenFoodFactsProductResult?> GetByBarcodeAsync(string barcode);
}