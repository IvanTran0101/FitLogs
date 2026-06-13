using FitLogs.Foods.FoodProducts;
using Volo.Abp.Mapperly;
using Riok.Mapperly.Abstractions;
namespace FitLogs.Foods;
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class FoodProductApplicationMapper  : MapperBase<FoodProduct, FoodProductDto>
{
    public override partial FoodProductDto Map(FoodProduct source);

    public override partial void Map(FoodProduct source, FoodProductDto destination);
}