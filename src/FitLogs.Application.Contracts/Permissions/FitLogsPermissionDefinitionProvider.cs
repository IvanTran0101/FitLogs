using FitLogs.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace FitLogs.Permissions;

public class FitLogsPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(FitLogsPermissions.GroupName, L("Permission:FitLogs"));


        var userProfiles = myGroup.AddPermission(FitLogsPermissions.UserProfiles.Default, L("Permission:UserProfiles"));
        userProfiles.AddChild(FitLogsPermissions.UserProfiles.Update, L("Permission:UserProfiles.Update"));

        var exercises = myGroup.AddPermission(FitLogsPermissions.Exercises.Default, L("Permission:Exercises"));
        exercises.AddChild(FitLogsPermissions.Exercises.Create, L("Permission:Exercises.Create"));
        exercises.AddChild(FitLogsPermissions.Exercises.Update, L("Permission:Exercises.Update"));
        exercises.AddChild(FitLogsPermissions.Exercises.Manage, L("Permission:Exercises.Manage"));

        var muscleGroups = myGroup.AddPermission(FitLogsPermissions.MuscleGroups.Default, L("Permission:MuscleGroups"));
        muscleGroups.AddChild(FitLogsPermissions.MuscleGroups.Create, L("Permission:MuscleGroups.Create"));
        muscleGroups.AddChild(FitLogsPermissions.MuscleGroups.Update, L("Permission:MuscleGroups.Update"));
        muscleGroups.AddChild(FitLogsPermissions.MuscleGroups.Manage, L("Permission:MuscleGroups.Manage"));

        var equipments = myGroup.AddPermission(FitLogsPermissions.Equipments.Default, L("Permission:Equipments"));
        equipments.AddChild(FitLogsPermissions.Equipments.Create, L("Permission:Equipments.Create"));
        equipments.AddChild(FitLogsPermissions.Equipments.Update, L("Permission:Equipments.Update"));
        equipments.AddChild(FitLogsPermissions.Equipments.Manage, L("Permission:Equipments.Manage"));

        var workoutPlans = myGroup.AddPermission(FitLogsPermissions.WorkoutPlans.Default, L("Permission:WorkoutPlans"));
        workoutPlans.AddChild(FitLogsPermissions.WorkoutPlans.Create, L("Permission:WorkoutPlans.Create"));
        workoutPlans.AddChild(FitLogsPermissions.WorkoutPlans.Update, L("Permission:WorkoutPlans.Update"));
        workoutPlans.AddChild(FitLogsPermissions.WorkoutPlans.Delete, L("Permission:WorkoutPlans.Delete"));
        workoutPlans.AddChild(FitLogsPermissions.WorkoutPlans.ManageExercises, L("Permission:WorkoutPlans.ManageExercises"));
        workoutPlans.AddChild(FitLogsPermissions.WorkoutPlans.Archive, L("Permission:WorkoutPlans.Archive"));
        workoutPlans.AddChild(FitLogsPermissions.WorkoutPlans.Restore, L("Permission:WorkoutPlans.Restore"));


        var workoutSessions = myGroup.AddPermission(FitLogsPermissions.WorkoutSessions.Default, L("Permission:WorkoutSessions"));
        workoutSessions.AddChild(FitLogsPermissions.WorkoutSessions.Create, L("Permission:WorkoutSessions.Create"));
        workoutSessions.AddChild(FitLogsPermissions.WorkoutSessions.Update, L("Permission:WorkoutSessions.Update"));
        workoutSessions.AddChild(FitLogsPermissions.WorkoutSessions.Delete, L("Permission:WorkoutSessions.Delete"));
        workoutSessions.AddChild(FitLogsPermissions.WorkoutSessions.ManageExercises, L("Permission:WorkoutSessions.ManageExercises"));
        workoutSessions.AddChild(FitLogsPermissions.WorkoutSessions.ManageSets, L("Permission:WorkoutSessions.ManageSets"));
        workoutSessions.AddChild(FitLogsPermissions.WorkoutSessions.Complete, L("Permission:WorkoutSessions.Complete"));
        workoutSessions.AddChild(FitLogsPermissions.WorkoutSessions.Cancel, L("Permission:WorkoutSessions.Cancel"));
        workoutSessions.AddChild(FitLogsPermissions.WorkoutSessions.History, L("Permission:WorkoutSessions.History"));
        
        myGroup.AddPermission(FitLogsPermissions.Dashboards.Default, L("Permission:Dashboards"));

    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<FitLogsResource>(name);
    }
}
