using Domain.Infra;
using FluentAssertions;
using IAcademy.Test.Integration.Base;
using IAcademy.Test.Integration.Configuration;
using IAcademy.Test.Shared.Builders;
using Microsoft.Extensions.DependencyInjection;

namespace IAcademy.Test.Integration.Specs.Infra.Repositories.MongoDb;

[Collection(Constants.WEB_API_TEST_COLLECTION_NAME)]
public class ChatCompletionsRepositoryTests : IntegrationTestBase
{
    private readonly IntegrationTestsFixture _fixture;

    public ChatCompletionsRepositoryTests(IntegrationTestsFixture fixture)
        : base(fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ShouldBeReturnChatCompletion()
    {
        var chatCompletion = new ChatCompletionsBuilder().Build();

        _fixture.DbContext.ChatCompletion.InsertOne(chatCompletion);

        var result = await _fixture.serviceProvider
            .GetRequiredService<IChatCompletionsRepository>().Get(chatCompletion.Id);

        result.Should().BeEquivalentTo(chatCompletion);
    }

    [Fact]
    public async Task ShouldInsertNewChatCompletion()
    {
        var chatCompletion = new ChatCompletionsBuilder().Build();

        var result = await _fixture.serviceProvider.GetRequiredService<IChatCompletionsRepository>().Save(chatCompletion);

        result.Should().NotBeEmpty();

        (await _fixture.DbContext.ChatCompletion.EstimatedDocumentCountAsync()).Should().Be(1);
    }
}
