namespace FitLogs;

public static class FitLogsDomainErrorCodes
{
    // UserProfile
    public const string InvalidDateOfBirth = "FitLogs:UserProfile:InvalidDateOfBirth";
    public const string InvalidHeightCm = "FitLogs:UserProfile:InvalidHeightCm";
    public const string InvalidWeightKg = "FitLogs:UserProfile:InvalidWeightKg";
    public const string InvalidDailyTargetCalories = "FitLogs:UserProfile:InvalidDailyTargetCalories";
    public const string UserProfileAlreadyExists =
        "FitLogs:UserProfile:AlreadyExists";
    public const string UserProfileNotFound =
        "FitLogs:UserProfile:NotFound";
    public const string ForbiddenProfileAccess =
        "FitLogs:UserProfile:ForbiddenProfileAccess";

    // Exercise
    public const string ExerciseNameAlreadyExists = "FitLogs:Exercise:NameAlreadyExists";
    public const string MuscleGroupNotFound = "FitLogs:Exercise:MuscleGroupNotFound";
    public const string MuscleGroupInvalidOrderDisplay = "FitLogs:Exercise:MuscleGroupInvalidOrderDisplay";
    public const string EquipmentInvalidDisplayOrder = "FitLogs:Equipment:0001";
    public const string EquipmentIsUsedByExercise = "FitLogs:Exercise:EquipmentIsUsedByExercise";
    public const string EquipmentNotFound = "FitLogs:Exercise:EquipmentNotFound";
    public const string EquipmentCodeAlreadyExists = "FitLogs:Exercise:EquipmentCodeAlreadyExists";

    public const string ExercisePrimaryMuscleGroupRequired = "FitLogs:Exercise:0001";
    public const string ExerciseInvalidDifficulty = "FitLogs:Exercise:0002";
    public const string ExerciseInvalidTrackingType = "FitLogs:Exercise:0003";
    public const string ExerciseSlugAlreadyExists = "FitLogs:ExerciseSlugAlreadyExists";
    public const string MuscleGroupIsUsedByExercise = "FitLogs:MuscleGroupIsUsedByExercise";
    public const string MuscleGroupCodeAlreadyExists = "FitLogs:MuscleGroupCodeAlreadyExists";


    // Workout
    public const string WorkoutSessionNotInProgress = "FitLogs:Workout:SessionNotInProgress";
    public const string InvalidExerciseSetValue = "FitLogs:Workout:InvalidExerciseSetValue";

    // Food
    public const string InvalidFoodQuantity = "FitLogs:Food:InvalidQuantity";
    public const string FoodProductNotFound = "FitLogs:Food:ProductNotFound";

}
