namespace FitLogs.Permissions;

public static class FitLogsPermissions
{
    public const string GroupName = "FitLogs";

    public static class Dashboard
    {
        public const string DashboardGroup = GroupName + ":Dashboard";
        public const string Host = DashboardGroup + ":Host";
        public const string Tenant = DashboardGroup + ":Tenant";
    }
    public static class UserProfiles
    {
        public const string Default = GroupName + ".UserProfiles";
        public const string Update = Default + ".Update";
    }

    public static class Exercises
    {
        public const string Default = GroupName + ".Exercises";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string Manage = Default + ".Manage";
    }

    public static class MuscleGroups
    {
        public const string Default = GroupName + ".MuscleGroups";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string Manage = Default + ".Manage";
    }

    public static class Equipments
    {
        public const string Default = GroupName + ".Equipments";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string Manage = Default + ".Manage";
    }

    public static class Workouts
    {
        public const string Default = GroupName + ".Workouts";

        public static class WorkoutPlans
        {
            public const string Default = Workouts.Default + ".WorkoutPlans";
            public const string Create = Default + ".Create";
            public const string Edit = Default + ".Edit";
            public const string Update = Default + ".Update";
            public const string Delete = Default + ".Delete";
            public const string ManageExercises = Default + ".ManageExercises";
            public const string Archive = Default + ".Archive";
            public const string Restore = Default + ".Restore";

        }

        public static class WorkoutSessions
        {
            public const string Default = Workouts.Default + ".WorkoutSessions";
            public const string Create = Default + ".Create";
            public const string Edit = Default + ".Edit";
            public const string Update = Default + ".Update";
            public const string Delete = Default + ".Delete";
            public const string ManageExercises = Default + ".ManageExercises";
            public const string ManageSets = Default + ".ManageSets";
            public const string Complete = Default + ".Complete";
            public const string Cancel = Default + ".Cancel";
            public const string History = Default + ".History";
        }
    }

    public static class WorkoutPlans
    {
        public const string Default = Workouts.WorkoutPlans.Default;
        public const string Create = Workouts.WorkoutPlans.Create;
        public const string Edit = Workouts.WorkoutPlans.Edit;
        public const string Update = Workouts.WorkoutPlans.Update;
        public const string Delete = Workouts.WorkoutPlans.Delete;
        public const string ManageExercises = Workouts.WorkoutPlans.ManageExercises;
        public const string Archive = Workouts.WorkoutPlans.Archive;
        public const string Restore = Workouts.WorkoutPlans.Restore;

    }

    public static class WorkoutSessions
    {
        public const string Default = Workouts.WorkoutSessions.Default;
        public const string Create = Workouts.WorkoutSessions.Create;
        public const string Edit = Workouts.WorkoutSessions.Edit;
        public const string Update = Workouts.WorkoutSessions.Update;
        public const string Delete = Workouts.WorkoutSessions.Delete;
        public const string ManageExercises = Workouts.WorkoutSessions.ManageExercises;
        public const string ManageSets = Workouts.WorkoutSessions.ManageSets;
        public const string Complete = Workouts.WorkoutSessions.Complete;
        public const string Cancel = Workouts.WorkoutSessions.Cancel;
        public const string History = Workouts.WorkoutSessions.History;
    }

    public static class Dashboards
    {
        public const string Default = GroupName + ".Dashboards";
    }
}
