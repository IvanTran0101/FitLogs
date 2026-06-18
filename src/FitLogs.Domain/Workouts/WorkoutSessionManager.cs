using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace FitLogs.Workouts;

public class WorkoutSessionManager : DomainService
{
    private readonly IWorkoutSessionRepository _workoutSessionRepository;

    public WorkoutSessionManager(IWorkoutSessionRepository workoutSessionRepository)
    {
        _workoutSessionRepository = workoutSessionRepository;
    }

    public async Task<WorkoutSession> CreateAsync(
        Guid userId,
        Guid? workoutPlanId,
        string name,
        DateTime startTime,
        string? note = null)
    {
        if (await _workoutSessionRepository.HasInProgressSessionAsync(userId))
        {
            throw new BusinessException(FitLogsDomainErrorCodes.UserHasInProgressWorkoutSession);
        }
        return new WorkoutSession(
            GuidGenerator.Create(),
            userId,
            workoutPlanId,
            name,
            startTime,
            note
        );
    }

    public async Task<WorkoutSession> CreateFromPlanAsync(
        Guid userId,
        WorkoutPlan workoutPlan,
        DateTime startTime,
        string? note = null)
    {
        Check.NotNull(workoutPlan, nameof(workoutPlan));

        if (await _workoutSessionRepository.HasInProgressSessionAsync(userId))
        {
            throw new BusinessException(FitLogsDomainErrorCodes.UserHasInProgressWorkoutSession);
        }

        if (workoutPlan.UserId != userId)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.WorkoutPlanNotBelongToUser);
        }

        if (!workoutPlan.IsActive)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.WorkoutPlanIsInactive);
        }

        var session = new WorkoutSession(
            GuidGenerator.Create(),
            userId,
            workoutPlan.Id,
            workoutPlan.Name,
            startTime,
            note
        );

        foreach (var planExercise in workoutPlan.Exercises.OrderBy(x => x.OrderIndex))
        {
            session.AddExercise(
                GuidGenerator.Create(),
                planExercise.ExerciseId,
                planExercise.OrderIndex,
                planExercise.DefaultSets,
                planExercise.DefaultReps,
                planExercise.DefaultWeightKg,
                planExercise.RestSeconds,
                planExercise.Note,
                planExercise.Id
            );
        }

        return session;
    }
    public async Task<WorkoutSession> CreateFreeSessionAsync(
        Guid userId,
        string name,
        DateTime startTime,
        string? note = null)
    {
        if (await _workoutSessionRepository.HasInProgressSessionAsync(userId))
        {
            throw new BusinessException(FitLogsDomainErrorCodes.UserHasInProgressWorkoutSession);
        }

        return new WorkoutSession(
            GuidGenerator.Create(),
            userId,
            null,
            name,
            startTime,
            note
        );
    }
}