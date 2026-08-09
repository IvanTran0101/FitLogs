namespace FitLogs.Foods;

/// <summary>Explains whether a food-log nutrition snapshot was calculated, overridden, or inherited from legacy data.</summary>
public enum FoodNutritionCalculationSource
{
    Calculated = 1,
    ManualOverride = 2,
    Legacy = 3
}
