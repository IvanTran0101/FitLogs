using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitLogs.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Linq;
namespace FitLogs.Exercises;

public class EfCoreMuscleGroupRepository : EfCoreRepository<FitLogsDbContext, MuscleGroup, Guid>, IMuscleGroupRepository
{
    public EfCoreMuscleGroupRepository(
        IDbContextProvider<FitLogsDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
        
    }

    public async Task<MuscleGroup?> FindByCodeAsync(string code)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(x => x.Code == code);
    }

    public async Task<bool> CodeExistsAsync(string code, Guid? excludedId = null)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.AnyAsync(x =>
            x.Code == code &&
            (!excludedId.HasValue || x.Id != excludedId.Value));

    }

    public async Task<List<MuscleGroup>> GetListAsync(
        string? filterText = null,
        bool? isActive = null,
        string? sorting = null,
        int maxResultCount = 50,
        int skipCount = 0)
    {
        var dbSet = await GetDbSetAsync();
        var query = ApplyFilter(
            dbSet,
            filterText,
            isActive);
        query = query.
            OrderBy(string.IsNullOrWhiteSpace(sorting)
                ? $"{nameof(MuscleGroup.DisplayOrder)} asc, {nameof(MuscleGroup.Name)} asc"
                : sorting)
            .Skip(skipCount)
            .Take(maxResultCount);;
        return await query.ToListAsync();
    }
    public async Task<long> GetCountAsync(
        string? filterText = null,
        bool? isActive = null)
    {
        var dbSet = await GetDbSetAsync();

        var query = ApplyFilter(
            dbSet,
            filterText,
            isActive
        );

        return await query.LongCountAsync();
    }

    private static IQueryable<MuscleGroup> ApplyFilter(IQueryable<MuscleGroup> query,
        string? filterText = null,
        bool? isActive = null)
    {
        return query
            .WhereIf(!string.IsNullOrWhiteSpace(filterText),
                x => 
                    x.Name.Contains(filterText!) ||
                    x.Code.Contains(filterText!) ||
                    x.Description != null && x.Description.Contains(filterText!))
            .WhereIf(
                isActive.HasValue,
                x => x.IsActive == isActive.Value);
    }
}