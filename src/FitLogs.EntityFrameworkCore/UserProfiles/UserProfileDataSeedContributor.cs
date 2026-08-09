using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using Volo.Abp.Identity;

namespace FitLogs.UserProfiles;

/// <summary>Backfills profiles for identity users created before the profile lifecycle handler existed.</summary>
public sealed class UserProfileDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly FitLogs.EntityFrameworkCore.FitLogsDbContext _dbContext;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly UserProfileManager _userProfileManager;

    public UserProfileDataSeedContributor(
        FitLogs.EntityFrameworkCore.FitLogsDbContext dbContext,
        IUserProfileRepository userProfileRepository,
        UserProfileManager userProfileManager)
    {
        _dbContext = dbContext;
        _userProfileRepository = userProfileRepository;
        _userProfileManager = userProfileManager;
    }

    /// <summary>Scans identity users and inserts only the profiles that are missing.</summary>
    [UnitOfWork]
    public async Task SeedAsync(DataSeedContext context)
    {
        var users = await _dbContext.Users
            .AsNoTracking()
            .Select(user => new { user.Id, user.Name, user.UserName })
            .ToListAsync();

        foreach (var user in users)
        {
            if (await _userProfileRepository.FindByUserIdAsync(user.Id) != null)
            {
                continue;
            }

            var displayName = string.IsNullOrWhiteSpace(user.Name) ? user.UserName : user.Name;
            var profile = await _userProfileManager.CreateAsync(user.Id, displayName);
            await _userProfileRepository.InsertAsync(profile, autoSave: false);
        }

        await _dbContext.SaveChangesAsync();
    }
}
