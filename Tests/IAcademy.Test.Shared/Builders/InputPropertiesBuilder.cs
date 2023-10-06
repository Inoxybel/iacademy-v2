using Domain.Entities.Configuration;

namespace IAcademy.Test.Shared.Builders;

public class InputPropertiesBuilder
{
    private InputProperties inputProperties;

    public InputPropertiesBuilder() => inputProperties = CreateDefault();

    private static InputProperties CreateDefault() => new()
    {
        InitialInput = "initial input",
        FinalInput = "final input"
    };

    public InputPropertiesBuilder WithInitialInput(string initialInput)
    {
        inputProperties.InitialInput = initialInput;
        return this;
    }

    public InputPropertiesBuilder WithFinalInput(string finalInput)
    {
        inputProperties.FinalInput = finalInput;
        return this;
    }

    public InputProperties Build() => inputProperties;
}