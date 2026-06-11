using System.ComponentModel.DataAnnotations;

namespace FitLogs.Workouts;

public class UpdateExerciseSetDto
{
    [Range(1, int.MaxValue)]
    public int SetNumber { get; set; }
    
    [Range(0, int.MaxValue)]
    public float WeightKg { get; set; }
    [Range(1, int.MaxValue)]
    public int Reps { get; set; }
    
    [Range(0, 10)]
    public int? Rpe { get; set; }
    [StringLength(ExerciseSetConsts.MaxNoteLength)]
    public string? Note { get; set; }
}