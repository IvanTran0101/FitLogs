using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Users;

namespace FitLogs.UserProfiles;

public class UserProfileIdentityUserCreatedHandler : IDistributedEventHandler<EntityCreatedEto<UserEto>>,
    ITransientDependency
{
    private readonly IUserProfileRepository _userProfileRepository;
    private UserProfileManager _userProfileManager;

    public UserProfileIdentityUserCreatedHandler(IUserProfileRepository userProfileRepository, UserProfileManager userProfileManager)
    {
        _userProfileRepository = userProfileRepository;
        _userProfileManager = userProfileManager;
    }

    public async Task HandleEventAsync(EntityCreatedEto<UserEto> eventData)
    {
        var user = eventData.Entity;
        if (await _userProfileRepository.FindByUserIdAsync(user.Id) != null)
        {
            return;
        }
        var displayName = string.IsNullOrWhiteSpace(user.Name)
            ? user.UserName
            : user.Name;

        var profile = await _userProfileManager.CreateAsync(
            user.Id,
            displayName);
        await _userProfileRepository.InsertAsync(profile, autoSave:true);
    }
}