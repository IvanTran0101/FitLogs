using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitLogs.Foods.FoodLogs;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Users;

namespace FitLogs.Foods;

[Authorize]
public class FoodLogAppService : ApplicationService, IFoodLogAppService
{
    private readonly IFoodLogRepository _foodLogRepository;
    private readonly FoodLogManager _foodLogManager;

    public FoodLogAppService(
        IFoodLogRepository foodLogRepository,
        FoodLogManager foodLogManager)
    {
        _foodLogRepository = foodLogRepository;
        _foodLogManager = foodLogManager;
    }

    public async Task<FoodLogDto> CreateAsync(CreateFoodLogDto input)
    {
        var foodLog = await _foodLogManager.CreateFromProductAsync(
            CurrentUser.GetId(),
            input.FoodProductId,
            input.LoggedAt ?? Clock.Now,
            input.MealType,
            input.Quantity,
            input.Unit,
            input.Note
        );

        ApplyNutritionOverrides(foodLog, input.OverrideCalories, input.OverrideProtein, input.OverrideCarb, input.OverrideFat);

        await _foodLogRepository.InsertAsync(foodLog, autoSave: true);

        return ObjectMapper.Map<FoodLog, FoodLogDto>(foodLog);
    }

    public async Task<FoodLogDto> UpdateAsync(Guid id, UpdateFoodLogDto input)
    {
        var foodLog = await _foodLogRepository.GetAsync(id);

        EnsureOwner(foodLog);

        await _foodLogManager.UpdateFromProductAsync(
            foodLog,
            input.FoodProductId,
            input.Quantity,
            input.Unit,
            input.MealType,
            input.LoggedAt ?? foodLog.LoggedAt,
            input.Note
        );

        ApplyNutritionOverrides(foodLog, input.OverrideCalories, input.OverrideProtein, input.OverrideCarb, input.OverrideFat);

        await _foodLogRepository.UpdateAsync(foodLog, autoSave: true);

        return ObjectMapper.Map<FoodLog, FoodLogDto>(foodLog);
    }

    public async Task DeleteAsync(Guid id)
    {
        var foodLog = await _foodLogRepository.GetAsync(id);

        EnsureOwner(foodLog);

        await _foodLogRepository.DeleteAsync(foodLog, autoSave: true);
    }

    public async Task<FoodLogDto> GetAsync(Guid id)
    {
        var foodLog = await _foodLogRepository.GetAsync(id);

        EnsureOwner(foodLog);

        return ObjectMapper.Map<FoodLog, FoodLogDto>(foodLog);
    }

    public async Task<List<FoodLogDto>> GetListByDateAsync(GetFoodLogListInput input)
    {
        var foodLogs = await _foodLogRepository.GetListByUserAndDateAsync(
            CurrentUser.GetId(),
            input.Date
        );

        return ObjectMapper.Map<List<FoodLog>, List<FoodLogDto>>(foodLogs);
    }

    public async Task<DailyFoodNutritionSummaryDto> GetDailySummaryAsync(GetFoodLogListInput input)
    {
        var foodLogs = await _foodLogRepository.GetListByUserAndDateAsync(
            CurrentUser.GetId(),
            input.Date
        );

        return new DailyFoodNutritionSummaryDto
        {
            Date = input.Date.Date,
            TotalCalories = foodLogs.Sum(x => x.Calories),
            TotalProtein = foodLogs.Sum(x => x.Protein ?? 0),
            TotalCarb = foodLogs.Sum(x => x.Carb ?? 0),
            TotalFat = foodLogs.Sum(x => x.Fat ?? 0)
        };
    }

    private void EnsureOwner(FoodLog foodLog)
    {
        if (foodLog.UserId != CurrentUser.GetId())
        {
            throw new BusinessException(FitLogsDomainErrorCodes.FoodLogAccessDenied);
        }
    }

    private void ApplyNutritionOverrides(
        FoodLog foodLog,
        decimal? overrideCalories,
        decimal? overrideProtein,
        decimal? overrideCarb,
        decimal? overrideFat)
    {
        if (!overrideCalories.HasValue &&
            !overrideProtein.HasValue &&
            !overrideCarb.HasValue &&
            !overrideFat.HasValue)
        {
            return;
        }

        _foodLogManager.ChangeNutrition(
            foodLog,
            overrideCalories ?? foodLog.Calories,
            overrideProtein ?? foodLog.Protein,
            overrideCarb ?? foodLog.Carb,
            overrideFat ?? foodLog.Fat
        );
    }
}