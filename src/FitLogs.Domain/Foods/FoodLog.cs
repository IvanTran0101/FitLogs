using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp;
using System;
namespace FitLogs.Foods;

public class FoodLog : FullAuditedAggregateRoot<Guid>
{
    public Guid UserId { get; private set; }
    public Guid FoodProductId { get; private set; }
    
    public string FoodName { get; private set; }
    public decimal Quantity { get; private set; }
    public FoodUnit Unit { get; private set; }
    
    public decimal Calories { get; private set; }
    public decimal? Protein { get; private set; }
    public decimal? Carb { get; private set; }
    public decimal? Fat  { get; private set; }
    
    public MealType MealType { get; private set; }
    public DateTime LoggedAt { get; private set; }
    public string? Note { get; private set; }
    
    protected FoodLog() { }

    public FoodLog(Guid id,
        Guid userId,
        Guid foodProductId,
        string foodName,
        decimal quantity,
        FoodUnit unit,
        decimal calories,
        decimal? protein,
        decimal? carb,
        decimal? fat,
        MealType mealType,
        DateTime loggedAt,
        string? note = null) : base(id)
    {
        SetUserId(userId);
        SetFoodProductId(foodProductId);
        SetFoodName(foodName);
        SetQuantity(quantity);
        SetUnit(unit);
        SetNutrition(calories, protein, carb, fat);
        SetMealType(mealType);
        SetLoggedAt(loggedAt);
        SetNote(note);
    }

    public void Update(
        Guid foodProductId,
        string foodName,
        decimal quantity,
        FoodUnit unit,
        decimal calories,
        decimal? protein,
        decimal? carb,
        decimal? fat,
        MealType mealType,
        DateTime loggedAt,
        string? note = null)
    {
        SetFoodProductId(foodProductId);
        SetFoodName(foodName);
        SetQuantity(quantity);
        SetUnit(unit);
        SetNutrition(calories, protein, carb, fat);
        SetMealType(mealType);
        SetLoggedAt(loggedAt);
        SetNote(note);
    }

    public void UpdateNutrition(
        decimal calories,
        decimal? protein,
        decimal? carb,
        decimal? fat)
    {
        SetNutrition(calories, protein, carb, fat);
    }

    public void ChangeQuantity(decimal quantity)
    {
        SetQuantity(quantity);
    }

    public void ChangeMealType(MealType mealType)
    {
        SetMealType(mealType);
    }

    public void ChangeLoggedAt(DateTime loggedAt)
    {
        SetLoggedAt(loggedAt);
    }

    public void ChangeNote(string? note)
    {
        SetNote(note);
    }

    private void SetUserId(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.FoodLogUserIdRequired);
        }

        UserId = userId;
    }

    private void SetFoodProductId(Guid foodProductId)
    {
        if (foodProductId == Guid.Empty)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.FoodLogFoodProductIdRequired);
        }

        FoodProductId = foodProductId;
    }

    private void SetFoodName(string foodName)
    {
        FoodName = Check.NotNullOrWhiteSpace(
            foodName,
            nameof(foodName),
            FoodLogConsts.MaxFoodNameLength
        );
    }

    private void SetQuantity(decimal quantity)
    {
        if (quantity < FoodLogConsts.MinQuantity)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.FoodLogQuantityMustBeGreaterThanZero);
        }

        Quantity = quantity;
    }

    private void SetUnit(FoodUnit unit)
    {
        if (!Enum.IsDefined(typeof(FoodUnit), unit))
        {
            throw new BusinessException(FitLogsDomainErrorCodes.FoodLogUnitInvalid);
        }

        Unit = unit;
    }

    private void SetNutrition(
        decimal calories,
        decimal? protein,
        decimal? carb,
        decimal? fat)
    {
        if (calories < FoodLogConsts.MinCalories)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.FoodLogCaloriesCannotBeNegative);
        }

        if (protein < FoodLogConsts.MinProtein)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.FoodLogProteinCannotBeNegative);
        }

        if (carb < FoodLogConsts.MinCarb)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.FoodLogCarbCannotBeNegative);
        }

        if (fat < FoodLogConsts.MinFat)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.FoodLogFatCannotBeNegative);
        }

        Calories = calories;
        Protein = protein;
        Carb = carb;
        Fat = fat;
    }

    private void SetMealType(MealType mealType)
    {
        if (!Enum.IsDefined(typeof(MealType), mealType))
        {
            throw new BusinessException(FitLogsDomainErrorCodes.FoodLogMealTypeInvalid);
        }

        MealType = mealType;
    }

    private void SetLoggedAt(DateTime loggedAt)
    {
        if (loggedAt == default)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.FoodLogLoggedAtRequired);
        }

        LoggedAt = loggedAt;
    }

    private void SetNote(string? note)
    {
        Note = NormalizeOptionalText(
            note,
            FoodLogConsts.MaxNoteLength,
            nameof(note)
        );
    }

    private static string? NormalizeOptionalText(string? value, int maxLength, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return Check.Length(value.Trim(), parameterName, maxLength);
    }
}