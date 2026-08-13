using System;
using System.Linq;
using Shouldly;
using Volo.Abp;
using Xunit;

namespace FitLogs.Workouts;

public class WorkoutSessionStateTests
{
    [Fact]
    public void Completing_the_target_sets_marks_the_exercise_and_advances_the_pointer()
    {
        var session = CreateSession();
        var firstExerciseId = Guid.NewGuid();
        var secondExerciseId = Guid.NewGuid();
        session.AddExercise(Guid.NewGuid(), firstExerciseId, 0, 1, 8);
        session.AddExercise(Guid.NewGuid(), secondExerciseId, 1, 1, 8);
        var firstSessionExercise = session.Exercises.First(x => x.ExerciseId == firstExerciseId);
        var secondSessionExercise = session.Exercises.First(x => x.ExerciseId == secondExerciseId);

        session.AddSetToExercise(firstSessionExercise.Id, Guid.NewGuid(), 1, 20, 8);
        session.CompleteSetInExercise(firstSessionExercise.Id, firstSessionExercise.Sets.Single().Id, DateTime.UtcNow);

        firstSessionExercise.Status.ShouldBe(WorkoutSessionExerciseStatus.Completed);
        session.CurrentWorkoutSessionExerciseId.ShouldBe(secondSessionExercise.Id);
        secondSessionExercise.Status.ShouldBe(WorkoutSessionExerciseStatus.InProgress);
    }

    [Fact]
    public void Removing_current_exercise_selects_the_next_available_exercise()
    {
        var session = CreateSession();
        session.AddExercise(Guid.NewGuid(), Guid.NewGuid(), 0, 1, 8);
        var currentId = session.CurrentWorkoutSessionExerciseId!.Value;
        session.AddExercise(Guid.NewGuid(), Guid.NewGuid(), 1, 1, 8);
        var nextId = session.Exercises.OrderBy(x => x.OrderIndex).Last().Id;

        session.RemoveExercise(currentId);

        session.CurrentWorkoutSessionExerciseId.ShouldBe(nextId);
    }

    [Fact]
    public void A_session_cannot_complete_without_a_completed_exercise()
    {
        var session = CreateSession();
        session.AddExercise(Guid.NewGuid(), Guid.NewGuid(), 0, 1, 8);

        var exception = Should.Throw<BusinessException>(() => session.Complete(DateTime.UtcNow));

        exception.Code.ShouldBe(FitLogsDomainErrorCodes.WorkoutSessionCannotCompleteWithoutCompletedExercise);
    }

    [Fact]
    public void Completing_a_session_reconciles_an_exercise_with_completed_target_sets()
    {
        var session = CreateSession();
        session.AddExercise(Guid.NewGuid(), Guid.NewGuid(), 0, 1, 8);
        var exercise = session.Exercises.Single();
        session.AddSetToExercise(exercise.Id, Guid.NewGuid(), 1, 20, 8);
        session.CompleteSetInExercise(exercise.Id, exercise.Sets.Single().Id, DateTime.UtcNow);
        exercise.MarkInProgress();

        session.Complete(DateTime.UtcNow);

        session.Status.ShouldBe(WorkoutSessionStatus.Completed);
        exercise.Status.ShouldBe(WorkoutSessionExerciseStatus.Completed);
    }

    [Fact]
    public void Skipping_a_set_keeps_it_visible_but_does_not_count_towards_the_target()
    {
        var session = CreateSession();
        session.AddExercise(Guid.NewGuid(), Guid.NewGuid(), 0, 1, 8);
        var exercise = session.Exercises.Single();
        var setId = Guid.NewGuid();
        session.AddSetToExercise(exercise.Id, setId, 1, 20, 8);

        var skippedAt = DateTime.UtcNow;
        session.SkipSetInExercise(exercise.Id, setId, skippedAt);

        var set = exercise.Sets.Single();
        set.IsSkipped.ShouldBeTrue();
        set.SkippedAt.ShouldBe(skippedAt);
        set.IsCompleted.ShouldBeFalse();
        exercise.HasReachedTargetSets.ShouldBeFalse();
        exercise.Status.ShouldBe(WorkoutSessionExerciseStatus.InProgress);
    }

    [Fact]
    public void A_skipped_set_can_be_reopened_and_completed_later()
    {
        var session = CreateSession();
        session.AddExercise(Guid.NewGuid(), Guid.NewGuid(), 0, 1, 8);
        var exercise = session.Exercises.Single();
        var setId = Guid.NewGuid();
        session.AddSetToExercise(exercise.Id, setId, 1, 20, 8);
        session.SkipSetInExercise(exercise.Id, setId, DateTime.UtcNow);

        session.UnskipSetInExercise(exercise.Id, setId);
        session.CompleteSetInExercise(exercise.Id, setId, DateTime.UtcNow);

        var set = exercise.Sets.Single();
        set.IsSkipped.ShouldBeFalse();
        set.SkippedAt.ShouldBeNull();
        set.IsCompleted.ShouldBeTrue();
        exercise.HasReachedTargetSets.ShouldBeTrue();
        exercise.Status.ShouldBe(WorkoutSessionExerciseStatus.Completed);
    }

    [Fact]
    public void A_skipped_set_cannot_be_completed_without_reopening_it()
    {
        var session = CreateSession();
        session.AddExercise(Guid.NewGuid(), Guid.NewGuid(), 0, 1, 8);
        var exercise = session.Exercises.Single();
        var setId = Guid.NewGuid();
        session.AddSetToExercise(exercise.Id, setId, 1, 20, 8);
        session.SkipSetInExercise(exercise.Id, setId, DateTime.UtcNow);

        var exception = Should.Throw<BusinessException>(() =>
            session.CompleteSetInExercise(exercise.Id, setId, DateTime.UtcNow));

        exception.Code.ShouldBe(FitLogsDomainErrorCodes.ExerciseSetAlreadySkipped);
    }

    private static WorkoutSession CreateSession()
    {
        return new WorkoutSession(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            "Morning workout",
            DateTime.UtcNow);
    }
}
