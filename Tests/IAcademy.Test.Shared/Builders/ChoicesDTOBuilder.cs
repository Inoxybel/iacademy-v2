using Domain.Entities;
using Domain.Entities.Chat;

namespace IAcademy.Test.Shared.Builders
{
    public class ChoicesBuilder
    {
        private Choices choice;

        public ChoicesBuilder() => choice = CreateDefault();

        private static Choices CreateDefault() => new()
        { 
            Message = new MessageBuilder().Build(), 
            FinishReason = "defaultFinishReason", 
            Index = 0 
        };

        public ChoicesBuilder WithMessage(Message message)
        {
            choice.Message = message;
            return this;
        }

        public ChoicesBuilder WithFinishReason(string reason)
        {
            choice.FinishReason = reason;
            return this;
        }

        public ChoicesBuilder WithIndex(int index)
        {
            choice.Index = index;
            return this;
        }

        public Choices Build() => choice;
    }
}
