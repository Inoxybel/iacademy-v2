using Domain.DTO.Content;
using Domain.Validators;
using FluentValidation.TestHelper;

namespace IAcademy.Test.Unit.Validators;

public class AIContentCreationRequestValidatorTests
{
    private readonly AIContentCreationRequestValidator _validator;

    public AIContentCreationRequestValidatorTests()
    {
        _validator = new AIContentCreationRequestValidator();
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("12345", true)]
    [InlineData("12345a", false)]
    [InlineData("abcde", false)]
    [InlineData(" ", false)]
    public void ValidateTopicIndex(string topicIndex, bool expected)
    {
        var model = new AIContentCreationRequest
        {
            TopicIndex = topicIndex
        };

        var result = _validator.TestValidate(model);

        if (expected)
        {
            result.ShouldNotHaveValidationErrorFor(x => x.TopicIndex);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.TopicIndex);
        }
    }
}

