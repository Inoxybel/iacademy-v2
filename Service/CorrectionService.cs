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
using System.Text;

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

        if (exerciseRecovered.Status != ExerciseStatus.WaitingToDo)
            return GetFailResponse("There is already a correction process for this exercise.");

        exerciseRecovered.Status = ExerciseStatus.WaitingCorrection;

        SaveAnswers(request.Exercises, exerciseRecovered.Exercises);

        var updateExerciseResult = await _exerciseRepository.Update(exerciseId, exerciseRecovered, cancellationToken);

        if (!updateExerciseResult)
            return GetFailResponse("Failed to update exercise.");

        var configurationRecovered = await _configurationRepository.Get(exerciseRecovered.ConfigurationId, cancellationToken);

        if (configurationRecovered is null)
            return GetFailResponse("Failed to get configurations.");

        var correction = await GetCorrectionFromOpenAI(exerciseRecovered.Exercises, configurationRecovered.Correction);

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
            var oldExercises = MakePendencyRequest(incorrectExercises);

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
        List<Activity> exercises,
        InputProperties configurations)
    {
        var exercisesJson = MakeCorrectionRequest(exercises);

        var message = $"{configurations.InitialInput} \n\n {exercisesJson} \n\n {configurations.FinalInput}";

        var response = await _openIAService.DoRequest(message);

        if (string.IsNullOrEmpty(response.Id))
            return new Correction();

        var correction = MapCorrectionFromResponse(response, exercises);

        return correction;
    }

    private static string MakeCorrectionRequest(List<Activity> exercises)
    {
        var resultBuilder = new StringBuilder();

        foreach (var exercise in exercises)
        {
            resultBuilder.AppendLine($"{exercise.Identification} - [{exercise.Type}] {exercise.Question}");

            if (exercise.Complementation.Any())
            {
                foreach (var complement in exercise.Complementation)
                {
                    resultBuilder.AppendLine(complement);
                }
            }

            resultBuilder.AppendLine($"Resposta informada: {exercise.Answer}");
        }

        return resultBuilder.ToString();
    }

    private static string MakePendencyRequest(List<CorrectionItem> corrections)
    {
        var resultBuilder = new StringBuilder();

        var count = 1;

        foreach (var correctionItem in corrections)
        {
            resultBuilder.AppendLine($"{count++} - {correctionItem.Question}");

            if (correctionItem.Complementation.Any())
            {
                foreach (var complement in correctionItem.Complementation)
                {
                    resultBuilder.AppendLine(complement);
                }
            }

            resultBuilder.AppendLine($"Resposta informada: {correctionItem.Answer}");
        }

        return resultBuilder.ToString();
    }

    private static void SaveAnswers(List<ActivityToCorrectDTO> answers, List<Activity> exercises)
    {
        exercises.ForEach(a => a.Answer = answers.FirstOrDefault(dto => dto.Identification == a.Identification)?.Answer ?? a.Answer);
    }

    private static Correction MapCorrectionFromResponse(OpenAIResponse response, List<Activity> exercises)
    {
        var completeResponse = response.Choices.First().Message.Content;

        var correction = JsonConvert.DeserializeObject<Correction>(completeResponse);

        if (correction is null)
            return new Correction();

        foreach (var correctionItem in correction.Corrections)
        {
            var matchingExercise = exercises.FirstOrDefault(e => e.Identification == correctionItem.Identification);

            if (matchingExercise != null)
            {
                correctionItem.Question = matchingExercise.Question;
                correctionItem.Complementation = matchingExercise.Complementation;
                correctionItem.Answer = matchingExercise.Answer;
            }
        }

        return correction;
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