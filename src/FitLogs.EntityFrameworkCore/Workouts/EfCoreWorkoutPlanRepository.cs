using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using FitLogs.EntityFrameworkCore;
using FitLogs.Workouts;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Linq;

namespace FitLogs.Workouts;

public class EfCoreWorkoutPlanRepository : EfCoreRepository<FitLogsDbContext, WorkoutPlan, Guid>,
    IWorkoutPlanRepository
{
   
    private readonly IAsyncQueryableExecuter _asyncExecuter;

    public EfCoreWorkoutPlanRepository(
        IDbContextProvider<FitLogsDbContext> dbContextProvider,
        IAsyncQueryableExecuter asyncExecuter) 
        : base(dbContextProvider)
    {
        _asyncExecuter = asyncExecuter;
    }

    public async Task<WorkoutPlan?> FindWithDetailsAsync(Guid id, bool includeDetails = true, CancellationToken cancellationToken = default)
    {
        var queryable = await GetQueryableAsync();
        if (includeDetails)
        {
            queryable = queryable.Include(x => x.Exercises);
        }
        return await queryable.FirstOrDefaultAsync(
            x => x.Id == id,
            GetCancellationToken(cancellationToken));
    }

    public async Task<WorkoutPlan?> FindByUserAndNameAsync(Guid userId, string name, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(
            x=> x.UserId == userId && x.Name == name && !x.IsArchived,
            GetCancellationToken(cancellationToken));
        
    }

    public async Task<bool> ExistsByUserAndNameAsync(Guid userId, string name, Guid? excludedId = null,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.AnyAsync(
            x =>
                x.UserId == userId && x.Name == name &&
                (!excludedId.HasValue || x.Id != excludedId.Value),
            GetCancellationToken(cancellationToken));
        
    }
    public async Task<List<WorkoutPlan>> GetListWithDetailsAsync(
        Guid userId,
        string? filterText = null,
        bool? isArchived = null,
        bool? isActive = null,
        WorkoutGoal? goal = null,
        WorkoutDifficulty? difficulty = null,
        string? sorting = null,
        int maxResultCount = 50,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        var queryable = await GetQueryableAsync();

        queryable = queryable
            .Include(x => x.Exercises)
            .Where(x => x.UserId == userId)
            .WhereIf(
                !string.IsNullOrWhiteSpace(filterText),
                x => x.Name.Contains(filterText!) ||
                     (x.Description != null && x.Description.Contains(filterText!))
            )
            .WhereIf(
                isActive.HasValue,
                x => x.IsActive == isActive!.Value
            )
            .WhereIf(
                isArchived.HasValue,
                x => x.IsArchived == isArchived!.Value
            )
            .WhereIf(
                !isArchived.HasValue,
                x => !x.IsArchived
            )
            .WhereIf(
                goal.HasValue,
                x => x.Goal == goal!.Value
            )
            .WhereIf(
                difficulty.HasValue,
                x => x.Difficulty == difficulty!.Value
            );

        queryable = queryable.OrderBy(
            string.IsNullOrWhiteSpace(sorting)
                ? $"{nameof(WorkoutPlan.Name)} asc"
                : sorting
        );

        queryable = queryable
            .Skip(skipCount)
            .Take(maxResultCount);

        return await _asyncExecuter.ToListAsync(
            queryable,
            GetCancellationToken(cancellationToken)
        );
    }
}