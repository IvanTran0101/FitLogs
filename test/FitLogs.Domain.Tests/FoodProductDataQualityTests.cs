using System;
using Shouldly;
using Xunit;

namespace FitLogs.Foods;

public class FoodProductDataQualityTests
{
    [Fact]
    public void Missing_calories_remain_unknown_and_are_not_treated_as_zero()
    {
        var product = new FoodProduct(
            Guid.NewGuid(),
            "4006381333931",
            "Partial product",
            null,
            null,
            null,
            1m,
            2m,
            3m,
            null,
            FoodProductSource.OpenFoodFacts,
            DateTime.UtcNow);

        product.CaloriesPer100g.ShouldBeNull();
        product.DataQuality.ShouldBe(FoodProductDataQuality.Partial);
        Should.Throw<Volo.Abp.BusinessException>(() => FoodNutritionCalculator.Calculate(product, 100m, FoodUnit.Gram))
            .Code.ShouldBe(FitLogsDomainErrorCodes.FoodProductNutritionUnknown);
    }
}
