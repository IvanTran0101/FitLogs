using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FitLogs.Workouts;

/// <summary>Command containing all exercises to add to a plan in one transaction.</summary>
public class AddWorkoutPlanExercisesDto
{
    [Required]
    [MinLength(1)]
    public List<CreateWorkoutPlanExerciseDto> Exercises { get; set; } = new();
}
