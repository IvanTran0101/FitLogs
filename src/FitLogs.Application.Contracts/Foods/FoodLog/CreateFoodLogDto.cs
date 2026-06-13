using System;
using System.ComponentModel.DataAnnotations;

namespace FitLogs.Foods.FoodLogs;

public class CreateFoodLogDto
{
    [Required]
    public Guid FoodProductId { get; set; }

    [Range(typeof(decimal), "0.01", "999999")]
    public decimal Quantity { get; set; }

    [Required]
    public FoodUnit Unit { get; set; }

    [Required]
    public MealType MealType { get; set; }

    public DateTime? LoggedAt { get; set; }

    [StringLength(FoodLogConsts.MaxNoteLength)]
    public string? Note { get; set; }

    public decimal? OverrideCalories { get; set; }
    public decimal? OverrideProtein { get; set; }
    public decimal? OverrideCarb { get; set; }
    public decimal? OverrideFat { get; set; }
}