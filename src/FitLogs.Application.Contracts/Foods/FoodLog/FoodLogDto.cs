using System;
using Volo.Abp.Application.Dtos;

namespace FitLogs.Foods.FoodLogs;

public class FoodLogDto : EntityDto<Guid>
{
    public Guid UserId { get; set; }
    public Guid FoodProductId { get; set; }

    public string FoodName { get; set; } = default!;
    public decimal Quantity { get; set; }
    public FoodUnit Unit { get; set; }

    public decimal Calories { get; set; }
    public decimal? Protein { get; set; }
    public decimal? Carb { get; set; }
    public decimal? Fat { get; set; }

    public MealType MealType { get; set; }
    public DateTime LoggedAt { get; set; }
    public string? Note { get; set; }
}