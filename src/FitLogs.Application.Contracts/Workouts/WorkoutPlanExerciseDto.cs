using System;
using Volo.Abp.Application.Dtos;

namespace FitLogs.Workouts;

public class WorkoutPlanExerciseDto : EntityDto<Guid>
{
    public Guid WorkoutPlanId { get; set; }
    public Guid ExerciseId { get; set; }
    
    public int OrderIndex { get; set; }
    public int DefaultSets { get; set; }
    public int DefaultReps { get; set; }
    public float? DefaultWeightKg { get; set; }
    public int? RestSeconds {get; set;}
    public string? Note { get; set; }
}