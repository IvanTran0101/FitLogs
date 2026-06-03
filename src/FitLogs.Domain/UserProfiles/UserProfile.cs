using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace FitLogs.UserProfiles;

public class UserProfile : FullAuditedAggregateRoot<Guid>
{
    public Guid UserId { get; private set; }
    public string DisplayName { get; private set; }
    public Gender? Gender { get; private set; }
    public DateTime? DateOfBirth { get; private set; }
    public decimal? HeightCm { get; private set; }
    public decimal? WeightKg { get; private set; }
    public FitnessGoal FitnessGoal { get; private set; }
    public int? DailyTargetCalories { get; private set; }
    
    protected UserProfile()
    {
        // For ORM
    }

    public UserProfile(Guid id, Guid userId, string displayName) : base(id)
    {
        UserId = userId;
        SetDisplayName(displayName);
        Gender = UserProfiles.Gender.Private;
        FitnessGoal = FitnessGoal.ImproveFitness;
    }

    public void SetDisplayName(string displayName)
    {
        DisplayName = Check.NotNullOrWhiteSpace(displayName, nameof(displayName), UserProfileConsts.MaxDisplayNameLength);
        
    }

    public void SetGender(Gender gender)
    {
        Gender = gender;
    }

    public void SetDateOfBirth(DateTime? dateOfBirth)
    {
        if (dateOfBirth.HasValue && dateOfBirth.Value > DateTime.UtcNow.Date)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.InvalidDateOfBirth);
        }
        DateOfBirth = dateOfBirth;
    }

    public void SetHeightCm(decimal? heightCm)
    {
        if (heightCm.HasValue && (heightCm.Value < 20 || heightCm.Value > 255))
        {
            throw new BusinessException(FitLogsDomainErrorCodes.InvalidHeightCm);
        }
        HeightCm = heightCm;
    }

    public void SetWeightKg(decimal? weightKg)
    {
        if (weightKg.HasValue && (weightKg.Value < 20 || weightKg.Value > 300))
        {
            throw new BusinessException(FitLogsDomainErrorCodes.InvalidWeightKg);
        }
        WeightKg = weightKg;
    }

    public void SetFitnessGoal(FitnessGoal fitnessGoal)
    {
        FitnessGoal = fitnessGoal;
    }

    public void SetDailyTargetCalories(int? dailyTargetCalories)
    {
        if (dailyTargetCalories.HasValue && (dailyTargetCalories.Value < 500 || dailyTargetCalories.Value > 10000))
        {
            throw new BusinessException(FitLogsDomainErrorCodes.InvalidDailyTargetCalories);
        }
        DailyTargetCalories = dailyTargetCalories;
    }
    
}