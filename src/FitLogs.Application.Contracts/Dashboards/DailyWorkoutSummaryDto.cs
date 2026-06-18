namespace FitLogs.Dashboards;

public class DailyWorkoutSummaryDto
{
    public int CompletedSessions { get; set; }
    public int TotalExercises { get; set; }
    public int TotalSets { get; set; }
    public double TotalDurationMinutes { get; set; }
    public decimal TotalWeightVolume { get; set;  }
    public bool HasWorkout { get; set; }
}