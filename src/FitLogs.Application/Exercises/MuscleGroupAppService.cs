using System;
using System.Linq;
using System.Threading.Tasks;
using FitLogs.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace FitLogs.Exercises;
[Authorize]
public class MuscleGroupAppService : ApplicationService,  IMuscleGroupAppService
{
    private readonly IMuscleGroupRepository _muscleGroupRepository;
    private readonly MuscleGroupManager _muscleGroupManager;

    public MuscleGroupAppService(IMuscleGroupRepository muscleGroupRepository, MuscleGroupManager muscleGroupManager)
    {
        _muscleGroupRepository = muscleGroupRepository;
        _muscleGroupManager = muscleGroupManager;
    }
    [Authorize(FitLogsPermissions.MuscleGroups.Default)]
    public async Task<MuscleGroupDto> GetAsync(Guid id)
    {
        var muscleGroup = await _muscleGroupRepository.GetAsync(id);
        return ObjectMapper.Map(muscleGroup, new MuscleGroupDto());
    }
    [Authorize(FitLogsPermissions.MuscleGroups.Default)]
    public async Task<PagedResultDto<MuscleGroupDto>> GetListAsync(GetMuscleGroupListInput input)
    {
        var totalCount = await _muscleGroupRepository.GetCountAsync(
            filterText: input.FilterText,
            isActive: input.IsActive);
        
        var muscleGroups = await _muscleGroupRepository.GetListAsync(
            filterText: input.FilterText,
            isActive: input.IsActive,
            sorting: input.Sorting,
            maxResultCount: input.MaxResultCount,
            skipCount: input.SkipCount);
        
        var items = muscleGroups
            .Select(x => ObjectMapper.Map<MuscleGroup, MuscleGroupDto>(x))
            .ToList();
        return new PagedResultDto<MuscleGroupDto>(totalCount, items);
    }
    [Authorize(FitLogsPermissions.MuscleGroups.Create)]
    public async Task<MuscleGroupDto> CreateAsync(CreateUpdateMuscleGroupDto input)
    {
        var muscleGroup = await _muscleGroupManager.CreateAsync(
            input.Name,
            input.Code,
            input.DisplayOrder,
            input.Description);
        
        await _muscleGroupRepository.InsertAsync(muscleGroup, autoSave: true);
        return ObjectMapper.Map<MuscleGroup, MuscleGroupDto>(muscleGroup);

    }
    [Authorize(FitLogsPermissions.MuscleGroups.Update)]
    public async Task<MuscleGroupDto> UpdateAsync(Guid id, CreateUpdateMuscleGroupDto input)
    {
        var muscleGroup = await _muscleGroupRepository.GetAsync(id);

        await _muscleGroupManager.ChangeNameAsync(muscleGroup, input.Name);
        await _muscleGroupManager.ChangeCodeAsync(muscleGroup, input.Code);
        _muscleGroupManager.ChangeDisplayOrder(muscleGroup, input.DisplayOrder);
        _muscleGroupManager.ChangeDescription(muscleGroup, input.Description);

        await _muscleGroupRepository.UpdateAsync(muscleGroup, autoSave: true);

        return ObjectMapper.Map<MuscleGroup, MuscleGroupDto>(muscleGroup);
    }
    [Authorize(FitLogsPermissions.MuscleGroups.Manage)]
    public async Task ActivateAsync(Guid id)
    {
        var muscleGroup = await _muscleGroupRepository.GetAsync(id);
        _muscleGroupManager.Activate(muscleGroup);
        await _muscleGroupRepository.UpdateAsync(muscleGroup, autoSave: true);
        
    }
    [Authorize(FitLogsPermissions.MuscleGroups.Manage)]
    public async Task DeactivateAsync(Guid id)
    {
        var muscleGroup = await _muscleGroupRepository.GetAsync(id);
        await _muscleGroupManager.DeactivateAsync(muscleGroup);
        await _muscleGroupRepository.UpdateAsync(muscleGroup, autoSave: true);
        
        
    }
}