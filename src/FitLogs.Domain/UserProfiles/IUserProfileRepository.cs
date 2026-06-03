using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace FitLogs.UserProfiles;

public interface IUserProfileRepository : IRepository<UserProfile, Guid>
{
    Task<UserProfile?> FindByUserIdAsync(Guid userId, CancellationToken cancellationToken= default);
    Task<UserProfile> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}