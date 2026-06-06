using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace FitLogs.Exercises;
[Mapper]
public partial class MuscleGroupMapper : MapperBase<MuscleGroup, MuscleGroupDto>
{
    public override partial MuscleGroupDto Map(MuscleGroup source);
    public override partial void Map(MuscleGroup source, MuscleGroupDto dto);
}