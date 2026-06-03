using Volo.Abp.AspNetCore.Mvc.UI.Bundling;

namespace FitLogs.Blazor;

public class FitLogsStyleBundleContributor : BundleContributor
{
    public override void ConfigureBundle(BundleConfigurationContext context)
    {
        context.Files.Add(new BundleFile("main.css", true));
    }
}
