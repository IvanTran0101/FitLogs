using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace FitLogs.Exercises;

public class Exercise : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; private set; }
    public string Slug { get; private set; }
    public string? Description { get; private set; }
    public Guid PrimaryMuscleGroupId { get; private set; }
    public Guid EquipmentId { get; private set; }
    public ExerciseDifficulty Difficulty { get; private set; }
    public ExerciseTrackingType TrackingType { get; private set; }
    public string? ImageUrl { get; private set; }
    public string? GifUrl { get; private set; }
    public string? Instructions {get; private set;}
    public string? FormTips { get; private set; }
    public string? CommonMistakes {get; private set;}
    public bool IsActive {get; private set;}
    protected Exercise()
    {
        // For ORM
    }

    public Exercise(Guid id,
        string name,
        string slug,
        ExerciseTrackingType trackingType,
        Guid primaryMuscleGroupId,
        Guid equipmentId,
        ExerciseDifficulty difficulty,
        string? description = null,
        string? formTips = null,
        string? imageUrl =null,
        string? gifUrl =null,
        string? instructions =null,
        string? commonMistakes =null) : base(id)
    {
        SetName(name);
        SetSlug(slug);
        SetPrimaryMuscleGroup(primaryMuscleGroupId);
        SetEquipment(equipmentId);
        SetDifficulty(difficulty);
        SetTrackingType(trackingType);

        Description = description;
        ImageUrl = imageUrl;
        GifUrl = gifUrl;
        Instructions = instructions;
        CommonMistakes = commonMistakes;
        FormTips = formTips;
        IsActive = true;

    }

    public void SetTrackingType(ExerciseTrackingType trackingType)
    {
        if (!Enum.IsDefined(typeof(ExerciseTrackingType), trackingType))
        {
            throw new BusinessException(FitLogsDomainErrorCodes.ExerciseInvalidTrackingType);
        }
        TrackingType = trackingType;
    }

    public void SetDifficulty(ExerciseDifficulty difficulty)
    {
        if (!Enum.IsDefined(typeof(ExerciseDifficulty), difficulty))
        {
            throw new BusinessException(FitLogsDomainErrorCodes.ExerciseInvalidDifficulty);
        }
        Difficulty = difficulty;
    }

    public void SetEquipment(Guid equipmentId)
    {
        if (equipmentId == Guid.Empty)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.ExerciseEquipmentRequired);
        }
        EquipmentId = equipmentId;
    }

    public void SetPrimaryMuscleGroup(Guid primaryMuscleGroupId)
    {
        if (primaryMuscleGroupId == Guid.Empty)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.ExercisePrimaryMuscleGroupRequired);

        }
        PrimaryMuscleGroupId = primaryMuscleGroupId;
    }

    public void SetSlug(string slug)
    {
        Slug = Check.NotNullOrWhiteSpace(slug, nameof(slug), ExerciseConsts.MaxSlugLength);
    }

    public void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), ExerciseConsts.MaxNameLength);
    }

    public void SetDescription(string? description)
    {
        Description = Check.Length(description, nameof(description), ExerciseConsts.MaxDescriptionLength);
        
    }

    public void SetMedia(string? imageUrl, string? gifUrl)
    {
        ImageUrl = Check.Length(imageUrl, nameof(imageUrl), ExerciseConsts.MaxUrlLength);
        GifUrl = Check.Length(gifUrl, nameof(gifUrl), ExerciseConsts.MaxUrlLength);
    }

    public void SetContent(string? instructions, string? formTips, string? commonMistakes)
    {
        Instructions = Check.Length(instructions, nameof(instructions), ExerciseConsts.MaxInstructionsLength);
        FormTips = Check.Length(formTips, nameof(formTips), ExerciseConsts.MaxFormTipsLength);
        CommonMistakes = Check.Length(commonMistakes, nameof(commonMistakes), ExerciseConsts.MaxCommonMistakesLength);
    }
    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}