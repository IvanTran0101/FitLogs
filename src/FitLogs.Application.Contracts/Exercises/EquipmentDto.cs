using System;
using Volo.Abp.Application.Dtos;

namespace FitLogs.Exercises;

public class EquipmentDto : EntityDto<Guid>
{
    public string Name { get; set; } =  null!;
    public string Code { get; set; } = null!;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}