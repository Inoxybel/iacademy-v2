using Domain.Entities;
using Domain.Validators;
using FluentValidation.TestHelper;

namespace IAcademy.Test.Unit.Validators;

public class InputPropertiesValidatorTests
{
    private readonly InputPropertiesValidator _validator;

    public InputPropertiesValidatorTests()
    {
        _validator = new InputPropertiesValidator();
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("Valid Initial Input", true)]
    [InlineData("TooLongInitialInput", false)]
    public void ValidateInitialInput(string initialInput, bool expected)
    {
        if (string.Equals(initialInput, "TooLongInitialInput"))
            initialInput = initialInput.PadRight(1001, 'a');

        var model = new InputProperties
        {
            InitialInput = initialInput,
            FinalInput = "Valid Final Input"
        };

        var result = _validator.TestValidate(model);

        if (expected)
        {
            result.ShouldNotHaveValidationErrorFor(x => x.InitialInput);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.InitialInput);
        }
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("Valid Final Input", true)]
    [InlineData("TooLongFinalInput", false)]
    public void ValidateFinalInput(string finalInput, bool expected)
    {
        if (string.Equals(finalInput, "TooLongFinalInput"))
            finalInput = finalInput.PadRight(1001, 'a');

        var model = new InputProperties
        {
            InitialInput = "Valid Initial Input",
            FinalInput = finalInput
        };

        var result = _validator.TestValidate(model);

        if (expected)
        {
            result.ShouldNotHaveValidationErrorFor(x => x.FinalInput);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.FinalInput);
        }
    }
}

