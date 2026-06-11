using System;
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
}