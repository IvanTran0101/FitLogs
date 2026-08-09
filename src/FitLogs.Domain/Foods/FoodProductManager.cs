using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace FitLogs.Foods;

public class FoodProductManager : DomainService
{
    private readonly IFoodProductRepository _foodProductRepository;

    public FoodProductManager(IFoodProductRepository foodProductRepository)
    {
        _foodProductRepository = foodProductRepository;
    }

    public async Task<FoodProduct> CreateAsync(
        string? barcode,
        string name,
        string? brand,
        string? imageUrl,
        decimal? caloriesPer100g,
        decimal? proteinPer100g,
        decimal? carbPer100g,
        decimal? fatPer100g,
        string? servingSize,
        FoodProductSource source,
        DateTime? lastSyncedAt = null,
        decimal nutritionBasisAmount = 100m,
        NutritionBasisUnit nutritionBasisUnit = NutritionBasisUnit.Gram,
        decimal? servingAmount = null,
        NutritionBasisUnit? servingUnit = null,
        decimal? pieceWeightGrams = null)
    {
        barcode = NormalizeBarcodeOrNull(barcode);
        await CheckBarcodeAsync(barcode);

        return new FoodProduct(
            GuidGenerator.Create(),
            barcode,
            name,
            brand,
            imageUrl,
            caloriesPer100g,
            proteinPer100g,
            carbPer100g,
            fatPer100g,
            servingSize,
            source,
            lastSyncedAt,
            nutritionBasisAmount,
            nutritionBasisUnit,
            servingAmount,
            servingUnit,
            pieceWeightGrams
        );
    }

    public async Task ChangeBarcodeAsync(
        FoodProduct foodProduct,
        string? barcode)
    {
        barcode = NormalizeBarcodeOrNull(barcode);
        await CheckBarcodeAsync(barcode, foodProduct.Id);

        foodProduct.UpdateBarcode(barcode);
    }

    public void ChangeDisplayInfo(
        FoodProduct foodProduct,
        string name,
        string? brand,
        string? imageUrl,
        string? servingSize)
    {
        foodProduct.UpdateDisplayInfo(
            name,
            brand,
            imageUrl,
            servingSize
        );
    }

    public void ChangeManualNutrition(
        FoodProduct foodProduct,
        decimal? caloriesPer100g,
        decimal? proteinPer100g,
        decimal? carbPer100g,
        decimal? fatPer100g,
        decimal? nutritionBasisAmount = null,
        NutritionBasisUnit? nutritionBasisUnit = null,
        decimal? servingAmount = null,
        NutritionBasisUnit? servingUnit = null,
        decimal? pieceWeightGrams = null)
    {
        foodProduct.UpdateManualNutrition(
            caloriesPer100g,
            proteinPer100g,
            carbPer100g,
            fatPer100g,
            nutritionBasisAmount,
            nutritionBasisUnit,
            servingAmount,
            servingUnit,
            pieceWeightGrams
        );
    }

    public void SyncFromOpenFoodFacts(
        FoodProduct foodProduct,
        string name,
        string? brand,
        string? imageUrl,
        decimal? caloriesPer100g,
        decimal? proteinPer100g,
        decimal? carbPer100g,
        decimal? fatPer100g,
        string? servingSize,
        DateTime syncedAt,
        FoodProductDataQuality? dataQuality = null,
        decimal? nutritionBasisAmount = null,
        NutritionBasisUnit? nutritionBasisUnit = null,
        decimal? servingAmount = null,
        NutritionBasisUnit? servingUnit = null,
        decimal? pieceWeightGrams = null)
    {
        foodProduct.UpdateFromOpenFoodFacts(
            name,
            brand,
            imageUrl,
            caloriesPer100g,
            proteinPer100g,
            carbPer100g,
            fatPer100g,
            servingSize,
            syncedAt,
            dataQuality,
            nutritionBasisAmount,
            nutritionBasisUnit,
            servingAmount,
            servingUnit,
            pieceWeightGrams
        );
    }

    public void Activate(FoodProduct foodProduct)
    {
        foodProduct.Activate();
    }

    public void Deactivate(FoodProduct foodProduct)
    {
        foodProduct.Deactivate();
    }

    public void MarkAsVerified(FoodProduct foodProduct)
    {
        foodProduct.MarkAsVerified();
    }

    public void MarkAsUnverified(FoodProduct foodProduct)
    {
        foodProduct.MarkAsUnverified();
    }

    private async Task CheckBarcodeAsync(
        string? barcode,
        Guid? excludedId = null)
    {
        if (string.IsNullOrWhiteSpace(barcode))
        {
            return;
        }
        if (!FoodBarcodeNormalizer.TryNormalize(barcode, out var normalizedBarcode))
        {
            throw new BusinessException(FitLogsDomainErrorCodes.FoodProductBarcodeInvalid);
        }
        if (await _foodProductRepository.BarcodeExistsAsync(normalizedBarcode, excludedId))
        {
            throw new BusinessException(FitLogsDomainErrorCodes.FoodProductBarcodeAlreadyExists);
        }
    }

    private static string? NormalizeBarcodeOrNull(string? barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode))
        {
            return null;
        }

        return FoodBarcodeNormalizer.TryNormalize(barcode, out var normalizedBarcode)
            ? normalizedBarcode
            : throw new BusinessException(FitLogsDomainErrorCodes.FoodProductBarcodeInvalid);
    }
}
