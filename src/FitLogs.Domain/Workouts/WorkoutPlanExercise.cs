using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace FitLogs.Workouts;

public class WorkoutPlanExercise : Entity<Guid>
{
    public Guid WorkoutPlanId { get; private set; }
    public Guid ExerciseId { get; private set; }
    
    public int OrderIndex { get; private set; }
    public int DefaultSets { get; private set; }
    public int DefaultReps { get; private set; }
    public float? DefaultWeightKg { get; private set; }
    public int? RestSeconds { get; private set; }
    public string? Note { get; private set; }
    
    protected WorkoutPlanExercise()
    {
        // For ORM
    }

    public WorkoutPlanExercise(Guid id,
        Guid workoutPlanId,
        Guid exerciseId,
        int orderIndex,
        int defaultSets,
        int defaultReps,
        float? defaultWeightKg = null,
        int? restSeconds = null,
        string? note = null) : base(id)
    {
        WorkoutPlanId = workoutPlanId;
        ExerciseId = exerciseId;
        
        SetOrderIndex(orderIndex);
        SetDefaultSets(defaultSets);
        SetDefaultReps(defaultReps);
        SetDefaultWeightKg(defaultWeightKg);
        SetRestSeconds(restSeconds);
        SetNote(note);
    }

    public void SetOrderIndex(int orderIndex)
    {
        if (orderIndex < 0)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.InvalidWorkoutPlanOrderIndex);
        }
        OrderIndex = orderIndex;
        
    }

    public void SetDefaultSets(int defaultSets)
    {
        if (defaultSets <= 0)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.InvalidWorkoutPlanDefaultSets);
        }
        DefaultSets = defaultSets;
        
    }

    public void SetDefaultReps(int defaultReps)
    {
        if (defaultReps <= 0)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.InvalidWorkoutPlanDefaultReps);
        }
        DefaultReps = defaultReps;
        
    }

    public void SetDefaultWeightKg(float? defaultWeightKg)
    {
        if (defaultWeightKg < 0)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.InvalidWorkoutPlanDefaultWeights);
        }
        DefaultWeightKg = defaultWeightKg;
    }

    public void SetRestSeconds(int? restSeconds)
    {
        if (restSeconds < 0)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.InvalidWorkoutPlanRestSeconds);
        }
        RestSeconds = restSeconds;
    }

    public void SetNote(string? note)
    {
        Note = Check.Length(note, nameof(note), WorkoutPlanExerciseConsts.MaxNoteLength);
        
    }
}