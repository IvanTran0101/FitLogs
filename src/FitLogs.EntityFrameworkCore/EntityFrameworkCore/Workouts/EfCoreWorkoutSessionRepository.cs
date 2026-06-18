using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FitLogs.Workouts;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace FitLogs.EntityFrameworkCore.Workouts;

public class EfCoreWorkoutSessionRepository : EfCoreRepository<FitLogsDbContext, WorkoutSession, Guid>,
    IWorkoutSessionRepository
{
    public EfCoreWorkoutSessionRepository(IDbContextProvider<FitLogsDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<WorkoutSession?> FindWithDetailsAsync(Guid id, bool includeDetails = true, CancellationToken cancellationToken = default)
    {
        var queryable = await GetQueryableAsync();
        if (includeDetails)
        {
            queryable = queryable
                .Include(x=>x.Exercises)
                .ThenInclude(x=>x.Sets);
        }
        return await queryable.FirstOrDefaultAsync(
            x => x.Id == id,
            GetCancellationToken(cancellationToken));
        
    }

    public async Task<WorkoutSession?> FindCurrentInProgressAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(
            x => x.UserId == userId &&
                 x.Status == WorkoutSessionStatus.InProgress,
            GetCancellationToken(cancellationToken));
    }

    public async Task<bool> HasInProgressSessionAsync(Guid userId, Guid? excludedId = null, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.AnyAsync(
            x => x.UserId == userId
            && x.Status == WorkoutSessionStatus.InProgress
            && (!excludedId.HasValue || x.Id != excludedId.Value),
            GetCancellationToken(cancellationToken));
        
    }

    public async Task<List<WorkoutSession>> GetCompletedListByUserAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        var queryable = await GetQueryableAsync();
        return await queryable
            .Include(x=> x.Exercises)
            .ThenInclude(x=>x.Sets)
            .Where(x=> x.UserId == userId)
            .Where(x=> x.Status == WorkoutSessionStatus.Completed)
            .Where(x=> x.StartedAt >= startDate && x.StartedAt <endDate)
            .OrderBy(x=> x.StartedAt)
            .ToListAsync(GetCancellationToken(cancellationToken));
        
    }
}