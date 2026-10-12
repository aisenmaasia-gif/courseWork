using TestSimulator.DAL.Interfaces;
using TestSimulator.Domain.Models;

namespace TestSimulator.BLL.Service;

public class AnalyticsService
{
    private readonly ITestRepository _repository;

    public AnalyticsService(ITestRepository repository)
    {
        _repository = repository;
    }

    public List<TestResult> GetUserHistory()
    {
        return _repository.GetAllResults();
    }

    public OverallStatistics GetOverallStatistics()
    {
        var history = _repository.GetAllResults();
        if (history.Count == 0)
        {
            return new OverallStatistics();
        }

        var stats = new OverallStatistics
        {
            TotalAttempts = history.Count,
            OverallAveragePercentage = Math.Round(history.Average(r => r.PercentageScore), 1)
        };

        var groupedTests = history.GroupBy(r => r.TestTitle);
        foreach (var group in groupedTests)
        {
            stats.TestSummaries.Add(new TestStatSummary
            {
                TestTitle = group.Key,
                AttemptsCount = group.Count(),
                AveragePercentage = Math.Round(group.Average(r => r.PercentageScore), 1)
            });
        }

        var allIncorrectIds = history.SelectMany(r => r.IncorrectQuestionIds).ToList();
        if (allIncorrectIds.Count > 0)
        {
            var allQuestions = _repository.GetAllTopics()
                .SelectMany(t => t.Tests)
                .SelectMany(t => t.Questions)
                .ToList();

            var errorGroups = allIncorrectIds.CountBy(id => id).OrderByDescending(x => x.Value).Take(5);

            foreach (var item in errorGroups)
            {
                var q = allQuestions.FirstOrDefault(x => x.Id == item.Key);
                string text = q != null ? q.Text : "[Видалене запитання]";

                stats.HardestQuestions.Add(new HardestQuestionSummary
                {
                    QuestionId = item.Key,
                    QuestionText = text,
                    ErrorCount = item.Value
                });
            }
        }

        return stats;
    }

    public void ClearHistory()
    {
        _repository.ClearAllResults();
    }
}