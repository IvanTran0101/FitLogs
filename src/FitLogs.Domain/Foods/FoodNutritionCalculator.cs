using System;
using Volo.Abp;

namespace FitLogs.Foods;

public sealed record FoodNutritionCalculation(
    decimal Calories,
    decimal? Protein,
    decimal? Carb,
    decimal? Fat,
    decimal BasisAmount,
    NutritionBasisUnit BasisUnit,
    decimal ConversionFactor);

/// <summary>Converts a logged quantity into the product's nutrition basis before multiplying nutrition values.</summary>
public static class FoodNutritionCalculator
{
    /// <summary>Calculates the nutrition snapshot for a quantity and unit selected by the user.</summary>
    public static FoodNutritionCalculation Calculate(
        FoodProduct foodProduct,
        decimal quantity,
        FoodUnit unit)
    {
        Check.NotNull(foodProduct, nameof(foodProduct));

        var (amountInBasisUnit, basisUnit) = ConvertToProductBasis(foodProduct, quantity, unit);
        var factor = amountInBasisUnit / foodProduct.NutritionBasisAmount;

        return new FoodNutritionCalculation(
            foodProduct.CaloriesPer100g * factor,
            foodProduct.ProteinPer100g * factor,
            foodProduct.CarbPer100g * factor,
            foodProduct.FatPer100g * factor,
            foodProduct.NutritionBasisAmount,
            basisUnit,
            factor);
    }

    /// <summary>Converts the requested unit into the physical unit used by the product's nutrition values.</summary>
    private static (decimal Amount, NutritionBasisUnit Unit) ConvertToProductBasis(
        FoodProduct foodProduct,
        decimal quantity,
        FoodUnit unit)
    {
        if (quantity < FoodLogConsts.MinQuantity)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.FoodLogQuantityMustBeGreaterThanZero);
        }

        return unit switch
        {
            FoodUnit.Gram => RequireMatchingBasis(foodProduct, quantity, NutritionBasisUnit.Gram),
            FoodUnit.Milliliter => RequireMatchingBasis(foodProduct, quantity, NutritionBasisUnit.Milliliter),
            FoodUnit.Serving => ConvertServing(foodProduct, quantity),
            FoodUnit.Piece => ConvertPiece(foodProduct, quantity),
            _ => throw new BusinessException(FitLogsDomainErrorCodes.FoodLogUnitInvalid)
        };
    }

    /// <summary>Converts servings by multiplying them by the product's declared serving amount.</summary>
    private static (decimal Amount, NutritionBasisUnit Unit) ConvertServing(
        FoodProduct foodProduct,
        decimal quantity)
    {
        if (!foodProduct.ServingAmount.HasValue || !foodProduct.ServingUnit.HasValue)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.FoodProductServingConversionRequired);
        }

        return RequireMatchingBasis(
            foodProduct,
            quantity * foodProduct.ServingAmount.Value,
            foodProduct.ServingUnit.Value);
    }

    /// <summary>Converts pieces by multiplying them by the product's declared gram weight.</summary>
    private static (decimal Amount, NutritionBasisUnit Unit) ConvertPiece(
        FoodProduct foodProduct,
        decimal quantity)
    {
        if (!foodProduct.PieceWeightGrams.HasValue)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.FoodProductPieceConversionRequired);
        }

        return RequireMatchingBasis(
            foodProduct,
            quantity * foodProduct.PieceWeightGrams.Value,
            NutritionBasisUnit.Gram);
    }

    /// <summary>Rejects conversions between grams and milliliters when no density information exists.</summary>
    private static (decimal Amount, NutritionBasisUnit Unit) RequireMatchingBasis(
        FoodProduct foodProduct,
        decimal amount,
        NutritionBasisUnit unit)
    {
        if (foodProduct.NutritionBasisUnit != unit)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.FoodLogUnitConversionUnavailable);
        }

        return (amount, unit);
    }
}
