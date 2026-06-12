using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace FitLogs.Foods;

public interface IFoodLogRepository : IRepository<FoodLog, Guid>
{
    Task<List<FoodLog>> GetListByUserAndDateAsync(Guid userId, DateTime date, CancellationToken cancellationToken = default);

    Task<List<FoodLog>> GetListByUserAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    Task<bool> ExistsByFoodProductIdAsync(Guid foodProductId, CancellationToken cancellationToken = default);


}