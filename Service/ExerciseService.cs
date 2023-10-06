using CrossCutting.Enums;
using Domain.DTO;
using Domain.DTO.Exercise;
using Domain.Entities.Exercise;
using Domain.Infra;
using Domain.Services;

namespace Service;

public class ExerciseService : IExerciseService
{
    private readonly IExerciseRepository _repository;
    private readonly IConfigurationService _configurationService;
    private readonly IGeneratorService _exerciseGenerator;
    private readonly ICorrectionService _correctionService;

    public ExerciseService(
        IExerciseRepository repository,
        IConfigurationService configurationService,
        IGeneratorService exerciseGenerator,
        ICorrectionService correctionService)
    {
        _repository = repository;
        _configurationService = configurationService;
        _exerciseGenerator = exerciseGenerator;
        _correctionService = correctionService;
    }

    public async Task<ServiceResult<Exercise>> Get(string exerciseId, CancellationToken cancellationToken = default)
    {
        var exercise = await _repository.Get(exerciseId, cancellationToken);

        if (exercise is null)
            return ServiceResult<Exercise>.MakeErrorResult("Exercise not found.");

        return ServiceResult<Exercise>.MakeSuccessResult(exercise);
    }

    public async Task<ServiceResult<List<Exercise>>> GetAllByIds(IEnumerable<string> exerciseIds, CancellationToken cancellationToken = default)
    {
        var exercises = await _repository.GetAllByIds(exerciseIds, cancellationToken);

        if (!exercises.Any())
            return ServiceResult<List<Exercise>>.MakeErrorResult("Exercises not found.");

        return ServiceResult<List<Exercise>>.MakeSuccessResult(exercises);
    }

    public async Task<ServiceResult<List<Exercise>>> GetAllByOwnerIdAndType(string ownerId, ExerciseType type, CancellationToken cancellationToken = default)
    {
        var exercises = await _repository.GetAllByOwnerIdAndType(ownerId, type, cancellationToken);
        if (!exercises.Any())
            return ServiceResult<List<Exercise>>.MakeErrorResult("No exercises found.");

        return ServiceResult<List<Exercise>>.MakeSuccessResult(exercises);
    }

    public async Task<ServiceResult<string>> MakeExercise(string contentId, CancellationToken cancellationToken = default)
    {
        var makeExerciseResult = await _exerciseGenerator.MakeExercise(contentId, cancellationToken);

        if (!makeExerciseResult.Success)
            return ServiceResult<string>.MakeErrorResult(makeExerciseResult.ErrorMessage);

        return new()
        {
            Success = true,
            Data = makeExerciseResult.Data
        };
    }

    public async Task<ServiceResult<string>> Save(ExerciseRequest request, CancellationToken cancellationToken = default)
    {
        var id = Guid.NewGuid().ToString();

        var exercise = new Exercise
        {
            Id = id,
            OwnerId = request.OwnerId,
            ConfigurationId = request.ConfigurationId,
            SummaryId = request.SummaryId,
            Status = request.Status,
            Type = request.Type,
            TopicIndex = request.TopicIndex,
            Title = request.Title,
            Exercises = request.Exercises
        };

        var success = await _repository.Save(exercise, cancellationToken);

        return success ? ServiceResult<string>.MakeSuccessResult(id) : ServiceResult<string>.MakeErrorResult("");
    }

    public async Task<ServiceResult<List<string>>> SaveAll(IEnumerable<Exercise> exercises, CancellationToken cancellationToken = default)
    {
        var repositoryResult = await _repository.SaveAll(exercises, cancellationToken);

        if (!repositoryResult.Any())
            return ServiceResult<List<string>>.MakeErrorResult("Error while save exercises");

        return ServiceResult<List<string>>.MakeSuccessResult(repositoryResult);
    }

    public async Task<ServiceResult<bool>> Update(string exerciseId, ExerciseRequest request, CancellationToken cancellationToken = default)
    {
        var exercise = await _repository.Get(exerciseId, cancellationToken);

        if (exercise == null)
            return ServiceResult<bool>.MakeErrorResult("Exercise not found.");

        var success = await _repository.Update(exerciseId, exercise, cancellationToken);

        return success ? ServiceResult<bool>.MakeSuccessResult(success) : ServiceResult<bool>.MakeErrorResult("");
    }
}