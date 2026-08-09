using System;
using System.ComponentModel.DataAnnotations;

namespace FitLogs.Workouts;

/// <summary>Command used to start an active session from an existing workout plan.</summary>
public class StartWorkoutFromPlanDto
{
    [Required]
    public Guid WorkoutPlanId { get; set; }

    public DateTime? StartedAt { get; set; }

    [StringLength(WorkoutSessionConsts.MaxNoteLength)]
    public string? Note { get; set; }
}
