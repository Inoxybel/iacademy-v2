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
public class CorrectionRepositoryTests : IntegrationTestBase
{
    private readonly IntegrationTestsFixture _fixture;

    public CorrectionRepositoryTests(IntegrationTestsFixture fixture)
        : base(fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ShouldGetCorrection()
    {
        var correction = new CorrectionBuilder().Build();

        _fixture.DbContext.Correction.InsertOne(correction);

        var result = await _fixture.serviceProvider.GetRequiredService<ICorrectionRepository>().Get(correction.Id);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(correction);
    }

    [Fact]
    public async Task ShouldSaveCorrection()
    {
        var correction = new CorrectionBuilder().Build();

        var isSuccess = await _fixture.serviceProvider.GetRequiredService<ICorrectionRepository>().Save(correction);

        isSuccess.Should().BeTrue();

        var filter = Builders<Correction>.Filter.Eq(c => c.Id, correction.Id);
        var updatedCorrection = _fixture.DbContext.Correction.FindSync<Correction>(filter).FirstOrDefault();

        updatedCorrection.Should().NotBeNull();
        updatedCorrection.Should().BeEquivalentTo(correction);
    }

    [Fact]
    public async Task ShouldUpdateCorrection()
    {
        var oldCorrection = new CorrectionBuilder().Build();

        _fixture.DbContext.Correction.InsertOne(oldCorrection);

        var updatedCorrection = new Correction
        {
            ExerciseId = "new",
            OwnerId = "new",
            UpdatedDate = DateTime.MaxValue,
            Corrections = new()
            {
                new CorrectionItemBuilder().Build()
            }
        };

        var result = await _fixture.serviceProvider.GetRequiredService<ICorrectionRepository>().Update(oldCorrection.Id, updatedCorrection);

        result.Should().BeTrue();

        var retrievedCorrection = _fixture.DbContext.Correction.FindSync(c => c.Id == oldCorrection.Id).FirstOrDefault();

        retrievedCorrection.Should().NotBeNull();
        retrievedCorrection.ExerciseId.Should().Be(updatedCorrection.ExerciseId);
        retrievedCorrection.OwnerId.Should().Be(updatedCorrection.OwnerId);
        retrievedCorrection.Corrections.Should().BeEquivalentTo(updatedCorrection.Corrections);
    }
}