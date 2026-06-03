using FitLogs.Exercises;
using FitLogs.UserProfiles;
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

            b.HasIndex(x => x.DisplayOrder);
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

            b.HasIndex(x => x.DisplayOrder);
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

            b.Property(x => x.IsActive)
                .IsRequired();

            b.HasIndex(x => x.Slug)
                .IsUnique();

            b.HasIndex(x => x.PrimaryMuscleGroupId);

            b.HasIndex(x => x.EquipmentId);

            b.HasOne<MuscleGroup>()
                .WithMany()
                .HasForeignKey(x => x.PrimaryMuscleGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne<Equipment>()
                .WithMany()
                .HasForeignKey(x => x.EquipmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
