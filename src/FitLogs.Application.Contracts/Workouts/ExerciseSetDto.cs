using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace FitLogs.Workouts;

public class ExerciseSetDto : EntityDto<Guid>
{
    public Guid WorkoutSessionExerciseId { get; set; }

    public int SetNumber { get; set; }

    public float WeightKg { get; set; }

    public int Reps { get; set; }

    public int? Rpe { get; set; }

    public string? Note { get; set; }

    public bool IsCompleted { get; set; }

    public DateTime? CompletedAt { get; set; }
}