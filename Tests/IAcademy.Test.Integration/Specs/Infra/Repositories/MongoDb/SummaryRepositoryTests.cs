using Domain.DTO.Summary;
using Domain.Entities.Summary;
using Domain.Infra;
using FluentAssertions;
using IAcademy.Test.Integration.Base;
using IAcademy.Test.Integration.Configuration;
using IAcademy.Test.Shared.Builders;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace IAcademy.Test.Integration.Specs.Infra.Repositories.MongoDb;

[Collection(Constants.WEB_API_TEST_COLLECTION_NAME)]
public class SummaryRepositoryTests : IntegrationTestBase
{
    private readonly IntegrationTestsFixture _fixture;

    public SummaryRepositoryTests(IntegrationTestsFixture fixture)
        : base(fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ShouldSaveDocument()
    {
        var summary = new SummaryBuilder().Build();

        var documentsCount = await _fixture.DbContext.Summary.EstimatedDocumentCountAsync();

        documentsCount.Should().Be(0);

        var insertResult = await _fixture.serviceProvider.GetRequiredService<ISummaryRepository>().Save(summary);

        insertResult.Should().BeTrue();

        documentsCount = await _fixture.DbContext.Summary.EstimatedDocumentCountAsync();

        documentsCount.Should().Be(1);
    }

    [Fact]
    public async Task ShouldGetDocument()
    {
        var summary = new SummaryBuilder().Build();

        _fixture.DbContext.Summary.InsertOne(summary);

        var result = await _fixture.serviceProvider.GetRequiredService<ISummaryRepository>().Get(summary.Id);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(summary);
    }

    [Fact]
    public async Task ShouldUpdateDocument()
    {
        var oldSummary = new SummaryBuilder().Build();

        _fixture.DbContext.Summary.InsertOne(oldSummary);

        var newSummary = new SummaryRequest()
        {
            OriginId = "New",
            OwnerId = "New",
            ConfigurationId = "New",
            IsAvaliable = true,
            Theme = "New",
            Subcategory = "New",
            Category = "New"
        };

        var result = await _fixture.serviceProvider.GetRequiredService<ISummaryRepository>()
            .Update(oldSummary.Id, newSummary);

        result.Should().BeTrue();

        var filter = Builders<Summary>.Filter.Eq(s => s.Id, oldSummary.Id);
        var updatedSummary = _fixture.DbContext.Summary.FindSync(filter).FirstOrDefault();

        updatedSummary.Should().NotBeNull();
        updatedSummary.Id.Should().Be(oldSummary.Id);
        updatedSummary.OwnerId.Should().Be(newSummary.OwnerId);
        updatedSummary.OriginId.Should().Be(newSummary.OriginId);
        updatedSummary.ConfigurationId.Should().Be(newSummary.ConfigurationId);
        updatedSummary.IsAvaliable.Should().Be(newSummary.IsAvaliable);
        updatedSummary.Theme.Should().Be(newSummary.Theme);
        updatedSummary.Subcategory.Should().Be(newSummary.Subcategory);
        updatedSummary.Category.Should().Be(newSummary.Category);
    }

    [Fact]
    public async Task ShouldReturnAllDocumentsByOwnerId()
    {
        var ownerId = Guid.NewGuid().ToString();

        var firstSummary = new SummaryBuilder().WithOwnerId(ownerId).Build();
        var secondSummary = new SummaryBuilder().WithOwnerId(ownerId).Build();

        await _fixture.DbContext.Summary.InsertManyAsync(new List<Summary>()
        {
            firstSummary,
            secondSummary
        });

        var result = await  _fixture.serviceProvider.GetRequiredService<ISummaryRepository>().GetAllByOwnerId(ownerId, false);

        result.Should().NotBeEmpty();
        result.Count.Should().Be(2);
    }

    [Fact]
    public async Task ShouldReturnAllDocumentsByCategory()
    {
        var category = "Test";

        var firstSummary = new SummaryBuilder().WithCategory(category).Build();
        var secondSummary = new SummaryBuilder().WithCategory(category).Build();

        await _fixture.DbContext.Summary.InsertManyAsync(new List<Summary>()
        {
            firstSummary,
            secondSummary
        });

        var summaryIds = new List<string>()
        {
            firstSummary.Id, 
            secondSummary.Id
        };

        var result = await _fixture.serviceProvider.GetRequiredService<ISummaryRepository>().GetAllByCategory(summaryIds, category, false);

        result.Should().NotBeEmpty();
        result.Count.Should().Be(2);
    }

    [Fact]
    public async Task ShouldReturnAllDocumentsBySubcategory()
    {
        var category = "Test";

        var firstSummary = new SummaryBuilder().WithCategory(category).Build();
        var secondSummary = new SummaryBuilder().WithCategory(category).Build();

        await _fixture.DbContext.Summary.InsertManyAsync(new List<Summary>()
        {
            firstSummary,
            secondSummary
        });

        var summaryIds = new List<string>()
        {
            firstSummary.Id,
            secondSummary.Id
        };

        var result = await _fixture.serviceProvider.GetRequiredService<ISummaryRepository>().GetAllByCategory(summaryIds, category, false);

        result.Should().NotBeEmpty();
        result.Count.Should().Be(2);
    }

    [Fact]
    public async Task ShouldReturnAllDocumentsByCategoryAndSubcategory()
    {
        var category = "TestCategory";
        var subcategory = "TestSubategory";

        var firstSummary = new SummaryBuilder().WithCategory(category).WithSubcategory(subcategory).Build();
        var secondSummary = new SummaryBuilder().WithCategory(category).WithSubcategory(subcategory).Build();

        await _fixture.DbContext.Summary.InsertManyAsync(new List<Summary>()
        {
            firstSummary,
            secondSummary
        });

        var summaryIds = new List<string>()
        {
            firstSummary.Id,
            secondSummary.Id
        };

        var result = await _fixture.serviceProvider.GetRequiredService<ISummaryRepository>().GetAllByCategoryAndSubcategory(summaryIds, category, subcategory, false);

        result.Should().NotBeEmpty();
        result.Count.Should().Be(2);
    }

    [Fact]
    public async Task ShouldReturnEnrollConfirmation()
    {
        var id = Guid.NewGuid().ToString();
        var originId = Guid.NewGuid().ToString();
        var ownerId = Guid.NewGuid().ToString();

        var summary = new SummaryBuilder()
            .WithId(id)
            .WithOriginId(originId)
            .WithOwnerId(ownerId)
            .Build();

        await _fixture.DbContext.Summary.InsertOneAsync(summary);

        var result = await _fixture.serviceProvider.GetRequiredService<ISummaryRepository>().IsEnrolled(originId, ownerId);

        result.Should().BeTrue();

        result = await _fixture.serviceProvider.GetRequiredService<ISummaryRepository>().IsEnrolled(id, ownerId);

        result.Should().BeFalse();
    }
}