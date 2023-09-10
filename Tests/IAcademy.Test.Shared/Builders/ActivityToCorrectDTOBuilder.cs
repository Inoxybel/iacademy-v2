using Domain.DTO.Correction;

namespace IAcademy.Test.Shared.Builders
{
    public class ActivityToCorrectDTOBuilder
    {
        private ActivityToCorrectDTO activity;

        public ActivityToCorrectDTOBuilder() => activity = CreateDefault();

        private static ActivityToCorrectDTO CreateDefault() => new()
        {
            Identification = 1,
            Answer = "DefaultAnswer"
        };

        public ActivityToCorrectDTOBuilder WithIdentification(int identification)
        {
            activity.Identification = identification;
            return this;
        }

        public ActivityToCorrectDTOBuilder WithAnswer(string answer)
        {
            activity.Answer = answer;
            return this;
        }

        public ActivityToCorrectDTO Build() => activity;
    }
}
