using Service.Integrations.OpenAI.DTO;

namespace IAcademy.Test.Shared.Builders
{
    public class ChoicesDTOBuilder
    {
        private ChoicesDTO choice;

        public ChoicesDTOBuilder() => choice = CreateDefault();

        private static ChoicesDTO CreateDefault() => new()
        { 
            Message = new MessageDTOBuilder().Build(), 
            FinishReason = "defaultFinishReason", 
            Index = 0 
        };

        public ChoicesDTOBuilder WithMessage(MessageDTO message)
        {
            choice.Message = message;
            return this;
        }

        public ChoicesDTOBuilder WithFinishReason(string reason)
        {
            choice.FinishReason = reason;
            return this;
        }

        public ChoicesDTOBuilder WithIndex(int index)
        {
            choice.Index = index;
            return this;
        }

        public ChoicesDTO Build() => choice;
    }
}
