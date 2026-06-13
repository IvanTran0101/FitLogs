using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitLogs.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Volo.Abp.Linq;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace FitLogs.Foods;

public class EfCoreFoodProductRepository : EfCoreRepository<FitLogsDbContext, FoodProduct, Guid>,
    IFoodProductRepository
{
    public EfCoreFoodProductRepository(IDbContextProvider<FitLogsDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    

    public async Task<FoodProduct?> FindByBarcodeAsync(string barcode)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .FirstOrDefaultAsync(x => x.Barcode == barcode);
        
    }

    public async Task<bool> BarcodeExistsAsync(string barcode, Guid? excludedId = null)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.AnyAsync(
            x => x.Barcode == barcode &&
                 (!excludedId.HasValue || x.Id != excludedId.Value));
        
    }

    

    public async Task<List<FoodProduct>> GetListAsync(
        string? filterText = null,
        string? barcode = null,
        FoodProductSource? source = null,
        bool? isActive = null,
        bool? isVerified = null,
        string? sorting = null,
        int maxResultCount = 50,
        int skipCount = 0)
    {
        var dbSet = await GetDbSetAsync();

        var query = ApplyFilter(
            dbSet,
            filterText,
            barcode,
            source,
            isActive,
            isVerified
        );

        query = query
            .OrderBy(string.IsNullOrWhiteSpace(sorting)
                ? nameof(FoodProduct.Name)
                : sorting)
            .Skip(skipCount)
            .Take(maxResultCount);

        return await query.ToListAsync();
    }

    public async Task<long> GetCountAsync(
        string? filterText = null,
        string? barcode = null,
        FoodProductSource? source = null,
        bool? isActive = null,
        bool? isVerified = null)
    {
        var dbSet = await GetDbSetAsync();

        var query = ApplyFilter(
            dbSet,
            filterText,
            barcode,
            source,
            isActive,
            isVerified
        );

        return await query.LongCountAsync();
    }

    private static IQueryable<FoodProduct> ApplyFilter(
        IQueryable<FoodProduct> query,
        string? filterText = null,
        string? barcode = null,
        FoodProductSource? source = null,
        bool? isActive = null,
        bool? isVerified = null)
    {
        return query
            .WhereIf(
                !string.IsNullOrWhiteSpace(filterText),
                x =>
                    x.Name.Contains(filterText!) ||
                    (x.Brand != null && x.Brand.Contains(filterText!)) ||
                    (x.Barcode != null && x.Barcode.Contains(filterText!)) ||
                    (x.ServingSize != null && x.ServingSize.Contains(filterText!))
            )
            .WhereIf(
                !string.IsNullOrWhiteSpace(barcode),
                x => x.Barcode == barcode
            )
            .WhereIf(
                source.HasValue,
                x => x.Source == source.Value
            )
            .WhereIf(
                isActive.HasValue,
                x => x.IsActive == isActive.Value
            )
            .WhereIf(
                isVerified.HasValue,
                x => x.IsVerified == isVerified.Value
            );
    }
}