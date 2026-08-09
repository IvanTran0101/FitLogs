using System;

namespace FitLogs.Workouts;

public sealed record CompletedWorkoutSessionMetric(
    DateTime StartedAt,
    DateTime CompletedAt,
    int CompletedExercises,
    int CompletedSets,
    decimal CompletedVolume);
