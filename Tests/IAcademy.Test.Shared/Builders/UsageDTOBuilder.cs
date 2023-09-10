using Domain.Entities;

namespace IAcademy.Test.Shared.Builders
{
    public class UsageBuilder
    {
        private Usage usage;

        public UsageBuilder() => usage = CreateDefault();

        private static Usage CreateDefault() => new()
        { 
            PromptTokens = 0, 
            CompletionTokens = 0, 
            TotalTokens = 0 
        };

        public UsageBuilder WithPromptTokens(int tokens)
        {
            usage.PromptTokens = tokens;
            return this;
        }

        public UsageBuilder WithCompletionTokens(int tokens)
        {
            usage.CompletionTokens = tokens;
            return this;
        }

        public UsageBuilder WithTotalTokens(int tokens)
        {
            usage.TotalTokens = tokens;
            return this;
        }

        public Usage Build() => usage;
    }
}
