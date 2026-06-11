using System;
using System.ComponentModel.DataAnnotations;

namespace FitLogs.Workouts;

public class CreateWorkoutPlanExerciseDto
{
    public Guid ExerciseId { get; set; }

    [Range(1, int.MaxValue)]

    public int OrderIndex { get; set; }

    [Range(1, int.MaxValue)]

    public int DefaultSets { get; set; }

    [Range(1, int.MaxValue)]

    public int DefaultReps { get; set; }

    [Range(0, float.MaxValue)]

    public float? DefaultWeightKg { get; set; }

    [Range(0, int.MaxValue)]

    public int? RestSeconds { get; set; }

    [StringLength(WorkoutPlanExerciseConsts.MaxNoteLength)]

    public string? Note { get; set; }
}