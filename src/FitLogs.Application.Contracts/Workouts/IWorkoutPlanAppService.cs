using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace FitLogs.Workouts;

public interface IWorkoutPlanAppService : IApplicationService
{
    Task<WorkoutPlanDto> GetAsync(Guid id);

    Task<PagedResultDto<WorkoutPlanDto>> GetListAsync(GetWorkoutPlanListDto input);

    Task<WorkoutPlanDto> CreateAsync(CreateWorkoutPlanDto input);

    Task<WorkoutPlanDto> UpdateAsync(Guid id, UpdateWorkoutPlanDto input);

    Task DeleteAsync(Guid id);

    Task<WorkoutPlanDto> AddExerciseAsync(Guid id, CreateWorkoutPlanExerciseDto input);

    Task<WorkoutPlanDto> UpdateExerciseAsync(

        Guid id,

        Guid workoutPlanExerciseId,

        UpdateWorkoutPlanExerciseDto input

    );

    Task<WorkoutPlanDto> RemoveExerciseAsync(Guid id, Guid workoutPlanExerciseId);
    
    Task ArchiveAsync(Guid id);
    
    Task<WorkoutPlanDto> RestoreAsync(Guid id);
    
    Task<WorkoutPlanDto> ReorderExercisesAsync(Guid id,
        ReorderWorkoutPlanExercisesDto input);
}