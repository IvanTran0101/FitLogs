using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FitLogs.Foods;
using FitLogs.Foods.FoodProducts;
using FitLogs.Permissions;
using Microsoft.AspNetCore.Authorization;
using Shouldly;
using Xunit;

namespace FitLogs.Tests;

/// <summary>Guards the food catalog boundary so a future refactor cannot silently make shared data public or writable.</summary>
public class FoodAuthorizationTests
{
    [Fact]
    public void Food_product_methods_should_require_the_expected_permissions()
    {
        var expectedPolicies = new Dictionary<string, string>
        {
            [nameof(FoodProductAppService.CreateAsync)] = FitLogsPermissions.FoodProducts.Create,
            [nameof(FoodProductAppService.UpdateAsync)] = FitLogsPermissions.FoodProducts.Update,
            [nameof(FoodProductAppService.DeactivateAsync)] = FitLogsPermissions.FoodProducts.Update,
            [nameof(FoodProductAppService.ActivateAsync)] = FitLogsPermissions.FoodProducts.Update,
            [nameof(FoodProductAppService.DeleteAsync)] = FitLogsPermissions.FoodProducts.Delete,
            [nameof(FoodProductAppService.VerifyAsync)] = FitLogsPermissions.FoodProducts.Verify,
            [nameof(FoodProductAppService.UnverifyAsync)] = FitLogsPermissions.FoodProducts.Verify
        };

        foreach (var expectedPolicy in expectedPolicies)
        {
            GetMethodPolicy(typeof(FoodProductAppService), expectedPolicy.Key)
                .ShouldBe(expectedPolicy.Value, expectedPolicy.Key);
        }

        GetMethodPolicy(typeof(FoodProductAppService), nameof(FoodProductAppService.GetAsync)).ShouldBeNull();
        GetMethodPolicy(typeof(FoodProductAppService), nameof(FoodProductAppService.GetListAsync)).ShouldBeNull();
    }

    [Fact]
    public void Barcode_lookup_methods_should_require_authenticated_catalog_access()
    {
        GetMethodPolicy(
                typeof(FoodProductLookupAppService),
                nameof(FoodProductLookupAppService.LookupByBarcodeAsync))
            .ShouldBeNull();

        GetMethodPolicy(
                typeof(FoodProductLookupAppService),
                nameof(FoodProductLookupAppService.RefreshFromOpenFoodFactsAsync))
            .ShouldBe(FitLogsPermissions.FoodProducts.Update);
    }

    [Fact]
    public void Food_product_contract_should_expose_every_catalog_operation()
    {
        var serviceMethods = typeof(FoodProductAppService)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .Where(x => x.DeclaringType == typeof(FoodProductAppService))
            .Select(x => x.Name)
            .ToHashSet(StringComparer.Ordinal);

        var contractMethods = typeof(IFoodProductAppService)
            .GetMethods()
            .Select(x => x.Name)
            .ToHashSet(StringComparer.Ordinal);

        serviceMethods.SetEquals(contractMethods).ShouldBeTrue();
    }

    private static string? GetMethodPolicy(Type serviceType, string methodName)
    {
        // Reading the attribute directly keeps this test independent from a running HTTP host.
        return serviceType
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .Single(x => x.Name == methodName)
            .GetCustomAttribute<AuthorizeAttribute>()?
            .Policy;
    }
}
