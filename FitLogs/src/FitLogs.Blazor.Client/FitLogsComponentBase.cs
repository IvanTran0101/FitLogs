using FitLogs.Localization;
using Volo.Abp.AspNetCore.Components;

namespace FitLogs.Blazor.Client;

public abstract class FitLogsComponentBase : AbpComponentBase
{
    protected FitLogsComponentBase()
    {
        LocalizationResource = typeof(FitLogsResource);
    }
}
