using System.ComponentModel.DataAnnotations;

namespace FitLogs.Exercises;

public class CreateUpdateEquipmentDto
{
    [Required]
    [StringLength(EquipmentConsts.MaxNameLength)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(EquipmentConsts.MaxCodeLength)]
    public string Code { get; set; } = null!;

    [StringLength(EquipmentConsts.MaxDescriptionLength)]
    public string? Description { get; set; }

    public int DisplayOrder { get; set; }
}