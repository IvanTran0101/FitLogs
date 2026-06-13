namespace FitLogs.Foods;

public class OpenFoodFactsProductResult
{
    public string Barcode { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string? Brand { get; set; }

    public string? ImageUrl { get; set; }

    public decimal CaloriesPer100g { get; set; }

    public decimal? ProteinPer100g { get; set; }

    public decimal? CarbPer100g { get; set; }

    public decimal? FatPer100g { get; set; }

    public string? ServingSize { get; set; }
}