using FitLogs.Foods.FoodLogs;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace FitLogs.Foods;
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class FoodLogApplicationMapper
    : MapperBase<FoodLog, FoodLogDto>
{
    public override partial FoodLogDto Map(FoodLog source);

    public override partial void Map(FoodLog source, FoodLogDto destination);
}