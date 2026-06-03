using FitLogs.Localization;
using Volo.Abp.Application.Services;

namespace FitLogs;

/* Inherit your application services from this class.
 */
public abstract class FitLogsAppService : ApplicationService
{
    protected FitLogsAppService()
    {
        LocalizationResource = typeof(FitLogsResource);
    }
}
