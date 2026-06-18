using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using FitLogs.Exercises;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;
using Volo.Abp.Users;

namespace FitLogs.Workouts;
[Authorize]
public class WorkoutPlanAppService : FitLogsAppService, IWorkoutPlanAppService
{
    private readonly IExerciseRepository _exerciseRepository;
    private readonly IWorkoutPlanRepository _workoutPlanRepository;
    private readonly WorkoutPlanManager _workoutPlanManager;

    public WorkoutPlanAppService(
        IWorkoutPlanRepository workoutPlanRepository,
        WorkoutPlanManager workoutPlanManager,
        IExerciseRepository exerciseRepository)
    {
        _workoutPlanRepository = workoutPlanRepository;
        _workoutPlanManager = workoutPlanManager;
        _exerciseRepository = exerciseRepository;
    }

    public async Task<WorkoutPlanDto> GetAsync(Guid id)
    {
        var workoutPlan = await GetWorkoutPlanWithDetailsAsync(id);

        EnsureWorkoutPlanOwner(workoutPlan);

        return ObjectMapper.Map<WorkoutPlan, WorkoutPlanDto>(workoutPlan);
    }

    public async Task<PagedResultDto<WorkoutPlanDto>> GetListAsync(GetWorkoutPlanListDto input)
    {
        var userId = GetCurrentUserId();

        var queryable = await _workoutPlanRepository.GetQueryableAsync();

        queryable = queryable
            .Where(x => x.UserId == userId)
            .WhereIf(
                !string.IsNullOrWhiteSpace(input.FilterText),
                x => x.Name.Contains(input.FilterText!) ||
                     (x.Description != null && x.Description.Contains(input.FilterText!))
            )
            .WhereIf(
                input.IsActive.HasValue,
                x => x.IsActive == input.IsActive!.Value
            )
            .WhereIf(
                input.Goal.HasValue,
                x => x.Goal == input.Goal!.Value
            )
            .WhereIf(
                input.Difficulty.HasValue,
                x => x.Difficulty == input.Difficulty!.Value
            );

        var totalCount = await AsyncExecuter.CountAsync(queryable);

        queryable = queryable.OrderBy(
            string.IsNullOrWhiteSpace(input.Sorting)
                ? $"{nameof(WorkoutPlan.Name)} asc"
                : input.Sorting
        );

        queryable = queryable
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount);

        var workoutPlans = await AsyncExecuter.ToListAsync(queryable);

        var dtos = workoutPlans
            .Select(ObjectMapper.Map<WorkoutPlan, WorkoutPlanDto>)
            .ToList();

        return new PagedResultDto<WorkoutPlanDto>(
            totalCount,
            dtos
        );
    }

    public async Task<WorkoutPlanDto> CreateAsync(CreateWorkoutPlanDto input)
    {
        var userId = GetCurrentUserId();

        var workoutPlan = await _workoutPlanManager.CreateAsync(
            userId,
            input.Name,
            input.Description,
            input.Goal,
            input.Difficulty

        );

        workoutPlan = await _workoutPlanRepository.InsertAsync(
            workoutPlan,
            autoSave: true
        );

        return ObjectMapper.Map<WorkoutPlan, WorkoutPlanDto>(workoutPlan);
    }

    public async Task<WorkoutPlanDto> UpdateAsync(Guid id, UpdateWorkoutPlanDto input)
    {
        var workoutPlan = await GetWorkoutPlanWithDetailsAsync(id);

        EnsureWorkoutPlanOwner(workoutPlan);

        await _workoutPlanManager.ChangeNameAsync(
            workoutPlan,
            input.Name
        );

        workoutPlan.SetDescription(input.Description);
        workoutPlan.SetGoal(input.Goal);
        workoutPlan.SetDifficulty(input.Difficulty);

        if (input.IsActive)
        {
            workoutPlan.Activate();
        }
        else
        {
            workoutPlan.Deactivate();
        }

        workoutPlan = await _workoutPlanRepository.UpdateAsync(
            workoutPlan,
            autoSave: true
        );

        return ObjectMapper.Map<WorkoutPlan, WorkoutPlanDto>(workoutPlan);
    }

    public async Task DeleteAsync(Guid id)
    {
        var workoutPlan = await GetWorkoutPlanWithDetailsAsync(id);

        EnsureWorkoutPlanOwner(workoutPlan);

        await _workoutPlanRepository.DeleteAsync(
            workoutPlan,
            autoSave: true
        );
    }

    public async Task<WorkoutPlanDto> AddExerciseAsync(
        Guid id,
        CreateWorkoutPlanExerciseDto input)
    {
        var workoutPlan = await GetWorkoutPlanWithDetailsAsync(id);

        EnsureWorkoutPlanOwner(workoutPlan);
        var exercise = await _exerciseRepository.GetAsync(input.ExerciseId);

        if (!exercise.IsActive)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.ExerciseIsInactive);
        }
        workoutPlan.AddExercise(
            GuidGenerator.Create(),
            input.ExerciseId,
            input.OrderIndex,
            input.DefaultSets,
            input.DefaultReps,
            input.DefaultWeightKg,
            input.RestSeconds,
            input.Note
        );

        workoutPlan = await _workoutPlanRepository.UpdateAsync(
            workoutPlan,
            autoSave: true
        );

        return ObjectMapper.Map<WorkoutPlan, WorkoutPlanDto>(workoutPlan);
    }

    public async Task<WorkoutPlanDto> UpdateExerciseAsync(
        Guid id,
        Guid workoutPlanExerciseId,
        UpdateWorkoutPlanExerciseDto input)
    {
        var workoutPlan = await GetWorkoutPlanWithDetailsAsync(id);

        EnsureWorkoutPlanOwner(workoutPlan);

        workoutPlan.UpdateExercise(
            workoutPlanExerciseId,
            input.OrderIndex,
            input.DefaultSets,
            input.DefaultReps,
            input.DefaultWeightKg,
            input.RestSeconds,
            input.Note
        );

        workoutPlan = await _workoutPlanRepository.UpdateAsync(
            workoutPlan,
            autoSave: true
        );

        return ObjectMapper.Map<WorkoutPlan, WorkoutPlanDto>(workoutPlan);
    }

    public async Task<WorkoutPlanDto> RemoveExerciseAsync(
        Guid id,
        Guid workoutPlanExerciseId)
    {
        var workoutPlan = await GetWorkoutPlanWithDetailsAsync(id);

        EnsureWorkoutPlanOwner(workoutPlan);

        workoutPlan.RemoveExercise(workoutPlanExerciseId);

        workoutPlan = await _workoutPlanRepository.UpdateAsync(
            workoutPlan,
            autoSave: true
        );

        return ObjectMapper.Map<WorkoutPlan, WorkoutPlanDto>(workoutPlan);
    }

    private Guid GetCurrentUserId()
    {
        return CurrentUser.GetId();
    }

    private async Task<WorkoutPlan> GetWorkoutPlanWithDetailsAsync(Guid id)
    {
        var workoutPlan = await _workoutPlanRepository.FindWithDetailsAsync(id);

        if (workoutPlan == null)
        {
            throw new EntityNotFoundException(typeof(WorkoutPlan), id);
        }

        return workoutPlan;
    }

    private void EnsureWorkoutPlanOwner(WorkoutPlan workoutPlan)
    {
        if (workoutPlan.UserId != GetCurrentUserId())
        {
            throw new BusinessException(FitLogsDomainErrorCodes.WorkoutPlanAccessDenied);
        }
    }
}