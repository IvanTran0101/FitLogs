using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace FitLogs.Exercises;

public interface IExerciseRepository : IRepository<Exercise, Guid>
{
    Task<Exercise?> FindBySlugAsync(string slug, bool includeDetails = true);
    Task<bool> SlugExistsAsync(string slug, Guid? excludedId = null);
    
    Task<List<Exercise>> GetListAsync(
        string? filterText = null,
        Guid? muscleGroupId = null,
        Guid? equipmentId = null,
        ExerciseDifficulty? exerciseDifficulty = null,
        ExerciseTrackingType? trackingType = null,
        bool? isActive = null,
        string? sorting = null,
        int maxResults = 50,
        int skipCount = 0);

    Task<long> GetCountAsync(
        string? filterText = null,
        Guid? muscleGroupId = null,
        Guid? equipmentId = null,
        ExerciseDifficulty? exerciseDifficulty = null,
        ExerciseTrackingType? trackingType = null,
        bool? isActive = null);
}