using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace FitLogs.Data;

/* This is used if database provider does't define
 * IFitLogsDbSchemaMigrator implementation.
 */
public class NullFitLogsDbSchemaMigrator : IFitLogsDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
