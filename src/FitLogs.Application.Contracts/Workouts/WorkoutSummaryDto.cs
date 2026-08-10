using System;

namespace FitLogs.Workouts;

public class WorkoutSummaryDto
{
    public Guid WorkoutSessionId { get; set; }
    public string Name { get; set; } = null!;
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public int DurationInMinutes { get; set; }
    
    public int TotalExercises { get; set; }
    public int CompletedExercises { get; set; }
    public int SkippedExercises { get; set; }
    public int TotalSets { get; set; }
    public int CompletedSets { get; set; }
    
    public decimal TotalVolume { get; set; }
}