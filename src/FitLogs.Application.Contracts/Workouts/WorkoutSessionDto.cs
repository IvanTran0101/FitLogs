using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace FitLogs.Workouts;

public class WorkoutSessionDto : FullAuditedEntityDto<Guid>
{
    public Guid UserId { get; set; }
    public Guid? WorkoutPlanId { get; set; }
    public string Name { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public WorkoutSessionStatus Status { get; set; }
    public string? Note { get; set; }

    public List<WorkoutSessionExerciseDto> Exercises { get; set; } = new();
}