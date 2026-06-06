using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace FitLogs.Exercises;

public interface IExerciseAppService : IApplicationService
{
    Task<ExerciseDto> GetAsync(Guid id);
    Task<ExerciseDto> GetBySlugAsync(string slug);
    Task<PagedResultDto<ExerciseDto>> GetListAsync(GetExerciseListInput input);
    Task<ExerciseDto> CreateAsync(CreateUpdateExerciseDto input);
    Task<ExerciseDto> UpdateAsync(Guid id, CreateUpdateExerciseDto input);
    Task ActivateAsync(Guid id);
    Task DeactivateAsync(Guid id);
}