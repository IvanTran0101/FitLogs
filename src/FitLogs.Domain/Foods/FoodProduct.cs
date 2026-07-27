using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace FitLogs.Foods;

public class FoodProduct : FullAuditedAggregateRoot<Guid>
{
    public string? Barcode { get; private set; }
    public string Name { get; private set; }
    public string? Brand { get; private set; }
    public string? ImageUrl { get; private set; }
    
    public decimal CaloriesPer100g {get; private set; }
    public decimal? ProteinPer100g { get; private set; }
    public decimal? CarbPer100g { get; private set; }
    public decimal? FatPer100g {get; private set; }
    
    public string? ServingSize { get; private set; }
    public FoodProductSource Source { get; private set; }
    public DateTime? LastSyncedAt { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsVerified { get; private set; }
    protected FoodProduct()
    {
        
    }

    public FoodProduct(
        Guid id,
        string? barcode,
        string name,
        string? brand,
        string? imageUrl,
        decimal caloriesPer100G,
        decimal? proteinPer100G,
        decimal? carbPer100G,
        decimal? fatPer100G,
        string? servingSize,
        FoodProductSource source,
        DateTime? lastSyncedAt = null) : base(id)
    {
        SetBarcode(barcode);
        SetName(name);
        Brand = NormalizeOptionalText(brand, FoodProductConsts.MaxBrandLength, nameof(brand));
        ImageUrl = NormalizeOptionalText(imageUrl, FoodProductConsts.MaxImageUrlLength, nameof(imageUrl));
        SetNutrition(caloriesPer100G, proteinPer100G, carbPer100G, fatPer100G);
        SetServingSize(servingSize);
        SetSource(source);
        LastSyncedAt = lastSyncedAt;
        IsActive = true;
        SetVerifiedBySource(source);
        
    }
    private void SetVerifiedBySource(FoodProductSource source)
    {
        IsVerified = source == FoodProductSource.System;
    }
    private void SetSource(FoodProductSource source)
    {
        if (!Enum.IsDefined(typeof(FoodProductSource), source))
        {
            throw new BusinessException(FitLogsDomainErrorCodes.FoodProductSourceInvalid);
            
        }
        Source = source;
        
    }

    public void UpdateFromOpenFoodFacts(
        string name,
        string? brand,
        string? imageUrl,
        decimal caloriesPer100G,
        decimal? proteinPer100G,
        decimal? carbPer100G,
        decimal? fatPer100G,
        string? servingSize,
        DateTime syncedAt)
    {
        SetName(name);
        Brand = NormalizeOptionalText(brand, FoodProductConsts.MaxBrandLength, nameof(brand));
        ImageUrl = NormalizeOptionalText(imageUrl, FoodProductConsts.MaxImageUrlLength, nameof(imageUrl));
        SetNutrition(caloriesPer100G, proteinPer100G, carbPer100G, fatPer100G);
        SetServingSize(servingSize);
        SetSource(FoodProductSource.OpenFoodFacts);
        LastSyncedAt = syncedAt;
        IsVerified = false;
    }

    public void UpdateManualNutrition(
        decimal caloriesPer100G,
        decimal? proteinPer100G,
        decimal? carbPer100G,
        decimal? fatPer100G)
    {
        SetNutrition(caloriesPer100G, proteinPer100G, carbPer100G, fatPer100G);
        SetSource(FoodProductSource.Manual);
        IsVerified = false;
        
    }
    public void UpdateBarcode(string? barcode)
    {
        SetBarcode(barcode);
    }
    public void UpdateDisplayInfo(string name, string? brand, string? imageUrl, string? servingSize)
    {
        SetName(name);
        Brand = NormalizeOptionalText(brand, FoodProductConsts.MaxBrandLength, nameof(brand));
        ImageUrl = NormalizeOptionalText(imageUrl, FoodProductConsts.MaxImageUrlLength, nameof(imageUrl));
        SetServingSize(servingSize);
    }

    private void SetBarcode(string? barcode)
    {
        Barcode = NormalizeOptionalText(
            barcode,
            FoodProductConsts.MaxBarcodeLength,
            nameof(barcode)
        );
    }

    private void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(
            name,
            nameof(name),
            FoodProductConsts.MaxNameLength
        );
    }

    private void SetServingSize(string? servingSize)
    {
        ServingSize = NormalizeOptionalText(
            servingSize,
            FoodProductConsts.MaxServingSizeLength,
            nameof(servingSize)
        );
    }

    private void SetNutrition(
        decimal caloriesPer100G,
        decimal? proteinPer100G,
        decimal? carbPer100G,
        decimal? fatPer100G)
    {
        if (caloriesPer100G < FoodProductConsts.MinCaloriesPer100g)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.FoodProductCaloriesCannotBeNegative);
        }

        if (proteinPer100G < FoodProductConsts.MinProteinPer100g)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.FoodProductProteinCannotBeNegative);
        }

        if (carbPer100G < FoodProductConsts.MinCarbPer100g)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.FoodProductCarbCannotBeNegative);
        }

        if (fatPer100G < FoodProductConsts.MinFatPer100g)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.FoodProductFatCannotBeNegative);
        }

        CaloriesPer100g = caloriesPer100G;
        ProteinPer100g = proteinPer100G;
        CarbPer100g = carbPer100G;
        FatPer100g = fatPer100G;
    }

    private static string? NormalizeOptionalText(string? value, int maxLength, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return Check.Length(value.Trim(), parameterName, maxLength);
    }
    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
    public void MarkAsVerified()
    {
        IsVerified = true;
    }

    public void MarkAsUnverified()
    {
        IsVerified = false;
    }
}