using System;

namespace FitLogs.Dashboards;

public class DailyDashboardDto
{
    public DateOnly Date { get; set; }
    public DailyNutritionSummaryDto Nutrition { get; set; } = default!;
    public DailyWorkoutSummaryDto Workout { get; set; } = default!;
}