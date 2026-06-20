using Volo.Abp.Application.Dtos;

namespace FitLogs.Workouts;

public class GetWorkoutPlanListDto : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }
    public bool? IsArchived { get; set; }
    public bool? IsActive { get; set; }
    public WorkoutGoal? Goal { get; set; }
    public WorkoutDifficulty? Difficulty { get; set; }
}