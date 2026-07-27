using System;
using Volo.Abp.Application.Dtos;

namespace FitLogs.Foods.FoodProducts;

public class FoodProductDto : EntityDto<Guid>
{
    public string? Barcode { get; set; }
    public string Name { get; set; } = default!;
    public string? Brand { get; set; }
    public string? ImageUrl { get; set; }

    public decimal CaloriesPer100g { get; set; }
    public decimal? ProteinPer100g { get; set; }
    public decimal? CarbPer100g { get; set; }
    public decimal? FatPer100g { get; set; }

    public string? ServingSize { get; set; }
    public FoodProductSource Source { get; set; }
    public DateTime? LastSyncedAt { get; set; }

    public bool IsActive { get; set; }
    public bool IsVerified { get; set; }
}