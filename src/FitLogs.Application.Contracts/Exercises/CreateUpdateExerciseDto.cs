using System;
using System.ComponentModel.DataAnnotations;

namespace FitLogs.Exercises;

public class CreateUpdateExerciseDto
{
    [Required]
    [StringLength(ExerciseConsts.MaxNameLength)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(ExerciseConsts.MaxSlugLength)]
    public string Slug { get; set; } = null!;

    [StringLength(ExerciseConsts.MaxDescriptionLength)]
    public string? Description { get; set; }

    [Required]
    public Guid PrimaryMuscleGroupId { get; set; }

    public Guid? EquipmentId { get; set; }

    [Required]
    public ExerciseDifficulty Difficulty { get; set; }

    [Required]
    public ExerciseTrackingType TrackingType { get; set; }

    [StringLength(ExerciseConsts.MaxUrlLength)]
    public string? ImageUrl { get; set; }

    [StringLength(ExerciseConsts.MaxUrlLength)]
    public string? GifUrl { get; set; }

    [StringLength(ExerciseConsts.MaxInstructionsLength)]
    public string? Instructions { get; set; }

    [StringLength(ExerciseConsts.MaxFormTipsLength)]
    public string? FormTips { get; set; }

    [StringLength(ExerciseConsts.MaxCommonMistakesLength)]
    public string? CommonMistakes { get; set; }
    
}