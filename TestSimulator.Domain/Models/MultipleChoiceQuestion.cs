using TestSimulator.Domain.Enums;
using TestSimulator.Domain.Extensions;

namespace TestSimulator.Domain.Models;

public class MultipleChoiceQuestion : Question
{
    public List<string> Options { get; set; } = new List<string>();
    public List<int> CorrectOptionIndices { get; set; } = new List<int>();

    public MultipleChoiceQuestion()
    {
        Type = QuestionType.MultipleChoice;
    }

    public override bool CheckAnswer(object userAnswer)
    {
        if (userAnswer is List<int> userAnswers)
        {
            if (userAnswers.Count != CorrectOptionIndices.Count)
            {
                return false;
            }

            var userSet = new HashSet<int>(userAnswers);
            return userSet.SetEquals(CorrectOptionIndices);
        }

        return false;
    }

    public override QuestionScore Evaluate(object? userAnswer)
    {
        if (userAnswer is not List<int> selected || selected.Count == 0 || CorrectOptionIndices.Count == 0)
        {
            return new QuestionScore(0, Points, false);
        }

        int correctCount = 0;
        int wrongCount = 0;

        foreach (int index in selected)
        {
            if (CorrectOptionIndices.Contains(index))
            {
                correctCount++;
            }
            else
            {
                wrongCount++;
            }
        }

        if (correctCount == 0)
        {
            return new QuestionScore(0, Points, false);
        }

        double pointsPerItem = Points / CorrectOptionIndices.Count;
        double earned = (correctCount - wrongCount) * pointsPerItem;

        if (earned < 0)
        {
            earned = 0;
        }

        earned = Math.Round(earned, 2);
        bool isFull = (correctCount == CorrectOptionIndices.Count && wrongCount == 0);

        return new QuestionScore(earned, Points, isFull);
    }

    public override Question Clone()
    {
        return new MultipleChoiceQuestion
        {
            Id = Id,
            Text = Text,
            Points = Points,
            Options = new List<string>(Options),
            CorrectOptionIndices = new List<int>(CorrectOptionIndices)
        };
    }

    public override void ShuffleOptions()
    {
        if (Options.Count <= 1)
        {
            return;
        }

        var correctTexts = new List<string>();
        foreach (int index in CorrectOptionIndices)
        {
            if (index >= 0 && index < Options.Count)
            {
                correctTexts.Add(Options[index]);
            }
        }

        Options.Shuffle();

        CorrectOptionIndices.Clear();
        for (int i = 0; i < Options.Count; i++)
        {
            if (correctTexts.Contains(Options[i]))
            {
                CorrectOptionIndices.Add(i);
            }
        }
    }
}