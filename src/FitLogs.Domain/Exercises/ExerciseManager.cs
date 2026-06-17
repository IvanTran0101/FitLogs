using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace FitLogs.Exercises;

public class ExerciseManager : DomainService
{
    private readonly IExerciseRepository _exerciseRepository;
    private readonly IMuscleGroupRepository _muscleGroupRepository;
    private readonly IEquipmentRepository _equipmentRepository;

    public ExerciseManager(IExerciseRepository exerciseRepository, IMuscleGroupRepository muscleGroupRepository, IEquipmentRepository equipmentRepository)
    {
        _exerciseRepository = exerciseRepository;
        _muscleGroupRepository = muscleGroupRepository;
        _equipmentRepository = equipmentRepository;
    }

    public async Task<Exercise> CreateAsync(
        string name,
        string slug,
        Guid primaryMuscleGroupId,
        Guid equipmentId,
        ExerciseDifficulty difficulty,
        ExerciseTrackingType trackingType,
        string? description = null,
        string? imageUrl = null,
        string? gifUrl = null,
        string? instructions = null,
        string? formTips = null,
        string? commonMistakes = null)
    {
        await CheckNameAsync(name, equipmentId);
        await CheckSlugAsync(slug);
        await CheckMuscleGroupAsync(primaryMuscleGroupId);
        await CheckEquipmentAsync(equipmentId);

        return new Exercise(
            GuidGenerator.Create(),
            name,
            slug,
            trackingType,
            primaryMuscleGroupId,
            equipmentId,
            difficulty,
            description,
            formTips,
            imageUrl,
            gifUrl,
            instructions,
            commonMistakes);
    }

    public async Task ChangeNameAsync(Exercise exercise, string name, Guid equipmentId)
    {
        await CheckNameAsync(name, equipmentId, exercise.Id);
        exercise.SetName(name);
    }

    public void ChangeDescription(Exercise exercise, string? description)
    {
        exercise.SetDescription(description);
    }

    public async Task ChangeSlugAsync(Exercise exercise, string slug)
    {
        await CheckSlugAsync(slug, exercise.Id);
        exercise.SetSlug(slug);
    }

    public async Task ChangePrimaryMuscleGroupAsync(Exercise exercise, Guid primaryMuscleGroupId)
    {
        await CheckMuscleGroupAsync(primaryMuscleGroupId);
        exercise.SetPrimaryMuscleGroup(primaryMuscleGroupId);
    }

    public async Task ChangeEquipmentAsync(Exercise exercise, Guid equipmentId)
    {
        await CheckEquipmentAsync(equipmentId);
        await CheckNameAsync(exercise.Name, equipmentId, exercise.Id);
        exercise.SetEquipment(equipmentId);
    }
    private async Task CheckNameAsync(string name, Guid equipmentId, Guid? excludedId = null)
    {
        if (await _exerciseRepository.AnyAsync(x =>
                x.Name == name &&
                x.EquipmentId == equipmentId &&
                (!excludedId.HasValue || x.Id != excludedId.Value)))
        {
            throw new BusinessException(FitLogsDomainErrorCodes.ExerciseNameAlreadyExists);
        }
    }

    private async Task CheckSlugAsync(string slug, Guid? excludedId = null)
    {
        if (await _exerciseRepository.SlugExistsAsync(slug, excludedId))
        {
            throw new BusinessException(FitLogsDomainErrorCodes.ExerciseSlugAlreadyExists);
        }
    }

    private async Task CheckMuscleGroupAsync(Guid muscleGroupId)
    {
        var exists = await _muscleGroupRepository.AnyAsync(
            x => x.Id == muscleGroupId && x.IsActive);
        if (!exists)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.MuscleGroupNotFound);
        }
    }

    private async Task CheckEquipmentAsync(Guid equipmentId)
    {
        if (equipmentId == Guid.Empty)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.ExerciseEquipmentRequired);
        }
        var exists = await _equipmentRepository.AnyAsync(
            x => x.Id == equipmentId && x.IsActive);
        if (!exists)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.EquipmentNotFound);
        }
    }
    public async Task ActivateAsync(Guid id)
    {
        var exercise = await _exerciseRepository.GetAsync(id);
        exercise.Activate();
    }

    public async Task DeactivateAsync(Guid id)
    {
        var exercise = await _exerciseRepository.GetAsync(id);
        exercise.Deactivate();
    }
}