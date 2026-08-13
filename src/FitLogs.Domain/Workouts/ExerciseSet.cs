using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace FitLogs.Workouts;

public class ExerciseSet : Entity<Guid>
{
  public Guid WorkoutSessionExerciseId { get; private set; }
  public int SetNumber { get; private set; }
  public float WeightKg { get; private set; }
  public int Reps  { get; private set; }
  public int? Rpe { get; private set; }
  public string? Note { get; private set; }
  public bool IsCompleted { get; private set; }
  public DateTime? CompletedAt { get; private set; }
  public bool IsSkipped { get; private set; }
  public DateTime? SkippedAt { get; private set; }
  
  protected ExerciseSet()
  {
    // For ORM
  }

  internal ExerciseSet(
    Guid id,
    Guid workoutSessionExerciseId,
    int setNumber,
    float weightKg,
    int reps,
    int? rpe = null,
    string? note = null) : base(id)
  {
    WorkoutSessionExerciseId = Check.NotDefaultOrNull<Guid>(

      workoutSessionExerciseId,

      nameof(workoutSessionExerciseId));    
    SetSetNumber(setNumber);
    SetWeightKg(weightKg);
    SetReps(reps);
    SetRpe(rpe);
    SetNote(note);
    
    IsCompleted = false;
    CompletedAt = null;
    IsSkipped = false;
    SkippedAt = null;
  }

  public void SetSetNumber(int setNumber)
  {
    if (setNumber <= 0)
    {
      throw new BusinessException(FitLogsDomainErrorCodes.InvalidExerciseSetNumber);
    }
    SetNumber = setNumber;
  }

  public void SetWeightKg(float weightKg)
  {
    if (weightKg < 0)
    {
      throw new BusinessException(FitLogsDomainErrorCodes.InvalidExerciseSetWeight);
    }
    WeightKg = weightKg;
  }

  public void SetReps(int reps)
  {
    if (reps <= 0)
    {
      throw new BusinessException(FitLogsDomainErrorCodes.InvalidExerciseSetReps);

    }
    Reps = reps;
  }

  public void SetRpe(int? rpe)
  {
    if (rpe is < 1 or > 10)
    {
      throw new BusinessException(FitLogsDomainErrorCodes.InvalidExerciseSetRpe);
    }

    Rpe = rpe;
  }

  public void SetNote(string? note)
  {
    Note = Check.Length(
      note,
      nameof(note),
      ExerciseSetConsts.MaxNoteLength
    );
  }
  public void Complete(DateTime completedAt)
  {
    if (IsCompleted)
    {
      throw new BusinessException(FitLogsDomainErrorCodes.ExerciseSetAlreadyCompleted);
    }

    if (IsSkipped)
    {
      throw new BusinessException(FitLogsDomainErrorCodes.ExerciseSetAlreadySkipped);
    }

    if (completedAt == default)
    {
      throw new BusinessException(FitLogsDomainErrorCodes.InvalidExerciseSetCompletedAt);
    }

    IsCompleted = true;
    CompletedAt = completedAt;
  }

  public void Uncomplete()
  {
    IsCompleted = false;
    CompletedAt = null;
  }

  /// <summary>Marks a set as intentionally skipped without counting it as performed work.</summary>
  public void Skip(DateTime skippedAt)
  {
    if (IsCompleted)
    {
      throw new BusinessException(FitLogsDomainErrorCodes.ExerciseSetAlreadyCompleted);
    }

    if (IsSkipped)
    {
      throw new BusinessException(FitLogsDomainErrorCodes.ExerciseSetAlreadySkipped);
    }

    if (skippedAt == default)
    {
      throw new BusinessException(FitLogsDomainErrorCodes.InvalidExerciseSetSkippedAt);
    }

    IsSkipped = true;
    SkippedAt = skippedAt;
  }

  /// <summary>Reopens a skipped set so it can be performed and completed later.</summary>
  public void Unskip()
  {
    IsSkipped = false;
    SkippedAt = null;
  }
}
