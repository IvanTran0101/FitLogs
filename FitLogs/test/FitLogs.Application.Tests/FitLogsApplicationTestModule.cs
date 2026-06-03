using Volo.Abp.Modularity;

namespace FitLogs;

[DependsOn(
    typeof(FitLogsApplicationModule),
    typeof(FitLogsDomainTestModule)
)]
public class FitLogsApplicationTestModule : AbpModule
{

}
