using Service.Integrations.OpenAI.DTO;

namespace IAcademy.Test.Shared.Builders
{
    public class UsageDTOBuilder
    {
        private UsageDTO usage;

        public UsageDTOBuilder() => usage = CreateDefault();

        private static UsageDTO CreateDefault() => new()
        { 
            PromptTokens = 0, 
            CompletionTokens = 0, 
            TotalTokens = 0 
        };

        public UsageDTOBuilder WithPromptTokens(int tokens)
        {
            usage.PromptTokens = tokens;
            return this;
        }

        public UsageDTOBuilder WithCompletionTokens(int tokens)
        {
            usage.CompletionTokens = tokens;
            return this;
        }

        public UsageDTOBuilder WithTotalTokens(int tokens)
        {
            usage.TotalTokens = tokens;
            return this;
        }

        public UsageDTO Build() => usage;
    }
}
