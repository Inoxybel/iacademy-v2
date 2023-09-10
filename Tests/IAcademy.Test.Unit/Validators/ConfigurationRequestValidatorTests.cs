using Domain.DTO.Configuration;
using Domain.Entities;
using Domain.Validators;
using FluentValidation.TestHelper;
using System.Linq.Expressions;

namespace IAcademy.Test.Unit.Validators;

public class ConfigurationRequestValidatorTests
{
    private readonly ConfigurationRequestValidator _validator;

    public ConfigurationRequestValidatorTests()
    {
        _validator = new ConfigurationRequestValidator();
    }

    [Theory]
    [InlineData("Valid Input", "Valid Input", true)]
    [InlineData("TooLongInput", "Valid Input", false)]
    [InlineData("Valid Input", "TooLongInput", false)]
    [InlineData("TooLongInput", "TooLongInput", false)]
    public void ValidateProperties(string initialInput, string finalInput, bool expected)
    {
        ValidateProperty(x => x.Summary, initialInput, finalInput, expected);
        ValidateProperty(x => x.FirstContent, initialInput, finalInput, expected);
        ValidateProperty(x => x.NewContent, initialInput, finalInput, expected);
        ValidateProperty(x => x.Exercise, initialInput, finalInput, expected);
        ValidateProperty(x => x.Correction, initialInput, finalInput, expected);
        ValidateProperty(x => x.Pendency, initialInput, finalInput, expected);
    }

    private void ValidateProperty(Expression<Func<ConfigurationRequest, InputProperties>> property, string initialInput, string finalInput, bool expected)
    {
        var validInput = new string('a', 10);
        var tooLongInput = new string('a', 1001);

        initialInput = initialInput == "TooLongInput" ? tooLongInput : validInput;
        finalInput = finalInput == "TooLongInput" ? tooLongInput : validInput;

        var testRequest = GetValidConfigurationRequest();
        var testProperty = property.Compile()(testRequest);
        testProperty.InitialInput = initialInput;
        testProperty.FinalInput = finalInput;

        var result = _validator.TestValidate(testRequest);

        Assert.Equal(expected, result.IsValid);
    }

    private static ConfigurationRequest GetValidConfigurationRequest()
    {
        var validInput = new string('a', 1000);

        var validInputProperties = new InputProperties
        {
            InitialInput = validInput,
            FinalInput = validInput
        };

        return new ConfigurationRequest
        {
            Summary = validInputProperties,
            FirstContent = validInputProperties,
            NewContent = validInputProperties,
            Exercise = validInputProperties,
            Correction = validInputProperties,
            Pendency = validInputProperties,
        };
    }
}
