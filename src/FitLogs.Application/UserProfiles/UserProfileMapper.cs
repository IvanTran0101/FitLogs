using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace FitLogs.UserProfiles;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class UserProfileMapper : MapperBase<UserProfile, UserProfileDto>
{
    public override partial UserProfileDto Map(UserProfile source);

    public override partial void Map(UserProfile source, UserProfileDto destination);
}