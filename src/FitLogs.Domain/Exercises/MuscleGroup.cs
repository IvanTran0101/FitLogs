using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace FitLogs.Exercises;

public class MuscleGroup : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; private set; }
    public string Code { get; private set; }
    public string? Description { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; }

    protected MuscleGroup()
    {
        // For ORM
    }

    public MuscleGroup(
        Guid id,
        string name,
        string code,
        int displayOrder,
        string? description = null) : base(id)
    {
        SetName(name);
        SetCode(code);
        SetDisplayOrder(displayOrder);
        SetDescription(description);
        IsActive = true;
    }

    public void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), MuscleGroupConsts.MaxNameLength);
    }

    public void SetCode(string code)
    {
        Code =  Check.NotNullOrWhiteSpace(code, nameof(code), MuscleGroupConsts.MaxCodeLength);
    }

    public void SetDisplayOrder(int displayOrder)
    {
        if (displayOrder < 0)
        {
            throw new BusinessException(FitLogsDomainErrorCodes.MuscleGroupInvalidOrderDisplay);
        }
        DisplayOrder = displayOrder;
    }

    public void SetDescription(string? description)
    {
        Description = Check.NotNullOrWhiteSpace(description, nameof(description), MuscleGroupConsts.MaxDescriptionLength);
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