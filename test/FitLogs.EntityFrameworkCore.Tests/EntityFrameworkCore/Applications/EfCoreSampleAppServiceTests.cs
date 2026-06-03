using FitLogs.Samples;
using Xunit;

namespace FitLogs.EntityFrameworkCore.Applications;

[Collection(FitLogsTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<FitLogsEntityFrameworkCoreTestModule>
{

}
