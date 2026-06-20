using System;
using System.Threading;
using System.Threading.Tasks;
using FitLogs.EntityFrameworkCore;
using FitLogs.Workouts;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace FitLogs.Workouts;

public class EfCoreWorkoutPlanRepository : EfCoreRepository<FitLogsDbContext, WorkoutPlan, Guid>,
    IWorkoutPlanRepository
{
   
    public EfCoreWorkoutPlanRepository(IDbContextProvider<FitLogsDbContext> dbContextProvider) : base(dbContextProvider)
    {
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
}