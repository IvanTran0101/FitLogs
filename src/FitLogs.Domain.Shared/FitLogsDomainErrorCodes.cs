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
    public const string InvalidGender = "FitLogs:UserProfile:InvalidGender";
    public const string InvalidFitnessGoal = "FitLogs:UserProfile:InvalidFitnessGoal";
    public const string InvalidTimeZoneId = "FitLogs:UserProfile:InvalidTimeZoneId";

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
    public const string WorkoutPlanBatchExerciseInvalid = "FitLogs:Workout:WorkoutPlanBatchExerciseInvalid";
    public const string WorkoutPlanExerciseOrderIndexAlreadyExists = "FitLogs:Workout:WorkoutPlanExerciseOrderIndexAlreadyExists";
    public const string WorkoutPlanExerciseNotFound = "FitLogs:Workout:WorkoutPlanExerciseNotFound";
    public const string WorkoutSessionAccessDenied = "FitLogs:Workout:WorkoutSessionAccessDenied";
    public const string InvalidWorkoutPlanExerciseOrder = "FitLogs:Workout:InvalidWorkoutPlanExerciseOrder";
    public const string InvalidOrderIndex = "FitLogs:Workout:InvalidOrderIndex";
    public const string WorkoutPlanIsArchived = "FitLogs:Workout:WorkoutPlanIsArchived";
    public const string CurrentWorkoutSessionExerciseNotFound = "FitLogs:Workout:CurrentWorkoutSessionExerciseNotFound";
    public const string NextWorkoutSessionExerciseNotFound = "FitLogs:Workout:NextWorkoutSessionExerciseNotFound";
    public const string PreviousWorkoutSessionExerciseNotFound = "FitLogs:Workout:PreviousWorkoutSessionExerciseNotFound";
    public const string CompletedWorkoutSessionCannotBeDeleted = "FitLogs:Workout:CompletedWorkoutSessionCannotBeDeleted";
    public const string WorkoutPlanNotFound = "FitLogs:Workout:WorkoutPlanNotFound";

    
    
    public const string WorkoutSessionStatusIsNotInProgress = "FitLogs:Workout:WorkoutSessionStatusIsNotInProgress";
    public const string InvalidWorkoutSessionEndedAt = "FitLogs:Workout:InvalidWorkoutSessionEndedAt";
    public const string InvalidWorkoutSessionStartedAt = "FitLogs:Workout:InvalidWorkoutSessionStartedAt";
    public const string WorkoutSessionNameRequiredForFreeWorkout = "FitLogs:Workout:FreeWorkoutNameRequired";
    public const string WorkoutSessionNameNotAllowedForPlanWorkout = "FitLogs:Workout:PlanWorkoutNameNotAllowed";
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
    public const string WorkoutPlanMustHaveAtLeastOneExercise = "FitLogs:Workout:WorkoutPlanMustHaveAtLeastOneExercise";
    public const string WorkoutPlanNotBelongToUser = "FitLogs:Workout:WorkoutPlanNotBelongToUser";
    public const string WorkoutPlanIsInactive = "FitLogs:Workout:WorkoutPlanIsInactive";
    public const string InvalidWorkoutGoal = "FitLogs:Workout:InvalidWorkoutGoal";
    public const string InvalidWorkoutDifficulty = "FitLogs:Workout:InvalidWorkoutDifficulty";
    public const string ExerciseIsInactive = "FitLogs:Workout:ExerciseIsInactive";

    public const string InvalidExerciseSetNumber = "FitLogs:Workout:InvalidExerciseSetNumber";
    public const string InvalidExerciseSetWeight = "FitLogs:Workout:InvalidExerciseSetWeight";
    public const string InvalidExerciseSetReps = "FitLogs:Workout:InvalidExerciseSetReps";
    public const string InvalidExerciseSetRpe = "FitLogs:Workout:InvalidExerciseSetRpe";
    public const string ExerciseSetAlreadyCompleted = "FitLogs:Workout:ExerciseSetAlreadyCompleted";
    public const string ExerciseSetAlreadySkipped = "FitLogs:Workout:ExerciseSetAlreadySkipped";
    public const string InvalidExerciseSetCompletedAt = "FitLogs:Workout:InvalidExerciseSetCompletedAt";
    public const string InvalidExerciseSetSkippedAt = "FitLogs:Workout:InvalidExerciseSetSkippedAt";

    public const string ExerciseSetNumberAlreadyExists = "FitLogs:Workout:ExerciseSetNumberAlreadyExists";
    public const string ExerciseSetNotFound = "FitLogs:Workout:ExerciseSetNotFound";
    public const string WorkoutSessionCannotCompleteWithoutCompletedExercise = "FitLogs:Workout:SessionRequiresCompletedExercise";
    public const string WorkoutSessionMustHaveAtLeastOneExercise = "FitLogs:Workout:WorkoutSessionMustHaveAtLeastOneExercise";
    public const string WorkoutSessionMustHaveAtLeastOneCompletedSet = "FitLogs:Workout:WorkoutSessionMustHaveAtLeastOneCompletedSet";


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
    public const string FoodProductUpstreamProductNotFound = "FitLogs:FoodProduct:UpstreamProductNotFound";
    public const string FoodProductUpstreamInvalidData = "FitLogs:FoodProduct:UpstreamInvalidData";
    public const string FoodProductUpstreamTimeout = "FitLogs:FoodProduct:UpstreamTimeout";
    public const string FoodProductUpstreamUnavailable = "FitLogs:FoodProduct:UpstreamUnavailable";
    public const string FoodProductPersistenceFailed = "FitLogs:FoodProduct:PersistenceFailed";
    public const string FoodProductNutritionUnknown = "FitLogs:FoodProduct:NutritionUnknown";
    public const string FoodProductNutritionBasisInvalid = "FitLogs:FoodProduct:NutritionBasisInvalid";
    public const string FoodProductServingConversionRequired = "FitLogs:FoodProduct:ServingConversionRequired";
    public const string FoodProductPieceConversionRequired = "FitLogs:FoodProduct:PieceConversionRequired";
    public const string FoodLogUnitConversionUnavailable = "FitLogs:FoodLog:UnitConversionUnavailable";


}
