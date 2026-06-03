using Xunit;

namespace FitLogs.EntityFrameworkCore;

[CollectionDefinition(FitLogsTestConsts.CollectionDefinitionName)]
public class FitLogsEntityFrameworkCoreCollection : ICollectionFixture<FitLogsEntityFrameworkCoreFixture>
{

}
