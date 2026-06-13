using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace FitLogs.Foods.FoodLogs;
public interface IFoodLogAppService : IApplicationService
{
    Task<FoodLogDto> CreateAsync(CreateFoodLogDto input);

    Task<FoodLogDto> UpdateAsync(Guid id, UpdateFoodLogDto input);

    Task DeleteAsync(Guid id);

    Task<FoodLogDto> GetAsync(Guid id);

    Task<List<FoodLogDto>> GetListByDateAsync(GetFoodLogListInput input);

    Task<DailyFoodNutritionSummaryDto> GetDailySummaryAsync(GetFoodLogListInput input);
}