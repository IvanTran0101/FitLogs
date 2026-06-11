using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace FitLogs.Workouts;

public class WorkoutSessionExerciseDto : EntityDto<Guid>
{
    public Guid WorkoutSessionId { get; set; }
    public Guid ExerciseId { get; set; }
    
    public int OrderIndex { get; set; }
    public int TargetSets { get; set; }
    public int TargetReps { get; set; }
    public float? TargetWeightKg { get; set; }
    public int? RestSeconds { get; set; }
    public string? Note { get; set; }

    public List<ExerciseSetDto> Sets { get; set; } = new();
}