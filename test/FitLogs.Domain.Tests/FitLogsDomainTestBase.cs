using Volo.Abp.Modularity;

namespace FitLogs;

/* Inherit from this class for your domain layer tests. */
public abstract class FitLogsDomainTestBase<TStartupModule> : FitLogsTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
