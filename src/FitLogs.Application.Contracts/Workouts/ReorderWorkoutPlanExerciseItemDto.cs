using System;

namespace FitLogs.Workouts;

public class ReorderWorkoutPlanExerciseItemDto
{
    public Guid WorkoutPlanExerciseId { get; set; }
    public int OrderIndex { get; set; }
}