using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitLogs.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace FitLogs.Exercises;

public class EfCoreExerciseRepository : EfCoreRepository<FitLogsDbContext, Exercise, Guid>, IExerciseRepository
{
       public EfCoreExerciseRepository(
        IDbContextProvider<FitLogsDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<Exercise?> FindBySlugAsync(
        string slug,
        bool includeDetails = true)
    {
        var dbSet = await GetDbSetAsync();

        return await dbSet
            .FirstOrDefaultAsync(x => x.Slug == slug);
    }

    public async Task<bool> SlugExistsAsync(
        string slug,
        Guid? excludedId = null)
    {
        var dbSet = await GetDbSetAsync();

        return await dbSet.AnyAsync(x =>
            x.Slug == slug &&
            (!excludedId.HasValue || x.Id != excludedId.Value)
        );
    }

    public async Task<List<Exercise>> GetListAsync(
        string? filterText = null,
        Guid? muscleGroupId = null,
        Guid? equipmentId = null,
        ExerciseDifficulty? difficulty = null,
        ExerciseTrackingType? trackingType = null,
        bool? isActive = null,
        string? sorting = null,
        int maxResultCount = 50,
        int skipCount = 0)
    {
        var dbSet = await GetDbSetAsync();

        var query = ApplyFilter(
            dbSet,
            filterText,
            muscleGroupId,
            equipmentId,
            difficulty,
            trackingType,
            isActive
        );

        query = query
            .OrderBy(string.IsNullOrWhiteSpace(sorting) ? nameof(Exercise.Name) : sorting)
            .Skip(skipCount)
            .Take(maxResultCount);

        return await query.ToListAsync();
    }

    public async Task<long> GetCountAsync(
        string? filterText = null,
        Guid? muscleGroupId = null,
        Guid? equipmentId = null,
        ExerciseDifficulty? difficulty = null,
        ExerciseTrackingType? trackingType = null,
        bool? isActive = null)
    {
        var dbSet = await GetDbSetAsync();

        var query = ApplyFilter(
            dbSet,
            filterText,
            muscleGroupId,
            equipmentId,
            difficulty,
            trackingType,
            isActive
        );

        return await query.LongCountAsync();
    }

    private static IQueryable<Exercise> ApplyFilter(
        IQueryable<Exercise> query,
        string? filterText = null,
        Guid? muscleGroupId = null,
        Guid? equipmentId = null,
        ExerciseDifficulty? difficulty = null,
        ExerciseTrackingType? trackingType = null,
        bool? isActive = null)
    {
        return query
            .WhereIf(
                !string.IsNullOrWhiteSpace(filterText),
                x =>
                    x.Name.Contains(filterText!) ||
                    x.Slug.Contains(filterText!) ||
                    (x.Description != null && x.Description.Contains(filterText!))
            )
            .WhereIf(
                muscleGroupId.HasValue,
                x => x.PrimaryMuscleGroupId == muscleGroupId.Value
            )
            .WhereIf(
                equipmentId.HasValue,
                x => x.EquipmentId == equipmentId.Value
            )
            .WhereIf(
                difficulty.HasValue,
                x => x.Difficulty == difficulty.Value
            )
            .WhereIf(
                trackingType.HasValue,
                x => x.TrackingType == trackingType.Value
            )
            .WhereIf(
                isActive.HasValue,
                x => x.IsActive == isActive.Value
            );
    }
}