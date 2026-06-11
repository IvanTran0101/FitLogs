namespace FitLogs.Permissions;

public static class FitLogsPermissions
{
    public const string GroupName = "FitLogs";

    public static class Dashboard
    {
        public const string DashboardGroup = GroupName + ".Dashboard";
        public const string Host = DashboardGroup + ".Host";
        public const string Tenant = DashboardGroup + ".Tenant";
    }

    
    //Add your own permission names. Example:
    //public const string MyPermission1 = GroupName + ".MyPermission1";
    public static class Workouts
    {
        public const string Default = GroupName + ".Workouts";

        public static class WorkoutPlans
        {
            public const string Default = Workouts.Default + ".WorkoutPlans";
            public const string Create = Default + ".Create";
            public const string Edit = Default + ".Edit";
            public const string Delete = Default + ".Delete";
        }

        public static class WorkoutSessions
        {
            public const string Default = Workouts.Default + ".WorkoutSessions";
            public const string Create = Default + ".Create";
            public const string Edit = Default + ".Edit";
            public const string Delete = Default + ".Delete";
        }
    }
}
