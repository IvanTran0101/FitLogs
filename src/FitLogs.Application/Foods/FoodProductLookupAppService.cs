using System;
using System.Threading.Tasks;
using FitLogs.Foods.FoodProducts;
using FitLogs.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Services;

namespace FitLogs.Foods;

public class FoodProductLookupAppService : ApplicationService, IFoodProductLookupAppService
{
    private readonly IFoodProductRepository _foodProductRepository;

    private readonly FoodProductManager _foodProductManager;

    private readonly IOpenFoodFactsClient _openFoodFactsClient;

    public FoodProductLookupAppService(IFoodProductRepository foodProductRepository, FoodProductManager foodProductManager, IOpenFoodFactsClient openFoodFactsClient)
    {
        _foodProductRepository = foodProductRepository;
        _foodProductManager = foodProductManager;
        _openFoodFactsClient = openFoodFactsClient;
    }

    [Authorize]
    public async Task<FoodProductLookupResultDto> LookupByBarcodeAsync(string barcode)
    {
        // This method first uses the local catalog, then imports a missing product once it is found upstream.
        var normalizedBarcode = NormalizeBarcode(barcode);
        var cachedProduct = await _foodProductRepository.FindByBarcodeAsync(normalizedBarcode);
        if (cachedProduct != null)
        {
            return MapToLookupResult(cachedProduct, fromCache: true);
        }
        var externalProduct = await _openFoodFactsClient.GetByBarcodeAsync(normalizedBarcode);
        if (externalProduct == null)
        {
            return new FoodProductLookupResultDto
            {
                Found = false,

                FromCache = false,

                Barcode = normalizedBarcode
            };
        }
        var foodProduct = await _foodProductManager.CreateAsync(

            normalizedBarcode,

            externalProduct.Name,

            externalProduct.Brand,

            externalProduct.ImageUrl,

            externalProduct.CaloriesPer100g,

            externalProduct.ProteinPer100g,

            externalProduct.CarbPer100g,

            externalProduct.FatPer100g,

            externalProduct.ServingSize,

            FoodProductSource.OpenFoodFacts,

            Clock.Now

        );
        await _foodProductRepository.InsertAsync(foodProduct, autoSave: true);

        return MapToLookupResult(foodProduct, fromCache: false);
        
    }
    /// <summary>Refreshes an existing catalog product from Open Food Facts for authorized catalog managers.</summary>
    [Authorize(FitLogsPermissions.FoodProducts.Update)]
    public async Task<FoodProductDto> RefreshFromOpenFoodFactsAsync(Guid foodProductId)

    {

        var foodProduct = await _foodProductRepository.GetAsync(foodProductId);

        if (string.IsNullOrWhiteSpace(foodProduct.Barcode))

        {

            throw new BusinessException(FitLogsDomainErrorCodes.FoodProductBarcodeInvalid);

        }

        var normalizedBarcode = NormalizeBarcode(foodProduct.Barcode);

        var externalProduct = await _openFoodFactsClient.GetByBarcodeAsync(normalizedBarcode);

        if (externalProduct == null)

        {

            throw new BusinessException(FitLogsDomainErrorCodes.FoodProductNotFoundFromOpenFoodFacts);

        }

        _foodProductManager.SyncFromOpenFoodFacts(

            foodProduct,

            externalProduct.Name,

            externalProduct.Brand,

            externalProduct.ImageUrl,

            externalProduct.CaloriesPer100g,

            externalProduct.ProteinPer100g,

            externalProduct.CarbPer100g,

            externalProduct.FatPer100g,

            externalProduct.ServingSize,

            Clock.Now

        );

        await _foodProductRepository.UpdateAsync(foodProduct, autoSave: true);

        return ObjectMapper.Map<FoodProduct, FoodProductDto>(foodProduct);

    }
    /// <summary>Rejects blank barcodes and trims harmless surrounding whitespace.</summary>
    private static string NormalizeBarcode(string? barcode)

    {

        if (string.IsNullOrWhiteSpace(barcode))

        {

            throw new BusinessException(FitLogsDomainErrorCodes.FoodProductBarcodeInvalid);

        }

        return barcode.Trim();

    }

    /// <summary>Builds the stable API response shared by cache hits and upstream imports.</summary>
    private static FoodProductLookupResultDto MapToLookupResult(

        FoodProduct foodProduct,

        bool fromCache)

    {

        return new FoodProductLookupResultDto

        {

            Found = true,

            FromCache = fromCache,

            FoodProductId = foodProduct.Id,

            Barcode = foodProduct.Barcode,

            Name = foodProduct.Name,

            Brand = foodProduct.Brand,

            ImageUrl = foodProduct.ImageUrl,

            CaloriesPer100g = foodProduct.CaloriesPer100g,

            ProteinPer100g = foodProduct.ProteinPer100g,

            CarbPer100g = foodProduct.CarbPer100g,

            FatPer100g = foodProduct.FatPer100g,

            ServingSize = foodProduct.ServingSize

        };

    }
}
