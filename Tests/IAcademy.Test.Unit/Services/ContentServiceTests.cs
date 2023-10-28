using CrossCutting.Enums;
using Domain.DTO;
using Domain.DTO.Content;
using Domain.DTO.Summary;
using Domain.Entities.Chat;
using Domain.Entities.Companies;
using Domain.Entities.Configuration;
using Domain.Entities.Contents;
using Domain.Entities.Exercise;
using Domain.Entities.Summary;
using Domain.Infra;
using Domain.Services;
using FluentAssertions;
using IAcademy.Test.Shared.Builders;
using Moq;
using Service;
using Service.Integrations.OpenAI.DTO;
using Service.Integrations.OpenAI.Interfaces;

namespace IAcademy.Test.Unit.Services;
public class ContentServiceTests
{
    private readonly Mock<IContentRepository> _mockContentRepository = new();
    private readonly Mock<ISummaryService> _mockSummaryService = new();
    private readonly Mock<IGeneratorService> _mockExerciseGeneratorService = new();
    private readonly Mock<IExerciseService> _mockExerciseService = new();
    private readonly Mock<IOpenAIService> _mockOpenAIService = new();
    private readonly Mock<IConfigurationService> _mockConfigurationService = new();
    private readonly Mock<IChatCompletionsService> _mockChatCompletionsService = new();
    private readonly Mock<ICompanyService> _mockCompanyService = new();
    private readonly ContentService contentService;
    private readonly SummaryMatriculationRequest _validRequest;
    private readonly SummaryMatriculationRequest _invalidRequest;
    private readonly List<TextGenre> textGenres;

    public ContentServiceTests()
    {
        textGenres = new()
        {
            TextGenre.Informativo,
            TextGenre.Explicativo,
            TextGenre.Narrativo,
            TextGenre.Argumentativo
        };

        contentService = new(
            _mockContentRepository.Object,
            _mockSummaryService.Object,
            _mockExerciseGeneratorService.Object,
            _mockOpenAIService.Object,
            _mockConfigurationService.Object,
            _mockExerciseService.Object,
            _mockChatCompletionsService.Object,
            _mockCompanyService.Object
        );

        _validRequest = new SummaryMatriculationRequest
        {
            SummaryId = "id"
        };

        _invalidRequest = new SummaryMatriculationRequest
        {
            SummaryId = "Invalid Guid"
        };

        _mockChatCompletionsService.Setup(c => c.Save(It.IsAny<ChatCompletion>(), default))
            .ReturnsAsync(new ServiceResult<string>
            {
                Success = false
            });
    }

    [Fact]
    public async Task Get_SHOULD_Return_Expected_Content_WHEN_Content_Exists()
    {
        var contentId = "someId";
        var content = new ContentBuilder().WithId(contentId).Build();

        _mockContentRepository.Setup(r => r.Get(content.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(content);

        var result = await contentService.Get(contentId);

        result.Success.Should().BeTrue();
        result.Data.Id.Should().Be(contentId);
    }

    [Fact]
    public async Task Get_SHOULD_Return_Error_WHEN_Content_Does_Not_Exist()
    {
        var contentId = "someId";

        var result = await contentService.Get(contentId);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Content not found.");
    }

    [Fact]
    public async Task Save_SHOULD_Return_Expected_Id_WHEN_Save_Is_Successful()
    {
        var contentRequest = new ContentRequestBuilder().Build();
        var contentId = "generatedId";

        _mockContentRepository.Setup(r => r.Save(It.IsAny<Content>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentId);

        var result = await contentService.Save(contentRequest);

        result.Success.Should().BeTrue();
        result.Data.Should().Be(contentId);
    }

    [Fact]
    public async Task Save_SHOULD_Return_Error_WHEN_Save_Fails()
    {
        var contentRequest = new ContentRequest();

        _mockContentRepository.Setup(r => r.Save(It.IsAny<Content>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(string.Empty);

        var result = await contentService.Save(contentRequest);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Failed to save content.");
    }

    [Fact]
    public async Task SaveAll_SHOULD_Return_Expected_Ids_WHEN_SaveAll_Is_Successful()
    {
        var contentRequests = new List<ContentRequest>() 
        { 
            new ContentRequestBuilder().Build(), 
            new ContentRequestBuilder().Build()
        };

        var firstContent = new ContentBuilder().Build();
        var secondContent = new ContentBuilder().Build();

        var contents = new List<Content> 
        { 
            firstContent,
            secondContent
        };

        _mockContentRepository.Setup(r => r.SaveAll(It.IsAny<List<Content>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contents.Select(c => c.Id).ToList());

        var result = await contentService.SaveAll(contentRequests);

        result.Success.Should().BeTrue();
        result.Data.Should().Contain(firstContent.Id);
        result.Data.Should().Contain(secondContent.Id);
    }

    [Fact]
    public async Task SaveAll_SHOULD_Return_Error_WHEN_No_Contents_Sent()
    {
        var result = await contentService.SaveAll(new List<ContentRequest>());

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("No contents sended");
    }

    [Fact]
    public async Task SaveAll_SHOULD_Return_Error_WHEN_SaveAll_Fails()
    {
        var contentRequests = new List<ContentRequest>()
        {
            new ContentRequestBuilder().Build(),
            new ContentRequestBuilder().Build()
        };

        _mockContentRepository.Setup(r => r.SaveAll(It.IsAny<List<Content>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<string>());

        var result = await contentService.SaveAll(contentRequests);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Failed to save all contents.");
    }

    [Fact]
    public async Task GetAllBySummaryId_SHOULD_Return_Expected_Contents_WHEN_Contents_Exist()
    {
        var summaryId = "someSummaryId";

        var firstContent = new ContentBuilder()
            .WithSummaryId(summaryId)
            .Build();

        var secondContent = new ContentBuilder()
            .WithSummaryId(summaryId)
            .Build();

        var contents = new List<Content>
        {
            firstContent,
            secondContent
        };

        _mockContentRepository.Setup(r => r.GetAllBySummaryId(summaryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(contents);

        var result = await contentService.GetAllBySummaryId(summaryId);

        result.Success.Should().BeTrue();
        result.Data.Count.Should().Be(2);
    }

    [Fact]
    public async Task GetAllBySummaryId_SHOULD_Return_Error_WHEN_Contents_Do_Not_Exist()
    {
        var summaryId = "someSummaryId";

        _mockContentRepository.Setup(r => r.GetAllBySummaryId(summaryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Content>());

        var result = await contentService.GetAllBySummaryId(summaryId);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("No contents found.");
    }

    [Fact]
    public async Task Update_SHOULD_Return_True_WHEN_Update_Is_Successful()
    {
        var contentId = "contentId";
        var contentRequest = new ContentRequestBuilder().Build();

        _mockContentRepository.Setup(r => r.Update(contentId, contentRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await contentService.Update(contentId, contentRequest);

        result.Success.Should().BeTrue();
        result.Data.Should().BeTrue();
    }

    [Fact]
    public async Task Update_SHOULD_Return_Error_WHEN_Update_Fails()
    {
        var contentId = "contentId";
        var contentRequest = new ContentRequestBuilder().Build();

        _mockContentRepository.Setup(r => r.Update(contentId, contentRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await contentService.Update(contentId, contentRequest);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Failed to update content.");
    }

    [Fact]
    public async Task UpdateAll_SHOULD_Return_True_WHEN_UpdateAll_Is_Successful()
    {
        var summaryId = "someSummaryId";

        var firstContent = new ContentBuilder().Build();
        var secondContent = new ContentBuilder().Build();

        var contents = new List<Content>
        {
            firstContent,
            secondContent
        };

        _mockContentRepository.Setup(r => r.UpdateAll(summaryId, contents, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await contentService.UpdateAll(summaryId, contents);

        result.Success.Should().BeTrue();
        result.Data.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAll_SHOULD_Return_Error_WHEN_UpdateAll_Fails()
    {
        var summaryId = "someSummaryId";
        var firstContent = new ContentBuilder().WithSummaryId(summaryId).Build();
        var secondContent = new ContentBuilder().WithSummaryId(summaryId).Build();

        var contents = new List<Content>
        {
            firstContent,
            secondContent
        };

        _mockContentRepository.Setup(r => r.UpdateAll(summaryId, contents, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await contentService.UpdateAll(summaryId, contents);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain($"Failed to update contents for summary ID {summaryId}.");
    }

    [Fact]
    public async Task MakeContent_SHOULD_Return_Error_WHEN_Summary_Fails()
    {
        var summaryId = "someSummaryId";
        var summaryResponse = new ServiceResult<Summary>() 
        { 
            Success = false
        };

        var aIContentCreationRequest = new AIContentCreationRequest()
        {
            SubtopicIndex = "1.1"
        };

        _mockSummaryService.Setup(s => s.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(summaryResponse);

        var result = await contentService.MakeContent(summaryId, aIContentCreationRequest);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Error to find summary");
    }

    [Fact]
    public async Task MakeContent_SHOULD_Return_Error_WHEN_Configuration_Fails()
    {
        var summaryId = "someSummaryId";
        var summaryResponse = new ServiceResult<Summary>()
        {
            Success = true,
            Data = new SummaryBuilder()
                .WithId(summaryId)
                .Build()
        };

        var aIContentCreationRequest = new AIContentCreationRequest()
        {
            SubtopicIndex = "1.1"
        };

        var configurationResult = new ServiceResult<Configuration>()
        {
            Success = false
        };

        _mockSummaryService.Setup(s => s.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(summaryResponse);

        _mockConfigurationService.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(configurationResult);

        var result = await contentService.MakeContent(summaryId, aIContentCreationRequest);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Error to find configuration");
    }

    [Fact]
    public async Task MakeContent_SHOULD_Return_Error_WHEN_Dont_Find_Index()
    {
        var summaryId = "someSummaryId";
        var summaryResponse = new ServiceResult<Summary>()
        {
            Success = true,
            Data = new SummaryBuilder()
                .WithId(summaryId)
                .Build()
        };

        var aIContentCreationRequest = new AIContentCreationRequest()
        {
            SubtopicIndex = "0"
        };

        var configurationResult = new ServiceResult<Configuration>()
        {
            Success = true,
            Data = new ConfigurationBuilder().Build()
        };

        _mockSummaryService.Setup(s => s.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(summaryResponse);

        _mockConfigurationService.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(configurationResult);

        var result = await contentService.MakeContent(summaryId, aIContentCreationRequest);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Error to find index on topic");
    }

    [Fact]
    public async Task MakeContent_SHOULD_Return_Error_WHEN_OpenAI_Fails()
    {
        var summaryId = "someSummaryId";
        var summaryResponse = new ServiceResult<Summary>()
        {
            Success = true,
            Data = new SummaryBuilder()
                .WithId(summaryId)
                .Build()
        };

        var aIContentCreationRequest = new AIContentCreationRequest()
        {
            SubtopicIndex = "1.1"
        };

        var configurationResult = new ServiceResult<Configuration>()
        {
            Success = true,
            Data = new ConfigurationBuilder().Build()
        };

        var openAIResponse = new OpenAIResponse();

        var chatCompletionsReturn = new ServiceResult<ChatCompletion>()
        {
            Success = false
        };

        _mockSummaryService.Setup(s => s.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(summaryResponse);

        _mockConfigurationService.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(configurationResult);

        _mockChatCompletionsService.Setup(c => c.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(chatCompletionsReturn);

        _mockOpenAIService.Setup(o => o.DoRequest(It.IsAny<InputProperties>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(openAIResponse);

        var result = await contentService.MakeContent(summaryId, aIContentCreationRequest);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Error to get OpenAI content create response");
    }

    [Fact]
    public async Task MakeContent_SHOULD_Return_Error_WHEN_Error_To_Save_Content()
    {
        var summaryId = "someSummaryId";
        var summaryResponse = new ServiceResult<Summary>()
        {
            Success = true,
            Data = new SummaryBuilder()
                .WithId(summaryId)
                .Build()
        };

        var aIContentCreationRequest = new AIContentCreationRequest()
        {
            SubtopicIndex = "1.1"
        };

        var configurationResult = new ServiceResult<Configuration>()
        {
            Success = true,
            Data = new ConfigurationBuilder().Build()
        };

        var openAIResponse = new OpenAIResponseBuilder()
            .WithChoices(new List<Choices>()
            {
                new ChoicesBuilder()
                .WithMessage(new MessageBuilder()
                    .WithContent(
                        "{\"Subtopic\":{\"Index\":\"1.1\",\"Title\":\"Introduction\",\"Content\":[\"content\"]}}"
                     )
                    .Build()
                )
                .Build()
            })
            .Build();

        var chatCompletionsReturn = new ServiceResult<ChatCompletion>()
        {
            Success = false
        };

        _mockSummaryService.Setup(s => s.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(summaryResponse);

        _mockConfigurationService.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(configurationResult);

        _mockChatCompletionsService.Setup(c => c.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(chatCompletionsReturn);

        _mockOpenAIService.Setup(o => o.DoRequest(It.IsAny<InputProperties>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(openAIResponse);

        var result = await contentService.MakeContent(summaryId, aIContentCreationRequest);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Error to save content");
    }

    [Fact]
    public async Task MakeContent_SHOULD_Return_Error_WHEN_Error_To_Make_Exercise()
    {
        var summaryId = "someSummaryId";
        var summaryResponse = new ServiceResult<Summary>()
        {
            Success = true,
            Data = new SummaryBuilder()
                .WithId(summaryId)
                .Build()
        };

        var aIContentCreationRequest = new AIContentCreationRequest()
        {
            SubtopicIndex = "1.1"
        };

        var configurationResult = new ServiceResult<Configuration>()
        {
            Success = true,
            Data = new ConfigurationBuilder().Build()
        };

        var openAIResponse = new OpenAIResponseBuilder()
            .WithChoices(new List<Choices>()
            {
                new ChoicesBuilder()
                .WithMessage(new MessageBuilder()
                    .WithContent(
                        "{\"Subtopic\":{\"Index\":\"1.1\",\"Title\":\"Introduction\",\"Content\":[\"content\"]}}"
                     )
                    .Build()
                )
                .Build()
            })
            .Build();

        var contentId = "contentId";

        var makeExerciseResult = new ServiceResult<string>()
        {
            Success = false
        };

        var chatCompletionsReturn = new ServiceResult<ChatCompletion>()
        {
            Success = false
        };

        _mockSummaryService.Setup(s => s.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(summaryResponse);

        _mockConfigurationService.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(configurationResult);

        _mockChatCompletionsService.Setup(c => c.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(chatCompletionsReturn);

        _mockOpenAIService.Setup(o => o.DoRequest(It.IsAny<InputProperties>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(openAIResponse);

        _mockContentRepository.Setup(r => r.Save(It.IsAny<Content>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentId);

        _mockExerciseGeneratorService.Setup(e => e.MakeExercise(It.IsAny<Content>(), It.IsAny<InputProperties>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(makeExerciseResult);

        var result = await contentService.MakeContent(summaryId, aIContentCreationRequest);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Error to make exercise");
    }

    [Fact]
    public async Task MakeContent_SHOULD_Return_Error_WHEN_Update_Summary_Fails()
    {
        var summaryId = "someSummaryId";
        var summaryResponse = new ServiceResult<Summary>()
        {
            Success = true,
            Data = new SummaryBuilder()
                .WithId(summaryId)
                .Build()
        };

        var aIContentCreationRequest = new AIContentCreationRequest()
        {
            SubtopicIndex = "1.1"
        };

        var configurationResult = new ServiceResult<Configuration>()
        {
            Success = true,
            Data = new ConfigurationBuilder().Build()
        };

        var openAIResponse = new OpenAIResponseBuilder()
            .WithChoices(new List<Choices>()
            {
                new ChoicesBuilder()
                .WithMessage(new MessageBuilder()
                    .WithContent(
                        "{\"Subtopic\":{\"Index\":\"1.1\",\"Title\":\"Introduction\",\"Content\":[\"content\"]}}"
                     )
                    .Build()
                )
                .Build()
            })
            .Build();

        var makeExerciseResult = new ServiceResult<string>()
        {
            Success = true,
            Data = "exerciseId"
        };

        var contentId = "someContentId";

        var updateSummaryResult = new ServiceResult<string>()
        {
            Success = false
        };

        var chatCompletionsReturn = new ServiceResult<ChatCompletion>()
        {
            Success = false
        };

        _mockSummaryService.Setup(s => s.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(summaryResponse);

        _mockConfigurationService.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(configurationResult);

        _mockChatCompletionsService.Setup(c => c.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(chatCompletionsReturn);

        _mockOpenAIService.Setup(o => o.DoRequest(It.IsAny<InputProperties>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(openAIResponse);

        _mockExerciseGeneratorService.Setup(e => e.MakeExercise(It.IsAny<Content>(), It.IsAny<InputProperties>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(makeExerciseResult);

        _mockContentRepository.Setup(r => r.Save(It.IsAny<Content>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentId);

        _mockSummaryService.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<SummaryRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(updateSummaryResult);

        var result = await contentService.MakeContent(summaryId, aIContentCreationRequest);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Error to update summary");
    }

    [Fact]
    public async Task MakeContent_SHOULD_Return_ContentIds_WHEN_Successful()
    {
        var summaryId = "someSummaryId";
        var summaryResponse = new ServiceResult<Summary>()
        {
            Success = true,
            Data = new SummaryBuilder()
                .WithId(summaryId)
                .Build()
        };

        var aIContentCreationRequest = new AIContentCreationRequest()
        {
            SubtopicIndex = "1.1"
        };

        var configurationResult = new ServiceResult<Configuration>()
        {
            Success = true,
            Data = new ConfigurationBuilder().Build()
        };

        var openAIResponse = new OpenAIResponseBuilder()
            .WithChoices(new List<Choices>()
            {
                new ChoicesBuilder()
                .WithMessage(new MessageBuilder()
                    .WithContent(
                       "{\"Subtopic\":{\"Index\":\"1.1\",\"Title\":\"Introduction\",\"Content\":[\"content\"]}}"
                     )
                    .Build()
                )
                .Build()
            })
            .Build();

        var makeExerciseResult = new ServiceResult<string>()
        {
            Success = true,
            Data = "exerciseId"
        };

        var contentId = "someContentId";

        var updateSummaryResult = new ServiceResult<string>()
        {
            Success = true,
            Data = summaryId
        };

        var chatCompletionsReturn = new ServiceResult<ChatCompletion>()
        {
            Success = false
        };

        _mockSummaryService.Setup(s => s.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(summaryResponse);

        _mockConfigurationService.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(configurationResult);

        _mockOpenAIService.Setup(o => o.DoRequest(It.IsAny<InputProperties>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(openAIResponse);

        _mockExerciseGeneratorService.Setup(e => e.MakeExercise(It.IsAny<Content>(), It.IsAny<InputProperties>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(makeExerciseResult);

        _mockContentRepository.Setup(r => r.Save(It.IsAny<Content>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentId);

        _mockSummaryService.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<SummaryRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(updateSummaryResult);

        _mockChatCompletionsService.Setup(c => c.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(chatCompletionsReturn);

        var result = await contentService.MakeContent(summaryId, aIContentCreationRequest);

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNullOrEmpty();
        result.Data.Length.Should().Be(36);
    }

    [Fact]
    public async Task MakeContent_SHOULD_Execute_DoRequest_With_ChatCompletions_WHEN_Chat_Was_Founded()
    {
        var summaryId = "someSummaryId";
        var summaryResponse = new ServiceResult<Summary>()
        {
            Success = true,
            Data = new SummaryBuilder()
                .WithId(summaryId)
                .Build()
        };

        var aIContentCreationRequest = new AIContentCreationRequest()
        {
            SubtopicIndex = "1.1"
        };

        var configurationResult = new ServiceResult<Configuration>()
        {
            Success = true,
            Data = new ConfigurationBuilder().Build()
        };

        var openAIResponse = new OpenAIResponseBuilder()
            .WithChoices(new List<Choices>()
            {
                new ChoicesBuilder()
                .WithMessage(new MessageBuilder()
                    .WithContent(
                        "{\"Subtopic\":{\"Index\":\"1.1\",\"Title\":\"Introduction\",\"Content\":[\"content\"]}}"
                     )
                    .Build()
                )
                .Build()
            })
            .Build();

        var makeExerciseResult = new ServiceResult<string>()
        {
            Success = true,
            Data = "exerciseId"
        };

        var contentId = "someContentId";

        var updateSummaryResult = new ServiceResult<string>()
        {
            Success = true,
            Data = summaryId
        };

        var chatCompletionsReturn = new ServiceResult<ChatCompletion>()
        {
            Success = true,
            Data = new ChatCompletionsBuilder().Build()
        };

        _mockSummaryService.Setup(s => s.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(summaryResponse);

        _mockConfigurationService.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(configurationResult);

        _mockOpenAIService.Setup(o => o.DoRequest(It.IsAny<ChatCompletion>(), It.IsAny<InputProperties>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(openAIResponse);

        _mockExerciseGeneratorService.Setup(e => e.MakeExercise(It.IsAny<Content>(), It.IsAny<InputProperties>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(makeExerciseResult);

        _mockContentRepository.Setup(r => r.Save(It.IsAny<Content>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentId);

        _mockSummaryService.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<SummaryRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(updateSummaryResult);

        _mockChatCompletionsService.Setup(c => c.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(chatCompletionsReturn);

        var result = await contentService.MakeContent(summaryId, aIContentCreationRequest);

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNullOrEmpty();
        result.Data.Length.Should().Be(36);

        _mockOpenAIService.Verify(o => o.DoRequest(It.IsAny<InputProperties>(), It.IsAny<string>(), It.IsAny<string>()),Times.Never);
        _mockOpenAIService.Verify(o => o.DoRequest(It.IsAny<ChatCompletion>(), It.IsAny<InputProperties>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task MakeContent_SHOULD_Execute_DoRequest_Without_ChatCompletions_WHEN_Chat_Wasnt_Founded()
    {
        var summaryId = "someSummaryId";
        var summaryResponse = new ServiceResult<Summary>()
        {
            Success = true,
            Data = new SummaryBuilder()
                .WithId(summaryId)
                .Build()
        };

        var aIContentCreationRequest = new AIContentCreationRequest()
        {
            SubtopicIndex = "1.1"
        };

        var configurationResult = new ServiceResult<Configuration>()
        {
            Success = true,
            Data = new ConfigurationBuilder().Build()
        };

        var openAIResponse = new OpenAIResponseBuilder()
            .WithChoices(new List<Choices>()
            {
                new ChoicesBuilder()
                .WithMessage(new MessageBuilder()
                    .WithContent(
                        "{\"Subtopic\":{\"Index\":\"1.1\",\"Title\":\"Introduction\",\"Content\":[\"content\"]}}"
                     )
                    .Build()
                )
                .Build()
            })
            .Build();

        var makeExerciseResult = new ServiceResult<string>()
        {
            Success = true,
            Data = "exerciseId"
        };

        var contentId = "someContentId";

        var updateSummaryResult = new ServiceResult<string>()
        {
            Success = true,
            Data = summaryId
        };

        var chatCompletionsReturn = new ServiceResult<ChatCompletion>()
        {
            Success = false,
            ErrorMessage = "Error"
        };

        _mockSummaryService.Setup(s => s.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(summaryResponse);

        _mockConfigurationService.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(configurationResult);

        _mockOpenAIService.Setup(o => o.DoRequest(It.IsAny<InputProperties>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(openAIResponse);

        _mockExerciseGeneratorService.Setup(e => e.MakeExercise(It.IsAny<Content>(), It.IsAny<InputProperties>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(makeExerciseResult);

        _mockContentRepository.Setup(r => r.Save(It.IsAny<Content>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentId);

        _mockSummaryService.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<SummaryRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(updateSummaryResult);

        _mockChatCompletionsService.Setup(c => c.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(chatCompletionsReturn);

        var result = await contentService.MakeContent(summaryId, aIContentCreationRequest);

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNullOrEmpty();
        result.Data.Length.Should().Be(36);

        _mockOpenAIService.Verify(o => o.DoRequest(It.IsAny<InputProperties>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _mockOpenAIService.Verify(o => o.DoRequest(It.IsAny<ChatCompletion>(), It.IsAny<InputProperties>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task MakeAlternativeContent_SHOULD_Return_ContentId_WHEN_Successful()
    {
        var contentId = "someContentId";

        var subcontentRecreationRequest = new SubcontentRecreationRequest()
        {
            SubcontentIndex = 0
        };

        var contentResponse = new ContentBuilder()
            .WithId(contentId)
            .WithOriginId("iacademy")
            .Build();

        var masterContentResponse = new ContentBuilder()
            .WithId("iacademy")
            .Build();

        var openAIResponse = new OpenAIResponseBuilder()
            .WithChoices(new List<Choices>()
            {
                new ChoicesBuilder()
                .WithMessage(new MessageBuilder()
                    .WithContent(
                        "{\"NewContent\":\"This is new  body content\"}"
                     )
                    .Build()
                )
                .Build()
            })
            .Build();

        var configurationResult = new ServiceResult<Configuration>()
        {
            Success = true,
            Data = new ConfigurationBuilder().Build()
        };

        _mockContentRepository.SetupSequence(r => r.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentResponse)
            .ReturnsAsync(masterContentResponse);

        _mockConfigurationService.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(configurationResult);

        _mockOpenAIService.Setup(o => o.DoRequest(It.IsAny<InputProperties>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(openAIResponse);

        _mockContentRepository.Setup(r => r.SaveAll(It.IsAny<List<Content>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<string>() { contentId, "iacademy" });

        var result = await contentService.MakeAlternativeContent(contentId, subcontentRecreationRequest, textGenres);

        result.Success.Should().BeTrue();
        result.Data.Should().Be(contentId);
    }

    [Fact]
    public async Task MakeAlternativeContent_SHOULD_Return_Error_WHEN_Content_Does_Not_Exist()
    {
        var subcontentRecreationRequest = new SubcontentRecreationRequest()
        {
            SubcontentIndex = 0
        };

        var contentId = "someContentId";

        var result = await contentService.MakeAlternativeContent(contentId, subcontentRecreationRequest, textGenres);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Content not found");
    }

    [Fact]
    public async Task MakeAlternativeContent_SHOULD_Return_Error_WHEN_OpenAI_Fails()
    {
        var subcontentRecreationRequest = new SubcontentRecreationRequest()
        {
            SubcontentIndex = 0
        };

        var contentId = "someContentId";

        var masterContentResponse = new ContentBuilder()
            .WithId("iacademy")
            .Build();

        var contentResponse = new ContentBuilder()
            .WithId(contentId)
            .WithOriginId("iacademy")
            .Build();

        var configurationResult = new ServiceResult<Configuration>()
        {
            Success = true,
            Data = new ConfigurationBuilder().Build()
        };

        var openAIResponse = new OpenAIResponse();

        _mockContentRepository.SetupSequence(r => r.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentResponse)
            .ReturnsAsync(masterContentResponse);

        _mockConfigurationService.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(configurationResult);

        _mockOpenAIService.Setup(o => o.DoRequest(It.IsAny<InputProperties>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(openAIResponse);

        var result = await contentService.MakeAlternativeContent(contentId, subcontentRecreationRequest, textGenres);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Error to get OpenAI response");
    }

    [Fact]
    public async Task CopyContentsToEnrollUser_SHOULD_ReturnError_WHEN_SummaryNotFound()
    {
        _mockSummaryService.Setup(s => s.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ServiceResult<Summary> { Success = false });

        var result = await contentService.CopyContentsToEnrollUser(_validRequest, "ownerId", "iacademy", "document");

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Summary not found.");
    }

    [Fact]
    public async Task CopyContentsToEnrollUser_SHOULD_ReturnError_WHEN_SummaryNotMaster()
    {
        _mockSummaryService.Setup(s => s.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ServiceResult<Summary>
            {
                Success = true,
                Data = new Summary 
                {
                    Id = "id",
                    OwnerId = "some_other_id" 
                }
            });

        MockCompanyService();

        var result = await contentService.CopyContentsToEnrollUser(_validRequest, "ownerId", "iacademy", "document");

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("This summary is not master.");
    }

    [Fact]
    public async Task CopyContentsToEnrollUser_SHOULD_ReturnError_WHEN_NoContentIdsFound()
    {
        var summary = new Summary
        {
            Id = "id",
            OwnerId = "iacademy",
            Topics = new List<Topic>
            {
                new Topic 
                { 
                    Subtopics = new List<Subtopic> 
                    { 
                        new Subtopic 
                        { 
                            ContentId = null 
                        } 
                    } 
                }
            }
        };

        MockCompanyService();

        _mockSummaryService.Setup(s => s.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ServiceResult<Summary> { Success = true, Data = summary });

        var result = await contentService.CopyContentsToEnrollUser(_validRequest, "ownerId", "companyRef", "document");

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Error to get content ids.");
    }

    [Fact]
    public async Task CopyContentsToEnrollUser_SHOULD_ReturnError_WHEN_FailedToGetContents()
    {
        var summary = new Summary
        {
            Id = "id",
            OwnerId = "iacademy",
            Topics = new List<Topic>
            {
                new Topic 
                { 
                    Subtopics = new List<Subtopic> 
                    { 
                        new Subtopic 
                        { 
                            ContentId = "valid_content_id" 
                        } 
                    } 
                }
            }
        };

        MockCompanyService();

        _mockSummaryService.Setup(s => s.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ServiceResult<Summary> { Success = true, Data = summary });

        _mockContentRepository.Setup(r => r.GetAllByIds(It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Content>());

        var result = await contentService.CopyContentsToEnrollUser(_validRequest, "ownerId", "iacademy", "document");

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Error to get contents.");
    }

    [Fact]
    public async Task CopyContentsToEnrollUser_SHOULD_ReturnError_WHEN_FailedToGetExerciseIds()
    {
        var summary = new SummaryBuilder()
            .WithOwnerId("iacademy")
            .WithId("id")
            .Build();

        var contents = new List<Content>()
        {
            new ContentBuilder()
                .WithExerciseId(null)
                .Build()
        };

        MockCompanyService();

        _mockSummaryService.Setup(s => s.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ServiceResult<Summary> { Success = true, Data = summary });

        _mockContentRepository.Setup(r => r.GetAllByIds(It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contents);

        var result = await contentService.CopyContentsToEnrollUser(_validRequest, "ownerId", "iacademy", "document");

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Error to get exercise ids.");
    }

    [Fact]
    public async Task CopyContentsToEnrollUser_SHOULD_ReturnError_WHEN_FailedToGetExercises()
    {
        var summary = new SummaryBuilder()
            .WithOwnerId("iacademy")
            .WithId("id")
            .Build();

        var contents = new List<Content>()
        {
            new ContentBuilder().Build()
        };

        MockCompanyService();

        _mockSummaryService.Setup(s => s.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ServiceResult<Summary> { Success = true, Data = summary });

        _mockContentRepository.Setup(r => r.GetAllByIds(It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contents);

        _mockExerciseService.Setup(e => e.GetAllByIds(It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ServiceResult<List<Exercise>> { Success = false });

        var result = await contentService.CopyContentsToEnrollUser(_validRequest, "ownerId", "iacademy", "document");

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Error to get exercises.");
    }

    [Fact]
    public async Task CopyContentsToEnrollUser_SHOULD_ReturnError_WHEN_FailedToSaveNewContents()
    {
        var summary = new SummaryBuilder()
            .WithOwnerId("iacademy")
            .WithId("id")
            .Build();

        var contents = new List<Content>()
        {
            new ContentBuilder()
                .WithId("DefaultContentId")
                .WithOwnerId("iacademy")
                .Build()
        };

        var exercises = new List<Exercise>()
        {
            new ExerciseBuilder()
                .WithId("DefaultExerciseId")
                .Build()
        };

        var exerciseGetAllByIdsResponse = new ServiceResult<List<Exercise>>
        {
            Success = true,
            Data = exercises
        };

        SetupCommonMocks(summary, contents, new List<Exercise>());

        _mockContentRepository.Setup(r => r.GetAllByIds(It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contents);

        _mockExerciseService.Setup(e => e.GetAllByIds(It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exerciseGetAllByIdsResponse);

        _mockContentRepository.Setup(r => r.SaveAll(It.IsAny<List<Content>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<string>());

        var result = await contentService.CopyContentsToEnrollUser(_validRequest, "ownerId", "iacademy", "document");

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Error to save new contents.");
    }

    [Fact]
    public async Task CopyContentsToEnrollUser_SHOULD_ReturnError_WHEN_FailedToSaveNewExercises()
    {
        var summary = new SummaryBuilder()
            .WithOwnerId("iacademy")
            .WithId("id")
            .Build();

        var contents = new List<Content>()
        {
            new ContentBuilder()
                .WithId("DefaultContentId")
                .WithOwnerId("iacademy")
                .Build()
        };

        var exercises = new List<Exercise>()
        {
            new ExerciseBuilder()
                .WithId("DefaultExerciseId")
                .Build()
        };

        var exerciseGetAllByIdsResponse = new ServiceResult<List<Exercise>>
        {
            Success = true,
            Data = exercises
        };

        var saveAllResponse = new ServiceResult<List<string>>
        {
            Success = true,
            Data = new List<string>()
        };

        var contentSaveAllResponse = new List<string>()
        {
            "id"
        };

        var exerciseSaveAllResponse = new ServiceResult<List<string>>
        {
            Success = false
        };

        SetupCommonMocks(summary, contents, exercises);

        _mockContentRepository.Setup(r => r.GetAllByIds(It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contents);

        _mockExerciseService.Setup(e => e.GetAllByIds(It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exerciseGetAllByIdsResponse);

        _mockContentRepository.Setup(r => r.SaveAll(It.IsAny<List<Content>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentSaveAllResponse);

        _mockExerciseService.Setup(e => e.SaveAll(It.IsAny<List<Exercise>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exerciseSaveAllResponse);

        var result = await contentService.CopyContentsToEnrollUser(_validRequest, "ownerId", "iacademy", "document");

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Error to save new exercises.");
    }

    [Fact]
    public async Task CopyContentsToEnrollUser_SHOULD_ReturnError_WHEN_FailedToGetCompany()
    {
        _mockSummaryService.Setup(s => s.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ServiceResult<Summary>
            {
                Success = true,
                Data = new Summary()
            });

        var result = await contentService.CopyContentsToEnrollUser(_validRequest, "ownerId", "iacademy", "document");

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Company not found.");
    }

    [Fact]
    public async Task CopyContentsToEnrollUser_SHOULD_ReturnError_WHEN_FailedToEnrollUser()
    {
        var summary = new SummaryBuilder()
            .WithOwnerId("iacademy")
            .WithId("id")
            .Build();

        var contents = new List<Content>()
        {
            new ContentBuilder()
                .WithId("DefaultContentId")
                .WithOwnerId("iacademy")
                .Build()
        };

        var exercises = new List<Exercise>()
        {
            new ExerciseBuilder()
                .WithId("DefaultExerciseId")
                .Build()
        };

        var saveResponse = new ServiceResult<string>
        {
            Success = false
        };

        var exerciseSaveAllResponse = new ServiceResult<List<string>>
        {
            Success = true,
            Data = new List<string>()
            {
                "id"
            }
        };

        var contentSaveAllResponse = new List<string>()
        {
            "id"
        };

        SetupCommonMocks(summary, contents, exercises);

        _mockSummaryService.Setup(s => s.Save(It.IsAny<SummaryRequest>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(saveResponse);

        _mockExerciseService.Setup(e => e.SaveAll(It.IsAny<List<Exercise>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exerciseSaveAllResponse);

        _mockContentRepository.Setup(r => r.SaveAll(It.IsAny<List<Content>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentSaveAllResponse);

        var result = await contentService.CopyContentsToEnrollUser(_validRequest, "ownerId", "iacademy", "document");

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Fail to enroll user, try again.");
    }

    [Fact]
    public async Task CopyContentsToEnrollUser_SHOULD_ReturnSuccess_WHEN_AllConditionsMet()
    {
        var summary = new SummaryBuilder()
            .WithOwnerId("iacademy")
            .WithId("id")
            .Build();

        var contents = new List<Content>()
        {
            new ContentBuilder()
                .WithId("DefaultContentId")
                .WithOwnerId("OwnerId")
                .Build()
        };

        var exercises = new List<Exercise>()
        {
            new ExerciseBuilder()
                .WithId("DefaultExerciseId")
                .Build()
        };

        var saveResponse = new ServiceResult<string>
        {
            Success = true,
            Data = "NewSummaryId"
        };

        var exerciseSaveAllResponse = new ServiceResult<List<string>>
        {
            Success = true,
            Data = new List<string>()
            {
                "id"
            }
        };

        var exerciseGetAllByIdsResponse = new ServiceResult<List<Exercise>>
        {
            Success = true,
            Data = exercises
        };

        var contentSaveAllResponse = new List<string>()
        {
            "id"
        };

        SetupCommonMocks(summary, contents, exercises);

        _mockContentRepository.Setup(r => r.GetAllByIds(It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contents);

        _mockExerciseService.Setup(e => e.GetAllByIds(It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exerciseGetAllByIdsResponse);

        _mockSummaryService.Setup(s => s.Save(It.IsAny<SummaryRequest>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(saveResponse);

        _mockExerciseService.Setup(e => e.SaveAll(It.IsAny<List<Exercise>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exerciseSaveAllResponse);

        _mockContentRepository.Setup(r => r.SaveAll(It.IsAny<List<Content>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentSaveAllResponse);

        _mockSummaryService.Setup(s => s.Save(It.IsAny<SummaryRequest>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(saveResponse);

        var result = await contentService.CopyContentsToEnrollUser(_validRequest, "ownerId", "iacademy", "document");

        result.Success.Should().BeTrue();
        result.Data.Should().Be("NewSummaryId");
    }

    private void SetupCommonMocks(Summary summary, List<Content> contents, List<Exercise> exercises)
    {
        _mockSummaryService.Setup(s => s.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ServiceResult<Summary> 
            { 
                Success = true, 
                Data = summary 
            });

        _mockContentRepository.Setup(r => r.GetAllByIds(It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contents);

        _mockExerciseService.Setup(e => e.GetAllByIds(It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ServiceResult<List<Exercise>> 
            { 
                Success = true, 
                Data = exercises 
            });

        MockCompanyService();
    }

    public void MockCompanyService()
    {
        _mockCompanyService.Setup(c => c.GetByRef(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Company()
            {
                Id = "id",
                Cnpj = "iacademy",
                Groups = new()
                {
                    new CompanyGroup()
                    {
                        GroupName = "GroupName",
                        Users = new()
                        {
                            new()
                            {
                                Document = "document",
                                Name = "Name"
                            }
                        },
                        AuthorizedTrainingIds = new()
                        {
                            "id"
                        }
                    }
                }
            });
    }
}
