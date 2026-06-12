using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace FitLogs.Workouts;
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class WorkoutPlanToDtoMapper : MapperBase<WorkoutPlan, WorkoutPlanDto>
{
    public override partial WorkoutPlanDto Map(WorkoutPlan source);
    public override partial void Map(WorkoutPlan source, WorkoutPlanDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class WorkoutPlanExerciseToDtoMapper : MapperBase<WorkoutPlanExercise, WorkoutPlanExerciseDto>
{
    public override partial WorkoutPlanExerciseDto Map(WorkoutPlanExercise source);
    public override partial void Map(WorkoutPlanExercise source, WorkoutPlanExerciseDto destination);
}