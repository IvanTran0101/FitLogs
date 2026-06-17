using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace FitLogs.Exercises;

public class MuscleGroupManager : DomainService
{
    private readonly IMuscleGroupRepository _muscleGroupRepository;
    private readonly IExerciseRepository _exerciseRepository;

    public MuscleGroupManager(IMuscleGroupRepository muscleGroupRepository, IExerciseRepository exerciseRepository)
    {
        _muscleGroupRepository = muscleGroupRepository;
        _exerciseRepository = exerciseRepository;
    }

    public async Task<MuscleGroup> CreateAsync(
        string name,
        string code,
        int displayOrder,
        string? description = null)
    {
        await CheckNameAsync(name);
        await CheckCodeAsync(code);

        return new MuscleGroup(
            GuidGenerator.Create(),
            name,
            code,
            displayOrder,
            description);
    }

    public async Task ChangeNameAsync(MuscleGroup muscleGroup, string name)
    {
        await CheckNameAsync(name, muscleGroup.Id);
        muscleGroup.SetName(name);
    }

    public async Task ChangeCodeAsync(MuscleGroup muscleGroup, string code)
    {
        await CheckCodeAsync(code, muscleGroup.Id);
        muscleGroup.SetCode(code);
    }

    public void ChangeDescription(MuscleGroup muscleGroup, string? description)
    {
        muscleGroup.SetDescription(description);
    }

    public void ChangeDisplayOrder(MuscleGroup muscleGroup, int displayOrder)
    {
        muscleGroup.SetDisplayOrder(displayOrder);
    }

    public async Task DeactivateAsync(MuscleGroup muscleGroup)
    {
        var isUsedByActiveExercise = await _exerciseRepository.AnyAsync(
            x => x.PrimaryMuscleGroupId == muscleGroup.Id && x.IsActive);

        if (isUsedByActiveExercise)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.MuscleGroupIsUsedByExercise);
        }

        muscleGroup.Deactivate();
    }

    public void Activate(MuscleGroup muscleGroup)
    {
        muscleGroup.Activate();
    }

    private async Task CheckNameAsync(string name, Guid? excludedId = null)
    {
        if (await _muscleGroupRepository.AnyAsync(x => x.Name == name && (!excludedId.HasValue || x.Id != excludedId.Value)))
        {
            throw new BusinessException(FitLogsDomainErrorCodes.MuscleGroupNameAlreadyExists);
        }
    }

    private async Task CheckCodeAsync(string code, Guid? excludedId = null)
    {
        if (await _muscleGroupRepository.CodeExistsAsync(code, excludedId))
        {
            throw new BusinessException(FitLogsDomainErrorCodes.MuscleGroupCodeAlreadyExists);
        }
    }
}