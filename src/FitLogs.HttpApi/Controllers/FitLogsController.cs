using FitLogs.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace FitLogs.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class FitLogsController : AbpControllerBase
{
    protected FitLogsController()
    {
        LocalizationResource = typeof(FitLogsResource);
    }
}
