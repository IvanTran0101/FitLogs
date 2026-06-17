using System;
using Volo.Abp.Application.Dtos;

namespace FitLogs.Exercises;

public class ExerciseDto : EntityDto<Guid>
{
    public string Name { get; set; } = null!;
    public string Slug  { get; set; } = null!;
    public string? Description { get; set; }
    
    public Guid PrimaryMuscleGroupId { get; set; }
    public Guid EquipmentId  { get; set; }
    
    public ExerciseDifficulty Difficulty { get; set; }
    public ExerciseTrackingType TrackingType { get; set; }
    
    public string? ImageUrl { get; set; }
    public string? GifUrl { get; set; }
    
    public string? Instructions { get; set; }
    public string? FormTips { get; set; }
    public string? CommonMistakes { get; set; }
    
    public bool IsActive { get; set; }
    
}