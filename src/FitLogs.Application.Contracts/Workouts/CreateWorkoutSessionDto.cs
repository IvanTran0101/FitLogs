using System;
using System.ComponentModel.DataAnnotations;

namespace FitLogs.Workouts;

public class CreateWorkoutSessionDto
{
    public Guid? WorkoutPlanId { get; set; }

    [Required]

    [StringLength(WorkoutSessionConsts.MaxNameLength)]

    public string Name { get; set; } = string.Empty;

    public DateTime StartedAt { get; set; } = DateTime.Now;

    [StringLength(WorkoutSessionConsts.MaxNoteLength)]

    public string? Note { get; set; }
}