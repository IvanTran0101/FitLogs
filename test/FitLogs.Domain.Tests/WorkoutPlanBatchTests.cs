using System;
using System.Linq;
using Shouldly;
using Volo.Abp;
using Xunit;

namespace FitLogs.Workouts;

public class WorkoutPlanBatchTests
{
    [Fact]
    public void Batch_add_validates_before_mutating_the_plan()
    {
        var plan = new WorkoutPlan(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Push",
            null,
            WorkoutGoal.GeneralFitness,
            WorkoutDifficulty.Beginner);

        var firstExerciseId = Guid.NewGuid();
        var secondExerciseId = Guid.NewGuid();
        var drafts = new[]
        {
            new WorkoutPlanExerciseDraft(firstExerciseId, 0, 3, 8),
            new WorkoutPlanExerciseDraft(secondExerciseId, 0, 3, 8)
        };

        Should.Throw<BusinessException>(() => plan.AddExercises(drafts));

        plan.Exercises.ShouldBeEmpty();
    }

    [Fact]
    public void Batch_adds_all_valid_exercises_as_one_operation()
    {
        var plan = new WorkoutPlan(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Push",
            null,
            WorkoutGoal.GeneralFitness,
            WorkoutDifficulty.Beginner);

        plan.AddExercises(new[]
        {
            new WorkoutPlanExerciseDraft(Guid.NewGuid(), 1, 3, 8),
            new WorkoutPlanExerciseDraft(Guid.NewGuid(), 2, 3, 8)
        });

        plan.Exercises.Count.ShouldBe(2);
        plan.Exercises.Select(x => x.OrderIndex).ShouldBe(new[] { 1, 2 });
    }
}
