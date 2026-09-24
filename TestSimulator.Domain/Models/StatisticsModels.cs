namespace TestSimulator.Domain.Models;

public class TestStatSummary
{
    public string TestTitle { get; set; } = string.Empty;
    public int AttemptsCount { get; set; }
    public double AveragePercentage { get; set; }
}

public class HardestQuestionSummary
{
    public Guid QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public int ErrorCount { get; set; }
}

public class OverallStatistics
{
    public int TotalAttempts { get; set; }
    public double OverallAveragePercentage { get; set; }
    public List<TestStatSummary> TestSummaries { get; set; } = new List<TestStatSummary>();
    public List<HardestQuestionSummary> HardestQuestions { get; set; } = new List<HardestQuestionSummary>();
}