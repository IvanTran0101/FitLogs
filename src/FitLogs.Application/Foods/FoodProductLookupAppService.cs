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

    public FoodProductLookupAppService(
        IFoodProductRepository foodProductRepository,
        FoodProductManager foodProductManager,
        IOpenFoodFactsClient openFoodFactsClient)
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
        FoodProduct? cachedProduct;
        try
        {
            cachedProduct = await _foodProductRepository.FindByBarcodeAsync(normalizedBarcode);
        }
        catch (Exception)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.FoodProductPersistenceFailed);
        }
        if (cachedProduct != null)
        {
            return MapToLookupResult(cachedProduct, fromCache: true);
        }

        var externalProduct = await GetExternalProductAsync(normalizedBarcode);
        if (externalProduct == null)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.FoodProductUpstreamProductNotFound);
        }

        FoodProduct foodProduct;
        try
        {
            foodProduct = await _foodProductManager.CreateAsync(
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
                Clock.Now);
        }
        catch (BusinessException exception) when (exception.Code == FitLogsDomainErrorCodes.FoodProductBarcodeAlreadyExists)
        {
            try
            {
                var winner = await _foodProductRepository.FindByBarcodeAsync(normalizedBarcode);
                if (winner != null)
                {
                    return MapToLookupResult(winner, fromCache: true);
                }
            }
            catch (Exception)
            {
                throw new BusinessException(FitLogsDomainErrorCodes.FoodProductPersistenceFailed);
            }

            throw new BusinessException(FitLogsDomainErrorCodes.FoodProductPersistenceFailed);
        }
        catch (BusinessException)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.FoodProductUpstreamInvalidData);
        }
        catch (Exception)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.FoodProductPersistenceFailed);
        }

        try
        {
            var persistedProduct = await _foodProductRepository.InsertImportedOrGetExistingAsync(foodProduct);
            return MapToLookupResult(persistedProduct, fromCache: persistedProduct.Id != foodProduct.Id);
        }
        catch (Exception)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.FoodProductPersistenceFailed);
        }
    }

    /// <summary>Refreshes an existing catalog product from Open Food Facts for authorized catalog managers.</summary>
    [Authorize(FitLogsPermissions.FoodProducts.Update)]
    public async Task<FoodProductDto> RefreshFromOpenFoodFactsAsync(Guid foodProductId)
    {
        var foodProduct = await _foodProductRepository.GetAsync(foodProductId);
        var normalizedBarcode = NormalizeBarcode(foodProduct.Barcode);
        var externalProduct = await GetExternalProductAsync(normalizedBarcode);

        if (externalProduct == null)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.FoodProductUpstreamProductNotFound);
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
            Clock.Now,
            dataQuality: externalProduct.DataQuality);

        try
        {
            await _foodProductRepository.UpdateAsync(foodProduct, autoSave: true);
        }
        catch (Exception)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.FoodProductPersistenceFailed);
        }
        return ObjectMapper.Map<FoodProduct, FoodProductDto>(foodProduct);
    }

    /// <summary>Rejects unsupported barcodes and returns one canonical digit-only representation.</summary>
    private static string NormalizeBarcode(string? barcode)
    {
        if (!FoodBarcodeNormalizer.TryNormalize(barcode, out var normalizedBarcode))
        {
            throw new BusinessException(FitLogsDomainErrorCodes.FoodProductBarcodeInvalid);
        }

        return normalizedBarcode;
    }

    /// <summary>Converts upstream transport and payload failures into stable application error categories.</summary>
    private async Task<OpenFoodFactsProductResult?> GetExternalProductAsync(string barcode)
    {
        try
        {
            return await _openFoodFactsClient.GetByBarcodeAsync(barcode);
        }
        catch (OpenFoodFactsUpstreamException exception)
        {
            var errorCode = exception.Kind switch
            {
                OpenFoodFactsFailureKind.InvalidData => FitLogsDomainErrorCodes.FoodProductUpstreamInvalidData,
                OpenFoodFactsFailureKind.Timeout => FitLogsDomainErrorCodes.FoodProductUpstreamTimeout,
                _ => FitLogsDomainErrorCodes.FoodProductUpstreamUnavailable
            };
            throw new BusinessException(errorCode);
        }
    }

    /// <summary>Builds the stable API response shared by cache hits and upstream imports.</summary>
    private static FoodProductLookupResultDto MapToLookupResult(FoodProduct foodProduct, bool fromCache)
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
            ServingSize = foodProduct.ServingSize,
            DataQuality = foodProduct.DataQuality
        };
    }
}
