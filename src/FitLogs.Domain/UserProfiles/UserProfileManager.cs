using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.Guids;

namespace FitLogs.UserProfiles;

public class UserProfileManager : DomainService
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IGuidGenerator _guidGenerator;
    
    public UserProfileManager(
        IUserProfileRepository userProfileRepository,
        IGuidGenerator guidGenerator
    )
    {
        _userProfileRepository = userProfileRepository;
        _guidGenerator = guidGenerator;
    }

    public async Task<UserProfile> CreateAsync(Guid userId, string displayName)
    {
        if (await _userProfileRepository.ExistsByUserIdAsync(userId))
        {
            throw new BusinessException(FitLogsDomainErrorCodes.UserProfileAlreadyExists)
                .WithData("UserId", userId);
        }
        return new UserProfile(
            _guidGenerator.Create(),
            userId,
            displayName
        );
    }
}