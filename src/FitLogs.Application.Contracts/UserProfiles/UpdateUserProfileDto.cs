using System;
using System.ComponentModel.DataAnnotations;

namespace FitLogs.UserProfiles;

public class UpdateUserProfileDto
{
    [Required]
    [MaxLength(UserProfileConsts.MaxDisplayNameLength)]
    public string DisplayName { get; set; } = string.Empty;

    public Gender Gender { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [Range(50, 250)]
    public decimal? HeightCm { get; set; }

    [Range(20, 300)]
    public decimal? WeightKg { get; set; }

    public FitnessGoal FitnessGoal { get; set; }

    [Range(800, 6000)]
    public int? DailyTargetCalories { get; set; }
}