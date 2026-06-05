using System;
using Volo.Abp.Application.Dtos;

namespace FitLogs.Exercises;

public class GetExerciseListInput : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }
    public Guid? MuscleGroupId { get; set; }
    public Guid? EquipmentId { get; set; }
    public ExerciseDifficulty? Difficulty { get; set; }
    public ExerciseTrackingType? TrackingType { get; set; }
    public bool? IsActive { get; set; }   
}