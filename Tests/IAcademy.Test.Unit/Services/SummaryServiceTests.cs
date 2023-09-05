using Domain.DTO.Summary;
using Domain.DTO;
using Domain.Entities;
using Service;
using Domain.Infra;
using FluentAssertions;
using Moq;
using Domain.Services;
using Service.Integrations.OpenAI.Interfaces;
using IAcademy.Test.Shared.Builders;
using Service.Integrations.OpenAI.DTO;

namespace IAcademy.Test.Unit.Services
{
    public class SummaryServiceTests
    {
        private readonly SummaryService _summaryService;
        private readonly Mock<ISummaryRepository> _mockSummaryRepository;
        private readonly Mock<IConfigurationService> _mockConfigurationService;
        private readonly Mock<IOpenAIService> _mockOpenAIService;

        public SummaryServiceTests()
        {
            _mockSummaryRepository = new Mock<ISummaryRepository>();
            _mockConfigurationService = new Mock<IConfigurationService>();
            _mockOpenAIService = new Mock<IOpenAIService>();

            _summaryService = new SummaryService(
                _mockSummaryRepository.Object,
                _mockConfigurationService.Object,
                _mockOpenAIService.Object
            );
        }

        [Fact]
        public async Task Get_SHOULD_Return_Success_WHEN_SummaryExists()
        {
            var summaryId = "summaryId";
            var summary = new SummaryBuilder()
                .WithId(summaryId)
                .Build();

            _mockSummaryRepository.Setup(m => m.Get(summaryId, default))
                .ReturnsAsync(summary);

            var result = await _summaryService.Get(summaryId);

            result.Success.Should().BeTrue();
            result.Data.Should().Be(summary);
        }

        [Fact]
        public async Task Get_SHOULD_Return_Failure_WHEN_SummaryDoesNotExist()
        {
            var summaryId = "summaryId";

            var result = await _summaryService.Get(summaryId);

            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Summary not found.");
        }

        [Fact]
        public async Task GetAllByCategory_SHOULD_Return_Success_WHEN_SummariesExist()
        {
            var category = "testCategory";
            _mockSummaryRepository.Setup(m => m.GetAllByCategory(category, false, default))
                .ReturnsAsync(new List<Summary> 
                { 
                    new SummaryBuilder().Build() 
                });

            var result = await _summaryService.GetAllByCategory(category);

            result.Success.Should().BeTrue();
            result.Data.Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetAllByCategory_SHOULD_Return_Failure_WHEN_NoSummariesExist()
        {
            var category = "testCategory";

            _mockSummaryRepository.Setup(m => m.GetAllByCategory(category, false, default))
                .ReturnsAsync(new List<Summary>());

            var result = await _summaryService.GetAllByCategory(category);

            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Summary not found.");
        }

        [Fact]
        public async Task GetAllByCategoryAndSubcategory_SHOULD_Return_Success_WHEN_SummariesExist()
        {
            var category = "testCategory";
            var subcategory = "testSubcategory";
            _mockSummaryRepository.Setup(m => m.GetAllByCategoryAndSubcategory(category, subcategory, false, default))
                .ReturnsAsync(new List<Summary> 
                { 
                    new SummaryBuilder().Build() 
                });

            var result = await _summaryService.GetAllByCategoryAndSubcategory(category, subcategory);

            result.Success.Should().BeTrue();
            result.Data.Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetAllByCategoryAndSubcategory_SHOULD_Return_Failure_WHEN_NoSummariesExist()
        {
            var category = "testCategory";
            var subcategory = "testSubcategory";

            _mockSummaryRepository.Setup(m => m.GetAllByCategoryAndSubcategory(category, subcategory, false, default))
                .ReturnsAsync(new List<Summary>());

            var result = await _summaryService.GetAllByCategoryAndSubcategory(category, subcategory);

            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Summary not found.");
        }



        [Fact]
        public async Task RequestCreationToAI_SHOULD_Return_Failure_WHEN_ConfigurationNotFound()
        {
            var request = new SummaryCreationRequestBuilder().Build();

            _mockConfigurationService.Setup(m => m.Get(request.ConfigurationId, default))
                .ReturnsAsync(new ServiceResult<Configuration>
                {
                    Success = false
                });

            var result = await _summaryService.RequestCreationToAI(request);

            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Configuration not founded");
        }

        [Fact]
        public async Task GetAllByOwnerId_SHOULD_Return_Success_WHEN_SummariesExist()
        {
            var ownerId = "testOwnerId";
            _mockSummaryRepository.Setup(m => m.GetAllByOwnerId(ownerId, false, default))
                .ReturnsAsync(new List<Summary> 
                { 
                    new SummaryBuilder()
                        .WithOwnerId(ownerId)
                        .Build()
                });

            var result = await _summaryService.GetAllByOwnerId(ownerId);

            result.Success.Should().BeTrue();
            result.Data.Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetAllBySubcategory_SHOULD_Return_Success_WHEN_SummariesExist()
        {
            var subcategory = "testSubcategory";
            _mockSummaryRepository.Setup(m => m.GetAllBySubcategory(subcategory, false, default))
                .ReturnsAsync(new List<Summary> 
                { 
                    new SummaryBuilder()
                        .WithSubcategory(subcategory)
                        .Build() 
                });

            var result = await _summaryService.GetAllBySubcategory(subcategory);

            result.Success.Should().BeTrue();
            result.Data.Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetAllBySubcategory_SHOULD_Return_Failure_WHEN_NoSummariesExist()
        {
            var subcategory = "testSubcategory";
            _mockSummaryRepository.Setup(m => m.GetAllBySubcategory(subcategory, false, default))
                .ReturnsAsync(new List<Summary>());

            var result = await _summaryService.GetAllBySubcategory(subcategory);

            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Summary not found.");
        }


        [Fact]
        public async Task GetAllByOwnerId_SHOULD_Return_Failure_WHEN_NoSummariesExist()
        {
            var ownerId = "testOwnerId";
            _mockSummaryRepository.Setup(m => m.GetAllByOwnerId(ownerId, false, default))
                .ReturnsAsync(new List<Summary>());

            var result = await _summaryService.GetAllByOwnerId(ownerId);

            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Summary not found.");
        }

        [Fact]
        public async Task Save_SHOULD_Return_Success_WHEN_SummaryIsSaved()
        {
            var summaryId = "summaryId";
            var request = new SummaryRequest();
            _mockSummaryRepository.Setup(m => m.Save(It.IsAny<Summary>(), default))
                .ReturnsAsync(true);

            var result = await _summaryService.Save(request);

            result.Success.Should().BeTrue();
            result.Data.Count().Should().Be(36);
        }

        [Fact]
        public async Task Save_SHOULD_Return_Failure_WHEN_SummaryIsNotSaved()
        {
            var summaryId = "summaryId";
            var request = new SummaryRequest();

            _mockSummaryRepository.Setup(m => m.Save(It.IsAny<Summary>(), default))
                .ReturnsAsync(false);

            var result = await _summaryService.Save(request);

            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Failed to save.");
        }


        [Fact]
        public async Task Update_SHOULD_Return_Failure_WHEN_SummaryNotFound()
        {
            var summaryId = "summaryId";
            var request = new SummaryRequest();

            var result = await _summaryService.Update(summaryId, request);

            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Summary not found.");
        }

        [Fact]
        public async Task Update_SHOULD_Return_Failure_WHEN_SummaryNotUpdated()
        {
            var summaryId = "summaryId";
            var request = new SummaryRequest();

            _mockSummaryRepository.Setup(s => s.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SummaryBuilder().Build());

            _mockSummaryRepository.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<SummaryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await _summaryService.Update(summaryId, request);

            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Failed to update.");
        }

        [Fact]
        public async Task Update_SHOULD_Return_Success_WHEN_Summary_Update()
        {
            var summaryId = "summaryId";
            var request = new SummaryRequest();

            _mockSummaryRepository.Setup(s => s.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SummaryBuilder().Build());

            _mockSummaryRepository.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<SummaryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _summaryService.Update(summaryId, request);

            result.Success.Should().BeTrue();
            result.Data.Should().Be(summaryId);
        }

        [Fact]
        public async Task RequestCreationToAI_SHOULD_Return_Success_WHEN_AllConditionsAreMet()
        {
            var configuration = new ConfigurationBuilder().Build();

            var request = new SummaryCreationRequestBuilder()
                .WithConfigurationId(configuration.Id)
                .Build();

            _mockConfigurationService.Setup(m => m.Get(request.ConfigurationId, default)).ReturnsAsync(new ServiceResult<Configuration>
            {
                Success = true,
                Data = configuration
            });

            _mockOpenAIService.Setup(m => m.DoRequest(It.IsAny<string>()))
                .ReturnsAsync(new OpenAIResponseBuilder()
                    .WithChoices(new List<ChoicesDTO>()
                    {
                        new ChoicesDTOBuilder()
                            .WithMessage(new MessageDTOBuilder()
                                .WithContent(
                                    "{\"Topics\":[{\"Index\":\"1\",\"Title\":\"Linear Equations\",\"Description\":" +
                                    "\"Introduction to linear equations.\",\"Subtopics\":[{\"Index\":\"1.1\"," +
                                    "\"Title\":\"Basics of Linear Equations\"}]}]}"
                                )
                                .Build()
                             )
                            .Build()
                    })
                    .Build()
                );

            _mockSummaryRepository.Setup(m => m.Save(It.IsAny<Summary>(), default)).ReturnsAsync(true);

            var result = await _summaryService.RequestCreationToAI(request);

            result.Success.Should().BeTrue();
        }



    }
}
