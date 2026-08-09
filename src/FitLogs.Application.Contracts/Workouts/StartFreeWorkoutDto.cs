using System;
using System.ComponentModel.DataAnnotations;

namespace FitLogs.Workouts;

/// <summary>Command used to start an active session without a plan.</summary>
public class StartFreeWorkoutDto
{
    [Required]
    [StringLength(WorkoutSessionConsts.MaxNameLength)]
    public string Name { get; set; } = string.Empty;

    public DateTime? StartedAt { get; set; }

    [StringLength(WorkoutSessionConsts.MaxNoteLength)]
    public string? Note { get; set; }
}
