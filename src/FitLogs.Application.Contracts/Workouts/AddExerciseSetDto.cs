using System.ComponentModel.DataAnnotations;

namespace FitLogs.Workouts;

public class AddExerciseSetDto
{
    [Range(1, int.MaxValue)]

    public int SetNumber { get; set; }

    [Range(0, float.MaxValue)]

    public float WeightKg { get; set; }

    [Range(1, int.MaxValue)]

    public int Reps { get; set; }

    [Range(1, 10)]

    public int? Rpe { get; set; }

    [StringLength(ExerciseSetConsts.MaxNoteLength)]

    public string? Note { get; set; }
}