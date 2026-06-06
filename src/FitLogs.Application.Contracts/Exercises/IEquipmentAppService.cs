using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace FitLogs.Exercises;

public interface IEquipmentAppService : IApplicationService
{
    Task<EquipmentDto> GetAsync(Guid id);
    Task<PagedResultDto<EquipmentDto>> GetListAsync(GetEquipmentListInput input);
    Task<EquipmentDto> CreateAsync(CreateUpdateEquipmentDto input);
    Task<EquipmentDto> UpdateAsync(Guid id, CreateUpdateEquipmentDto input);
    Task ActivateAsync(Guid id);
    Task DeactivateAsync(Guid id);
}