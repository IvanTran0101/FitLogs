using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace FitLogs.Dashboards;

public interface IDashboardAppService : IApplicationService
{
    Task<DailyDashboardDto> GetTodayAsync();
    Task<DailyDashboardDto> GetDailyAsync(GetDailyDashboardInput input);
    Task<DailyNutritionSummaryDto> GetDailyNutritionAsync(GetDailyDashboardInput input);
    Task<DailyWorkoutSummaryDto> GetDailyWorkoutAsync(GetDailyDashboardInput input);
}