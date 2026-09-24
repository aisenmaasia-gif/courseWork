namespace TestSimulator.Domain.Models;

public struct QuestionScore
{
    public double EarnedPoints { get; set; }
    public double MaxPoints { get; set; }
    public bool IsFullyCorrect { get; set; }

    public QuestionScore(double earnedPoints, double maxPoints, bool isFullyCorrect)
    {
        EarnedPoints = earnedPoints;
        MaxPoints = maxPoints;
        IsFullyCorrect = isFullyCorrect;
    }
}