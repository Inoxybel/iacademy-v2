using CrossCutting.Enums;
using CrossCutting.Extensions;
using Domain.DTO;
using Domain.DTO.Exercise;
using Domain.Entities;
using Domain.Infra;
using Domain.Services;

namespace Service;

public class ExerciseService : IExerciseService
{
    private readonly IExerciseRepository _repository;
    private readonly IConfigurationService _configurationService;
    private readonly IGeneratorService _exerciseGenerator;

    public ExerciseService(
        IExerciseRepository repository,
        IConfigurationService configurationService,
        IGeneratorService exerciseGenerator)
    {
        _repository = repository;
        _configurationService = configurationService;
        _exerciseGenerator = exerciseGenerator;
    }

    public async Task<ServiceResult<Exercise>> Get(string exerciseId, CancellationToken cancellationToken = default)
    {
        var exercise = await _repository.Get(exerciseId, cancellationToken);

        if (exercise is null)
            return new()
            {
                Success = false,
                ErrorMessage = "Exercise not found."
            };

        return new()
        {
            Data = exercise,
            Success = true
        };
    }

    public async Task<ServiceResult<List<Exercise>>> GetAllByOwnerIdAndType(string ownerId, ExerciseType type, CancellationToken cancellationToken = default)
    {
        var exercises = await _repository.GetAllByOwnerIdAndType(ownerId, type, cancellationToken);
        if (!exercises.Any())
            return new()
            {
                Success = false,
                ErrorMessage = "No exercises found."
            };

        return new()
        {
            Data = exercises,
            Success = true
        };
    }

    public async Task<ServiceResult<string>> MakePendency(string exerciseId, CancellationToken cancellationToken = default)
    {
        var oldExercise = await _repository.Get(exerciseId, cancellationToken);

        if (oldExercise is null)
            return new()
            {
                Success = false,
                ErrorMessage = "Error to get old exercise",
            };

        var configurationGetResult = await _configurationService.Get(oldExercise.ConfigurationId, cancellationToken);

        if (!configurationGetResult.Success)
            return new()
            {
                Success = false,
                ErrorMessage = configurationGetResult.ErrorMessage
            };

        var makePendencyResult = await _exerciseGenerator.MakePendency(oldExercise.ContentId, oldExercise.Exercises.Serialize(), cancellationToken);

        if (!makePendencyResult.Success)
            return new()
            {
                Success = false,
                ErrorMessage = makePendencyResult.ErrorMessage
            };

        return new()
        {
            Success = true,
            Data = makePendencyResult.Data
        };
    }

    public async Task<ServiceResult<string>> MakeExercise(string contentId, CancellationToken cancellationToken = default)
    {
        var makeExerciseResult = await _exerciseGenerator.MakeExercise(contentId, cancellationToken);

        if (!makeExerciseResult.Success)
            return new()
            {
                Success = false,
                ErrorMessage = makeExerciseResult.ErrorMessage
            };

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
            Status = request.Status,
            Type = request.Type,
            TopicIndex = request.TopicIndex,
            Title = request.Title,
            Exercises = request.Exercises
        };

        var success = await _repository.Save(exercise, cancellationToken);

        return new()
        {
            Data = id,
            Success = success
        };
    }

    public async Task<ServiceResult<bool>> Update(string exerciseId, ExerciseRequest request, CancellationToken cancellationToken = default)
    {
        var exercise = await _repository.Get(exerciseId, cancellationToken);

        if (exercise == null)
            return new ServiceResult<bool>
            {
                Success = false,
                ErrorMessage = "Exercise not found."
            };

        var success = await _repository.Update(exerciseId, exercise, cancellationToken);

        return new()
        {
            Data = success,
            Success = success
        };
    }
}