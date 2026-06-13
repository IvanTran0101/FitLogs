using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FitLogs.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace FitLogs.Foods;

public class EfCoreFoodLogRepository : EfCoreRepository<FitLogsDbContext, FoodLog, Guid>, IFoodLogRepository
{
    public EfCoreFoodLogRepository(IDbContextProvider<FitLogsDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<List<FoodLog>> GetListByUserAndDateAsync(Guid userId, DateTime date, CancellationToken cancellationToken = default)
    {
        var start = date.Date;
        var end = start.AddDays(1);
        return await GetListByUserAndDateRangeAsync(userId, start, end, cancellationToken);
        
    }

    public async Task<List<FoodLog>> GetListByUserAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x=>
                x.UserId == userId &&
                x.LoggedAt >= startDate &&
                x.LoggedAt < endDate)
            .OrderBy(x=>x.LoggedAt)
            .ToListAsync(GetCancellationToken(cancellationToken));
        
    }

    public async Task<bool> ExistsByFoodProductIdAsync(Guid foodProductId, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.AnyAsync(
            x=> x.FoodProductId == foodProductId,
            GetCancellationToken(cancellationToken));
        
    }
}