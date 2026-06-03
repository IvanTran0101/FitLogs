using Volo.Abp.Modularity;

namespace FitLogs;

[DependsOn(
    typeof(FitLogsDomainModule),
    typeof(FitLogsTestBaseModule)
)]
public class FitLogsDomainTestModule : AbpModule
{

}
