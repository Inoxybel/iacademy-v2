using Domain.Infra;
using FluentAssertions;
using IAcademy.Test.Integration.Base;
using IAcademy.Test.Integration.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        var createConfiguration = new Domain.Entities.Configuration()
        {
            Id = Guid.NewGuid().ToString()
        };

        _fixture.DbContext.Configuration.InsertOne(createConfiguration);

        var result = await _fixture.serviceProvider
            .GetRequiredService<IConfigurationRepository>().Get(createConfiguration.Id);

        result.Should().BeEquivalentTo(createConfiguration);
    }
}