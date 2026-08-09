using System.ComponentModel.DataAnnotations;

namespace FitLogs.Foods.FoodProducts;

public class CreateUpdateFoodProductDto
{
    [StringLength(FoodProductConsts.MaxBarcodeLength)]
    public string? Barcode { get; set; }

    [Required]
    [StringLength(FoodProductConsts.MaxNameLength)]
    public string Name { get; set; } = default!;

    [StringLength(FoodProductConsts.MaxBrandLength)]
    public string? Brand { get; set; }

    [StringLength(FoodProductConsts.MaxImageUrlLength)]
    public string? ImageUrl { get; set; }

    // Legacy field names are retained for API compatibility; the denominator is NutritionBasisAmount/NutritionBasisUnit.
    [Range(typeof(decimal), "0", "999999")]
    public decimal? CaloriesPer100g { get; set; }

    [Range(typeof(decimal), "0", "999999")]
    public decimal? ProteinPer100g { get; set; }

    [Range(typeof(decimal), "0", "999999")]
    public decimal? CarbPer100g { get; set; }

    [Range(typeof(decimal), "0", "999999")]
    public decimal? FatPer100g { get; set; }

    [Range(typeof(decimal), "0.01", "999999")]
    public decimal NutritionBasisAmount { get; set; } = 100m;

    public NutritionBasisUnit NutritionBasisUnit { get; set; } = NutritionBasisUnit.Gram;

    [Range(typeof(decimal), "0.01", "999999")]
    public decimal? ServingAmount { get; set; }

    public NutritionBasisUnit? ServingUnit { get; set; }

    [Range(typeof(decimal), "0.01", "999999")]
    public decimal? PieceWeightGrams { get; set; }

    [StringLength(FoodProductConsts.MaxServingSizeLength)]
    public string? ServingSize { get; set; }
}
