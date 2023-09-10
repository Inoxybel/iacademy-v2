using Domain.DTO;
using Domain.Entities;
using Domain.Infra;
using Domain.Services;
using Moq;
using Service.Integrations.OpenAI.Interfaces;
using Service;
using FluentAssertions;
using Domain.DTO.Content;
using Service.Integrations.OpenAI.DTO;
using IAcademy.Test.Shared.Builders;

namespace IAcademy.Test.Unit.Services
{
    public class ExerciseGeneratorServiceTests
    {
        private readonly Mock<IConfigurationService> _mockConfigurationService;
        private readonly Mock<IExerciseRepository> _mockExerciseRepository;
        private readonly Mock<IContentRepository> _mockContentRepository;
        private readonly Mock<IOpenAIService> _mockOpenAIService;

        private readonly GeneratorService _exerciseGeneratorService;

        public ExerciseGeneratorServiceTests()
        {
            _mockConfigurationService = new Mock<IConfigurationService>();
            _mockExerciseRepository = new Mock<IExerciseRepository>();
            _mockContentRepository = new Mock<IContentRepository>();
            _mockOpenAIService = new Mock<IOpenAIService>();

            _exerciseGeneratorService = new GeneratorService(
                _mockConfigurationService.Object,
                _mockExerciseRepository.Object,
                _mockContentRepository.Object,
                _mockOpenAIService.Object
            );
        }

        [Fact]
        public async Task MakeExercise_SHOULD_Return_Success_WHEN_All_Steps_Succeed()
        {
            SetupDefaultMocks();

            var result = await _exerciseGeneratorService.MakeExercise("testContentId");

            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task MakeExercise_SHOULD_Return_Error_WHEN_ContentService_Fails()
        {
            SetupDefaultMocks();

            _mockContentRepository.Setup(m => m.Get(It.IsAny<string>(), default))
                .ReturnsAsync((Content)null);

            var result = await _exerciseGeneratorService.MakeExercise("testContentId");

            result.Success.Should().BeFalse();
        }

        [Fact]
        public async Task MakeExercise_SHOULD_Return_Error_WHEN_ConfigurationService_Fails()
        {
            SetupDefaultMocks();

            _mockConfigurationService.Setup(m => m.Get(It.IsAny<string>(), default))
                .ReturnsAsync(new ServiceResult<Configuration> 
                { 
                    Success = false
                });

            var result = await _exerciseGeneratorService.MakeExercise("testContentId");

            result.Success.Should().BeFalse();
        }

        [Fact]
        public async Task MakeExercise_SHOULD_Return_Error_WHEN_OpenAIService_Fails()
        {
            SetupDefaultMocks();

            var openAIResponse = new OpenAIResponse();

            _mockOpenAIService.Setup(m => m.DoRequest(It.IsAny<string>()))
                .ReturnsAsync(openAIResponse);

            var result = await _exerciseGeneratorService.MakeExercise("testContentId");

            result.Success.Should().BeFalse();
        }

        [Fact]
        public async Task MakeExercise_SHOULD_Return_Error_WHEN_Repository_Save_Fails()
        {
            SetupDefaultMocks();

            _mockExerciseRepository.Setup(m => m.Save(It.IsAny<Exercise>(), default))
                .ReturnsAsync(false);

            var result = await _exerciseGeneratorService.MakeExercise("testContentId");

            result.Success.Should().BeFalse();
        }

        [Fact]
        public async Task MakeExercise_SHOULD_Return_Error_WHEN_ContentService_Update_Fails()
        {
            SetupDefaultMocks();

            _mockContentRepository.Setup(m => m.Update(It.IsAny<string>(), It.IsAny<ContentRequest>(), default))
                .ReturnsAsync(false);

            var result = await _exerciseGeneratorService.MakeExercise("testContentId");

            result.Success.Should().BeFalse();
        }

        [Fact]
        public async Task MakePendency_SHOULD_Return_Success_WHEN_All_Steps_Succeed()
        {
            SetupDefaultMocks();

            var result = await _exerciseGeneratorService.MakePendency("testContentId", "oldExercise");

            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task MakePendency_SHOULD_Return_Error_WHEN_ContentService_Fails()
        {
            SetupDefaultMocks();

            _mockContentRepository.Setup(m => m.Get(It.IsAny<string>(), default))
                .ReturnsAsync((Content)null);

            var result = await _exerciseGeneratorService.MakePendency("testContentId", "oldExercise");

            result.Success.Should().BeFalse();
        }

        [Fact]
        public async Task MakePendency_SHOULD_Return_Error_WHEN_ConfigurationService_Fails()
        {
            SetupDefaultMocks();

            _mockConfigurationService.Setup(m => m.Get(It.IsAny<string>(), default))
                .ReturnsAsync(new ServiceResult<Configuration> 
                { 
                    Success = false 
                });

            var result = await _exerciseGeneratorService.MakePendency("testContentId", "oldExercise");

            result.Success.Should().BeFalse();
        }

        [Fact]
        public async Task MakePendency_SHOULD_Return_Error_WHEN_OpenAIService_Fails()
        {
            SetupDefaultMocks();

            var openAIResponse = new OpenAIResponse();

            _mockOpenAIService.Setup(m => m.DoRequest(It.IsAny<string>()))
                .ReturnsAsync(openAIResponse);

            var result = await _exerciseGeneratorService.MakePendency("testContentId", "oldExercise");

            result.Success.Should().BeFalse();
        }

        [Fact]
        public async Task MakePendency_SHOULD_Return_Error_WHEN_Repository_Save_Fails()
        {
            SetupDefaultMocks();

            _mockExerciseRepository.Setup(m => m.Save(It.IsAny<Exercise>(), default))
                .ReturnsAsync(false);

            var result = await _exerciseGeneratorService.MakePendency("testContentId", "oldExercise");

            result.Success.Should().BeFalse();
        }

        [Fact]
        public async Task MakePendency_SHOULD_Return_Error_WHEN_ContentService_Update_Fails()
        {
            SetupDefaultMocks();

            _mockContentRepository.Setup(m => m.Update(It.IsAny<string>(), It.IsAny<ContentRequest>(), default))
                .ReturnsAsync(false);

            var result = await _exerciseGeneratorService.MakePendency("testContentId", "oldExercise");

            result.Success.Should().BeFalse();
        }

        private void SetupDefaultMocks()
        {
            var content = new ContentBuilder().Build();
            var config = new ConfigurationBuilder().Build();
            var openAIResponse = new OpenAIResponseBuilder()
                .WithChoices(new List<Choices>()
                {
                    new ChoicesBuilder()
                    .WithMessage(new MessageBuilder()
                        .WithContent(
                            "[{\"Identification\":1,\"Type\":0,\"Question\":\"Which are the prime numbers?\"," +
                            "\"Complementation\":[\"1\",\"2\",\"3\",\"4\"],\"Answer\":\"2,3\"}," +
                            "{\"Identification\":2,\"Type\":2,\"Question\":\"Write a Hello World in C#.\"," +
                            "\"Complementation\":[],\"Answer\":\"Console.WriteLine(\\\"Hello World\\\");\"}]"
                         )
                        .Build()
                    )
                    .Build()
                })
                .Build();

            _mockContentRepository.Setup(m => m.Get(It.IsAny<string>(), default))
                .ReturnsAsync(new ContentBuilder().Build());

            _mockConfigurationService.Setup(m => m.Get(It.IsAny<string>(), default))
                .ReturnsAsync(new ServiceResult<Configuration>
                { 
                    Success = true, 
                    Data = config 
                });

            _mockOpenAIService.Setup(m => m.DoRequest(It.IsAny<string>()))
                .ReturnsAsync(openAIResponse);

            _mockExerciseRepository.Setup(m => m.Save(It.IsAny<Exercise>(), default))
                .ReturnsAsync(true);

            _mockContentRepository.Setup(m => m.Update(It.IsAny<string>(), It.IsAny<ContentRequest>(), default))
                .ReturnsAsync(true);
        }
    }
}
