using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FitLogs.Data;
using Volo.Abp.DependencyInjection;

namespace FitLogs.EntityFrameworkCore;

public class EntityFrameworkCoreFitLogsDbSchemaMigrator
    : IFitLogsDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreFitLogsDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the FitLogsDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<FitLogsDbContext>()
            .Database
            .MigrateAsync();
    }
}
