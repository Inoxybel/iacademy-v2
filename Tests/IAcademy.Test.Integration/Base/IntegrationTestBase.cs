using IAcademy.Test.Integration.Configuration;
using Infra.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq.AutoMock;

namespace IAcademy.Test.Integration.Base;

public class IntegrationTestBase
{
    public static AutoMocker Mocker { get; } = new();

    public IntegrationTestBase(IntegrationTestsFixture fixture)
    {
        ResetDatabase(fixture);
    }

    private static void ResetDatabase(IntegrationTestsFixture fixture)
    {
        var databaseOptions = fixture.serviceProvider.GetRequiredService<IOptions<DatabaseInstanceOptions>>().Value;
        var mongoClient = new MongoClient(databaseOptions.ConnectionString);
        mongoClient.DropDatabase(databaseOptions.Name);
    }
}