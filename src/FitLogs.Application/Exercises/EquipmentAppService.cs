using System;
using System.Linq;
using System.Threading.Tasks;
using FitLogs.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace FitLogs.Exercises;
[Authorize]
public class EquipmentAppService : ApplicationService, IEquipmentAppService
{
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly EquipmentManager _equipmentManager;

    public EquipmentAppService(IEquipmentRepository equipmentRepository, EquipmentManager equipmentManager)
    {
        _equipmentRepository = equipmentRepository;
        _equipmentManager = equipmentManager;
    }
    [Authorize(FitLogsPermissions.Equipments.Default)]
    public async Task<EquipmentDto> GetAsync(Guid id)
    {
        var equipment = await _equipmentRepository.GetAsync(id);
        return ObjectMapper.Map<Equipment, EquipmentDto>(equipment);
    }
    [Authorize(FitLogsPermissions.Equipments.Default)]

    public async Task<PagedResultDto<EquipmentDto>> GetListAsync(GetEquipmentListInput input)
    {
        var totalCount = await _equipmentRepository.GetCountAsync(
            filterText: input.FilterText,
            isActive: input.IsActive);

        var equipmentList = await _equipmentRepository.GetListAsync(
            filterText: input.FilterText,
            isActive: input.IsActive,
            sorting: input.Sorting,
            maxResultCount: input.MaxResultCount,
            skipCount: input.SkipCount);

        var items = equipmentList
            .Select(x => ObjectMapper.Map<Equipment, EquipmentDto>(x))
            .ToList();
        return new PagedResultDto<EquipmentDto>(totalCount, items);
    }
    [Authorize(FitLogsPermissions.Equipments.Create)]
    public async Task<EquipmentDto> CreateAsync(CreateUpdateEquipmentDto input)
    {
        var equipment = await _equipmentManager.CreateAsync(
            input.Name,
            input.Code,
            input.DisplayOrder,
            input.Description);
        await _equipmentRepository.InsertAsync(equipment, autoSave: true);
        return ObjectMapper.Map<Equipment, EquipmentDto>(equipment);
    }
    [Authorize(FitLogsPermissions.Equipments.Update)]
    public async Task<EquipmentDto> UpdateAsync(Guid id, CreateUpdateEquipmentDto input)
    {
        var equipment = await _equipmentRepository.GetAsync(id);
        await _equipmentManager.ChangeNameAsync(equipment, input.Name);
        await _equipmentManager.ChangeCodeAsync(equipment, input.Code);
        _equipmentManager.ChangeDisplayOrder(equipment, input.DisplayOrder);
        _equipmentManager.ChangeDescription(equipment, input.Description);
        await _equipmentRepository.UpdateAsync(equipment, autoSave: true);
        return ObjectMapper.Map<Equipment, EquipmentDto>(equipment);
    }
    [Authorize(FitLogsPermissions.Equipments.Manage)]
    public async Task ActivateAsync(Guid id)
    {
        var equipment = await _equipmentRepository.GetAsync(id);

        _equipmentManager.Activate(equipment);
        await _equipmentRepository.UpdateAsync(equipment, autoSave: true);
    }
    [Authorize(FitLogsPermissions.Equipments.Manage)]
    public async Task DeactivateAsync(Guid id)
    {
        var equipment = await _equipmentRepository.GetAsync(id);
        await _equipmentManager.DeactivateAsync(equipment);
        await _equipmentRepository.UpdateAsync(equipment, autoSave: true);
    }
}