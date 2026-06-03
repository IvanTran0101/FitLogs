using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace FitLogs.UserProfiles;

public interface IUserProfileAppService : IApplicationService
{
    Task<UserProfileDto> GetMyProfileAsync();
    Task<UserProfileDto> UpdateMyProfileAsync(UpdateUserProfileDto input);
}