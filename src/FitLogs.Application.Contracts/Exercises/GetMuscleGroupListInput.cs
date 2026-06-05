using Volo.Abp.Application.Dtos;

namespace FitLogs.Exercises;

public class GetMuscleGroupListInput : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }
    public bool? IsActive { get; set; }
}