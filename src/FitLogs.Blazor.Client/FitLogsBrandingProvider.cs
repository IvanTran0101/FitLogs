using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;
using Microsoft.Extensions.Localization;
using FitLogs.Localization;

namespace FitLogs.Blazor.Client;

[Dependency(ReplaceServices = true)]
public class FitLogsBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<FitLogsResource> _localizer;

    public FitLogsBrandingProvider(IStringLocalizer<FitLogsResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
