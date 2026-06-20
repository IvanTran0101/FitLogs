using FitLogs.Exercises;
using FitLogs.Foods;
using FitLogs.UserProfiles;
using FitLogs.Workouts;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.UserInvitations;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.LanguageManagement.EntityFrameworkCore;
using Volo.Abp.TextTemplateManagement.EntityFrameworkCore;
using Volo.Saas.EntityFrameworkCore;
using Volo.Saas.Editions;
using Volo.Saas.Tenants;
using Volo.Abp.Gdpr;

namespace FitLogs.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityProDbContext))]
[ReplaceDbContext(typeof(ISaasDbContext))]
[ConnectionStringName("Default")]
public class FitLogsDbContext :
    AbpDbContext<FitLogsDbContext>,
    ISaasDbContext,
    IIdentityProDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    public DbSet<UserProfile> UserProfiles { get; set; }

    public DbSet<Exercise> Exercises { get; set; }
    public DbSet<MuscleGroup> MuscleGroups { get; set; }
    public DbSet<Equipment> Equipment { get; set; }
    public DbSet<WorkoutPlan> WorkoutPlans { get; set; }
    public DbSet<WorkoutSession> WorkoutSessions { get; set; }
    
    public DbSet<FoodProduct> FoodProducts { get; set; }
    public DbSet<FoodLog> FoodLogs { get; set; }
    #region Entities from the modules

    /* Notice: We only implemented IIdentityProDbContext and ISaasDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityProDbContext and ISaasDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    // Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }
    public DbSet<UserInvitation> UserInvitations { get; set; }

    // SaaS
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Edition> Editions { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion

    public FitLogsDbContext(DbContextOptions<FitLogsDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureFeatureManagement();
        builder.ConfigureIdentityPro();
        builder.ConfigureOpenIddictPro();
        builder.ConfigureLanguageManagement();
        builder.ConfigureSaas();
        builder.ConfigureTextTemplateManagement();
        builder.ConfigureGdpr();
        builder.ConfigureBlobStoring();
        
        /* Configure your own tables/entities inside here */

        //builder.Entity<YourEntity>(b =>
        //{
        //    b.ToTable(FitLogsConsts.DbTablePrefix + "YourEntities", FitLogsConsts.DbSchema);
        //    b.ConfigureByConvention(); //auto configure for the base class props
        //    //...
        //});
        builder.Entity<UserProfile>(b =>
        {
            b.ToTable(FitLogsConsts.DbTablePrefix + "UserProfiles", FitLogsConsts.DbSchema);

            b.ConfigureByConvention();

            b.Property(x => x.UserId)
                .IsRequired();

            b.Property(x => x.DisplayName)
                .IsRequired()
                .HasMaxLength(UserProfileConsts.MaxDisplayNameLength);

            b.Property(x => x.Gender)
                .IsRequired();

            b.Property(x => x.FitnessGoal)
                .IsRequired();

            b.HasIndex(x => x.UserId)
                .IsUnique();
        });
        builder.Entity<MuscleGroup>(b =>
        {
            b.ToTable(FitLogsConsts.DbTablePrefix + "MuscleGroups", FitLogsConsts.DbSchema);

            b.ConfigureByConvention();

            b.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(MuscleGroupConsts.MaxNameLength);

            b.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(MuscleGroupConsts.MaxCodeLength);

            b.Property(x => x.Description)
                .HasMaxLength(MuscleGroupConsts.MaxDescriptionLength);

            b.HasIndex(x => x.Code)
                .IsUnique();

            b.HasIndex(x => x.Name)
                .IsUnique();

            b.HasIndex(x => x.DisplayOrder);

            b.HasIndex(x => x.IsActive);
        });
        builder.Entity<Equipment>(b =>
        {
            b.ToTable(FitLogsConsts.DbTablePrefix + "Equipment", FitLogsConsts.DbSchema);

            b.ConfigureByConvention();

            b.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(EquipmentConsts.MaxNameLength);

            b.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(EquipmentConsts.MaxCodeLength);

            b.Property(x => x.Description)
                .HasMaxLength(EquipmentConsts.MaxDescriptionLength);

            b.HasIndex(x => x.Code)
                .IsUnique();

            b.HasIndex(x => x.Name)
                .IsUnique();

            b.HasIndex(x => x.DisplayOrder);

            b.HasIndex(x => x.IsActive);
        });
        
        builder.Entity<Exercise>(b =>
        {
            b.ToTable(FitLogsConsts.DbTablePrefix + "Exercises", FitLogsConsts.DbSchema);

            b.ConfigureByConvention();

            b.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(ExerciseConsts.MaxNameLength);

            b.Property(x => x.Slug)
                .IsRequired()
                .HasMaxLength(ExerciseConsts.MaxSlugLength);

            b.Property(x => x.Description)
                .HasMaxLength(ExerciseConsts.MaxDescriptionLength);

            b.Property(x => x.ImageUrl)
                .HasMaxLength(ExerciseConsts.MaxUrlLength);

            b.Property(x => x.GifUrl)
                .HasMaxLength(ExerciseConsts.MaxUrlLength);

            b.Property(x => x.Instructions)
                .HasMaxLength(ExerciseConsts.MaxInstructionsLength);

            b.Property(x => x.FormTips)
                .HasMaxLength(ExerciseConsts.MaxFormTipsLength);

            b.Property(x => x.CommonMistakes)
                .HasMaxLength(ExerciseConsts.MaxCommonMistakesLength);

            b.Property(x => x.Difficulty)
                .IsRequired();

            b.Property(x => x.TrackingType)
                .IsRequired();

            b.Property(x => x.PrimaryMuscleGroupId)
                .IsRequired();

            b.Property(x => x.EquipmentId)
                .IsRequired();

            b.Property(x => x.IsActive)
                .IsRequired();

            b.HasIndex(x => x.Slug)
                .IsUnique();

            b.HasIndex(x => new
            {
                x.Name,
                x.EquipmentId
            }).IsUnique();

            b.HasIndex(x => x.PrimaryMuscleGroupId);

            b.HasIndex(x => x.EquipmentId);

            b.HasIndex(x => x.Difficulty);

            b.HasIndex(x => x.IsActive);

            b.HasOne<MuscleGroup>()
                .WithMany()
                .HasForeignKey(x => x.PrimaryMuscleGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne<Equipment>()
                .WithMany()
                .HasForeignKey(x => x.EquipmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });
         builder.Entity<WorkoutPlan>(b =>
        {
            b.ToTable(FitLogsConsts.DbTablePrefix + "WorkoutPlans", FitLogsConsts.DbSchema);

            b.ConfigureByConvention();

            b.Property(x => x.UserId)
                .IsRequired();

            b.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(WorkoutPlanConsts.MaxNameLength);

            b.Property(x => x.Description)
                .HasMaxLength(WorkoutPlanConsts.MaxDescriptionLength);

            b.Property(x => x.Goal)
                .IsRequired();

            b.Property(x => x.Difficulty)
                .IsRequired();

            b.Property(x => x.IsActive)
                .IsRequired();

            b.Property(x => x.IsArchived)
                .IsRequired();

            b.HasIndex(x => x.IsArchived);

            b.HasIndex(x => x.UserId);

            b.HasMany(x => x.Exercises)
                .WithOne()
                .HasForeignKey(x => x.WorkoutPlanId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            b.Navigation(x => x.Exercises)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        builder.Entity<WorkoutPlanExercise>(b =>
        {
            b.ToTable(FitLogsConsts.DbTablePrefix + "WorkoutPlanExercises", FitLogsConsts.DbSchema);

            b.ConfigureByConvention();

            b.Property(x => x.WorkoutPlanId)
                .IsRequired();

            b.Property(x => x.ExerciseId)
                .IsRequired();

            b.Property(x => x.OrderIndex)
                .IsRequired();

            b.Property(x => x.DefaultSets)
                .IsRequired();

            b.Property(x => x.DefaultReps)
                .IsRequired();

            b.Property(x => x.DefaultWeightKg);

            b.Property(x => x.RestSeconds);

            b.Property(x => x.Note)
                .HasMaxLength(WorkoutPlanExerciseConsts.MaxNoteLength);

            b.HasIndex(x => x.WorkoutPlanId);

            b.HasIndex(x => x.ExerciseId);

            b.HasIndex(x => new
            {
                x.WorkoutPlanId,
                x.OrderIndex
            }).IsUnique();

            b.HasIndex(x => new
            {
                x.WorkoutPlanId,
                x.ExerciseId
            }).IsUnique();
        });

        builder.Entity<WorkoutSession>(b =>
        {
            b.ToTable(FitLogsConsts.DbTablePrefix + "WorkoutSessions", FitLogsConsts.DbSchema);

            b.ConfigureByConvention();

            b.Property(x => x.UserId)
                .IsRequired();

            b.Property(x => x.WorkoutPlanId);

            b.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(WorkoutSessionConsts.MaxNameLength);

            b.Property(x => x.StartedAt)
                .IsRequired();

            b.Property(x => x.EndedAt);

            b.Property(x => x.Status)
                .IsRequired();

            b.Property(x => x.Note)
                .HasMaxLength(WorkoutSessionConsts.MaxNoteLength);
            b.Property(x=> x.CurrentWorkoutSessionExerciseId)
                .IsRequired(false);
            b.HasIndex(x => x.CurrentWorkoutSessionExerciseId);
            b.HasIndex(x => x.UserId);

            b.HasIndex(x => x.WorkoutPlanId);

            b.HasMany(x => x.Exercises)
                .WithOne()
                .HasForeignKey(x => x.WorkoutSessionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne<WorkoutPlan>()
                .WithMany()
                .HasForeignKey(x => x.WorkoutPlanId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            b.Navigation(x => x.Exercises)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        builder.Entity<WorkoutSessionExercise>(b =>
        {
            b.ToTable(FitLogsConsts.DbTablePrefix + "WorkoutSessionExercises", FitLogsConsts.DbSchema);

            b.ConfigureByConvention();

            b.Property(x => x.WorkoutSessionId)
                .IsRequired();

            b.Property(x => x.ExerciseId)
                .IsRequired();

            b.Property(x => x.OrderIndex)
                .IsRequired();

            b.Property(x => x.TargetSets)
                .IsRequired();

            b.Property(x => x.TargetReps)
                .IsRequired();

            b.Property(x => x.TargetWeightKg);

            b.Property(x => x.RestSeconds);
            b.Property(x => x.WorkoutPlanExerciseId)
                .IsRequired(false);

            b.Property(x => x.Note)
                .HasMaxLength(WorkoutSessionExerciseConsts.MaxNoteLength);

            b.HasIndex(x => x.WorkoutPlanExerciseId);
            b.HasIndex(x => x.WorkoutSessionId);

            b.HasIndex(x => x.ExerciseId);

            b.HasIndex(x => new
            {
                x.WorkoutSessionId,
                x.OrderIndex
            }).IsUnique();

            b.HasIndex(x => new
            {
                x.WorkoutSessionId,
                x.ExerciseId
            }).IsUnique();

            b.HasMany(x => x.Sets)
                .WithOne()
                .HasForeignKey(x => x.WorkoutSessionExerciseId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            b.Navigation(x => x.Sets)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        builder.Entity<ExerciseSet>(b =>
        {
            b.ToTable(FitLogsConsts.DbTablePrefix + "ExerciseSets", FitLogsConsts.DbSchema);

            b.ConfigureByConvention();

            b.Property(x => x.WorkoutSessionExerciseId)
                .IsRequired();

            b.Property(x => x.SetNumber)
                .IsRequired();

            b.Property(x => x.WeightKg)
                .IsRequired();

            b.Property(x => x.Reps)
                .IsRequired();

            b.Property(x => x.Rpe);

            b.Property(x => x.Note)
                .HasMaxLength(ExerciseSetConsts.MaxNoteLength);

            b.Property(x => x.IsCompleted)
                .IsRequired();

            b.Property(x => x.CompletedAt);

            b.HasIndex(x => x.WorkoutSessionExerciseId);

            b.HasIndex(x => new
            {
                x.WorkoutSessionExerciseId,
                x.SetNumber
            }).IsUnique();
        });
        
        builder.Entity<FoodProduct>(b =>
        {
            b.ToTable(FitLogsConsts.DbTablePrefix + "FoodProducts", FitLogsConsts.DbSchema);

            b.ConfigureByConvention();

            b.Property(x => x.Barcode)
                .HasMaxLength(FoodProductConsts.MaxBarcodeLength);

            b.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(FoodProductConsts.MaxNameLength);

            b.Property(x => x.Brand)
                .HasMaxLength(FoodProductConsts.MaxBrandLength);

            b.Property(x => x.ImageUrl)
                .HasMaxLength(FoodProductConsts.MaxImageUrlLength);

            b.Property(x => x.ServingSize)
                .HasMaxLength(FoodProductConsts.MaxServingSizeLength);

            b.Property(x => x.Source)
                .IsRequired();

            b.Property(x => x.LastSyncedAt);

            b.Property(x => x.IsActive)
                .IsRequired();

            b.Property(x => x.IsVerified)
                .IsRequired();

            b.Property(x => x.CaloriesPer100g)
                .HasColumnType("decimal(18,2)");

            b.Property(x => x.ProteinPer100g)
                .HasColumnType("decimal(18,2)");

            b.Property(x => x.CarbPer100g)
                .HasColumnType("decimal(18,2)");

            b.Property(x => x.FatPer100g)
                .HasColumnType("decimal(18,2)");

            b.HasIndex(x => x.Barcode)
                .IsUnique()
                .HasFilter("\"Barcode\" IS NOT NULL");

            b.HasIndex(x => x.Name);
            b.HasIndex(x => x.Source);
            b.HasIndex(x => x.IsActive);
            b.HasIndex(x => x.IsVerified);
        });
        
        builder.Entity<FoodLog>(b =>
        {
            b.ToTable(FitLogsConsts.DbTablePrefix + "FoodLogs", FitLogsConsts.DbSchema);

            b.ConfigureByConvention();

            b.Property(x => x.UserId)
                .IsRequired();

            b.Property(x => x.FoodProductId)
                .IsRequired();

            b.Property(x => x.FoodName)
                .IsRequired()
                .HasMaxLength(FoodLogConsts.MaxFoodNameLength);

            b.Property(x => x.Quantity)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            b.Property(x => x.Unit)
                .IsRequired();

            b.Property(x => x.Calories)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            b.Property(x => x.Protein)
                .HasColumnType("decimal(18,2)");

            b.Property(x => x.Carb)
                .HasColumnType("decimal(18,2)");

            b.Property(x => x.Fat)
                .HasColumnType("decimal(18,2)");

            b.Property(x => x.MealType)
                .IsRequired();

            b.Property(x => x.LoggedAt)
                .IsRequired();

            b.Property(x => x.Note)
                .HasMaxLength(FoodLogConsts.MaxNoteLength);

            b.HasOne<FoodProduct>()
                .WithMany()
                .HasForeignKey(x => x.FoodProductId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => new { x.UserId, x.LoggedAt });

            b.HasIndex(x => x.FoodProductId);

            b.HasIndex(x => x.MealType);
        });
    }
    
}
