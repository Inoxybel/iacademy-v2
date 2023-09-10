using Domain.Entities;

namespace IAcademy.Test.Shared.Builders
{
    public class MessageBuilder
    {
        private Message message;

        public MessageBuilder() => message = CreateDefault();

        private static Message CreateDefault() => new()
        {
            Role = "defaultRole",
            Content = "defaultContent"
        };

        public MessageBuilder WithRole(string role)
        {
            message.Role = role;
            return this;
        }

        public MessageBuilder WithContent(string content)
        {
            message.Content = content;
            return this;
        }

        public Message Build() => message;
    }
}
