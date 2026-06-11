using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace FitLogs.Workouts;

public class WorkoutPlanDto : FullAuditedEntityDto<Guid>
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public WorkoutGoal Goal { get; set; }
    public WorkoutDifficulty Difficulty { get; set; }
    public bool IsActive { get; set; }

    public List<WorkoutPlanExerciseDto> Exercises { get; set; } = new();
}