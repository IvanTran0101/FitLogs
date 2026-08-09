using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Users;
using Volo.Abp;
using FitLogs.Permissions;

namespace FitLogs.UserProfiles;
[Authorize(FitLogsPermissions.UserProfiles.Default)]
public class UserProfileAppService : ApplicationService, IUserProfileAppService
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly UserProfileManager _userProfileManager;
    private readonly UserProfileMapper _userProfileMapper;

    public UserProfileAppService(
        IUserProfileRepository userProfileRepository,
        UserProfileManager userProfileManager,
        UserProfileMapper userProfileMapper)
    {
        _userProfileRepository = userProfileRepository;
        _userProfileManager = userProfileManager;
        _userProfileMapper = userProfileMapper;
    }

    /// <summary>Returns the current profile, creating the legacy-missing profile on first access.</summary>
    public async Task<UserProfileDto> GetMyProfileAsync()
    {
        var userId = CurrentUser.GetId();
        var userProfile = await GetOrCreateProfileAsync(userId);
        return _userProfileMapper.Map(userProfile);
    }

    [Authorize(FitLogsPermissions.UserProfiles.Update)]
    public async Task<UserProfileDto> UpdateMyProfileAsync(UpdateUserProfileDto input)
    {
        var userId = CurrentUser.GetId();
        var userProfile = await GetOrCreateProfileAsync(userId);
        userProfile.SetDisplayName(input.DisplayName);
        userProfile.SetGender(input.Gender);
        userProfile.SetDateOfBirth(input.DateOfBirth);
        userProfile.SetHeightCm(input.HeightCm);
        userProfile.SetWeightKg(input.WeightKg);
        userProfile.SetFitnessGoal(input.FitnessGoal);
        userProfile.SetDailyTargetCalories(input.DailyTargetCalories);
        userProfile.SetTimeZoneId(input.TimeZoneId);
        await _userProfileRepository.UpdateAsync(userProfile);
        return _userProfileMapper.Map(userProfile);
    }

    /// <summary>Loads the current profile or persists a default profile so older users can use the app safely.</summary>
    private async Task<UserProfile> GetOrCreateProfileAsync(Guid userId)
    {
        var existingProfile = await _userProfileRepository.FindByUserIdAsync(userId);
        if (existingProfile != null)
        {
            return existingProfile;
        }

        var displayName = CurrentUser.Name ?? CurrentUser.UserName ?? "FitLogs user";
        var profile = await _userProfileManager.CreateAsync(userId, displayName);
        return await _userProfileRepository.InsertAsync(profile, autoSave: true);
    }
}
