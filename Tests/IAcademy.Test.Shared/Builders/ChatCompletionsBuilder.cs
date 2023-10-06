using Domain.Entities;
using Domain.Entities.Chat;

namespace IAcademy.Test.Shared.Builders;

public class ChatCompletionsBuilder
{
    private ChatCompletion chatCompletion;

    public ChatCompletionsBuilder() => chatCompletion = CreateDefault();

    private static ChatCompletion CreateDefault() => new()
    {
        Id = "defaultId",
        Object = "defaultObject",
        Created = "defaultCreated",
        Model = "defaultModel",
        Usage = new UsageBuilder().Build(),
        Choices = new()
            {
                new ChoicesBuilder().Build()
            }
    };

    public ChatCompletionsBuilder WithId(string id)
    {
        chatCompletion.Id = id;
        return this;
    }

    public ChatCompletionsBuilder WithObject(string obj)
    {
        chatCompletion.Object = obj;
        return this;
    }

    public ChatCompletionsBuilder WithCreated(string created)
    {
        chatCompletion.Created = created;
        return this;
    }

    public ChatCompletionsBuilder WithModel(string model)
    {
        chatCompletion.Model = model;
        return this;
    }

    public ChatCompletionsBuilder WithUsage(Usage usage)
    {
        chatCompletion.Usage = usage;
        return this;
    }

    public ChatCompletionsBuilder WithChoices(List<Choices> choices)
    {
        chatCompletion.Choices = choices;
        return this;
    }

    public ChatCompletion Build() => chatCompletion;
}
