using FitLogs.Workouts;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace FitLogs;

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

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class WorkoutSessionToDtoMapper : MapperBase<WorkoutSession, WorkoutSessionDto>
{
    public override partial WorkoutSessionDto Map(WorkoutSession source);

    public override partial void Map(WorkoutSession source, WorkoutSessionDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class WorkoutSessionExerciseToDtoMapper : MapperBase<WorkoutSessionExercise, WorkoutSessionExerciseDto>
{
    public override partial WorkoutSessionExerciseDto Map(WorkoutSessionExercise source);

    public override partial void Map(WorkoutSessionExercise source, WorkoutSessionExerciseDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ExerciseSetToDtoMapper : MapperBase<ExerciseSet, ExerciseSetDto>
{
    public override partial ExerciseSetDto Map(ExerciseSet source);

    public override partial void Map(ExerciseSet source, ExerciseSetDto destination);
}