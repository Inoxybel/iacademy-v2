using Domain.DTO.Content;
using Domain.Entities;
using Domain.Infra;
using FluentAssertions;
using IAcademy.Test.Integration.Base;
using IAcademy.Test.Integration.Configuration;
using IAcademy.Test.Shared.Builders;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace IAcademy.Test.Integration.Specs.Infra.Repositories.MongoDb;

[Collection(Constants.WEB_API_TEST_COLLECTION_NAME)]
public class ContentRepositoryTests : IntegrationTestBase
{
    private readonly IntegrationTestsFixture _fixture;

    public ContentRepositoryTests(IntegrationTestsFixture fixture)
        : base(fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ShouldGetContent()
    {
        var content = new ContentBuilder().Build();

        _fixture.DbContext.Content.InsertOne(content);

        var result = await _fixture.serviceProvider.GetRequiredService<IContentRepository>().Get(content.Id);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(content);
    }

    [Fact]
    public async Task ShouldGetAllContents()
    {
        var firstContent = new ContentBuilder().Build();
        var secondContent = new ContentBuilder().Build();

        _fixture.DbContext.Content.InsertMany(new List<Content>()
        {
            firstContent,
            secondContent
        });

        var result = await _fixture.serviceProvider.GetRequiredService<IContentRepository>().GetAllByIds(new List<string>()
        {
            firstContent.Id,
            secondContent.Id
        });

        result.Any().Should().BeTrue();
        result.Count().Should().Be(2);
    }

    [Fact]
    public async Task ShouldGetAllContentBySummaryId()
    {
        var summaryId = Guid.NewGuid().ToString();

        var firstContent = new ContentBuilder().WithSummaryId(summaryId).Build();
        var secondContent = new ContentBuilder().WithSummaryId(summaryId).Build();

        await _fixture.DbContext.Content.InsertManyAsync(new List<Content>
        {
            firstContent,
            secondContent
        });

        var result = await _fixture.serviceProvider.GetRequiredService<IContentRepository>().GetAllBySummaryId(summaryId);

        result.Should().NotBeEmpty();
        result.Count.Should().Be(2);
    }

    [Fact]
    public async Task ShouldSaveContent()
    {
        var content = new ContentBuilder().Build();

        var id = await _fixture.serviceProvider.GetRequiredService<IContentRepository>().Save(content);

        id.Should().NotBeNull();

        var filter = Builders<Content>.Filter.Eq(c => c.Id, content.Id);
        var savedContent = _fixture.DbContext.Content.FindSync(filter).FirstOrDefault();

        savedContent.Should().NotBeNull();
        savedContent.Should().BeEquivalentTo(content);
    }

    [Fact]
    public async Task ShouldSaveAllContents()
    {
        var firstContent = new ContentBuilder().Build();
        var secondContent = new ContentBuilder().Build();

        var ids = await _fixture.serviceProvider.GetRequiredService<IContentRepository>().SaveAll(new List<Content>
        {
            firstContent,
            secondContent
        });

        ids.Should().HaveCount(2);
    }

    [Fact]
    public async Task ShouldUpdateContent()
    {
        var oldContent = new ContentBuilder().Build();

        _fixture.DbContext.Content.InsertOne(oldContent);

        var newContentRequest = new ContentRequestBuilder()
            .WithConfigurationId("New")
            .WithExerciseId("New")
            .WithOwnerId(oldContent.OwnerId)
            .WithSubtopicIndex("New")
            .WithTheme("New")
            .WithTitle("New")
            .WithSummaryId("New")
            .Build();

        var result = await _fixture.serviceProvider.GetRequiredService<IContentRepository>().Update(oldContent.Id, newContentRequest);

        result.Should().BeTrue();

        var updatedContent = _fixture.DbContext.Content.FindSync(c => c.Id == oldContent.Id).FirstOrDefault();

        updatedContent.Should().NotBeNull();
        updatedContent.ConfigurationId.Should().Be(newContentRequest.ConfigurationId);
        updatedContent.ExerciseId.Should().Be(newContentRequest.ExerciseId);
        updatedContent.OwnerId.Should().Be(newContentRequest.OwnerId);
        updatedContent.SubtopicIndex.Should().Be(newContentRequest.SubtopicIndex);
        updatedContent.Theme.Should().Be(newContentRequest.Theme);
        updatedContent.Title.Should().Be(newContentRequest.Title);
        updatedContent.SummaryId.Should().Be(newContentRequest.SummaryId);
    }

    [Fact]
    public async Task ShouldUpdateAllContents()
    {
        var summaryId = Guid.NewGuid().ToString();

        var firstContent = new ContentBuilder().WithSummaryId(summaryId).Build();
        var secondContent = new ContentBuilder().WithSummaryId(summaryId).Build();
        var newFirstContent = new ContentBuilder().WithId(firstContent.Id).Build();
        var newSecondContent = new ContentBuilder().WithId(secondContent.Id).Build();

        _fixture.DbContext.Content.InsertMany(new List<Content>
        {
            firstContent,
            secondContent
        });

        var newContents = new List<Content>
        {
            newFirstContent,
            newSecondContent
        };

        var result = await _fixture.serviceProvider.GetRequiredService<IContentRepository>().UpdateAll(summaryId, newContents);

        result.Should().BeTrue();

        var updatedFirstContent = _fixture.DbContext.Content.FindSync(c => c.Id == firstContent.Id).FirstOrDefault();
        updatedFirstContent.Should().NotBeNull();
        updatedFirstContent.ConfigurationId.Should().Be(newFirstContent.ConfigurationId);
        updatedFirstContent.ExerciseId.Should().Be(newFirstContent.ExerciseId);
        updatedFirstContent.OwnerId.Should().Be(newFirstContent.OwnerId);
        updatedFirstContent.SubtopicIndex.Should().Be(newFirstContent.SubtopicIndex);
        updatedFirstContent.Theme.Should().Be(newFirstContent.Theme);
        updatedFirstContent.Title.Should().Be(newFirstContent.Title);
        updatedFirstContent.SummaryId.Should().Be(newFirstContent.SummaryId);

        var updatedSecondContent = _fixture.DbContext.Content.FindSync(c => c.Id == secondContent.Id).FirstOrDefault();
        updatedSecondContent.Should().NotBeNull();
        updatedSecondContent.ConfigurationId.Should().Be(newSecondContent.ConfigurationId);
        updatedSecondContent.ExerciseId.Should().Be(newSecondContent.ExerciseId);
        updatedSecondContent.OwnerId.Should().Be(newSecondContent.OwnerId);
        updatedSecondContent.SubtopicIndex.Should().Be(newSecondContent.SubtopicIndex);
        updatedSecondContent.Theme.Should().Be(newSecondContent.Theme);
        updatedSecondContent.Title.Should().Be(newSecondContent.Title);
        updatedSecondContent.SummaryId.Should().Be(newSecondContent.SummaryId);
    }
}
