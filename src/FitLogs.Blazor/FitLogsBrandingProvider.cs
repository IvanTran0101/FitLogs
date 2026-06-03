using Microsoft.Extensions.Localization;
using FitLogs.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace FitLogs.Blazor;

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
