using System;
using FitLogs.ExternalServices.OpenFoodFacts;
using FitLogs.Foods;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace FitLogs.ExternalServices;

public class FitLogsExternalServicesModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddSingleton<OpenFoodFactsCircuitBreaker>();
        context.Services.AddHttpClient<IOpenFoodFactsClient, OpenFoodFactsClient>(client =>
        {
            client.BaseAddress = new Uri("https://world.openfoodfacts.org/");
            client.Timeout = TimeSpan.FromSeconds(4);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("FitLogs/1.0 (+https://github.com/IvanTran0101/FitLogs)");
            client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
        });
    }
}
