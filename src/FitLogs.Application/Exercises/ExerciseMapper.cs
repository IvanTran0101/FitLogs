using System.Data;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;
using FitLogs.Exercises;

namespace FitLogs.Exercises;
[Mapper]
public partial class ExerciseMapper : MapperBase<Exercise, ExerciseDto>
{
    public override partial ExerciseDto Map(Exercise source);
    public override partial void Map(Exercise source, ExerciseDto destination);
}