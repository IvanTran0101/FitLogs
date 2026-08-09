using System;

namespace FitLogs.Workouts;

/// <summary>Validated values used to add several exercises to a plan as one aggregate operation.</summary>
public sealed record WorkoutPlanExerciseDraft(
    Guid ExerciseId,
    int OrderIndex,
    int DefaultSets,
    int DefaultReps,
    float? DefaultWeightKg = null,
    int? RestSeconds = null,
    string? Note = null);
