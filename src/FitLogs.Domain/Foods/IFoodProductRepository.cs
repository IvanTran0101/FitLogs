using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace FitLogs.Foods;

public interface IFoodProductRepository : IRepository<FoodProduct, Guid>
{
    Task<FoodProduct?> FindByBarcodeAsync(string barcode);

    Task<bool> BarcodeExistsAsync(
        string barcode,
        Guid? excludedId = null
    );

    Task<List<FoodProduct>> GetListAsync(
        string? filterText = null,
        string? barcode = null,
        FoodProductSource? source = null,
        bool? isActive = null,
        bool? isVerified = null,
        string? sorting = null,
        int maxResultCount = 50,
        int skipCount = 0
    );

    Task<long> GetCountAsync(
        string? filterText = null,
        string? barcode = null,
        FoodProductSource? source = null,
        bool? isActive = null,
        bool? isVerified = null
    );
}