using FitLogs.Samples;
using Xunit;

namespace FitLogs.EntityFrameworkCore.Domains;

[Collection(FitLogsTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<FitLogsEntityFrameworkCoreTestModule>
{

}
