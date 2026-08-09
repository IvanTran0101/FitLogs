using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace FitLogs.Workouts;

public class WorkoutPlan : FullAuditedAggregateRoot<Guid>
{
    public Guid UserId { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public WorkoutGoal Goal { get; private set; }
    public WorkoutDifficulty Difficulty { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsArchived { get; private set; }
    private readonly List<WorkoutPlanExercise> _exercises = new();

    public IReadOnlyCollection<WorkoutPlanExercise> Exercises => _exercises.AsReadOnly();

    protected WorkoutPlan()
    {
        
    }
    public WorkoutPlan(Guid id,
        Guid userId,
        string name,
        string? description,
        WorkoutGoal goal,
        WorkoutDifficulty difficulty,
        bool isActive = false,
        bool isArchived = false) : base(id)
    {
        UserId = Check.NotDefaultOrNull<Guid>(userId, nameof(userId));
        SetName(name);
        SetDescription(description);
        SetGoal(goal);
        SetDifficulty(difficulty);
        IsActive = isActive;
        IsArchived = isArchived;
    }

    public void SetName(string name)
    {
            Name = Check.NotNullOrWhiteSpace(name, nameof(name), WorkoutPlanConsts.MaxNameLength);
            
    }

    public void SetDescription(string? description)
    {
        Description = Check.Length(description, nameof(description), WorkoutPlanConsts.MaxDescriptionLength);        
    }

    public void SetGoal(WorkoutGoal goal)
    {
        if (!Enum.IsDefined(typeof(WorkoutGoal), goal))
        {
            throw new BusinessException(FitLogsDomainErrorCodes.InvalidWorkoutGoal);
        }
        Goal = goal;

    }

    public void SetDifficulty(WorkoutDifficulty difficulty)
    {
        if (!Enum.IsDefined(typeof(WorkoutDifficulty), difficulty))
        {
            throw new BusinessException(FitLogsDomainErrorCodes.InvalidWorkoutDifficulty);
        }
        Difficulty = difficulty;
    }

    public void Activate()
    {
        EnsureNotArchived();
        if (!_exercises.Any())
        {
            throw new BusinessException(FitLogsDomainErrorCodes.WorkoutPlanMustHaveAtLeastOneExercise);
        }
        IsActive = true;
    }

    public void Deactivate()
    {
        EnsureNotArchived();

        IsActive = false;
    }

    public void Archive()
    {
        EnsureNotArchived();

        IsActive = false;
        IsArchived = true;
    }

    public void Restore()
    {
        IsArchived = false;
    }

    public void AddExercise(
        Guid id,
        Guid exerciseId,
        int orderIndex,
        int defaultSets,
        int defaultReps,
        float? defaultWeightKg = null,
        int? restSeconds = null,
        string? note = null)
    {
        EnsureNotArchived();
        if (_exercises.Any(x => x.ExerciseId == exerciseId))
        {
            throw new BusinessException(FitLogsDomainErrorCodes.WorkoutPlanExerciseAlreadyExists);
        }

        if (_exercises.Any(x => x.OrderIndex == orderIndex))
        {
            throw new BusinessException(FitLogsDomainErrorCodes.WorkoutPlanExerciseOrderIndexAlreadyExists);
            
        }
        _exercises.Add(new WorkoutPlanExercise(
            id,
            Id,
            exerciseId,
            orderIndex,
            defaultSets,
            defaultReps,
            defaultWeightKg,
            restSeconds,
            note));
    }

    /// <summary>Validates every draft before mutating the plan, preventing partially applied batch additions.</summary>
    public void AddExercises(IEnumerable<WorkoutPlanExerciseDraft> drafts)
    {
        EnsureNotArchived();
        var items = drafts?.ToList() ?? throw new ArgumentNullException(nameof(drafts));

        if (items.Count == 0 ||
            items.GroupBy(x => x.ExerciseId).Any(group => group.Count() > 1) ||
            items.GroupBy(x => x.OrderIndex).Any(group => group.Count() > 1))
        {
            throw new BusinessException(FitLogsDomainErrorCodes.WorkoutPlanExerciseAlreadyExists);
        }

        if (items.Any(item => _exercises.Any(existing =>
                existing.ExerciseId == item.ExerciseId || existing.OrderIndex == item.OrderIndex)))
        {
            throw new BusinessException(FitLogsDomainErrorCodes.WorkoutPlanExerciseAlreadyExists);
        }

        foreach (var item in items)
        {
            // Validation above completes before this loop, so one invalid item cannot leave a half-updated plan.
            _exercises.Add(new WorkoutPlanExercise(
                Guid.NewGuid(),
                Id,
                item.ExerciseId,
                item.OrderIndex,
                item.DefaultSets,
                item.DefaultReps,
                item.DefaultWeightKg,
                item.RestSeconds,
                item.Note));
        }
    }

    public void UpdateExercise(
        Guid workoutPlanExerciseId,
        int orderIndex,
        int defaultSets,
        int defaultReps,
        float? defaultWeightKg = null,
        int? restSeconds = null,
        string? note = null
    )
    {
        var exercise = _exercises.FirstOrDefault(x => x.Id == workoutPlanExerciseId);

        if (exercise == null)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.WorkoutPlanExerciseNotFound);
        }

        if (_exercises.Any(x => x.Id != workoutPlanExerciseId && x.OrderIndex == orderIndex))
        {
            throw new BusinessException(FitLogsDomainErrorCodes.WorkoutPlanExerciseOrderIndexAlreadyExists);
        }
        EnsureNotArchived();

        exercise.SetOrderIndex(orderIndex);
        exercise.SetDefaultSets(defaultSets);
        exercise.SetDefaultReps(defaultReps);
        exercise.SetDefaultWeightKg(defaultWeightKg);
        exercise.SetRestSeconds(restSeconds);
        exercise.SetNote(note);
    }
    public void RemoveExercise(Guid workoutPlanExerciseId)
    {
        var exercise = _exercises.FirstOrDefault(x => x.Id == workoutPlanExerciseId);

        if (exercise == null)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.WorkoutPlanExerciseNotFound);
        }
        EnsureNotArchived();

        _exercises.Remove(exercise);
    }

    
    //Reorder exercises
    public void ValidateReorderExercises(Dictionary<Guid, int> orderIndexes)
    {
        EnsureNotArchived();

        if (orderIndexes.Count != _exercises.Count)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.InvalidWorkoutPlanExerciseOrder);
        }

        if (orderIndexes.Values.Any(x => x <= 0))
        {
            throw new BusinessException(FitLogsDomainErrorCodes.InvalidOrderIndex);
        }

        if (orderIndexes.Values.Distinct().Count() != orderIndexes.Count)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.WorkoutPlanExerciseOrderIndexAlreadyExists);
        }

        foreach (var item in orderIndexes)
        {
            if (_exercises.All(x => x.Id != item.Key))
            {
                throw new BusinessException(FitLogsDomainErrorCodes.WorkoutPlanExerciseNotFound);
            }
        }
    }

    public void MoveExercisesToTemporaryOrderIndexes(int startingOrderIndex)
    {
        EnsureNotArchived();

        var orderedExercises = _exercises
            .OrderBy(x => x.OrderIndex)
            .ToList();

        for (var index = 0; index < orderedExercises.Count; index++)
        {
            orderedExercises[index].SetOrderIndex(startingOrderIndex + index);
        }
    }

    public void ReorderExercises(Dictionary<Guid, int> orderIndexes)
    {
        ValidateReorderExercises(orderIndexes);

        foreach (var item in orderIndexes)
        {
            var exercise = _exercises.First(x => x.Id == item.Key);
            exercise.SetOrderIndex(item.Value);
        }
    }
    
    //Helpers
    private void EnsureNotArchived()
    {
        if (IsArchived)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.WorkoutPlanIsArchived);            
        }
    }
}
