using Domain.Entities;

namespace IAcademy.Test.Shared.Builders;

public class CorrectionItemBuilder
{
    private CorrectionItem correctionItem;

    public CorrectionItemBuilder() => correctionItem = CreateDefault();

    private static CorrectionItem CreateDefault() => new CorrectionItem
    {
        Identification = 0,
        Question = "DefaultQuestion",
        Complementation = new List<string>(),
        Asnwer = "DefaultAnswer",
        IsCorrect = false,
        Feedback = "DefaultFeedback"
    };

    public CorrectionItemBuilder WithIdentification(int identification)
    {
        correctionItem.Identification = identification;
        return this;
    }

    public CorrectionItemBuilder WithQuestion(string question)
    {
        correctionItem.Question = question;
        return this;
    }

    public CorrectionItemBuilder WithComplementation(List<string> complementation)
    {
        correctionItem.Complementation = complementation;
        return this;
    }

    public CorrectionItemBuilder WithAnswer(string answer)
    {
        correctionItem.Asnwer = answer;
        return this;
    }

    public CorrectionItemBuilder WithIsCorrect(bool isCorrect)
    {
        correctionItem.IsCorrect = isCorrect;
        return this;
    }

    public CorrectionItemBuilder WithFeedback(string feedback)
    {
        correctionItem.Feedback = feedback;
        return this;
    }

    public CorrectionItem Build() => correctionItem;
}
