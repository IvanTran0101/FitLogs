using System.ComponentModel.DataAnnotations;

namespace FitLogs.Workouts;

public class UpdateWorkoutPlanDto
{
    [Required]
    [StringLength(WorkoutPlanConsts.MaxNameLength)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(WorkoutPlanConsts.MaxDescriptionLength)]
    public string? Description { get; set; }
    public WorkoutGoal Goal { get; set; }
    public WorkoutDifficulty Difficulty { get; set; }
    public bool IsActive { get; set; }
}