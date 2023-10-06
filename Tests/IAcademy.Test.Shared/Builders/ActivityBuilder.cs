using CrossCutting.Enums;
using Domain.Entities.Exercise;

namespace IAcademy.Test.Shared.Builders;

public class ActivityBuilder
{
    private Activity activity;

    public ActivityBuilder() => activity = CreateDefault();

    private static Activity CreateDefault() => new()
    {
        Identification = 1,
        Type = ActivityType.SingleChoice,
        Question = "DefaultQuestion",
        Complementation = new List<string>(),
        Answer = "DefaultAnswer"
    };

    public ActivityBuilder WithIdentification(int identification)
    {
        activity.Identification = identification;
        return this;
    }

    public ActivityBuilder WithType(ActivityType type)
    {
        activity.Type = type;
        return this;
    }

    public ActivityBuilder WithQuestion(string question)
    {
        activity.Question = question;
        return this;
    }

    public ActivityBuilder WithComplementation(List<string> complementation)
    {
        activity.Complementation = complementation;
        return this;
    }

    public ActivityBuilder WithAnswer(string answer)
    {
        activity.Answer = answer;
        return this;
    }

    public Activity Build() => activity;
}