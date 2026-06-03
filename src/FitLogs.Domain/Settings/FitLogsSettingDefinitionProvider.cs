using Volo.Abp.Settings;

namespace FitLogs.Settings;

public class FitLogsSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(FitLogsSettings.MySetting1));
    }
}
