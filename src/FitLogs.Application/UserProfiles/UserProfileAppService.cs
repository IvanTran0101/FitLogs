using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Users;
using Volo.Abp;


namespace FitLogs.UserProfiles;
[Authorize]
public class UserProfileAppService : ApplicationService, IUserProfileAppService
{
    private readonly IUserProfileRepository _userProfileRepository;

    private readonly UserProfileMapper _userProfileMapper;

    public UserProfileAppService(
        IUserProfileRepository userProfileRepository,
        UserProfileMapper userProfileMapper)
    {
        _userProfileRepository = userProfileRepository;
        _userProfileMapper = userProfileMapper;
    }


    public async Task<UserProfileDto> GetMyProfileAsync()
    {
        var userId = CurrentUser.GetId();
        var userProfile = await _userProfileRepository.FindByUserIdAsync(userId);
        if (userProfile == null)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.UserProfileNotFound);
        }
        if (userProfile.UserId != CurrentUser.GetId())
        {
            throw new BusinessException(FitLogsDomainErrorCodes.ForbiddenProfileAccess);
        }
        return _userProfileMapper.Map(userProfile);
    }

    public async Task<UserProfileDto> UpdateMyProfileAsync(UpdateUserProfileDto input)
    {
        var userId = CurrentUser.GetId();
        var userProfile = await _userProfileRepository.FindByUserIdAsync(userId);
        if (userProfile == null)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.UserProfileNotFound);
        }
        if (userProfile.UserId != CurrentUser.GetId())
        {
            throw new BusinessException(FitLogsDomainErrorCodes.ForbiddenProfileAccess);
        }
        userProfile.SetDisplayName(input.DisplayName);
        userProfile.SetGender(input.Gender);
        userProfile.SetDateOfBirth(input.DateOfBirth);
        userProfile.SetHeightCm(input.HeightCm);
        userProfile.SetWeightKg(input.WeightKg);
        userProfile.SetFitnessGoal(input.FitnessGoal);
        userProfile.SetDailyTargetCalories(input.DailyTargetCalories);
        await _userProfileRepository.UpdateAsync(userProfile);
        return _userProfileMapper.Map(userProfile);
        
    }
}