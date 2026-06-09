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

    public void Complete(DateTime endedAt)
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

    public void AddExercise(
        Guid id,
        Guid exerciseId,
        int orderIndex,
        int targetSets,
        int targetReps,
        float? targetWeightKg = null,
        int? restSeconds = null,
        string? note = null)
    {
        EnsureInProgress();
        if (_exercises.Any(e => e.ExerciseId == exerciseId))
        {
            throw new BusinessException(FitLogsDomainErrorCodes.WorkoutSessionExerciseAlreadyExists);
        }

        if (_exercises.Any(x => x.OrderIndex == orderIndex))
        {
            throw new BusinessException(FitLogsDomainErrorCodes.WorkoutSessionExerciseOrderIndexAlreadyExists);
        }
        _exercises.Add(new WorkoutSessionExercise(
            id,
            Id,
            exerciseId,
            orderIndex,
            targetSets,
            targetReps,
            targetWeightKg,
            restSeconds,
            note));
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
        var exercise = _exercises.FirstOrDefault(x => x.ExerciseId == workoutSessionExerciseId);
        if (exercise == null)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.WorkoutSessionExerciseNotFound);
            
        }
        if (_exercises.Any(x => x.Id != workoutSessionExerciseId && x.OrderIndex == orderIndex))
        {
            throw new BusinessException(FitLogsDomainErrorCodes.WorkoutSessionExerciseOrderIndexAlreadyExists);
        }
        exercise.SetOrderIndex(orderIndex);
        exercise.SetTargetSets(targetSets);
        exercise.SetTargetReps(targetReps);
        exercise.SetTargetWeightKg(targetWeightKg);
        exercise.SetRestSeconds(restSeconds);
        exercise.SetNote(note);
    }
    public void RemoveExercise(Guid workoutSessionExerciseId)
    {
        EnsureInProgress();

        var exercise = _exercises.FirstOrDefault(x => x.Id == workoutSessionExerciseId);

        if (exercise == null)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.WorkoutSessionExerciseNotFound);
        }

        _exercises.Remove(exercise);
    }

    private void EnsureInProgress()
    {
        if (Status != WorkoutSessionStatus.InProgress)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.WorkoutSessionStatusIsNotInProgress);
        }
    }
}