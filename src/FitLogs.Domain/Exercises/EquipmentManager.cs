using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace FitLogs.Exercises;

public class EquipmentManager : DomainService
{
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IExerciseRepository _exerciseRepository;

    public EquipmentManager(IEquipmentRepository equipmentRepository, IExerciseRepository exerciseRepository)
    {
        _equipmentRepository = equipmentRepository;
        _exerciseRepository = exerciseRepository;
    }

    public async Task<Equipment> CreateAsync(
        string name,
        string code,
        int displayOrder,
        string? description = null)
    {
        await CheckNameAsync(name);
        await CheckCodeAsync(code);

        return new Equipment(
            GuidGenerator.Create(),
            name,
            code,
            displayOrder,
            description);
    }

    public async Task ChangeNameAsync(Equipment equipment, string name)
    {
        await CheckNameAsync(name, equipment.Id);
        equipment.SetName(name);
    }

    public async Task ChangeCodeAsync(Equipment equipment, string code)
    {
        await CheckCodeAsync(code, equipment.Id);
        equipment.SetCode(code);
    }

    public void ChangeDisplayOrder(Equipment equipment, int displayOrder)
    {
        equipment.SetDisplayOrder(displayOrder);
    }

    public void ChangeDescription(Equipment equipment, string? description)
    {
        equipment.SetDescription(description);
    }

    public async Task DeactivateAsync(Equipment equipment)
    {
        var isUsedByActiveExercise = await _exerciseRepository.AnyAsync(
            x => x.EquipmentId == equipment.Id && x.IsActive);
        if (isUsedByActiveExercise)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.EquipmentIsUsedByExercise);
        }
        equipment.Deactivate();
    }

    public void Activate(Equipment equipment)
    {
        equipment.Activate();
    }

    private async Task CheckNameAsync(string name, Guid? excludedId = null)
    {
        if (await _equipmentRepository.AnyAsync(x => x.Name == name && (!excludedId.HasValue || x.Id != excludedId.Value)))
        {
            throw new BusinessException(FitLogsDomainErrorCodes.EquipmentNameAlreadyExists);
        }
    }

    private async Task CheckCodeAsync(string code, Guid? excludedId = null)
    {
        if (await _equipmentRepository.CodeExistsAsync(code, excludedId))
        {
            throw new BusinessException(FitLogsDomainErrorCodes.EquipmentCodeAlreadyExists);
        }
    }
}