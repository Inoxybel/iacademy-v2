using CrossCutting.Enums;
using CrossCutting.Extensions;
using Domain.DTO;
using Domain.DTO.Correction;
using Domain.Entities;
using Domain.Infra;
using Domain.Services;
using Newtonsoft.Json;
using Service.Integrations.OpenAI.DTO;
using Service.Integrations.OpenAI.Extensions;
using Service.Integrations.OpenAI.Interfaces;

namespace Service;

public class CorrectionService : ICorrectionService
{
    private readonly ICorrectionRepository _correctionRepository;
    private readonly IExerciseRepository _exerciseRepository;
    private readonly IConfigurationRepository _configurationRepository;
    private readonly IOpenAIService _openIAService;

    public CorrectionService(
        ICorrectionRepository correctionRepository,
        IExerciseRepository exerciseRepository,
        IConfigurationRepository configurationRepository,
        IOpenAIService openIAService)
    {
        _correctionRepository = correctionRepository;
        _exerciseRepository = exerciseRepository;
        _configurationRepository = configurationRepository;
        _openIAService = openIAService;
    }

    public async Task<ServiceResult<Correction>> Get(string correctionId, CancellationToken cancellationToken = default)
    {
        var correction = await _correctionRepository.Get(correctionId, cancellationToken);

        if (correction is null)
        {
            return new ServiceResult<Correction> { Success = false, ErrorMessage = "Correction not found." };
        }

        return new ServiceResult<Correction> { Data = correction, Success = true };
    }

    public async Task<ServiceResult<Correction>> MakeCorrection(string exerciseId, CreateCorrectionRequest request, CancellationToken cancellationToken = default)
    {
        var exerciseRecovered = await _exerciseRepository.Get(exerciseId, cancellationToken);

        if (exerciseRecovered is null)
            return GetFailResponse("Failed to get exercise.");

        exerciseRecovered.Status = ExerciseStatus.WaitingCorrection;

        var updateExerciseResult = await _exerciseRepository.Update(exerciseId, exerciseRecovered, cancellationToken);

        if (!updateExerciseResult)
            return GetFailResponse("Failed to update exercise.");

        var configurationRecovered = await _configurationRepository.Get(exerciseRecovered.ConfigurationId, cancellationToken);

        if (configurationRecovered is null)
            return GetFailResponse("Failed to get configurations.");

        var correction = await GetCorrectionFromOpenAI(request.Exercises, exerciseRecovered.Exercises, configurationRecovered.Correction);

        if (correction.Corrections is null || !correction.Corrections.Any())
            return GetFailResponse("Failed to deserialize IA response");

        correction = CompleteCorrectionAttributes(correction, exerciseRecovered);

        var correctionSaveResult = await _correctionRepository.Save(correction, cancellationToken);

        if (!correctionSaveResult)
            return GetFailResponse("Failed to save correction");

        var newExercise = UpdateExercise(correction.Id, exerciseRecovered);

        var exerciseUpdateResult = await _exerciseRepository.Update(exerciseId, newExercise, cancellationToken);

        if (!exerciseUpdateResult)
            return GetFailResponse("Failed to update exercise");

        var incorrectExercises = correction.Corrections.Where(c => c.IsCorrect == false).ToList();

        if (incorrectExercises.Any())
        {
            var oldExercises = incorrectExercises.Serialize();

            var pendencyExercise = await RequestPendencyExercicesToAI(oldExercises, configurationRecovered.Pendency);

            pendencyExercise.OwnerId = exerciseRecovered.OwnerId;
            pendencyExercise.ConfigurationId = exerciseRecovered.ConfigurationId;
            pendencyExercise.TopicIndex = exerciseRecovered.TopicIndex;

            var savePendencyResult = await _exerciseRepository.Save(pendencyExercise, cancellationToken);

            if (!savePendencyResult)
                return GetFailResponse("Failed to save pendency");
        }

        return new ServiceResult<Correction>
        {
            Data = correction,
            Success = true
        };
    }

    public async Task<ServiceResult<bool>> Update(string correctionId, CorrectionUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var correction = new Correction()
        {
            Id = correctionId,
            OwnerId = request.OwnerId,
            ExerciseId = request.ExerciseId,
            UpdatedDate = DateTime.UtcNow,
            Corrections = request.Corrections
        };

        var success = await _correctionRepository.Update(correctionId, correction, cancellationToken);

        if (!success)
            return new()
            {
                Success = false,
                ErrorMessage = "Failed to update."
            };

        return new()
        {
            Data = true,
            Success = true
        };
    }

    private async Task<Exercise> RequestPendencyExercicesToAI(string wrongExercisesStringfied, InputProperties configurations)
    {
        var requestBody = $"{configurations.InitialInput} {wrongExercisesStringfied} {configurations.FinalInput}";

        var openAIResponse = await _openIAService.DoRequest(requestBody);

        if (string.IsNullOrEmpty(openAIResponse.Id))
            return new Exercise();

        var exercise = openAIResponse.MapTopicsFromResponse<Exercise>();

        exercise.Id = Guid.NewGuid().ToString();
        exercise.Status = ExerciseStatus.WaitingToDo;
        exercise.Type = ExerciseType.Pendency;

        return exercise;
    }

    private static ServiceResult<Correction> GetFailResponse(string message) => new()
    {
        Success = false,
        ErrorMessage = message
    };

    private static Exercise UpdateExercise(string correctionId, Exercise oldExercise) => new()
    {
        Id = oldExercise.Id,
        OwnerId = oldExercise.OwnerId,
        CorrectionId = correctionId,
        ConfigurationId = oldExercise.ConfigurationId,
        SendedAt = DateTime.UtcNow,
        Type = oldExercise.Type,
        Status = ExerciseStatus.Finished,
        TopicIndex = oldExercise.TopicIndex,
        Title = oldExercise.Title,
        Exercises = oldExercise.Exercises
    };

    private async Task<Correction> GetCorrectionFromOpenAI(
        List<ActivityToCorrectDTO> answers,
        List<Activity> exercises,
        InputProperties configurations)
    {
        var exercisesJson = MakeCorrectionRequest(answers, exercises);

        var message = $"{configurations.InitialInput} {exercisesJson} {configurations.FinalInput} {new Correction().Serialize()}";

        var response = await _openIAService.DoRequest(message);

        if (string.IsNullOrEmpty(response.Id))
            return new Correction();

        var correction = MapCorrectionFromResponse(response);

        return correction;
    }

    private static string MakeCorrectionRequest(List<ActivityToCorrectDTO> answers, List<Activity> exercises)
    {
        exercises.ForEach(a => a.Answer = answers.FirstOrDefault(dto => dto.Identification == a.Identification)?.Answer ?? a.Answer);

        return exercises.Serialize();
    }

    private static Correction MapCorrectionFromResponse(OpenAIResponse response)
    {
        var completeResponse = response.Choices.First().Message.Content;

        var startIndex = completeResponse.IndexOf('{');
        var endIndex = completeResponse.LastIndexOf('}');

        if (startIndex != -1 && endIndex != -1)
        {
            var extractedJson = completeResponse.Substring(startIndex, endIndex - startIndex + 1);

            var objectFromResponse = JsonConvert.DeserializeObject<Correction>(extractedJson);

            if (objectFromResponse is not null)
                return objectFromResponse;
        }

        return new Correction();
    }

    private static Correction CompleteCorrectionAttributes(Correction correction, Exercise exerciseRecovered)
    {
        correction.Id = Guid.NewGuid().ToString();
        correction.ExerciseId = exerciseRecovered.Id;
        correction.OwnerId = exerciseRecovered.OwnerId;
        correction.CreatedDate = DateTime.UtcNow;
        correction.UpdatedDate = DateTime.UtcNow;

        return correction;
    }
}