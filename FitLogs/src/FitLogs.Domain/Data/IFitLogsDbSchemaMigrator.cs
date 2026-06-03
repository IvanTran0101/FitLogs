using System.Threading.Tasks;

namespace FitLogs.Data;

public interface IFitLogsDbSchemaMigrator
{
    Task MigrateAsync();
}
