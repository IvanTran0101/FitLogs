using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using System.Linq;
namespace FitLogs.Workouts;

public class WorkoutSession : FullAuditedAggregateRoot<Guid>
{
    public Guid UserId { get; private set; }
    public Guid? WorkoutPlanId { get; private set; }
    public string Name { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime? EndedAt { get; private set; }
    public WorkoutSessionStatus Status { get; private set; }
    public Guid? CurrentWorkoutSessionExerciseId { get; private set; }
    public string? Note { get;  private set; }
    private readonly List<WorkoutSessionExercise> _exercises = new();

    public IReadOnlyCollection<WorkoutSessionExercise> Exercises => _exercises.AsReadOnly();
    
    protected WorkoutSession()
    {
        // For ORM
    }

    public WorkoutSession(Guid id,
        Guid userId,
        Guid? workoutPlanId,
        string name,
        DateTime startedAt,
        string? note = null) : base(id)
    {
        UserId = Check.NotDefaultOrNull<Guid>(userId, nameof(userId));
        WorkoutPlanId = workoutPlanId;
        
        SetName(name);
        SetStartedAt(startedAt);
        SetNote(note);
        Status = WorkoutSessionStatus.InProgress;
        EndedAt = null;
    }

    public void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name),
            WorkoutSessionConsts.MaxNameLength);
        
    }

    public void SetStartedAt(DateTime startedAt)
    {
        if (startedAt == default)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.InvalidWorkoutSessionStartedAt);
        }
        StartedAt = startedAt;
    }

    public void SetNote(string? note)
    {
        Note = Check.Length(note, nameof(note), WorkoutSessionConsts.MaxNoteLength);
        
    }

    /// <summary>Completes the session only after at least one exercise reaches its target sets.</summary>
    public void Complete(DateTime endedAt)
    {
        if (!_exercises.Any())
        {
            throw new BusinessException(FitLogsDomainErrorCodes.WorkoutSessionMustHaveAtLeastOneExercise);
        }

        if (Status != WorkoutSessionStatus.InProgress)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.WorkoutSessionStatusIsNotInProgress);
        }

        if (endedAt < StartedAt)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.InvalidWorkoutSessionEndedAt);
        }
        var completedExercises = _exercises
            .Where(x => x.HasReachedTargetSets)
            .ToList();
        if (completedExercises.Count == 0)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.WorkoutSessionCannotCompleteWithoutCompletedExercise);
        }

        foreach (var exercise in completedExercises)
        {
            exercise.MarkCompletedIfTargetReached();
        }

        EndedAt = endedAt;
        Status = WorkoutSessionStatus.Completed;
    }

    public void Cancel(DateTime endedAt)
    {
        if (Status != WorkoutSessionStatus.InProgress)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.WorkoutSessionStatusIsNotInProgress);
        }

        if (endedAt < StartedAt)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.InvalidWorkoutSessionEndedAt);
        }
        EndedAt = endedAt;
        Status = WorkoutSessionStatus.Cancelled;
    }

    /// <summary>Adds an exercise and assigns the first available exercise as the current pointer.</summary>
    public void AddExercise(
        Guid id,
        Guid exerciseId,
        int orderIndex,
        int targetSets,
        int targetReps,
        float? targetWeightKg = null,
        int? restSeconds = null,
        string? note = null,
        Guid? workoutPlanExerciseId = null)
    {
        EnsureInProgress();
        EnsureExerciseDoesNotExist(exerciseId);
        EnsureOrderIndexDoesNotExist(orderIndex);

        var sessionExercise = new WorkoutSessionExercise(
            id,
            Id,
            exerciseId,
            orderIndex,
            targetSets,
            targetReps,
            targetWeightKg,
            restSeconds,
            note,
            workoutPlanExerciseId);
        _exercises.Add(sessionExercise);
        if (CurrentWorkoutSessionExerciseId == null)
        {
            CurrentWorkoutSessionExerciseId = sessionExercise.Id;
            sessionExercise.MarkInProgress();
        }

    }

    public void UpdateExercise(
        Guid workoutSessionExerciseId,
        int orderIndex,
        int targetSets,
        int targetReps,
        float? targetWeightKg = null,
        int? restSeconds = null,
        string? note = null)
    {
        EnsureInProgress();

        var exercise = GetExerciseOrThrow(workoutSessionExerciseId);
        EnsureOrderIndexDoesNotExist(orderIndex, workoutSessionExerciseId);
        exercise.SetOrderIndex(orderIndex);
        exercise.SetTargetSets(targetSets);
        exercise.SetTargetReps(targetReps);
        exercise.SetTargetWeightKg(targetWeightKg);
        exercise.SetRestSeconds(restSeconds);
        exercise.SetNote(note);
    }
    /// <summary>Removes an exercise and repairs the current pointer to the nearest available item.</summary>
    public void RemoveExercise(Guid workoutSessionExerciseId)
    {
        EnsureInProgress();

        var exercise = GetExerciseOrThrow(workoutSessionExerciseId);

        var wasCurrent = CurrentWorkoutSessionExerciseId == exercise.Id;
        _exercises.Remove(exercise);

        if (wasCurrent)
        {
            CurrentWorkoutSessionExerciseId = FindNextAvailableExercise(exercise.OrderIndex)?.Id
                ?? FindPreviousAvailableExercise(exercise.OrderIndex)?.Id;
            MarkCurrentExerciseInProgress();
        }
    }
    public void AddSetToExercise(
        Guid workoutSessionExerciseId,
        Guid exerciseSetId,
        int setNumber,
        float weightKg,
        int reps,
        int? rpe = null,
        string? note = null
    )
    {
        EnsureInProgress();

        var exercise = GetExerciseOrThrow(workoutSessionExerciseId);

        exercise.AddSet(
            exerciseSetId,
            setNumber,
            weightKg,
            reps,
            rpe,
            note
        );

    }

    public void UpdateSetInExercise(
        Guid workoutSessionExerciseId,
        Guid exerciseSetId,
        int setNumber,
        float weightKg,
        int reps,
        int? rpe = null,
        string? note = null)
    {
        EnsureInProgress();

        var exercise = GetExerciseOrThrow(workoutSessionExerciseId);

        exercise.UpdateSet(
            exerciseSetId,
            setNumber,
            weightKg,
            reps,
            rpe,
            note
        );
    }

    public void RemoveSetFromExercise(
        Guid workoutSessionExerciseId,
        Guid exerciseSetId)
    {
        EnsureInProgress();

        var exercise = GetExerciseOrThrow(workoutSessionExerciseId);

        exercise.RemoveSet(exerciseSetId);
    }

    /// <summary>Completes a set, marks the exercise complete at its target, and advances the pointer.</summary>
    public void CompleteSetInExercise(
        Guid workoutSessionExerciseId,
        Guid exerciseSetId,
        DateTime completedAt)
    {
        EnsureInProgress();

        var exercise = GetExerciseOrThrow(workoutSessionExerciseId);

        exercise.CompleteSet(
            exerciseSetId,
            completedAt
        );

        if (CurrentWorkoutSessionExerciseId == workoutSessionExerciseId &&
            exercise.Status == WorkoutSessionExerciseStatus.Completed)
        {
            CurrentWorkoutSessionExerciseId = FindNextAvailableExercise(exercise.OrderIndex)?.Id;
            MarkCurrentExerciseInProgress();
        }
    }

    /// <summary>Moves forward to the next pending or in-progress exercise.</summary>
    public void MoveToNextExercise()
    {
        EnsureInProgress();
        if (!_exercises.Any())
        {
            throw new BusinessException(FitLogsDomainErrorCodes.WorkoutSessionExerciseNotFound);
            
        }

        if (CurrentWorkoutSessionExerciseId is not Guid currentExerciseId)
        {
            CurrentWorkoutSessionExerciseId = FindFirstAvailableExercise()?.Id;
            MarkCurrentExerciseInProgress();
            if (CurrentWorkoutSessionExerciseId == null)
                throw new BusinessException(FitLogsDomainErrorCodes.NextWorkoutSessionExerciseNotFound);
            return;
        }
        var currentExercise = GetExerciseOrThrow(currentExerciseId);
        var nextExercise = FindNextAvailableExercise(currentExercise.OrderIndex);
        if (nextExercise == null)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.NextWorkoutSessionExerciseNotFound);
        }
        CurrentWorkoutSessionExerciseId = nextExercise.Id;
        MarkCurrentExerciseInProgress();
    }

    /// <summary>Moves backward to the previous pending or in-progress exercise.</summary>
    public void MoveToPreviousExercise()
    {
        EnsureInProgress();
        if (!_exercises.Any())
        {
            throw new BusinessException(FitLogsDomainErrorCodes.WorkoutSessionExerciseNotFound);
        }

        if (CurrentWorkoutSessionExerciseId is not Guid currentExerciseId)
        {
            CurrentWorkoutSessionExerciseId = FindFirstAvailableExercise()?.Id;
            MarkCurrentExerciseInProgress();
            if (CurrentWorkoutSessionExerciseId == null)
                throw new BusinessException(FitLogsDomainErrorCodes.PreviousWorkoutSessionExerciseNotFound);
            return;
        }
        var currentExercise = GetExerciseOrThrow(currentExerciseId);
        var previousExercise = FindPreviousAvailableExercise(currentExercise.OrderIndex);
        if (previousExercise == null)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.PreviousWorkoutSessionExerciseNotFound);
        }
        CurrentWorkoutSessionExerciseId = previousExercise.Id;
        MarkCurrentExerciseInProgress();
    }
    
    public void UncompleteSetInExercise(
        Guid workoutSessionExerciseId,
        Guid exerciseSetId)
    {
        EnsureInProgress();

        var exercise = GetExerciseOrThrow(workoutSessionExerciseId);

        exercise.UncompleteSet(exerciseSetId);
        CurrentWorkoutSessionExerciseId = exercise.Id;
    }

    public WorkoutSessionExercise GetCurrentExercise()
    {
        if (CurrentWorkoutSessionExerciseId == null)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.CurrentWorkoutSessionExerciseNotFound);
            
        }
        return GetExerciseOrThrow(CurrentWorkoutSessionExerciseId.Value);
    }

    /// <summary>Skips the current exercise and chooses the next or previous exercise still available.</summary>
    public void SkipCurrentExercise()
    {
        EnsureInProgress();
        var currentExercise = GetCurrentExercise();
        currentExercise.Skip();
        var nextExercise = FindNextAvailableExercise(currentExercise.OrderIndex)
            ?? FindPreviousAvailableExercise(currentExercise.OrderIndex);
        CurrentWorkoutSessionExerciseId = nextExercise?.Id;
        MarkCurrentExerciseInProgress();
    }
    private void EnsureInProgress()
    {
        if (Status != WorkoutSessionStatus.InProgress)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.WorkoutSessionStatusIsNotInProgress);
        }
    }

    private WorkoutSessionExercise GetExerciseOrThrow(Guid workoutSessionExerciseId)
    {
        var exercise = _exercises.FirstOrDefault(x => x.Id == workoutSessionExerciseId);

        if (exercise == null)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.WorkoutSessionExerciseNotFound);
        }

        return exercise;
    }

    /// <summary>Finds the next exercise that can still be performed, skipping completed and skipped items.</summary>
    private WorkoutSessionExercise? FindNextAvailableExercise(int orderIndex)
    {
        return _exercises
            .Where(x => x.OrderIndex > orderIndex && IsAvailable(x))
            .OrderBy(x => x.OrderIndex)
            .FirstOrDefault();
    }

    /// <summary>Finds the previous exercise that can still be performed, skipping completed and skipped items.</summary>
    private WorkoutSessionExercise? FindPreviousAvailableExercise(int orderIndex)
    {
        return _exercises
            .Where(x => x.OrderIndex < orderIndex && IsAvailable(x))
            .OrderByDescending(x => x.OrderIndex)
            .FirstOrDefault();
    }

    private WorkoutSessionExercise? FindFirstAvailableExercise()
    {
        return _exercises
            .Where(IsAvailable)
            .OrderBy(x => x.OrderIndex)
            .FirstOrDefault();
    }

    private static bool IsAvailable(WorkoutSessionExercise exercise)
    {
        return exercise.Status is WorkoutSessionExerciseStatus.Pending or WorkoutSessionExerciseStatus.InProgress;
    }

    private void MarkCurrentExerciseInProgress()
    {
        if (CurrentWorkoutSessionExerciseId is Guid currentId)
        {
            var current = _exercises.FirstOrDefault(x => x.Id == currentId);
            if (current != null && current.Status == WorkoutSessionExerciseStatus.Pending)
            {
                current.MarkInProgress();
            }
        }
    }

    private void EnsureExerciseDoesNotExist(Guid exerciseId)
    {
        if (_exercises.Any(e => e.ExerciseId == exerciseId))
        {
            throw new BusinessException(FitLogsDomainErrorCodes.WorkoutSessionExerciseAlreadyExists);
        }
    }

    private void EnsureOrderIndexDoesNotExist(int orderIndex, Guid? excludedWorkoutSessionExerciseId = null)
    {
        if (_exercises.Any(x => x.Id != excludedWorkoutSessionExerciseId && x.OrderIndex == orderIndex))
        {
            throw new BusinessException(FitLogsDomainErrorCodes.WorkoutSessionExerciseOrderIndexAlreadyExists);
        }
    }
}
