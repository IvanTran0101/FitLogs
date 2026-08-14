// These names mirror FitLogsPermissions in the verified backend permission definition provider.
export const FITLOGS_PERMISSIONS = {
  dashboards: {
    default: 'FitLogs.Dashboard',
  },
  userProfiles: {
    default: 'FitLogs.UserProfiles',
    update: 'FitLogs.UserProfiles.Update',
  },
  workoutPlans: {
    default: 'FitLogs.WorkoutPlans',
    create: 'FitLogs.WorkoutPlans.Create',
    update: 'FitLogs.WorkoutPlans.Update',
    delete: 'FitLogs.WorkoutPlans.Delete',
    manageExercises: 'FitLogs.WorkoutPlans.ManageExercises',
    archive: 'FitLogs.WorkoutPlans.Archive',
    restore: 'FitLogs.WorkoutPlans.Restore',
  },
  workoutSessions: {
    default: 'FitLogs.WorkoutSessions',
    create: 'FitLogs.WorkoutSessions.Create',
    update: 'FitLogs.WorkoutSessions.Update',
    delete: 'FitLogs.WorkoutSessions.Delete',
    manageExercises: 'FitLogs.WorkoutSessions.ManageExercises',
    manageSets: 'FitLogs.WorkoutSessions.ManageSets',
    complete: 'FitLogs.WorkoutSessions.Complete',
    cancel: 'FitLogs.WorkoutSessions.Cancel',
    history: 'FitLogs.WorkoutSessions.History',
  },
} as const
