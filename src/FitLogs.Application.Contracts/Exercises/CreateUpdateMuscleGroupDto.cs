using System.ComponentModel.DataAnnotations;

namespace FitLogs.Exercises;

public class CreateUpdateMuscleGroupDto
{
    [Required]
    [StringLength(MuscleGroupConsts.MaxNameLength)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(MuscleGroupConsts.MaxCodeLength)]
    public string Code { get; set; } = null!;

    [StringLength(MuscleGroupConsts.MaxDescriptionLength)]
    public string? Description { get; set; }

    public int DisplayOrder { get; set; }
}