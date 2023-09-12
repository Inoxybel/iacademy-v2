using CrossCutting.Enums;
using Domain.DTO.Exercise;
using Domain.Validators;
using FluentValidation.TestHelper;

namespace IAcademy.Test.Unit.Validators;

public class ExerciseRequestValidatorTests
{
    private readonly ExerciseRequestValidator _validator;

    public ExerciseRequestValidatorTests()
    {
        _validator = new ExerciseRequestValidator();
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("123456789012345678901234567890123456", true)]
    [InlineData("12345", false)]
    public void ValidateOwnerId(string ownerId, bool expected)
    {
        var model = new ExerciseRequest
        {
            OwnerId = ownerId,
            Exercises = new()
        };

        var result = _validator.TestValidate(model);

        if (expected)
        {
            result.ShouldNotHaveValidationErrorFor(x => x.OwnerId);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.OwnerId);
        }
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("123456789012345678901234567890123456", true)]
    [InlineData("12345", false)]
    public void ValidateCorrectionId(string correctionId, bool expected)
    {
        var model = new ExerciseRequest 
        { 
            CorrectionId = correctionId,
            Exercises = new()
        };

        var result = _validator.TestValidate(model);

        if (expected)
        {
            result.ShouldNotHaveValidationErrorFor(x => x.CorrectionId);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.CorrectionId);
        }
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("123456789012345678901234567890123456", true)]
    [InlineData("12345", false)]
    public void ValidateConfigurationId(string configurationId, bool expected)
    {
        var model = new ExerciseRequest 
        { 
            ConfigurationId = configurationId,
            Exercises = new()
        };
        var result = _validator.TestValidate(model);

        if (expected)
        {
            result.ShouldNotHaveValidationErrorFor(x => x.ConfigurationId);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.ConfigurationId);
        }
    }

    [Theory]
    [InlineData(ExerciseStatus.WaitingCorrection, true)]
    [InlineData(ExerciseStatus.WaitingToDo, true)]
    [InlineData(ExerciseStatus.Finished, true)]
    public void ValidateStatus(ExerciseStatus status, bool expected)
    {
        var model = new ExerciseRequest 
        { 
            Status = status,
            Exercises = new()
        };

        var result = _validator.TestValidate(model);

        if (expected)
        {
            result.ShouldNotHaveValidationErrorFor(x => x.Status);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.Status);
        }
    }

    [Theory]
    [InlineData(ExerciseType.Pendency, true)]
    [InlineData(ExerciseType.Default, true)]
    public void ValidateType(ExerciseType type, bool expected)
    {
        var model = new ExerciseRequest 
        {
            Type = type,
            Exercises = new()
        };

        var result = _validator.TestValidate(model);

        if (expected)
        {
            result.ShouldNotHaveValidationErrorFor(x => x.Type);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.Type);
        }
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("1.14", false)]
    [InlineData("12", true)]
    [InlineData("1.3", true)]
    [InlineData("1a", false)]
    public void ValidateTopicIndex(string topicIndex, bool expected)
    {
        var model = new ExerciseRequest 
        { 
            TopicIndex = topicIndex,
            Exercises = new()
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

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("Valid Title Here", true)]
    public void ValidateTitle(string title, bool expected)
    {
        var model = new ExerciseRequest 
        { 
            Title = title,
            Exercises = new()
        };

        var result = _validator.TestValidate(model);

        if (expected)
        {
            result.ShouldNotHaveValidationErrorFor(x => x.Title);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }
    }
}
