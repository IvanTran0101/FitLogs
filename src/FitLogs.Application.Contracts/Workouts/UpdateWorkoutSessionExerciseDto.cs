using System.ComponentModel.DataAnnotations;

namespace FitLogs.Workouts;

public class UpdateWorkoutSessionExerciseDto
{
    [Range(1, int.MaxValue)]

    public int OrderIndex { get; set; }

    [Range(1, int.MaxValue)]

    public int TargetSets { get; set; }

    [Range(1, int.MaxValue)]

    public int TargetReps { get; set; }

    [Range(0, float.MaxValue)]

    public float? TargetWeightKg { get; set; }

    [Range(0, int.MaxValue)]

    public int? RestSeconds { get; set; }

    [StringLength(WorkoutSessionExerciseConsts.MaxNoteLength)]

    public string? Note { get; set; }
}