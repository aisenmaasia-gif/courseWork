using TestSimulator.Domain.Enums;

namespace TestSimulator.Domain.Models;

public class OpenAnswerQuestion : Question
{
    public List<string> AcceptableAnswers { get; set; } = new List<string>();

    public OpenAnswerQuestion()
    {
        Type = QuestionType.OpenAnswer;
    }

    public override bool CheckAnswer(object userAnswer)
    {
        if (userAnswer is string userText && !string.IsNullOrWhiteSpace(userText))
        {
            string cleanUserAnswer = userText.Trim();

            foreach (string answer in AcceptableAnswers)
            {
                if (string.Equals(cleanUserAnswer, answer.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public override Question Clone()
    {
        return new OpenAnswerQuestion
        {
            Id = Id,
            Text = Text,
            Points = Points,
            AcceptableAnswers = new List<string>(AcceptableAnswers)
        };
    }
}