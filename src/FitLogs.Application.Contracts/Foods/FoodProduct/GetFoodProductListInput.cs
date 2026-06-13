using Volo.Abp.Application.Dtos;

namespace FitLogs.Foods.FoodProducts;

public class GetFoodProductListInput : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }
    public bool OnlyActive { get; set; } = true;
}