using System;
using System.Threading.Tasks;
using FitLogs.Foods.FoodProducts;
using Volo.Abp.Application.Services;

namespace FitLogs.Foods;

public interface IFoodProductLookupAppService : IApplicationService
{
    Task<FoodProductLookupResultDto> LookupByBarcodeAsync(string barcode);
    Task<FoodProductDto> RefreshFromOpenFoodFactsAsync(Guid foodProductId);
}