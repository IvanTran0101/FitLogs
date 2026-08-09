using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitLogs.Foods;
using FitLogs.UserProfiles;
using FitLogs.Workouts;
using FitLogs.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Timing;
using Volo.Abp.Users;

namespace FitLogs.Dashboards;
[Authorize(FitLogsPermissions.Dashboards.Default)]
public class DashboardAppService : ApplicationService, IDashboardAppService
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IFoodLogRepository _foodLogRepository;
    private readonly IWorkoutSessionRepository _workoutSessionRepository;
    private readonly IClock _clock;

    public DashboardAppService(IUserProfileRepository userProfileRepository, IFoodLogRepository foodLogRepository, IWorkoutSessionRepository workoutSessionRepository, IClock clock)
    {
        _userProfileRepository = userProfileRepository;
        _foodLogRepository = foodLogRepository;
        _workoutSessionRepository = workoutSessionRepository;
        _clock = clock;
    }

    public async Task<DailyDashboardDto> GetTodayAsync()
    {
        var profile = await _userProfileRepository.FindByUserIdAsync(CurrentUser.GetId());
        var timeZoneId = profile?.TimeZoneId ?? UserProfileConsts.DefaultTimeZoneId;
        return await GetDailyAsync(new GetDailyDashboardInput
        {
            Date = UserTimeZone.GetLocalDate(_clock.Now, timeZoneId)
        });
        
    }

    public async Task<DailyDashboardDto> GetDailyAsync(GetDailyDashboardInput input)
    {
        var profile = await _userProfileRepository.FindByUserIdAsync(CurrentUser.GetId());
        var selectedDate = ResolveSelectedDate(input.Date, profile?.TimeZoneId);
        var nutrition = await GetDailyNutritionAsync(new GetDailyDashboardInput
        {
            Date = selectedDate
        });
        var workout = await GetDailyWorkoutAsync(new GetDailyDashboardInput
        {
            Date = selectedDate
        });
        return new DailyDashboardDto
        {
            Date = selectedDate,
            Nutrition = nutrition,
            Workout = workout
        };
    }

    public async Task<DailyNutritionSummaryDto> GetDailyNutritionAsync(GetDailyDashboardInput input)
    {
        var userId = CurrentUser.GetId();
        var userProfile = await _userProfileRepository.FindByUserIdAsync(userId);
        var selectedDate = ResolveSelectedDate(input.Date, userProfile?.TimeZoneId);
        var (startDate, endDate) = GetDateRange(selectedDate, userProfile?.TimeZoneId);
        var foodLogs = await _foodLogRepository.GetListByUserAndDateRangeAsync(
            userId,
            startDate,
            endDate);
        return BuildNutritionSummary(userProfile, foodLogs);
        
    }

    public async Task<DailyWorkoutSummaryDto> GetDailyWorkoutAsync(GetDailyDashboardInput input)
    {
        var userId = CurrentUser.GetId();
        var userProfile = await _userProfileRepository.FindByUserIdAsync(userId);
        var selectedDate = ResolveSelectedDate(input.Date, userProfile?.TimeZoneId);
        var (startDate, endDate) = GetDateRange(selectedDate, userProfile?.TimeZoneId);
        var workoutMetrics = await _workoutSessionRepository.GetCompletedMetricsByUserAndDateRangeAsync(
            userId,
            startDate,
            endDate);
        return BuildWorkoutSummary(workoutMetrics);
        
    }

    private DailyNutritionSummaryDto BuildNutritionSummary(
        UserProfile? userProfile,
        List<FoodLog> foodLogs)
    {
        var totalCalories = foodLogs.Sum(x=> x.Calories );
        var totalProtein = foodLogs.Sum(x=> x.Protein ?? 0m);
        var totalCarbs = foodLogs.Sum(x => x.Carb ?? 0m);
        var totalFat = foodLogs.Sum(x => x.Fat ?? 0m);
        var dailyCaloriesTarget = userProfile?.DailyTargetCalories;

        return new DailyNutritionSummaryDto
        {
            HasNutritionData = foodLogs.Any(),
            DailyCaloriesTarget = dailyCaloriesTarget,
            HasCaloriesTarget = dailyCaloriesTarget.HasValue,
            TotalCalories = totalCalories,
            RemainingCalories = dailyCaloriesTarget.HasValue
                ? dailyCaloriesTarget.Value - totalCalories
                : null,
            TotalProtein = totalProtein,
            TotalCarbs = totalCarbs,
            TotalFat = totalFat,
            CaloriesByMealType = foodLogs.GroupBy(x => x.MealType)
                .Select(g => new MealCaloriesBreakdownDto
                {
                    MealType = g.Key,
                    Calories = g.Sum(x => x.Calories ),
                    Protein = g.Sum(x => x.Protein ?? 0m),
                    Carbs = g.Sum(x => x.Carb ?? 0m),
                    Fat = g.Sum(x => x.Fat ?? 0m)
                })
                .OrderBy(x => x.MealType)
                .ToList()
        };
    }

    private static DailyWorkoutSummaryDto BuildWorkoutSummary(
        List<CompletedWorkoutSessionMetric> workoutSessions)
    {
        return new DailyWorkoutSummaryDto
        {
            HasWorkout = workoutSessions.Any(),
            CompletedSessions = workoutSessions.Count,
            TotalExercises = workoutSessions.Sum(x => x.CompletedExercises),
            TotalSets = workoutSessions.Sum(x => x.CompletedSets),
            TotalDurationMinutes = workoutSessions
                .Sum(x => (x.CompletedAt - x.StartedAt).TotalMinutes),
            TotalWeightVolume = workoutSessions.Sum(x => x.CompletedVolume)
        };
    }

    /// <summary>Uses the user's local date when the caller does not provide an explicit calendar date.</summary>
    private DateOnly ResolveSelectedDate(DateOnly? date, string? timeZoneId)
    {
        return date ?? UserTimeZone.GetLocalDate(
            _clock.Now,
            string.IsNullOrWhiteSpace(timeZoneId)
                ? UserProfileConsts.DefaultTimeZoneId
                : timeZoneId);
    }

    private static (DateTime StartDate, DateTime EndDate) GetDateRange(DateOnly selectedDate, string? timeZoneId)
    {
        return UserTimeZone.GetUtcDateRange(
            selectedDate,
            string.IsNullOrWhiteSpace(timeZoneId)
                ? UserProfileConsts.DefaultTimeZoneId
                : timeZoneId);
    }
}
