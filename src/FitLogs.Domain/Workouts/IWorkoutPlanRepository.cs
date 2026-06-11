using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace FitLogs.Workouts;

public interface IWorkoutPlanRepository : IRepository<WorkoutPlan,Guid>
{
    Task<WorkoutPlan?> FindWithDetailsAsync(
        Guid id,
        bool includeDetails = true,
        CancellationToken cancellationToken = default
    );

    Task<WorkoutPlan?> FindByUserAndNameAsync(
        Guid userId,
        string name,
        CancellationToken cancellationToken = default
    );

    Task<bool> ExistsByUserAndNameAsync(
        Guid userId,
        string name,
        Guid? excludedId = null,
        CancellationToken cancellationToken = default
    );
    
}