using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FitLogs.Foods.FoodLogs;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace FitLogs.Foods.FoodProducts;
public interface IFoodProductAppService : IApplicationService
{
    Task<FoodProductDto> CreateAsync(CreateUpdateFoodProductDto input);

    Task<FoodProductDto> UpdateAsync(Guid id, CreateUpdateFoodProductDto input);

    Task<FoodProductDto> GetAsync(Guid id);

    Task<PagedResultDto<FoodProductDto>> GetListAsync(GetFoodProductListInput input);

    Task DeactivateAsync(Guid id);

    Task VerifyAsync(Guid id);
}