using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace FitLogs.Exercises;

public interface IEquipmentRepository : IRepository<Equipment, Guid> 
{
    Task<Equipment?> FindByCodeAsync(
        string code
    );

    Task<bool> CodeExistsAsync(
        string code,
        Guid? excludedId = null
    );

    Task<List<Equipment>> GetListAsync(
        string? filterText = null,
        bool? isActive = null,
        string? sorting = null,
        int maxResultCount = 50,
        int skipCount = 0
    );

    Task<long> GetCountAsync(
        string? filterText = null,
        bool? isActive = null
    );
}