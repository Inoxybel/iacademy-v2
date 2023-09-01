using Domain.Infra;
using FluentAssertions;
using IAcademy.Test.Integration.Base;
using IAcademy.Test.Integration.Configuration;
using IAcademy.Test.Shared.Builders;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using ConfigurationEntity = Domain.Entities.Configuration;

namespace IAcademy.Test.Integration.Specs.Infra.Repositories.MongoDb;

[Collection(Constants.WEB_API_TEST_COLLECTION_NAME)]
public class ConfigurationRepositoryTests : IntegrationTestBase
{
    private readonly IntegrationTestsFixture _fixture;

    public ConfigurationRepositoryTests(IntegrationTestsFixture fixture)
        : base(fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ShouldBeReturnConfiguration()
    {
        var createConfiguration = new ConfigurationBuilder().Build();

        _fixture.DbContext.Configuration.InsertOne(createConfiguration);

        var result = await _fixture.serviceProvider
            .GetRequiredService<IConfigurationRepository>().Get(createConfiguration.Id);

        result.Should().BeEquivalentTo(createConfiguration);
    }

    [Fact]
    public async Task ShouldInsertNewConfiguration()
    {
        var configuration = new ConfigurationBuilder().Build();

        var result = await _fixture.serviceProvider.GetRequiredService<IConfigurationRepository>().Save(configuration);

        result.Should().BeTrue();

        (await _fixture.DbContext.Configuration.EstimatedDocumentCountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task ShouldUpdateExistingConfiguration()
    {
        var oldConfiguration = new ConfigurationBuilder().Build();

        await _fixture.DbContext.Configuration.InsertOneAsync(oldConfiguration);

        var newConfiguration = new ConfigurationBuilder()
            .WithNewContent(new InputPropertiesBuilder()
                .WithInitialInput("new")
                .WithFinalInput("new")
                .Build())
            .WithFirstContent(new InputPropertiesBuilder()
                .WithInitialInput("new")
                .WithFinalInput("new")
                .Build())
            .WithCorrection(new InputPropertiesBuilder()
                .WithInitialInput("new")
                .WithFinalInput("new")
                .Build())
            .WithPendency(new InputPropertiesBuilder()
                .WithInitialInput("new")
                .WithFinalInput("new")
                .Build())
            .WithSummary(new InputPropertiesBuilder()
                .WithInitialInput("new")
                .WithFinalInput("new")
                .Build())
            .WithExercise(new InputPropertiesBuilder()
                .WithInitialInput("new")
                .WithFinalInput("new")
                .Build())
            .Build();

        var filter = Builders<ConfigurationEntity>.Filter.Eq(c => c.Id, oldConfiguration.Id);
        var document = _fixture.DbContext.Configuration.FindSync(filter).FirstOrDefault();

        document.Should().NotBeNull();
        document.Summary.InitialInput.Should().NotBe("new");
        document.Summary.FinalInput.Should().NotBe("new");
        document.NewContent.InitialInput.Should().NotBe("new");
        document.NewContent.FinalInput.Should().NotBe("new");
        document.FirstContent.InitialInput.Should().NotBe("new");
        document.FirstContent.FinalInput.Should().NotBe("new");
        document.Exercise.InitialInput.Should().NotBe("new");
        document.Exercise.FinalInput.Should().NotBe("new");
        document.Correction.InitialInput.Should().NotBe("new");
        document.Correction.FinalInput.Should().NotBe("new");
        document.Pendency.InitialInput.Should().NotBe("new");
        document.Pendency.FinalInput.Should().NotBe("new");

        var result = await _fixture.serviceProvider.GetRequiredService<IConfigurationRepository>().Update(
            oldConfiguration.Id,
            newConfiguration
        );

        result.Should().BeTrue();

        document = _fixture.DbContext.Configuration.FindSync(filter).FirstOrDefault();

        document.Should().NotBeNull();
        document.Summary.InitialInput.Should().Be("new");
        document.Summary.FinalInput.Should().Be("new");
        document.NewContent.InitialInput.Should().Be("new");
        document.NewContent.FinalInput.Should().Be("new");
        document.FirstContent.InitialInput.Should().Be("new");
        document.FirstContent.FinalInput.Should().Be("new");
        document.Exercise.InitialInput.Should().Be("new");
        document.Exercise.FinalInput.Should().Be("new");
        document.Correction.InitialInput.Should().Be("new");
        document.Correction.FinalInput.Should().Be("new");
        document.Pendency.InitialInput.Should().Be("new");
        document.Pendency.FinalInput.Should().Be("new");
    }
}