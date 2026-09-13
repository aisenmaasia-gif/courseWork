using TestSimulator.Domain.Enums;
using TestSimulator.Domain.Extensions;

namespace TestSimulator.Domain.Models;

public class SingleChoiceQuestion : Question
{
    public List<string> Options { get; set; } = new List<string>();
    public int CorrectOptionIndex { get; set; }

    public SingleChoiceQuestion()
    {
        Type = QuestionType.SingleChoice;
    }

    public override bool CheckAnswer(object userAnswer)
    {
        if (userAnswer is int answerIndex)
        {
            return answerIndex == CorrectOptionIndex;
        }

        return false;
    }

    public override Question Clone()
    {
        return new SingleChoiceQuestion
        {
            Id = Id,
            Text = Text,
            Points = Points,
            Options = new List<string>(Options),
            CorrectOptionIndex = CorrectOptionIndex
        };
    }

    public override void ShuffleOptions()
    {
        if (Options.Count <= 1 || CorrectOptionIndex < 0 || CorrectOptionIndex >= Options.Count)
        {
            return;
        }

        string correctOptionText = Options[CorrectOptionIndex];

        Options.Shuffle();

        CorrectOptionIndex = Options.IndexOf(correctOptionText);
    }
}