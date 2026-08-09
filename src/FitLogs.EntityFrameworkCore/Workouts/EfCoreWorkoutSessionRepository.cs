using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FitLogs.EntityFrameworkCore;
using FitLogs.Workouts;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Volo.Abp;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace FitLogs.Workouts;

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
        return await dbSet
            .Include(x => x.Exercises)
            .ThenInclude(x => x.Sets)
            .FirstOrDefaultAsync(x => x.UserId == userId &&
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
        // Completed history belongs to the local day in which the session ended, not the day it was created.
        var queryable = await GetQueryableAsync();
        return await queryable
            .Where(x=> x.UserId == userId)
            .Where(x=> x.Status == WorkoutSessionStatus.Completed)
            .Where(x=> x.EndedAt.HasValue && x.EndedAt >= startDate && x.EndedAt < endDate)
            .Include(x=> x.Exercises)
            .ThenInclude(x=>x.Sets)
            .OrderBy(x=> x.EndedAt)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    /// <summary>Projects completed-set counts and volume in SQL so dashboards do not load unfinished set rows.</summary>
    public async Task<List<CompletedWorkoutSessionMetric>> GetCompletedMetricsByUserAndDateRangeAsync(
        Guid userId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var rows = await dbSet
            .Where(x => x.UserId == userId &&
                        x.Status == WorkoutSessionStatus.Completed &&
                        x.EndedAt.HasValue &&
                        x.EndedAt >= startDate &&
                        x.EndedAt < endDate)
            .OrderBy(x => x.EndedAt)
            .Select(x => new
            {
                x.StartedAt,
                CompletedAt = x.EndedAt!.Value,
                CompletedExercises = x.Exercises.Count(exercise =>
                    exercise.Sets.Any(set => set.IsCompleted)),
                CompletedSets = x.Exercises
                    .SelectMany(exercise => exercise.Sets)
                    .Count(set => set.IsCompleted),
                CompletedVolume = x.Exercises
                    .SelectMany(exercise => exercise.Sets)
                    .Where(set => set.IsCompleted)
                    .Select(set => (decimal)set.WeightKg * set.Reps)
                    .Sum()
            })
            .ToListAsync(GetCancellationToken(cancellationToken));

        return rows
            .Select(row => new CompletedWorkoutSessionMetric(
                row.StartedAt,
                row.CompletedAt,
                row.CompletedExercises,
                row.CompletedSets,
                row.CompletedVolume))
            .ToList();
    }

    public override async Task<WorkoutSession> InsertAsync(
        WorkoutSession entity,
        bool autoSave = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await base.InsertAsync(entity, autoSave, cancellationToken);
        }
        catch (DbUpdateException exception) when (IsInProgressSessionUniqueViolation(exception))
        {
            throw new BusinessException(FitLogsDomainErrorCodes.UserHasInProgressWorkoutSession);
        }
    }

    private static bool IsInProgressSessionUniqueViolation(DbUpdateException exception)
    {
        return exception.InnerException is PostgresException postgresException
               && postgresException.SqlState == PostgresErrorCodes.UniqueViolation
               && postgresException.ConstraintName == "IX_AppWorkoutSessions_UserId_InProgress";
    }
}
