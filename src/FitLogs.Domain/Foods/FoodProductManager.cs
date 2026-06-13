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
        decimal caloriesPer100g,
        decimal? proteinPer100g,
        decimal? carbPer100g,
        decimal? fatPer100g,
        string? servingSize,
        FoodProductSource source,
        DateTime? lastSyncedAt = null)
    {
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
            lastSyncedAt
        );
    }

    public async Task ChangeBarcodeAsync(
        FoodProduct foodProduct,
        string? barcode)
    {
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
        decimal caloriesPer100g,
        decimal? proteinPer100g,
        decimal? carbPer100g,
        decimal? fatPer100g)
    {
        foodProduct.UpdateManualNutrition(
            caloriesPer100g,
            proteinPer100g,
            carbPer100g,
            fatPer100g
        );
    }

    public void SyncFromOpenFoodFacts(
        FoodProduct foodProduct,
        string name,
        string? brand,
        string? imageUrl,
        decimal caloriesPer100g,
        decimal? proteinPer100g,
        decimal? carbPer100g,
        decimal? fatPer100g,
        string? servingSize,
        DateTime syncedAt)
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
            syncedAt
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
        var normalizedBarcode = barcode.Trim();
        if (await _foodProductRepository.BarcodeExistsAsync(normalizedBarcode, excludedId))
        {
            throw new BusinessException(FitLogsDomainErrorCodes.FoodProductBarcodeAlreadyExists);
        }
    }
}