using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace FitLogs.Exercises;
[Mapper]
public partial class EquipmentMapper : MapperBase<Equipment, EquipmentDto>
{
    public override partial EquipmentDto Map(Equipment source);
    public override partial void Map(Equipment source, EquipmentDto dto);
}