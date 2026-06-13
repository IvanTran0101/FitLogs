using System;

namespace FitLogs.Foods.FoodProducts;

public class FoodProductLookupResultDto
{
    public bool Found { get; set; }
    public bool FromCache { get; set; }

    public Guid? FoodProductId { get; set; }
    public string? Barcode { get; set; }
    public string? Name { get; set; }
    public string? Brand { get; set; }
    public string? ImageUrl { get; set; }

    public decimal? CaloriesPer100g { get; set; }
    public decimal? ProteinPer100g { get; set; }
    public decimal? CarbPer100g { get; set; }
    public decimal? FatPer100g { get; set; }

    public string? ServingSize { get; set; }
}