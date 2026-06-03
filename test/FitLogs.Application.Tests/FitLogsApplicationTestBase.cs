using Volo.Abp.Modularity;

namespace FitLogs;

public abstract class FitLogsApplicationTestBase<TStartupModule> : FitLogsTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
