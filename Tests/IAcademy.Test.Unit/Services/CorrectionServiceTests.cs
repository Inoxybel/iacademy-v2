using Domain.DTO.Correction;
using Domain.Infra;
using Moq;
using Service.Integrations.OpenAI.Interfaces;
using Service;
using FluentAssertions;
using Service.Integrations.OpenAI.DTO;
using IAcademy.Test.Shared.Builders;
using CrossCutting.Enums;
using Domain.Entities.Exercise;
using Domain.Entities.Feedback;
using Domain.Entities.Configuration;
using Domain.Services;

namespace IAcademy.Test.Unit.Services;

public class CorrectionServiceTests
{
    private readonly Mock<ICorrectionRepository> _mockCorrectionRepository;
    private readonly Mock<IExerciseRepository> _mockExerciseRepository;
    private readonly Mock<IConfigurationRepository> _mockConfigurationRepository;
    private readonly Mock<ISummaryService> _mockSummaryService;
    private readonly Mock<IOpenAIService> _mockOpenAIService;
    private readonly CorrectionService correctionService;

    public CorrectionServiceTests()
    {
        _mockCorrectionRepository = new();
        _mockExerciseRepository = new();
        _mockConfigurationRepository = new();
        _mockSummaryService = new();
        _mockOpenAIService = new();

        correctionService = new(
            _mockCorrectionRepository.Object,
            _mockExerciseRepository.Object,
            _mockConfigurationRepository.Object,
            _mockSummaryService.Object,
            _mockOpenAIService.Object
        );
    }

    [Fact]
    public async Task Get_SHOULD_ReturnFailResponse_WHEN_CorrectionNotFound()
    {
        var result = await correctionService.Get("someId", "ownerId");

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Correction not found.");
    }

    [Fact]
    public async Task Get_SHOULD_ReturnSuccessResponse_WHEN_CorrectionFound()
    {
        var expectedCorrection = new Correction();

        _mockCorrectionRepository.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCorrection);

        var result = await correctionService.Get("someId", "ownerId");

        result.Success.Should().BeTrue();
        result.Data.Should().Be(expectedCorrection);
    }

    [Fact]
    public async Task Update_SHOULD_ReturnFailResponse_WHEN_FailedToUpdate()
    {
        _mockCorrectionRepository.Setup(x => x.Update(It.IsAny<string>(), It.IsAny<Correction>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var request = new CorrectionUpdateRequest();

        var result = await correctionService.Update("someId", request);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Failed to update.");
    }

    [Fact]
    public async Task Update_SHOULD_ReturnSuccessResponse_WHEN_UpdateSucceeds()
    {
        _mockCorrectionRepository.Setup(x => x.Update(It.IsAny<string>(), It.IsAny<Correction>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var request = new CorrectionUpdateRequest();

        var result = await correctionService.Update("someId", request);

        result.Success.Should().BeTrue();
        result.Data.Should().BeTrue();
    }

    [Fact]
    public async Task MakeCorrection_SHOULD_ReturnFailResponse_WHEN_FailedToGetExercise()
    { 
        var request = new CreateCorrectionRequest();

        var result = await correctionService.MakeCorrection("exerciseId", "ownerId", request);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Failed to get exercise.");
    }

    [Fact]
    public async Task MakeCorrection_SHOULD_ReturnFailResponse_WHEN_FailedToUpdateExercise()
    {
        var exercise = new ExerciseBuilder()
            .WithOwnerId("ownerId")
            .Build();

        _mockExerciseRepository.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exercise);

        _mockExerciseRepository.Setup(x => x.Update(It.IsAny<string>(), It.IsAny<Exercise>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var request = new CreateCorrectionRequest()
        {
            Exercises = new List<ActivityToCorrectDTO>()
            {
                new ActivityToCorrectDTOBuilder().Build()
            }
        };

        var result = await correctionService.MakeCorrection("exerciseId", "ownerId", request);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Failed to update exercise.");
    }

    [Fact]
    public async Task MakeCorrection_SHOULD_ReturnFailResponse_WHEN_ExerciseStatus_Is_Not_Equal_WaitingToDo()
    {
        var exercise = new ExerciseBuilder()
            .WithOwnerId("ownerId")
            .WithStatus(ExerciseStatus.WaitingCorrection)
            .Build();

        _mockExerciseRepository.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exercise);

        var request = new CreateCorrectionRequest();

        var result = await correctionService.MakeCorrection(exercise.Id, "ownerId", request);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("There is already a correction process for this exercise.");
    }

    [Fact]
    public async Task MakeCorrection_SHOULD_ReturnFailResponse_WHEN_FailedToGetConfigurations()
    {
        var exercise = new ExerciseBuilder()
            .WithOwnerId("ownerId")
            .Build();

        _mockExerciseRepository.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exercise);

        _mockExerciseRepository.Setup(x => x.Update(It.IsAny<string>(), It.IsAny<Exercise>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var request = new CreateCorrectionRequest()
        {
            Exercises = new List<ActivityToCorrectDTO>()
            {
                new ActivityToCorrectDTOBuilder().Build()
            }
        };

        var result = await correctionService.MakeCorrection(exercise.Id, "ownerId", request);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Failed to get configurations.");
    }

    [Fact]
    public async Task MakeCorrection_SHOULD_ReturnFailResponse_WHEN_FailedToDeserializeIAResponse()
    {
        var exercise = new ExerciseBuilder()
            .WithOwnerId("ownerId")
            .Build();

        var configuration = new ConfigurationBuilder().Build();

        _mockExerciseRepository.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exercise);

        _mockExerciseRepository.Setup(x => x.Update(It.IsAny<string>(), It.IsAny<Exercise>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockConfigurationRepository.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(configuration);

        _mockOpenAIService.Setup(x => x.DoRequest(It.IsAny<InputProperties>(), It.IsAny<string>()))
            .ReturnsAsync(new OpenAIResponse 
            { 
                Id = string.Empty 
            });

        var request = new CreateCorrectionRequestBuilder().Build();

        var result = await correctionService.MakeCorrection(exercise.Id, "ownerId", request);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Failed to deserialize IA response");
    }

    [Fact]
    public async Task MakeCorrection_SHOULD_ReturnFailResponse_WHEN_FailedToSaveCorrection()
    {
        var exercise = new ExerciseBuilder()
            .WithOwnerId("ownerId")
            .Build();

        var configuration = new ConfigurationBuilder().Build();
        var openAIResponse = new OpenAIResponseBuilder()
            .WithChoices(new()
            {
                new ChoicesBuilder()
                    .WithMessage(new MessageBuilder()
                        .WithContent(
                            $"{{\"Id\":\"12345\",\"ExerciseId\":\"exercise123\",\"OwnerId\":\"owner789\",\"CreatedDate\":" +
                            $"\"2023-09-01T12:34:56.789Z\",\"UpdatedDate\":\"2023-09-01T13:34:56.789Z\",\"Corrections\":" +
                            $"[{{\"Identification\":1,\"Question\":\"What is the capital of France?\",\"Complementation\":" +
                            $"[\"Hint: It's known as the city of love.\"],\"Answer\":\"Paris\",\"IsCorrect\":true,\"Feedback\":" +
                            $"\"Correct answer!\"}}]}}"
                        )
                        .Build()
                    )
                    .Build()
            })
            .Build();

        _mockExerciseRepository.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exercise);

        _mockExerciseRepository.Setup(x => x.Update(It.IsAny<string>(), It.IsAny<Exercise>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockConfigurationRepository.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(configuration);

        _mockOpenAIService.Setup(x => x.DoRequest(It.IsAny<InputProperties>(), It.IsAny<string>()))
            .ReturnsAsync(openAIResponse);

        _mockCorrectionRepository.Setup(x => x.Save(It.IsAny<Correction>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var request = new CreateCorrectionRequestBuilder().Build();

        var result = await correctionService.MakeCorrection(exercise.Id, "ownerId", request);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Failed to save correction");
    }

    [Fact]
    public async Task MakeCorrection_SHOULD_ReturnFailResponse_WHEN_FailedToSavePendency()
    {
        var exercise = new ExerciseBuilder()
            .WithOwnerId("ownerId")
            .Build();

        var configuration = new ConfigurationBuilder().Build();
        var correction = new CorrectionBuilder()
            .WithCorrections(new()
            {
                new CorrectionItemBuilder()
                    .WithIsCorrect(false)
                    .Build()
            })
            .Build();
                
        var openAIResponse = new OpenAIResponseBuilder()
            .WithChoices(new()
            {
                new ChoicesBuilder()
                    .WithMessage(new MessageBuilder()
                        .WithContent(
                            $"{{\"Id\":\"12345\",\"ExerciseId\":\"exercise123\",\"OwnerId\":\"owner789\",\"CreatedDate\":" +
                            $"\"2023-09-01T12:34:56.789Z\",\"UpdatedDate\":\"2023-09-01T13:34:56.789Z\",\"Corrections\":" +
                            $"[{{\"Identification\":1,\"Question\":\"What is the capital of France?\",\"Complementation\":" +
                            $"[\"Hint: It's known as the city of love.\"],\"Answer\":\"Brasilandia\",\"IsCorrect\":false,\"Feedback\":" +
                            $"\"Correct answer!\"}}]}}"
                        )
                        .Build()
                    )
                    .Build()
            })
            .Build();

        _mockExerciseRepository.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exercise);

        _mockConfigurationRepository.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(configuration);

        _mockOpenAIService.Setup(x => x.DoRequest(It.IsAny<InputProperties>(), It.IsAny<string>()))
            .ReturnsAsync(openAIResponse);

        _mockExerciseRepository.Setup(x => x.Update(It.IsAny<string>(), It.IsAny<Exercise>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockCorrectionRepository.Setup(x => x.Save(It.IsAny<Correction>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockExerciseRepository.Setup(x => x.Save(It.IsAny<Exercise>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var request = new CreateCorrectionRequestBuilder().Build();

        var result = await correctionService.MakeCorrection(exercise.Id, "ownerId", request);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Failed to save pendency");
    }
}
