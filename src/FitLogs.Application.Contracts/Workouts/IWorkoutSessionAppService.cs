using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace FitLogs.Workouts;

public interface IWorkoutSessionAppService : IApplicationService
{
    Task<WorkoutSessionDto> GetAsync(Guid id);

    Task<PagedResultDto<WorkoutSessionDto>> GetListAsync(GetWorkoutSessionListDto input);

    Task<WorkoutSessionDto> CreateAsync(CreateWorkoutSessionDto input);

    Task<WorkoutSessionDto> AddExerciseAsync(Guid id, AddWorkoutSessionExerciseDto input);

    Task<WorkoutSessionDto> UpdateExerciseAsync(

        Guid id,

        Guid workoutSessionExerciseId,

        UpdateWorkoutSessionExerciseDto input

    );

    Task<WorkoutSessionDto> RemoveExerciseAsync(Guid id, Guid workoutSessionExerciseId);

    Task<WorkoutSessionDto> AddSetAsync(

        Guid id,

        Guid workoutSessionExerciseId,

        AddExerciseSetDto input

    );

    Task<WorkoutSessionDto> UpdateSetAsync(

        Guid id,

        Guid workoutSessionExerciseId,

        Guid exerciseSetId,

        UpdateExerciseSetDto input

    );

    Task<WorkoutSessionDto> RemoveSetAsync(

        Guid id,

        Guid workoutSessionExerciseId,

        Guid exerciseSetId

    );

    Task<WorkoutSessionDto> CompleteSetAsync(

        Guid id,

        Guid workoutSessionExerciseId,

        Guid exerciseSetId

    );

    Task<WorkoutSessionDto> UncompleteSetAsync(

        Guid id,

        Guid workoutSessionExerciseId,

        Guid exerciseSetId

    );

    Task<WorkoutSessionDto> CompleteAsync(Guid id);

    Task<WorkoutSessionDto> CancelAsync(Guid id);

    Task DeleteAsync(Guid id);
    Task<WorkoutSessionDto?> GetActiveAsync();

    Task<WorkoutSessionExerciseDto> GetCurrentExerciseAsync(Guid id);
    
    Task<WorkoutSessionDto> MoveToNextExerciseAsync(Guid id);
    Task<WorkoutSessionDto> MoveToPreviousExerciseAsync(Guid id);
    
    Task<WorkoutSessionDto> SkipCurrentExerciseAsync(Guid id);

}