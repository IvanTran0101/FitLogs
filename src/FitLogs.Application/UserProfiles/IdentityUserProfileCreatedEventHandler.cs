using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.Identity;
using Volo.Abp.Domain.Entities.Events;

namespace FitLogs.UserProfiles;

/// <summary>Creates the fitness profile immediately after ABP persists a new identity user.</summary>
public sealed class IdentityUserProfileCreatedEventHandler
    : ILocalEventHandler<EntityCreatedEventData<IdentityUser>>, ITransientDependency
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly UserProfileManager _userProfileManager;

    public IdentityUserProfileCreatedEventHandler(
        IUserProfileRepository userProfileRepository,
        UserProfileManager userProfileManager)
    {
        _userProfileRepository = userProfileRepository;
        _userProfileManager = userProfileManager;
    }

    /// <summary>Checks for an existing profile, then creates a default profile only when needed.</summary>
    public async Task HandleEventAsync(EntityCreatedEventData<IdentityUser> eventData)
    {
        var identityUser = eventData.Entity;
        if (await _userProfileRepository.FindByUserIdAsync(identityUser.Id) != null)
        {
            return;
        }

        // The username is always available and gives a stable default before the user edits their profile.
        var displayName = string.IsNullOrWhiteSpace(identityUser.Name)
            ? identityUser.UserName
            : identityUser.Name;
        var profile = await _userProfileManager.CreateAsync(identityUser.Id, displayName);
        await _userProfileRepository.InsertAsync(profile, autoSave: true);
    }
}
