using System;
using Volo.Abp.Application.Dtos;

namespace FitLogs.Workouts;

public class GetWorkoutSessionListDto : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }

    public WorkoutSessionStatus? Status { get; set; }

    public Guid? WorkoutPlanId { get; set; }

    public DateTime? StartedFrom { get; set; }

    public DateTime? StartedTo { get; set; }
}