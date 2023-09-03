using CrossCutting.Enums;
using Domain.DTO;
using Domain.DTO.Exercise;
using Domain.Entities;
using Domain.Infra;
using Domain.Services;
using FluentAssertions;
using IAcademy.Test.Shared.Builders;
using Moq;
using Service;

namespace IAcademy.Test.Unit.Services
{
    public class ExerciseServiceTests
    {
        private readonly Mock<IExerciseRepository> _mockExerciseRepository;
        private readonly Mock<IConfigurationService> _mockConfigurationService;
        private readonly Mock<IGeneratorService> _mockGeneratorService;

        private readonly ExerciseService _exerciseService;

        public ExerciseServiceTests()
        {
            _mockExerciseRepository = new Mock<IExerciseRepository>();
            _mockConfigurationService = new Mock<IConfigurationService>();
            _mockGeneratorService = new Mock<IGeneratorService>();

            _exerciseService = new ExerciseService(
                _mockExerciseRepository.Object,
                _mockConfigurationService.Object,
                _mockGeneratorService.Object
            );
        }

        [Fact]
        public async Task Get_SHOULD_Return_Error_WHEN_ExerciseNotFound()
        {
            var result = await _exerciseService.Get("testExerciseId");

            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Exercise not found.");
        }

        [Fact]
        public async Task Get_SHOULD_Return_Exercise_WHEN_ExerciseExists()
        {
            var exercise = new ExerciseBuilder().Build();

            _mockExerciseRepository.Setup(m => m.Get(It.IsAny<string>(), default))
                .ReturnsAsync(exercise);

            var result = await _exerciseService.Get("testExerciseId");

            result.Success.Should().BeTrue();
            result.Data.Should().Be(exercise);
        }

        [Fact]
        public async Task Save_SHOULD_Return_Success_WithId_WHEN_SavedSuccessfully()
        {
            var request = new ExerciseRequest();
            _mockExerciseRepository.Setup(m => m.Save(It.IsAny<Exercise>(), default)).ReturnsAsync(true);

            var result = await _exerciseService.Save(request);

            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
        }

        [Fact]
        public async Task Save_SHOULD_Return_Failure_WHEN_NotSavedSuccessfully()
        {
            var request = new ExerciseRequest();
            _mockExerciseRepository.Setup(m => m.Save(It.IsAny<Exercise>(), default)).ReturnsAsync(false);

            var result = await _exerciseService.Save(request);

            result.Success.Should().BeFalse();
        }

        [Fact]
        public async Task Update_SHOULD_Return_Failure_WHEN_ExerciseNotFound()
        {
            var request = new ExerciseRequest();

            var result = await _exerciseService.Update("exerciseId", request);

            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Exercise not found.");
        }

        [Fact]
        public async Task Update_SHOULD_Return_Success_WHEN_UpdatedSuccessfully()
        {
            var existingExercise = new ExerciseBuilder().Build();
            var request = new ExerciseRequest();

            _mockExerciseRepository.Setup(m => m.Get(It.IsAny<string>(), default))
                .ReturnsAsync(existingExercise);

            _mockExerciseRepository.Setup(m => m.Update(It.IsAny<string>(), It.IsAny<Exercise>(), default))
                .ReturnsAsync(true);

            var result = await _exerciseService.Update("exerciseId", request);

            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task Update_SHOULD_Return_Failure_WHEN_NotUpdatedSuccessfully()
        {
            var existingExercise = new ExerciseBuilder().Build();
            var request = new ExerciseRequest();

            _mockExerciseRepository.Setup(m => m.Get(It.IsAny<string>(), default))
                .ReturnsAsync(existingExercise);

            _mockExerciseRepository.Setup(m => m.Update(It.IsAny<string>(), It.IsAny<Exercise>(), default))
                .ReturnsAsync(false);

            var result = await _exerciseService.Update("exerciseId", request);

            result.Success.Should().BeFalse();
        }

        [Fact]
        public async Task GetAllByOwnerIdAndType_SHOULD_Return_Error_WHEN_NoExercisesFound()
        {
            _mockExerciseRepository.Setup(m => m.GetAllByOwnerIdAndType(It.IsAny<string>(), It.IsAny<ExerciseType>(), default))
                .ReturnsAsync(new List<Exercise>());

            var result = await _exerciseService.GetAllByOwnerIdAndType("ownerId", ExerciseType.Default);

            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("No exercises found.");
        }

        [Fact]
        public async Task GetAllByOwnerIdAndType_SHOULD_Return_Success_WHEN_ExercisesFound()
        {
            _mockExerciseRepository.Setup(m => m.GetAllByOwnerIdAndType(It.IsAny<string>(), It.IsAny<ExerciseType>(), default))
                .ReturnsAsync(new List<Exercise>()
                {
                    new ExerciseBuilder().Build()
                });

            var result = await _exerciseService.GetAllByOwnerIdAndType("ownerId", ExerciseType.Default);

            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task MakeExercise_SHOULD_Return_Error_WHEN_ErrorToGetOldExercise()
        {
            _mockGeneratorService.Setup(g => g.MakeExercise(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResult<string>()
                {
                    Success = false,
                    ErrorMessage = "Error to get old exercise"
                });

            var result = await _exerciseService.MakeExercise("exerciseId");

            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Error to get old exercise");
        }

        [Fact]
        public async Task MakeExercise_SHOULD_Return_Success_WHEN_All_Steps_Succeed()
        {
            var oldExercise = new Exercise();
            var configuration = new Configuration();

            _mockExerciseRepository.Setup(m => m.Get(It.IsAny<string>(), default))
                .ReturnsAsync(oldExercise);

            _mockConfigurationService.Setup(m => m.Get(It.IsAny<string>(), default))
                .ReturnsAsync(new ServiceResult<Configuration>
                {
                    Data = configuration,
                    Success = true
                });

            _mockGeneratorService.Setup(m => m.MakeExercise(It.IsAny<string>(), default))
                .ReturnsAsync(new ServiceResult<string>
                {
                    Success = true,
                    Data = "exerciseId"
                });

            var result = await _exerciseService.MakeExercise("exerciseId");

            result.Success.Should().BeTrue();
            result.Data.Should().Be("exerciseId");
        }

        [Fact]
        public async Task MakeExercise_SHOULD_Return_Failure_WHEN_OldExerciseNotFound()
        {
            _mockGeneratorService.Setup(m => m.MakeExercise(It.IsAny<string>(), default))
                .ReturnsAsync(new ServiceResult<string>
                {
                    Success = false,
                    ErrorMessage = "Error to get old exercise"
                });

            var result = await _exerciseService.MakeExercise("exerciseId");

            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Error to get old exercise");
        }

        [Fact]
        public async Task MakeExercise_SHOULD_Return_Failure_WHEN_ConfigurationFetchFails()
        {
            var oldExercise = new ExerciseBuilder().Build();

            _mockExerciseRepository.Setup(m => m.Get(It.IsAny<string>(), default))
                .ReturnsAsync(oldExercise);

            _mockConfigurationService.Setup(m => m.Get(It.IsAny<string>(), default))
                .ReturnsAsync(new ServiceResult<Configuration>
                {
                    Success = false,
                    ErrorMessage = "Configuration fetch error."
                });

            _mockGeneratorService.Setup(m => m.MakeExercise(It.IsAny<string>(), default))
                .ReturnsAsync(new ServiceResult<string>
                {
                    Success = false,
                    ErrorMessage = "Configuration fetch error."
                });

            var result = await _exerciseService.MakeExercise("exerciseId");

            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Configuration fetch error.");
        }

        [Fact]
        public async Task MakeExercise_SHOULD_Return_Failure_WHEN_MakePendencyFails()
        {
            var oldExercise = new ExerciseBuilder().Build();
            var configuration = new ConfigurationBuilder().Build();

            _mockExerciseRepository.Setup(m => m.Get(It.IsAny<string>(), default))
                .ReturnsAsync(oldExercise);

            _mockConfigurationService.Setup(m => m.Get(It.IsAny<string>(), default))
                .ReturnsAsync(new ServiceResult<Configuration>
                {
                    Data = configuration,
                    Success = true
                });

            _mockGeneratorService.Setup(m => m.MakeExercise(It.IsAny<string>(), default))
                .ReturnsAsync(new ServiceResult<string>
                {
                    Success = false,
                    ErrorMessage = "MakeExercise error."
                });

            var result = await _exerciseService.MakeExercise("exerciseId");

            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("MakeExercise error.");
        }
    }
}
