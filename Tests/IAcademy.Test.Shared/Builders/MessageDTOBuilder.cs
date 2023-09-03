using Service.Integrations.OpenAI.DTO;

namespace IAcademy.Test.Shared.Builders
{
    public class MessageDTOBuilder
    {
        private MessageDTO message;

        public MessageDTOBuilder() => message = CreateDefault();

        private static MessageDTO CreateDefault() => new()
        {
            Role = "defaultRole",
            Content = "defaultContent"
        };

        public MessageDTOBuilder WithRole(string role)
        {
            message.Role = role;
            return this;
        }

        public MessageDTOBuilder WithContent(string content)
        {
            message.Content = content;
            return this;
        }

        public MessageDTO Build() => message;
    }
}
