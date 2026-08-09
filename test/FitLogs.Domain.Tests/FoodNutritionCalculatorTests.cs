using System;
using Shouldly;
using Volo.Abp;
using Xunit;

namespace FitLogs.Foods;

public class FoodNutritionCalculatorTests
{
    [Fact]
    public void Calculates_grams_against_the_declared_basis()
    {
        var product = CreateProduct();

        var result = FoodNutritionCalculator.Calculate(product, 50m, FoodUnit.Gram);

        result.Calories.ShouldBe(194.5m);
        result.Protein.ShouldBe(8.45m);
        result.ConversionFactor.ShouldBe(0.5m);
    }

    [Fact]
    public void Converts_servings_to_the_product_basis()
    {
        var product = CreateProduct(servingAmount: 40m, servingUnit: NutritionBasisUnit.Gram);

        var result = FoodNutritionCalculator.Calculate(product, 2m, FoodUnit.Serving);

        result.Calories.ShouldBe(311.2m);
        result.BasisAmount.ShouldBe(100m);
        result.ConversionFactor.ShouldBe(0.8m);
    }

    [Fact]
    public void Converts_pieces_using_piece_weight()
    {
        var product = CreateProduct(pieceWeightGrams: 50m);

        var result = FoodNutritionCalculator.Calculate(product, 2m, FoodUnit.Piece);

        result.Calories.ShouldBe(389m);
        result.ConversionFactor.ShouldBe(1m);
    }

    [Fact]
    public void Supports_milliliter_products_without_treating_them_as_grams()
    {
        var product = CreateProduct(
            nutritionBasisAmount: 250m,
            nutritionBasisUnit: NutritionBasisUnit.Milliliter);

        var result = FoodNutritionCalculator.Calculate(product, 125m, FoodUnit.Milliliter);

        result.Calories.ShouldBe(194.5m);
        result.BasisUnit.ShouldBe(NutritionBasisUnit.Milliliter);
    }

    [Fact]
    public void Rejects_servings_without_conversion_metadata()
    {
        var product = CreateProduct();

        var exception = Should.Throw<BusinessException>(() =>
            FoodNutritionCalculator.Calculate(product, 1m, FoodUnit.Serving));

        exception.Code.ShouldBe(FitLogsDomainErrorCodes.FoodProductServingConversionRequired);
    }

    [Fact]
    public void Rejects_units_with_an_unavailable_physical_conversion()
    {
        var product = CreateProduct(
            nutritionBasisAmount: 250m,
            nutritionBasisUnit: NutritionBasisUnit.Milliliter);

        var exception = Should.Throw<BusinessException>(() =>
            FoodNutritionCalculator.Calculate(product, 1m, FoodUnit.Gram));

        exception.Code.ShouldBe(FitLogsDomainErrorCodes.FoodLogUnitConversionUnavailable);
    }

    private static FoodProduct CreateProduct(
        decimal nutritionBasisAmount = 100m,
        NutritionBasisUnit nutritionBasisUnit = NutritionBasisUnit.Gram,
        decimal? servingAmount = null,
        NutritionBasisUnit? servingUnit = null,
        decimal? pieceWeightGrams = null)
    {
        return new FoodProduct(
            Guid.NewGuid(),
            null,
            "Oats",
            null,
            null,
            389m,
            16.9m,
            66.3m,
            6.9m,
            "40 g",
            FoodProductSource.Manual,
            null,
            nutritionBasisAmount,
            nutritionBasisUnit,
            servingAmount,
            servingUnit,
            pieceWeightGrams);
    }
}
