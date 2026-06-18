using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace FitLogs.Workouts;

public class WorkoutSessionExercise : Entity<Guid>
{
    public Guid WorkoutSessionId { get; private set; }
    public Guid ExerciseId { get; private set; }
    public Guid? WorkoutPlanExerciseId { get; private set; }

    public int OrderIndex { get; private set; }
    public int TargetSets { get; private set; }
    public int TargetReps { get; private set; }
    public float? TargetWeightKg { get; private set; }
    public int? RestSeconds { get; private set; }
    public string? Note { get; private set; }
    private readonly List<ExerciseSet> _sets = new();
    public IReadOnlyCollection<ExerciseSet> Sets => _sets.AsReadOnly();
    protected WorkoutSessionExercise()
    {
        // For ORM
    }

    public WorkoutSessionExercise(Guid id,
        Guid workoutSessionId,
        Guid exerciseId,
        int orderIndex,
        int targetSets,
        int targetReps,
        float? targetWeightKg = null,
        int? restSeconds = null,
        string? note = null,
        Guid? workoutPlanExerciseId = null) : base(id)
    {
        WorkoutSessionId = Check.NotDefaultOrNull<Guid>(workoutSessionId, nameof(workoutSessionId));
        ExerciseId = Check.NotDefaultOrNull<Guid>(exerciseId, nameof(exerciseId));
        WorkoutPlanExerciseId = workoutPlanExerciseId;

        SetOrderIndex(orderIndex);
        SetTargetSets(targetSets);
        SetTargetReps(targetReps);
        SetTargetWeightKg(targetWeightKg);
        SetRestSeconds(restSeconds);
        SetNote(note);
    }

    public void SetOrderIndex(int orderIndex)
    {
        if (orderIndex < 0 || orderIndex > 100)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.InvalidWorkoutSessionExerciseOrderIndex);
        }
        OrderIndex = orderIndex;
        
    }

    public void SetTargetSets(int targetSets)
    {
        if (targetSets < 1 || targetSets > 20)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.InvalidWorkoutSessionExerciseTargetSets);
        }
        TargetSets = targetSets;
    }

    public void SetTargetReps(int targetReps)
    {
        if (targetReps < 1 || targetReps > 100)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.InvalidWorkoutSessionExerciseTargetReps);

        }
        TargetReps = targetReps;
        
    }

    public void SetTargetWeightKg(float? targetWeightKg)
    {
        if (targetWeightKg < 0 || targetWeightKg > 200)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.InvalidWorkoutSessionExerciseTargetWeightKg);

        }
        TargetWeightKg = targetWeightKg;
    }

    public void SetRestSeconds(int? restSeconds)
    {
        if (restSeconds < 0 || restSeconds > 200)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.InvalidWorkoutSessionExerciseRestSeconds);

        }
        RestSeconds = restSeconds;
        
    }

    public void SetNote(string? note)
    {
        Note = Check.Length(note, nameof(note), WorkoutSessionExerciseConsts.MaxNoteLength);
    }
    public void AddSet(
        Guid id,
        int setNumber,
        float weightKg,
        int reps,
        int? rpe = null,
        string? note = null
    )
    {
        if (_sets.Any(x => x.SetNumber == setNumber))
        {
            throw new BusinessException(FitLogsDomainErrorCodes.ExerciseSetNumberAlreadyExists);
        }

        _sets.Add(new ExerciseSet(
            id,
            Id,
            setNumber,
            weightKg,
            reps,
            rpe,
            note
        ));
    }

    public void UpdateSet(
        Guid exerciseSetId,
        int setNumber,
        float weightKg,
        int reps,
        int? rpe = null,
        string? note = null
    )
    {
        var set = GetSetOrThrow(exerciseSetId);

        if (_sets.Any(x => x.Id != exerciseSetId && x.SetNumber == setNumber))
        {
            throw new BusinessException(FitLogsDomainErrorCodes.ExerciseSetNumberAlreadyExists);
        }

        set.SetSetNumber(setNumber);
        set.SetWeightKg(weightKg);
        set.SetReps(reps);
        set.SetRpe(rpe);
        set.SetNote(note);
    }

    public void RemoveSet(Guid exerciseSetId)
    {
        var set = GetSetOrThrow(exerciseSetId);

        _sets.Remove(set);
    }

    public void CompleteSet(Guid exerciseSetId, DateTime completedAt)
    {
        var set = GetSetOrThrow(exerciseSetId);

        set.Complete(completedAt);
    }

    public void UncompleteSet(Guid exerciseSetId)
    {
        var set = GetSetOrThrow(exerciseSetId);

        set.Uncomplete();
    }

    private ExerciseSet GetSetOrThrow(Guid exerciseSetId)
    {
        var set = _sets.FirstOrDefault(x => x.Id == exerciseSetId);

        if (set == null)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.ExerciseSetNotFound);
        }

        return set;
    }
}