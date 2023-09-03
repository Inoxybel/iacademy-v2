using Domain.DTO.Correction;

namespace IAcademy.Test.Shared.Builders
{
    public class CreateCorrectionRequestBuilder
    {
        private CreateCorrectionRequest request;

        public CreateCorrectionRequestBuilder() => request = CreateDefault();

        private static CreateCorrectionRequest CreateDefault() => new()
        {
            Exercises = new()
            {
                new ActivityToCorrectDTOBuilder().Build()
            }
        };

        public CreateCorrectionRequestBuilder WithExercises(List<ActivityToCorrectDTO> exercises)
        {
            request.Exercises = exercises;
            return this;
        }

        public CreateCorrectionRequest Build() => request;
    }
}
