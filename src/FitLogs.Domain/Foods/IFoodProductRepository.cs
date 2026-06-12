using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace FitLogs.Foods;

public interface IFoodProductRepository : IRepository<FoodProduct, Guid>
{
    Task<FoodProduct?> FindByBarcodeAsync(string barcode
        , CancellationToken cancellationToken = default);
    
    Task<bool> BarcodeExistsAsync(
        string barcode,
        Guid? excludedId = null,
        CancellationToken cancellationToken = default);
    
    Task<List<FoodProduct>> GetActiveListAsync(
        string? filterText = null,
        int maxResultCount = 20,
        CancellationToken cancellationToken = default);
}