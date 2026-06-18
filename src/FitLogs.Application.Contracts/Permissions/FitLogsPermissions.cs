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

    public static class Exercises

    {

        public const string Default = GroupName + ".Exercises";

        public const string Create = Default + ".Create";

        public const string Update = Default + ".Update";

        public const string Manage = Default + ".Manage";

    }

    public static class MuscleGroups

    {

        public const string Default = GroupName + ".MuscleGroups";

        public const string Create = Default + ".Create";

        public const string Update = Default + ".Update";

        public const string Manage = Default + ".Manage";

    }

    public static class Equipments

    {

        public const string Default = GroupName + ".Equipments";

        public const string Create = Default + ".Create";

        public const string Update = Default + ".Update";

        public const string Manage = Default + ".Manage";

    }
    //Add your own permission names. Example:
    //public const string MyPermission1 = GroupName + ".MyPermission1";
    public static class WorkoutPlans

    {

        public const string Default = GroupName + ".WorkoutPlans";

        public const string Create = Default + ".Create";

        public const string Update = Default + ".Update";

        public const string Delete = Default + ".Delete";

        public const string ManageExercises = Default + ".ManageExercises";

    }

    public static class WorkoutSessions

    {

        public const string Default = GroupName + ".WorkoutSessions";

        public const string Create = Default + ".Create";

        public const string Update = Default + ".Update";

        public const string Delete = Default + ".Delete";

        public const string ManageExercises = Default + ".ManageExercises";

        public const string ManageSets = Default + ".ManageSets";

        public const string Complete = Default + ".Complete";

        public const string Cancel = Default + ".Cancel";

        public const string History = Default + ".History";

    }
    public static class UserProfiles
    {
        public const string Default = GroupName + ".UserProfiles";
        public const string Update = Default + ".Update";
    }
    public static class Dashboards
    {
        public const string Default = GroupName + ".Dashboard";
    }
}
