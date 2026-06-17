using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;

namespace FitLogs.Exercises;
[Authorize]
public class ExerciseAppService : ApplicationService, IExerciseAppService
{
    private readonly IExerciseRepository _exerciseRepository;
    private readonly ExerciseManager _exerciseManager;

    public ExerciseAppService(IExerciseRepository exerciseRepository, ExerciseManager exerciseManager)
    {
        _exerciseRepository = exerciseRepository;
        _exerciseManager = exerciseManager;
    }

    public async Task<ExerciseDto> GetAsync(Guid id)
    {
        var exercise = await _exerciseRepository.GetAsync(id);
        return ObjectMapper.Map<Exercise, ExerciseDto>(exercise);
        
    }

    public async Task<ExerciseDto> GetBySlugAsync(string slug)
    {
        var exercise = await _exerciseRepository.FindBySlugAsync(slug);
        if (exercise == null)
        {
            throw new EntityNotFoundException(typeof(Exercise), slug);
        }
        return ObjectMapper.Map<Exercise, ExerciseDto>(exercise);
        
    }

    public async Task<PagedResultDto<ExerciseDto>> GetListAsync(GetExerciseListInput input)
    {
        var totalCount = await _exerciseRepository.GetCountAsync(
            filterText:input.FilterText,
            muscleGroupId:input.MuscleGroupId,
            equipmentId:input.EquipmentId,
            exerciseDifficulty:input.Difficulty,
            trackingType:input.TrackingType,
            isActive:input.IsActive);
        
        var exercises = await _exerciseRepository.GetListAsync(
            filterText:input.FilterText,
            muscleGroupId:input.MuscleGroupId,
            equipmentId:input.EquipmentId,
            exerciseDifficulty:input.Difficulty,
            trackingType:input.TrackingType,
            isActive:input.IsActive,
            sorting:input.Sorting,
            maxResults:input.MaxResultCount,
            skipCount:input.SkipCount);
        
        var items = exercises.
            Select(x=> ObjectMapper.Map<Exercise, ExerciseDto>(x)).ToList();
        return new PagedResultDto<ExerciseDto>(totalCount, items);
        
    }

    public async Task<ExerciseDto> CreateAsync(CreateUpdateExerciseDto input)
    {
        var exercise = await _exerciseManager.CreateAsync(
            input.Name,
            input.Slug,
            input.PrimaryMuscleGroupId,
            input.EquipmentId,
            input.Difficulty,
            input.TrackingType,
            input.Description,
            input.ImageUrl,
            input.GifUrl,
            input.Instructions,
            input.FormTips,
            input.CommonMistakes);
        await _exerciseRepository.InsertAsync(exercise, autoSave: true);
        return ObjectMapper.Map<Exercise, ExerciseDto>(exercise);
    }

    public async Task<ExerciseDto> UpdateAsync(Guid id, CreateUpdateExerciseDto input)
    {
        var exercise = await _exerciseRepository.GetAsync(id);
        await _exerciseManager.ChangeNameAsync(exercise, input.Name, input.EquipmentId);
        await _exerciseManager.ChangeSlugAsync(exercise, input.Slug);
        await _exerciseManager.ChangeEquipmentAsync(exercise, input.EquipmentId);
        await _exerciseManager.ChangePrimaryMuscleGroupAsync(exercise, input.PrimaryMuscleGroupId);
        
        _exerciseManager.ChangeDescription(exercise, input.Description);
        exercise.SetDifficulty(input.Difficulty);
        exercise.SetTrackingType(input.TrackingType);
        exercise.SetMedia(input.ImageUrl, input.GifUrl);
        exercise.SetContent(input.Instructions, input.FormTips, input.CommonMistakes);
        
        await _exerciseRepository.UpdateAsync(exercise, autoSave: true);
        return ObjectMapper.Map<Exercise, ExerciseDto>(exercise);
        
    }

    public async Task ActivateAsync(Guid id)
    {
        var exercise = await _exerciseRepository.GetAsync(id);
        await _exerciseManager.ActivateAsync(exercise.Id);
        await _exerciseRepository.UpdateAsync(exercise, autoSave: true);
        
    }

    public async Task DeactivateAsync(Guid id)
    {
        var exercise = await _exerciseRepository.GetAsync(id);
        await _exerciseManager.DeactivateAsync(exercise.Id);
        await _exerciseRepository.UpdateAsync(exercise, autoSave: true);
        
    }

    public async Task<PagedResultDto<ExerciseDto>> GetSelectableListAsync(GetExerciseListInput input)
    {
        var totalCount = await _exerciseRepository.GetCountAsync(
            filterText: input.FilterText,
            muscleGroupId: input.MuscleGroupId,
            equipmentId: input.EquipmentId,
            exerciseDifficulty: input.Difficulty,
            trackingType: input.TrackingType,
            isActive: true);
        
        var exercises = await _exerciseRepository.GetListAsync(
            filterText: input.FilterText,
            muscleGroupId: input.MuscleGroupId,
            equipmentId: input.EquipmentId,
            exerciseDifficulty: input.Difficulty,
            trackingType: input.TrackingType,
            isActive:true,
            sorting:input.Sorting,
            maxResults:input.MaxResultCount,
            skipCount:input.SkipCount);
        var items = exercises.
            Select(x=> ObjectMapper.Map<Exercise, ExerciseDto>(x)).ToList();
        
        return new PagedResultDto<ExerciseDto>(totalCount, items);
        
    }
}