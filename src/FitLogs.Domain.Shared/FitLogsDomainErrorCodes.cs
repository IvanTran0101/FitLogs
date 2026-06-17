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
    public const string ExerciseEquipmentRequired = "FitLogs:Exercise:ExerciseEquipmentRequired";

    public const string ExercisePrimaryMuscleGroupRequired = "FitLogs:Exercise:0001";
    public const string ExerciseInvalidDifficulty = "FitLogs:Exercise:0002";
    public const string ExerciseInvalidTrackingType = "FitLogs:Exercise:0003";
    public const string ExerciseSlugAlreadyExists = "FitLogs:ExerciseSlugAlreadyExists";
    public const string MuscleGroupIsUsedByExercise = "FitLogs:MuscleGroupIsUsedByExercise";
    public const string MuscleGroupCodeAlreadyExists = "FitLogs:MuscleGroupCodeAlreadyExists";
    public const string EquipmentNameAlreadyExists = "FitLogs:EquipmentNameAlreadyExists";
    public const string MuscleGroupNameAlreadyExists = "FitLogs:MuscleGroupNameAlreadyExists";


    // Workout
    public const string WorkoutSessionNotInProgress = "FitLogs:Workout:SessionNotInProgress";
    public const string InvalidExerciseSetValue = "FitLogs:Workout:InvalidExerciseSetValue";
    public const string InvalidWorkoutPlanDefaultSets = "FitLogs:Workout:InvalidWorkoutPlanDefaultSets";
    public const string InvalidWorkoutPlanDefaultReps = "FitLogs:Workout:InvalidWorkoutPlanDefaultReps";
    public const string InvalidWorkoutPlanOrderIndex = "FitLogs:Workout:InvalidWorkoutPlanOrderIndex";
    public const string InvalidWorkoutPlanDefaultWeights = "FitLogs:Workout:InvalidWorkoutPlanDefaultWeights";
    public const string InvalidWorkoutPlanRestSeconds = "FitLogs:Workout:InvalidWorkoutPlanRestSeconds";
    public const string WorkoutPlanExerciseAlreadyExists = "FitLogs:Workout:WorkoutPlanExerciseAlreadyExists";
    public const string WorkoutPlanExerciseOrderIndexAlreadyExists = "FitLogs:Workout:WorkoutPlanExerciseOrderIndexAlreadyExists";
    public const string WorkoutPlanExerciseNotFound = "FitLogs:Workout:WorkoutPlanExerciseNotFound";
    public const string WorkoutSessionAccessDenied = "FitLogs:Workout:WorkoutSessionAccessDenied";

    
    
    public const string WorkoutSessionStatusIsNotInProgress = "FitLogs:Workout:WorkoutSessionStatusIsNotInProgress";
    public const string InvalidWorkoutSessionEndedAt = "FitLogs:Workout:InvalidWorkoutSessionEndedAt";
    public const string InvalidWorkoutSessionStartedAt = "FitLogs:Workout:InvalidWorkoutSessionStartedAt";
    public const string InvalidWorkoutSessionExerciseOrderIndex = "FitLogs:Workout:InvalidWorkoutSessionExerciseOrderIndex";
    public const string InvalidWorkoutSessionExerciseTargetSets = "FitLogs:Workout:InvalidWorkoutSessionExerciseTargetSets";
    public const string InvalidWorkoutSessionExerciseTargetReps = "FitLogs:Workout:InvalidWorkoutSessionExerciseTargetReps";
    public const string InvalidWorkoutSessionExerciseTargetWeightKg = "FitLogs:Workout:InvalidWorkoutSessionExerciseTargetWeightKg";
    public const string InvalidWorkoutSessionExerciseRestSeconds = "FitLogs:Workout:InvalidWorkoutSessionExerciseRestSeconds";
    public const string WorkoutSessionExerciseAlreadyExists = "FitLogs:Workout:WorkoutSessionExerciseAlreadyExists";
    public const string WorkoutSessionExerciseOrderIndexAlreadyExists = "FitLogs:Workout:WorkoutSessionExerciseOrderIndexAlreadyExists";
    public const string WorkoutSessionExerciseNotFound = "FitLogs:Workout:WorkoutSessionExerciseNotFound";
    public const string WorkoutPlanNameAlreadyExists = "FitLogs:Workout:WorkoutPlanNameAlreadyExists";
    public const string UserHasInProgressWorkoutSession = "FitLogs:Workout:UserHasInProgressWorkoutSession";
    public const string WorkoutPlanAccessDenied = "FitLogs:Workout:WorkoutPlanAccessDenied";

    public const string InvalidExerciseSetNumber = "FitLogs:Workout:InvalidExerciseSetNumber";
    public const string InvalidExerciseSetWeight = "FitLogs:Workout:InvalidExerciseSetWeight";
    public const string InvalidExerciseSetReps = "FitLogs:Workout:InvalidExerciseSetReps";
    public const string InvalidExerciseSetRpe = "FitLogs:Workout:InvalidExerciseSetRpe";
    public const string ExerciseSetAlreadyCompleted = "FitLogs:Workout:ExerciseSetAlreadyCompleted";
    public const string InvalidExerciseSetCompletedAt = "FitLogs:Workout:InvalidExerciseSetCompletedAt";

    public const string ExerciseSetNumberAlreadyExists = "FitLogs:Workout:ExerciseSetNumberAlreadyExists";
    public const string ExerciseSetNotFound = "FitLogs:Workout:ExerciseSetNotFound";


    // Food
    public const string InvalidFoodQuantity = "FitLogs:Food:InvalidQuantity";
    public const string FoodProductNotFound = "FitLogs:Food:ProductNotFound";
    public const string FoodProductCaloriesCannotBeNegative = "FitLogs:FoodProduct:001";
    public const string FoodProductProteinCannotBeNegative = "FitLogs:FoodProduct:002";
    public const string FoodProductCarbCannotBeNegative = "FitLogs:FoodProduct:003";
    public const string FoodProductFatCannotBeNegative = "FitLogs:FoodProduct:004";
    public const string FoodLogUserIdRequired = "FitLogs:FoodLog:001";
    public const string FoodLogQuantityMustBeGreaterThanZero = "FitLogs:FoodLog:002";
    public const string FoodLogCaloriesCannotBeNegative = "FitLogs:FoodLog:003";
    public const string FoodLogProteinCannotBeNegative = "FitLogs:FoodLog:004";
    public const string FoodLogCarbCannotBeNegative = "FitLogs:FoodLog:005";
    public const string FoodLogFatCannotBeNegative = "FitLogs:FoodLog:006";
    public const string FoodLogLoggedAtRequired = "FitLogs:FoodLog:007";
    public const string FoodLogUnitInvalid = "FitLogs:FoodLog:008";
    public const string FoodLogMealTypeInvalid = "FitLogs:FoodLog:009";
    public const string FoodProductSourceInvalid = "FitLogs:FoodProduct:005";
    public const string FoodProductBarcodeAlreadyExists = "FitLogs:FoodProduct:006";
    public const string FoodProductInactive = "FitLogs:FoodProduct:007";
    public const string FoodLogFoodProductIdRequired = "FitLogs:FoodLog:010";
    public const string FoodLogUserRequired = "FitLogs:FoodLog:0001";
    public const string FoodProductRequired = "FitLogs:FoodLog:0002";
    public const string FoodLogInvalidQuantity = "FitLogs:FoodLog:0004";
    public const string FoodLogInvalidNutrition = "FitLogs:FoodLog:0005";
    public const string FoodLogNotOwnedByUser = "FitLogs:FoodLog:0006";
    public const string FoodLogAccessDenied = "FitLogs:FoodLog:FoodLogAccessDenied";
    
    public const string FoodProductNotFoundFromOpenFoodFacts = "FitLogs:FoodLog:FoodProductNotFoundFromOpenFoodFacts";
    public const string FoodProductBarcodeInvalid = "FitLogs:FoodProduct:BarcodeInvalid";


}
