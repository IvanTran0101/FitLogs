using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;

namespace FitLogs.Foods;

public class FoodProductDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IFoodProductRepository _foodProductRepository;
    private readonly FoodProductManager _foodProductManager;

    public FoodProductDataSeedContributor(
        IFoodProductRepository foodProductRepository,
        FoodProductManager foodProductManager)
    {
        _foodProductRepository = foodProductRepository;
        _foodProductManager = foodProductManager;
    }

    [UnitOfWork]
    public async Task SeedAsync(DataSeedContext context)
    {
        await SeedProductAsync(
            barcode: "8900000000012",
            name: "Yến mạch cán dẹt",
            brand: "FitLogs Demo",
            caloriesPer100g: 389m,
            proteinPer100g: 16.9m,
            carbPer100g: 66.3m,
            fatPer100g: 6.9m,
            servingSize: "40 g");

        await SeedProductAsync(
            barcode: "8900000000029",
            name: "Ức gà",
            brand: "FitLogs Demo",
            caloriesPer100g: 165m,
            proteinPer100g: 31m,
            carbPer100g: 0m,
            fatPer100g: 3.6m,
            servingSize: "100 g");

        await SeedProductAsync(
            barcode: "8900000000036",
            name: "Trứng gà",
            brand: "FitLogs Demo",
            caloriesPer100g: 143m,
            proteinPer100g: 12.6m,
            carbPer100g: 0.7m,
            fatPer100g: 9.5m,
            servingSize: "1 quả");

        await SeedProductAsync(
            barcode: "8900000000043",
            name: "Gạo lứt nấu chín",
            brand: "FitLogs Demo",
            caloriesPer100g: 111m,
            proteinPer100g: 2.6m,
            carbPer100g: 23m,
            fatPer100g: 0.9m,
            servingSize: "150 g");

        await SeedProductAsync(
            barcode: "8900000000050",
            name: "Sữa chua Hy Lạp",
            brand: "FitLogs Demo",
            caloriesPer100g: 59m,
            proteinPer100g: 10.3m,
            carbPer100g: 3.6m,
            fatPer100g: 0.4m,
            servingSize: "100 g");

        await SeedProductAsync(
            barcode: "8900000000067",
            name: "Chuối",
            brand: "FitLogs Demo",
            caloriesPer100g: 89m,
            proteinPer100g: 1.1m,
            carbPer100g: 22.8m,
            fatPer100g: 0.3m,
            servingSize: "1 quả");

        await SeedProductAsync(
            barcode: "8900000000074",
            name: "Bơ đậu phộng",
            brand: "FitLogs Demo",
            caloriesPer100g: 588m,
            proteinPer100g: 25m,
            carbPer100g: 20m,
            fatPer100g: 50m,
            servingSize: "15 g");

        await SeedProductAsync(
            barcode: "8900000000081",
            name: "Whey protein",
            brand: "FitLogs Demo",
            caloriesPer100g: 400m,
            proteinPer100g: 80m,
            carbPer100g: 8m,
            fatPer100g: 6m,
            servingSize: "30 g");
    }

    // Creates one verified system product only when its barcode is not already present.
    private async Task SeedProductAsync(
        string barcode,
        string name,
        string brand,
        decimal caloriesPer100g,
        decimal proteinPer100g,
        decimal carbPer100g,
        decimal fatPer100g,
        string servingSize)
    {
        var existingProduct = await _foodProductRepository.FindByBarcodeAsync(barcode);
        if (existingProduct != null)
        {
            return;
        }

        var product = await _foodProductManager.CreateAsync(
            barcode,
            name,
            brand,
            imageUrl: null,
            caloriesPer100g,
            proteinPer100g,
            carbPer100g,
            fatPer100g,
            servingSize,
            FoodProductSource.System,
            lastSyncedAt: null);

        await _foodProductRepository.InsertAsync(product, autoSave: true);
    }
}
