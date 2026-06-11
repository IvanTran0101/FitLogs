using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace FitLogs.Workouts;

public class WorkoutPlanManager : DomainService
{
    private readonly IWorkoutPlanRepository _workoutPlanRepository;

    public WorkoutPlanManager(IWorkoutPlanRepository workoutPlanRepository)
    {
        _workoutPlanRepository = workoutPlanRepository;
    }

    public async Task<WorkoutPlan> CreateAsync(
        Guid userId,
        string name,
        string? description,
        WorkoutGoal goal,
        WorkoutDifficulty difficulty,
        bool isActive = true)
    
    {
        if (await _workoutPlanRepository.ExistsByUserAndNameAsync(userId, name))
        {
            throw new BusinessException(FitLogsDomainErrorCodes.WorkoutPlanNameAlreadyExists);
        }
        return new WorkoutPlan(
            GuidGenerator.Create(),
            userId,
            name,
            description,
            goal,
            difficulty,
            isActive
        );
    }

    public async Task ChangeNameAsync(WorkoutPlan workoutPlan, string newName)
    {
        if (await _workoutPlanRepository.ExistsByUserAndNameAsync(workoutPlan.UserId, newName, workoutPlan.Id))
        {
            throw new BusinessException(FitLogsDomainErrorCodes.WorkoutPlanNameAlreadyExists);
        }
        workoutPlan.SetName(newName);
    }
}