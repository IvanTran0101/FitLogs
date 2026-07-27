using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitLogs.Foods.FoodProducts;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Linq;

namespace FitLogs.Foods;

[Authorize]
public class FoodProductAppService : ApplicationService, IFoodProductAppService
{
    private readonly IFoodProductRepository _foodProductRepository;
    private readonly IFoodLogRepository _foodLogRepository;
    private readonly FoodProductManager _foodProductManager;

    public FoodProductAppService(
        IFoodProductRepository foodProductRepository,
        IFoodLogRepository foodLogRepository,
        FoodProductManager foodProductManager)
    {
        _foodProductRepository = foodProductRepository;
        _foodLogRepository = foodLogRepository;
        _foodProductManager = foodProductManager;
    }

    public async Task<FoodProductDto> CreateAsync(CreateUpdateFoodProductDto input)
    {
        var foodProduct = await _foodProductManager.CreateAsync(
            input.Barcode,
            input.Name,
            input.Brand,
            input.ImageUrl,
            input.CaloriesPer100g,
            input.ProteinPer100g,
            input.CarbPer100g,
            input.FatPer100g,
            input.ServingSize,
            FoodProductSource.Manual
        );

        await _foodProductRepository.InsertAsync(foodProduct, autoSave: true);

        return ObjectMapper.Map<FoodProduct, FoodProductDto>(foodProduct);
    }

    public async Task<FoodProductDto> UpdateAsync(Guid id, CreateUpdateFoodProductDto input)
    {
        var foodProduct = await _foodProductRepository.GetAsync(id);

        await _foodProductManager.ChangeBarcodeAsync(foodProduct, input.Barcode);

        foodProduct.UpdateDisplayInfo(
            input.Name,
            input.Brand,
            input.ImageUrl,
            input.ServingSize
        );

        foodProduct.UpdateManualNutrition(
            input.CaloriesPer100g,
            input.ProteinPer100g,
            input.CarbPer100g,
            input.FatPer100g
        );

        await _foodProductRepository.UpdateAsync(foodProduct, autoSave: true);

        return ObjectMapper.Map<FoodProduct, FoodProductDto>(foodProduct);
    }

    public async Task<FoodProductDto> GetAsync(Guid id)
    {
        var foodProduct = await _foodProductRepository.GetAsync(id);

        return ObjectMapper.Map<FoodProduct, FoodProductDto>(foodProduct);
    }

    public async Task<PagedResultDto<FoodProductDto>> GetListAsync(GetFoodProductListInput input)
    {
        var queryable = await _foodProductRepository.GetQueryableAsync();

        var query = queryable;

        if (input.OnlyActive)
        {
            query = query.Where(x => x.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(input.FilterText))
        {
            var filterText = input.FilterText.Trim();

            query = query.Where(x =>
                x.Name.Contains(filterText) ||
                (x.Brand != null && x.Brand.Contains(filterText)) ||
                (x.Barcode != null && x.Barcode.Contains(filterText))
            );
        }

        var totalCount = await AsyncExecuter.CountAsync(query);

        query = ApplySorting(query, input);
        query = query.PageBy(input);

        var foodProducts = await AsyncExecuter.ToListAsync(query);

        return new PagedResultDto<FoodProductDto>(
            totalCount,
            ObjectMapper.Map<List<FoodProduct>, List<FoodProductDto>>(foodProducts)
        );
    }

    public async Task DeactivateAsync(Guid id)
    {
        var foodProduct = await _foodProductRepository.GetAsync(id);

        foodProduct.Deactivate();

        await _foodProductRepository.UpdateAsync(foodProduct, autoSave: true);
    }

    public async Task ActivateAsync(Guid id)
    {
        var foodProduct = await _foodProductRepository.GetAsync(id);

        foodProduct.Activate();

        await _foodProductRepository.UpdateAsync(foodProduct, autoSave: true);
    }

    public async Task VerifyAsync(Guid id)
    {
        var foodProduct = await _foodProductRepository.GetAsync(id);

        foodProduct.MarkAsVerified();

        await _foodProductRepository.UpdateAsync(foodProduct, autoSave: true);
    }

    public async Task UnverifyAsync(Guid id)
    {
        var foodProduct = await _foodProductRepository.GetAsync(id);

        foodProduct.MarkAsUnverified();

        await _foodProductRepository.UpdateAsync(foodProduct, autoSave: true);
    }

    public async Task DeleteAsync(Guid id)
    {
        var foodProduct = await _foodProductRepository.GetAsync(id);

        var hasFoodLogs = await _foodLogRepository.ExistsByFoodProductIdAsync(foodProduct.Id);

        if (hasFoodLogs)
        {
            foodProduct.Deactivate();
            await _foodProductRepository.UpdateAsync(foodProduct, autoSave: true);
            return;
        }

        await _foodProductRepository.DeleteAsync(foodProduct, autoSave: true);
    }

    private static IQueryable<FoodProduct> ApplySorting(
        IQueryable<FoodProduct> query,
        GetFoodProductListInput input)
    {
        if (string.IsNullOrWhiteSpace(input.Sorting))
        {
            return query.OrderBy(x => x.Name);
        }

        return input.Sorting switch
        {
            nameof(FoodProduct.Name) => query.OrderBy(x => x.Name),
            nameof(FoodProduct.Brand) => query.OrderBy(x => x.Brand),
            nameof(FoodProduct.CaloriesPer100g) => query.OrderBy(x => x.CaloriesPer100g),
            nameof(FoodProduct.CreationTime) => query.OrderBy(x => x.CreationTime),
            _ => query.OrderBy(x => x.Name)
        };
    }
}