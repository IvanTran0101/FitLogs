using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FitLogs.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace FitLogs.UserProfiles;

public class EfCoreUserProfileRepository
    : EfCoreRepository<FitLogsDbContext, UserProfile, Guid>,
        IUserProfileRepository
{
    public EfCoreUserProfileRepository(
        IDbContextProvider<FitLogsDbContext> dbContextProvider
    ) : base(dbContextProvider)
    {
    }

    public async Task<UserProfile?> FindByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        var dbSet = await GetDbSetAsync();

        return await dbSet
            .FirstOrDefaultAsync(
                x => x.UserId == userId,
                GetCancellationToken(cancellationToken)
            );
    }

    public async Task<UserProfile> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        var dbSet = await GetDbSetAsync();

        return await dbSet
            .FirstAsync(
                x => x.UserId == userId,
                GetCancellationToken(cancellationToken)
            );
    }

    public async Task<bool> ExistsByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        var dbSet = await GetDbSetAsync();

        return await dbSet.AnyAsync(
            x => x.UserId == userId,
            GetCancellationToken(cancellationToken)
        );
    }
}