using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace FitLogs.Exercises;

public interface IEquipmentAppService : IApplicationService
{
    Task<EquipmentDto> GetAsync(Guid id);
    Task<PagedResultDto<EquipmentDto>> GetListAsync(GetEquipmentListInput input);
    Task<MuscleGroupDto> CreateAsync(CreateUpdateMuscleGroupDto input);
    Task<MuscleGroupDto> UpdateAsync(Guid id, CreateUpdateMuscleGroupDto input);
    Task ActivateAsync(Guid id);
    Task DeactivateAsync(Guid id);
}