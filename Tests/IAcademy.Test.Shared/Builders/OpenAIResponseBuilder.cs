using Domain.Entities;
using Domain.Entities.Chat;
using Service.Integrations.OpenAI.DTO;

namespace IAcademy.Test.Shared.Builders
{
    public class OpenAIResponseBuilder
    {
        private OpenAIResponse response;

        public OpenAIResponseBuilder() => response = CreateDefault();

        private static OpenAIResponse CreateDefault() => new()
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

        public OpenAIResponseBuilder WithId(string id)
        {
            response.Id = id;
            return this;
        }

        public OpenAIResponseBuilder WithObject(string obj)
        {
            response.Object = obj;
            return this;
        }

        public OpenAIResponseBuilder WithCreated(string created)
        {
            response.Created = created;
            return this;
        }

        public OpenAIResponseBuilder WithModel(string model)
        {
            response.Model = model;
            return this;
        }

        public OpenAIResponseBuilder WithUsage(Usage usage)
        {
            response.Usage = usage;
            return this;
        }

        public OpenAIResponseBuilder WithChoices(List<Choices> choices)
        {
            response.Choices = choices;
            return this;
        }

        public OpenAIResponse Build() => response;
    }
}
