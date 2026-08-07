using System;
using System.Collections.Generic;
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
    Task<List<WorkoutPlan>> GetListWithDetailsAsync(
        Guid userId,
        string? filterText = null,
        bool? isArchived = null,
        bool? isActive = null,
        WorkoutGoal? goal = null,
        WorkoutDifficulty? difficulty = null,
        string? sorting = null,
        int maxResultCount = 50,
        int skipCount = 0,
        CancellationToken cancellationToken = default
    );
}