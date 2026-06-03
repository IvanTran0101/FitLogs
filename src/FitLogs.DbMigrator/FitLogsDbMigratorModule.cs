using FitLogs.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace FitLogs.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(FitLogsEntityFrameworkCoreModule),
    typeof(FitLogsApplicationContractsModule)
)]
public class FitLogsDbMigratorModule : AbpModule
{
}
