using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;

namespace FitLogs.Exercises;

public class ExerciseDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IMuscleGroupRepository _muscleGroupRepository;
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IExerciseRepository _exerciseRepository;
    private readonly MuscleGroupManager _muscleGroupManager;
    private readonly EquipmentManager _equipmentManager;
    private readonly ExerciseManager _exerciseManager;

    public ExerciseDataSeedContributor(
        IMuscleGroupRepository muscleGroupRepository,
        IEquipmentRepository equipmentRepository,
        IExerciseRepository exerciseRepository,
        MuscleGroupManager muscleGroupManager,
        EquipmentManager equipmentManager,
        ExerciseManager exerciseManager)
    {
        _muscleGroupRepository = muscleGroupRepository;
        _equipmentRepository = equipmentRepository;
        _exerciseRepository = exerciseRepository;
        _muscleGroupManager = muscleGroupManager;
        _equipmentManager = equipmentManager;
        _exerciseManager = exerciseManager;
    }

    [UnitOfWork]
    public async Task SeedAsync(DataSeedContext context)
    {
        var muscleGroups = await SeedMuscleGroupsAsync();
        var equipments = await SeedEquipmentsAsync();

        await SeedExercisesAsync(muscleGroups, equipments);
    }

    private async Task<Dictionary<string, MuscleGroup>> SeedMuscleGroupsAsync()
    {
        var result = new Dictionary<string, MuscleGroup>();

        result["chest"] = await GetOrCreateMuscleGroupAsync("Ngực", "chest", 1);
        result["back"] = await GetOrCreateMuscleGroupAsync("Lưng", "back", 2);
        result["legs"] = await GetOrCreateMuscleGroupAsync("Chân", "legs", 3);
        result["shoulders"] = await GetOrCreateMuscleGroupAsync("Vai", "shoulders", 4);
        result["arms"] = await GetOrCreateMuscleGroupAsync("Tay", "arms", 5);
        result["core"] = await GetOrCreateMuscleGroupAsync("Core", "core", 6);

        return result;
    }

    private async Task<Dictionary<string, Equipment>> SeedEquipmentsAsync()
    {
        var result = new Dictionary<string, Equipment>();

        result["barbell"] = await GetOrCreateEquipmentAsync("Barbell", "barbell", 1);
        result["dumbbell"] = await GetOrCreateEquipmentAsync("Dumbbell", "dumbbell", 2);
        result["bodyweight"] = await GetOrCreateEquipmentAsync("Bodyweight", "bodyweight", 3);
        result["machine"] = await GetOrCreateEquipmentAsync("Machine", "machine", 4);
        result["cable"] = await GetOrCreateEquipmentAsync("Cable", "cable", 5);

        return result;
    }

    private async Task SeedExercisesAsync(
        Dictionary<string, MuscleGroup> muscleGroups,
        Dictionary<string, Equipment> equipments)
    {
        await GetOrCreateExerciseAsync(
            name: "Bench Press",
            slug: "bench-press",
            muscleGroup: muscleGroups["chest"],
            equipment: equipments["barbell"],
            difficulty: ExerciseDifficulty.Intermediate,
            trackingType: ExerciseTrackingType.RepsAndWeight,
            description: "Bài đẩy ngực với thanh tạ, tập trung vào ngực, vai trước và tay sau.",
            instructions: "Nằm chắc trên ghế, hạ thanh tạ có kiểm soát, đẩy lên mạnh nhưng không khoá khớp quá mức.",
            formTips: "Giữ bả vai siết lại, bàn chân đặt chắc, cổ tay thẳng.",
            commonMistakes: "Cong cổ tay, bật mông khỏi ghế, hạ thanh quá nhanh hoặc để khuỷu tay xoè quá rộng."
        );

        await GetOrCreateExerciseAsync(
            name: "Squat",
            slug: "squat",
            muscleGroup: muscleGroups["legs"],
            equipment: equipments["barbell"],
            difficulty: ExerciseDifficulty.Intermediate,
            trackingType: ExerciseTrackingType.RepsAndWeight,
            description: "Bài compound cho chân, mông và core.",
            instructions: "Đặt thanh tạ chắc trên lưng trên, hạ người xuống có kiểm soát rồi đứng lên.",
            formTips: "Giữ lưng trung lập, đầu gối đi theo hướng mũi chân.",
            commonMistakes: "Gập lưng dưới, nhấc gót chân, để đầu gối đổ vào trong."
        );

        await GetOrCreateExerciseAsync(
            name: "Deadlift",
            slug: "deadlift",
            muscleGroup: muscleGroups["back"],
            equipment: equipments["barbell"],
            difficulty: ExerciseDifficulty.Advanced,
            trackingType: ExerciseTrackingType.RepsAndWeight,
            description: "Bài kéo tạ từ sàn, tác động mạnh vào posterior chain.",
            instructions: "Setup sát thanh tạ, giữ lưng chắc, kéo tạ bằng hông và chân.",
            formTips: "Không giật tạ khỏi sàn, giữ thanh tạ gần người.",
            commonMistakes: "Để lưng cong, kéo bằng tay quá nhiều, để thanh tạ rời xa thân người."
        );

        await GetOrCreateExerciseAsync(
            name: "Pull Up",
            slug: "pull-up",
            muscleGroup: muscleGroups["back"],
            equipment: equipments["bodyweight"],
            difficulty: ExerciseDifficulty.Advanced,
            trackingType: ExerciseTrackingType.RepsOnly,
            description: "Bài kéo xà dùng trọng lượng cơ thể, tập trung vào lưng và tay trước.",
            instructions: "Nắm xà chắc, kéo ngực hướng lên xà, hạ người có kiểm soát.",
            formTips: "Giữ core siết, không đung đưa người quá mức.",
            commonMistakes: "Dùng đà quá nhiều, không kiểm soát pha hạ người."
        );

        await GetOrCreateExerciseAsync(
            name: "Overhead Press",
            slug: "overhead-press",
            muscleGroup: muscleGroups["shoulders"],
            equipment: equipments["barbell"],
            difficulty: ExerciseDifficulty.Intermediate,
            trackingType: ExerciseTrackingType.RepsAndWeight,
            description: "Bài đẩy vai với thanh tạ.",
            instructions: "Đẩy thanh tạ từ vai lên qua đầu, giữ thân người chắc.",
            formTips: "Siết core, không ngửa lưng quá mức.",
            commonMistakes: "Ưỡn lưng mạnh, đẩy thanh đi quá xa thân người."
        );
    }

    private async Task<MuscleGroup> GetOrCreateMuscleGroupAsync(
        string name,
        string code,
        int displayOrder)
    {
        var existing = await _muscleGroupRepository.FindByCodeAsync(code);

        if (existing != null)
        {
            return existing;
        }

        var muscleGroup = await _muscleGroupManager.CreateAsync(
            name,
            code,
            displayOrder);

        return await _muscleGroupRepository.InsertAsync(muscleGroup, autoSave: true);
    }

    private async Task<Equipment> GetOrCreateEquipmentAsync(
        string name,
        string code,
        int displayOrder)
    {
        var existing = await _equipmentRepository.FindByCodeAsync(code);

        if (existing != null)
        {
            return existing;
        }

        var equipment = await _equipmentManager.CreateAsync(
            name,
            code,
            displayOrder);

        return await _equipmentRepository.InsertAsync(equipment, autoSave: true);
    }

    private async Task GetOrCreateExerciseAsync(
        string name,
        string slug,
        MuscleGroup muscleGroup,
        Equipment equipment,
        ExerciseDifficulty difficulty,
        ExerciseTrackingType trackingType,
        string description,
        string instructions,
        string formTips,
        string commonMistakes)
    {
        if (await _exerciseRepository.SlugExistsAsync(slug))
        {
            return;
        }

        var exercise = await _exerciseManager.CreateAsync(
            name,
            slug,
            muscleGroup.Id,
            equipment.Id,
            difficulty,
            trackingType,
            description,
            imageUrl: null,
            gifUrl: null,
            instructions,
            formTips,
            commonMistakes);

        await _exerciseRepository.InsertAsync(exercise, autoSave: true);
    }
}