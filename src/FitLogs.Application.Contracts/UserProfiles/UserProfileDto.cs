using System;

namespace FitLogs.UserProfiles;

public class UserProfileDto
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    public Gender Gender { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public decimal? HeightCm { get; set; }

    public decimal? WeightKg { get; set; }

    public FitnessGoal FitnessGoal { get; set; }

    public int? DailyTargetCalories { get; set; }

}