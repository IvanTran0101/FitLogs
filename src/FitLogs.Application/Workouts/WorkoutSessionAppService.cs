using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using FitLogs.Exercises;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Linq;
using Volo.Abp.Users;

namespace FitLogs.Workouts;
[Authorize]
public class WorkoutSessionAppService : FitLogsAppService, IWorkoutSessionAppService
{
    private readonly IExerciseRepository _exerciseRepository;
    private readonly IWorkoutPlanRepository _workoutPlanRepository;
    private readonly IWorkoutSessionRepository _workoutSessionRepository;
    private readonly WorkoutSessionManager _workoutSessionManager;

    public WorkoutSessionAppService(
        IWorkoutSessionRepository workoutSessionRepository,
        WorkoutSessionManager workoutSessionManager,
        IWorkoutPlanRepository workoutPlanRepository,
        IExerciseRepository exercisesRepository)
    {
        _workoutSessionRepository = workoutSessionRepository;
        _workoutSessionManager = workoutSessionManager;
        _workoutPlanRepository = workoutPlanRepository;
        _exerciseRepository = exercisesRepository;
    }

    public async Task<WorkoutSessionDto> GetAsync(Guid id)
    {
        var workoutSession = await GetWorkoutSessionWithDetailsAsync(id);

        EnsureWorkoutSessionOwner(workoutSession);

        return ObjectMapper.Map<WorkoutSession, WorkoutSessionDto>(workoutSession);
    }

    public async Task<PagedResultDto<WorkoutSessionDto>> GetListAsync(GetWorkoutSessionListDto input)
    {
        var userId = GetCurrentUserId();

        var queryable = await _workoutSessionRepository.GetQueryableAsync();

        queryable = queryable
            .Where(x => x.UserId == userId)
            .WhereIf(
                !string.IsNullOrWhiteSpace(input.FilterText),
                x => x.Name.Contains(input.FilterText!) ||
                     (x.Note != null && x.Note.Contains(input.FilterText!))
            )
            .WhereIf(
                input.Status.HasValue,
                x => x.Status == input.Status!.Value
            )
            .WhereIf(
                input.WorkoutPlanId.HasValue,
                x => x.WorkoutPlanId == input.WorkoutPlanId!.Value
            )
            .WhereIf(
                input.StartedFrom.HasValue,
                x => x.StartedAt >= input.StartedFrom!.Value
            )
            .WhereIf(
                input.StartedTo.HasValue,
                x => x.StartedAt <= input.StartedTo!.Value
            );

        var totalCount = await AsyncExecuter.CountAsync(queryable);

        queryable = queryable.OrderBy(
            string.IsNullOrWhiteSpace(input.Sorting)
                ? $"{nameof(WorkoutSession.StartedAt)} desc"
                : input.Sorting
        );

        queryable = queryable
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount);

        var workoutSessions = await AsyncExecuter.ToListAsync(queryable);

        var dtos = workoutSessions
            .Select(ObjectMapper.Map<WorkoutSession, WorkoutSessionDto>)
            .ToList();

        return new PagedResultDto<WorkoutSessionDto>(
            totalCount,
            dtos
        );
    }

    public async Task<WorkoutSessionDto> CreateAsync(CreateWorkoutSessionDto input)
    {
        var userId = GetCurrentUserId();
        WorkoutSession workoutSession;
        if (input.WorkoutPlanId.HasValue)
        {
            var workoutPlan = await _workoutPlanRepository.GetAsync(input.WorkoutPlanId.Value);
            workoutSession = await _workoutSessionManager.CreateFromPlanAsync(
                userId,
                workoutPlan,
                input.StartedAt,
                input.Note
            );
        }
        else
        {
            workoutSession = await _workoutSessionManager.CreateFreeSessionAsync(
                userId,
                input.Name,
                input.StartedAt,
                input.Note
            );
        }
        workoutSession = await _workoutSessionRepository.InsertAsync(
            workoutSession,
            autoSave: true
        );
        return ObjectMapper.Map<WorkoutSession, WorkoutSessionDto>(workoutSession);
    }

    public async Task<WorkoutSessionDto> AddExerciseAsync(
        Guid id,
        AddWorkoutSessionExerciseDto input)
    {
        var workoutSession = await GetWorkoutSessionWithDetailsAsync(id);

        EnsureWorkoutSessionOwner(workoutSession);
        var exercise = await _exerciseRepository.GetAsync(input.ExerciseId);
        if (exercise == null)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.ExerciseIsInactive);
        }
        workoutSession.AddExercise(
            GuidGenerator.Create(),
            input.ExerciseId,
            input.OrderIndex,
            input.TargetSets,
            input.TargetReps,
            input.TargetWeightKg,
            input.RestSeconds,
            input.Note
        );

        workoutSession = await _workoutSessionRepository.UpdateAsync(
            workoutSession,
            autoSave: true
        );

        return ObjectMapper.Map<WorkoutSession, WorkoutSessionDto>(workoutSession);
    }

    public async Task<WorkoutSessionDto> UpdateExerciseAsync(
        Guid id,
        Guid workoutSessionExerciseId,
        UpdateWorkoutSessionExerciseDto input)
    {
        var workoutSession = await GetWorkoutSessionWithDetailsAsync(id);

        EnsureWorkoutSessionOwner(workoutSession);

        workoutSession.UpdateExercise(
            workoutSessionExerciseId,
            input.OrderIndex,
            input.TargetSets,
            input.TargetReps,
            input.TargetWeightKg,
            input.RestSeconds,
            input.Note
        );

        workoutSession = await _workoutSessionRepository.UpdateAsync(
            workoutSession,
            autoSave: true
        );

        return ObjectMapper.Map<WorkoutSession, WorkoutSessionDto>(workoutSession);
    }

    public async Task<WorkoutSessionDto> RemoveExerciseAsync(
        Guid id,
        Guid workoutSessionExerciseId)
    {
        var workoutSession = await GetWorkoutSessionWithDetailsAsync(id);

        EnsureWorkoutSessionOwner(workoutSession);

        workoutSession.RemoveExercise(workoutSessionExerciseId);

        workoutSession = await _workoutSessionRepository.UpdateAsync(
            workoutSession,
            autoSave: true
        );

        return ObjectMapper.Map<WorkoutSession, WorkoutSessionDto>(workoutSession);
    }

    public async Task<WorkoutSessionDto> AddSetAsync(
        Guid id,
        Guid workoutSessionExerciseId,
        AddExerciseSetDto input)
    {
        var workoutSession = await GetWorkoutSessionWithDetailsAsync(id);

        EnsureWorkoutSessionOwner(workoutSession);

        workoutSession.AddSetToExercise(
            workoutSessionExerciseId,
            GuidGenerator.Create(),
            input.SetNumber,
            input.WeightKg,
            input.Reps,
            input.Rpe,
            input.Note
        );

        workoutSession = await _workoutSessionRepository.UpdateAsync(
            workoutSession,
            autoSave: true
        );

        return ObjectMapper.Map<WorkoutSession, WorkoutSessionDto>(workoutSession);
    }

    public async Task<WorkoutSessionDto> UpdateSetAsync(
        Guid id,
        Guid workoutSessionExerciseId,
        Guid exerciseSetId,
        UpdateExerciseSetDto input)
    {
        var workoutSession = await GetWorkoutSessionWithDetailsAsync(id);

        EnsureWorkoutSessionOwner(workoutSession);

        workoutSession.UpdateSetInExercise(
            workoutSessionExerciseId,
            exerciseSetId,
            input.SetNumber,
            input.WeightKg,
            input.Reps,
            input.Rpe,
            input.Note
        );

        workoutSession = await _workoutSessionRepository.UpdateAsync(
            workoutSession,
            autoSave: true
        );

        return ObjectMapper.Map<WorkoutSession, WorkoutSessionDto>(workoutSession);
    }

    public async Task<WorkoutSessionDto> RemoveSetAsync(
        Guid id,
        Guid workoutSessionExerciseId,
        Guid exerciseSetId)
    {
        var workoutSession = await GetWorkoutSessionWithDetailsAsync(id);

        EnsureWorkoutSessionOwner(workoutSession);

        workoutSession.RemoveSetFromExercise(
            workoutSessionExerciseId,
            exerciseSetId
        );

        workoutSession = await _workoutSessionRepository.UpdateAsync(
            workoutSession,
            autoSave: true
        );

        return ObjectMapper.Map<WorkoutSession, WorkoutSessionDto>(workoutSession);
    }

    public async Task<WorkoutSessionDto> CompleteSetAsync(
        Guid id,
        Guid workoutSessionExerciseId,
        Guid exerciseSetId)
    {
        var workoutSession = await GetWorkoutSessionWithDetailsAsync(id);

        EnsureWorkoutSessionOwner(workoutSession);

        workoutSession.CompleteSetInExercise(
            workoutSessionExerciseId,
            exerciseSetId,
            Clock.Now
        );

        workoutSession = await _workoutSessionRepository.UpdateAsync(
            workoutSession,
            autoSave: true
        );

        return ObjectMapper.Map<WorkoutSession, WorkoutSessionDto>(workoutSession);
    }

    public async Task<WorkoutSessionDto> UncompleteSetAsync(
        Guid id,
        Guid workoutSessionExerciseId,
        Guid exerciseSetId)
    {
        var workoutSession = await GetWorkoutSessionWithDetailsAsync(id);

        EnsureWorkoutSessionOwner(workoutSession);

        workoutSession.UncompleteSetInExercise(
            workoutSessionExerciseId,
            exerciseSetId
        );

        workoutSession = await _workoutSessionRepository.UpdateAsync(
            workoutSession,
            autoSave: true
        );

        return ObjectMapper.Map<WorkoutSession, WorkoutSessionDto>(workoutSession);
    }

    public async Task<WorkoutSessionDto> CompleteAsync(Guid id)
    {
        var workoutSession = await GetWorkoutSessionWithDetailsAsync(id);

        EnsureWorkoutSessionOwner(workoutSession);

        workoutSession.Complete(Clock.Now);

        workoutSession = await _workoutSessionRepository.UpdateAsync(
            workoutSession,
            autoSave: true
        );

        return ObjectMapper.Map<WorkoutSession, WorkoutSessionDto>(workoutSession);
    }

    public async Task<WorkoutSessionDto> CancelAsync(Guid id)
    {
        var workoutSession = await GetWorkoutSessionWithDetailsAsync(id);

        EnsureWorkoutSessionOwner(workoutSession);

        workoutSession.Cancel(Clock.Now);

        workoutSession = await _workoutSessionRepository.UpdateAsync(
            workoutSession,
            autoSave: true
        );

        return ObjectMapper.Map<WorkoutSession, WorkoutSessionDto>(workoutSession);
    }

    public async Task DeleteAsync(Guid id)
    {
        var workoutSession = await GetWorkoutSessionWithDetailsAsync(id);

        EnsureWorkoutSessionOwner(workoutSession);

        await _workoutSessionRepository.DeleteAsync(
            workoutSession,
            autoSave: true
        );
    }

    private Guid GetCurrentUserId()
    {
        return CurrentUser.GetId();
    }

    private async Task<WorkoutSession> GetWorkoutSessionWithDetailsAsync(Guid id)
    {
        var workoutSession = await _workoutSessionRepository.FindWithDetailsAsync(id);

        if (workoutSession == null)
        {
            throw new EntityNotFoundException(typeof(WorkoutSession), id);
        }

        return workoutSession;
    }

    private void EnsureWorkoutSessionOwner(WorkoutSession workoutSession)
    {
        if (workoutSession.UserId != GetCurrentUserId())
        {
            throw new BusinessException(FitLogsDomainErrorCodes.WorkoutSessionAccessDenied);
        }
    }

    private WorkoutSessionExercise GetWorkoutSessionExerciseOrThrow(
        WorkoutSession workoutSession,
        Guid workoutSessionExerciseId)
    {
        var exercise = workoutSession.Exercises
            .FirstOrDefault(x => x.Id == workoutSessionExerciseId);

        if (exercise == null)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.WorkoutSessionExerciseNotFound);
        }

        return exercise;
    }
}