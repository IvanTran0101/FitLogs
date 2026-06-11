using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace FitLogs.Workouts;

public interface IWorkoutSessionRepository : IRepository<WorkoutSession, Guid>
{
    Task<WorkoutSession?> FindWithDetailsAsync(
        Guid id,
        bool includeDetails = true,
        CancellationToken cancellationToken = default
    );

    Task<WorkoutSession?> FindCurrentInProgressAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    );

    Task<bool> HasInProgressSessionAsync(
        Guid userId,
        Guid? excludedId = null,
        CancellationToken cancellationToken = default
    );
}