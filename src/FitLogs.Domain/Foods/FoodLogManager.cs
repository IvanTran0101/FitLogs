using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace FitLogs.Foods;

public class FoodLogManager : DomainService
{
    private readonly IFoodProductRepository _foodProductRepository;

    public FoodLogManager(IFoodProductRepository foodProductRepository)
    {
        _foodProductRepository = foodProductRepository;
    }

    public async Task<FoodLog> CreateFromProductAsync(
        Guid userId,
        Guid foodProductId,
        DateTime loggedAt,
        MealType mealType,
        decimal quantity,
        FoodUnit unit,
        string? note = null)
    {
        CheckUserId(userId);
        CheckFoodProductId(foodProductId);
        CheckQuantity(quantity);

        var foodProduct = await GetActiveFoodProductAsync(foodProductId);
        var nutrition = CalculateNutrition(foodProduct, quantity);

        return new FoodLog(
            GuidGenerator.Create(),
            userId,
            foodProduct.Id,
            foodProduct.Name,
            quantity,
            unit,
            nutrition.Calories,
            nutrition.Protein,
            nutrition.Carb,
            nutrition.Fat,
            mealType,
            loggedAt,
            note
        );
    }

    public async Task UpdateFromProductAsync(
        FoodLog foodLog,
        Guid foodProductId,
        decimal quantity,
        FoodUnit unit,
        MealType mealType,
        DateTime loggedAt,
        string? note = null)
    {
        Check.NotNull(foodLog, nameof(foodLog));
        CheckFoodProductId(foodProductId);
        CheckQuantity(quantity);

        var foodProduct = await GetActiveFoodProductAsync(foodProductId);
        var nutrition = CalculateNutrition(foodProduct, quantity);

        foodLog.Update(
            foodProduct.Id,
            foodProduct.Name,
            quantity,
            unit,
            nutrition.Calories,
            nutrition.Protein,
            nutrition.Carb,
            nutrition.Fat,
            mealType,
            loggedAt,
            note
        );
    }

    public void ChangeNutrition(
        FoodLog foodLog,
        decimal calories,
        decimal? protein,
        decimal? carb,
        decimal? fat)
    {
        Check.NotNull(foodLog, nameof(foodLog));

        foodLog.UpdateNutrition(
            calories,
            protein,
            carb,
            fat
        );
    }

    public void ChangeMealType(
        FoodLog foodLog,
        MealType mealType)
    {
        Check.NotNull(foodLog, nameof(foodLog));
        foodLog.ChangeMealType(mealType);
    }

    public void ChangeLoggedAt(
        FoodLog foodLog,
        DateTime loggedAt)
    {
        Check.NotNull(foodLog, nameof(foodLog));
        foodLog.ChangeLoggedAt(loggedAt);
    }

    public void ChangeNote(
        FoodLog foodLog,
        string? note)
    {
        Check.NotNull(foodLog, nameof(foodLog));
        foodLog.ChangeNote(note);
    }

    private async Task<FoodProduct> GetActiveFoodProductAsync(Guid foodProductId)
    {
        var foodProduct = await _foodProductRepository.GetAsync(foodProductId);

        if (!foodProduct.IsActive)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.FoodProductInactive);
        }

        return foodProduct;
    }

    private static FoodNutritionValues CalculateNutrition(
        FoodProduct foodProduct,
        decimal quantity)
    {
        var factor = quantity / 100m;

        return new FoodNutritionValues(
            foodProduct.CaloriesPer100g * factor,
            foodProduct.ProteinPer100g * factor,
            foodProduct.CarbPer100g * factor,
            foodProduct.FatPer100g * factor
        );
    }

    private static void CheckUserId(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.FoodLogUserIdRequired);
        }
    }

    private static void CheckFoodProductId(Guid foodProductId)
    {
        if (foodProductId == Guid.Empty)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.FoodLogFoodProductIdRequired);
        }
    }

    private static void CheckQuantity(decimal quantity)
    {
        if (quantity < FoodLogConsts.MinQuantity)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.FoodLogQuantityMustBeGreaterThanZero);
        }
    }

    private sealed record FoodNutritionValues(
        decimal Calories,
        decimal? Protein,
        decimal? Carb,
        decimal? Fat
    );
}