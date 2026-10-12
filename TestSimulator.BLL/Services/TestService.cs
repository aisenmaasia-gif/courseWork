using TestSimulator.DAL.Interfaces;
using TestSimulator.Domain.Config;
using TestSimulator.Domain.Enums;
using TestSimulator.Domain.Exceptions;
using TestSimulator.Domain.Extensions;
using TestSimulator.Domain.Models;

namespace TestSimulator.BLL.Service;

public class TestService
{
    private readonly ITestRepository _repository;
    private AppConfig _config;

    public event Action<string>? OnLogMessage;

    public TestService(ITestRepository repository)
    {
        _repository = repository;
        _config = _repository.LoadConfig();
    }

    public List<Topic> GetAllTopics() => _repository.GetAllTopics();

    public void SaveTopic(Topic topic)
    {
        _repository.SaveTopic(topic);
        OnLogMessage?.Invoke($"Збережено тему: '{topic.Name}'");
    }

    public void DeleteTopic(Guid topicId) => _repository.DeleteTopic(topicId);

    public TestSession StartSession(Guid testId, int? questionsCount = null, DifficultyLevel difficulty = DifficultyLevel.Hard)
    {
        var allTopics = _repository.GetAllTopics();
        var test = allTopics.SelectMany(t => t.Tests).FirstOrDefault(t => t.Id == testId);

        if (test == null)
        {
            throw new TestNotFoundException(testId);
        }

        if (test.Questions.Count == 0)
        {
            throw new TestSimulatorException("Цей тест порожній.");
        }

        var questionsForSession = new List<Question>();
        foreach (var question in test.Questions)
        {
            questionsForSession.Add(question.Clone());
        }

        questionsForSession = questionsForSession.Where(q => q.Difficulty <= difficulty).ToList();

        if (questionsForSession.Count == 0)
        {
            throw new TestSimulatorException("У цьому тесті немає запитань обраного рівня складності.");
        }

        if (_config.ShuffleQuestions)
        {
            questionsForSession.Shuffle();
            foreach (var q in questionsForSession)
            {
                q.ShuffleOptions();
            }
        }

        if (questionsCount.HasValue)
        {
            if (questionsCount <= 0)
            {
                throw new TestSimulatorException("Кількість запитань має бути більшою за нуль.");
            }

            if (questionsCount.Value > questionsForSession.Count)
            {
                throw new TestSimulatorException($"Доступно лише {questionsForSession.Count} запитань.");
            }

            questionsForSession = questionsForSession.Take(questionsCount.Value).ToList();
        }

        return new TestSession
        {
            TestId = test.Id,
            SessionQuestions = questionsForSession
        };
    }

    public TestResult FinishSession(TestSession session)
    {
        session.EndTime = DateTime.Now;

        double totalScore = 0;
        double maxScore = 0;
        var incorrectIds = new List<Guid>();

        foreach (var question in session.SessionQuestions)
        {
            maxScore += question.Points;

            session.UserAnswers.TryGetValue(question.Id, out var userAnswer);
            var scoreResult = question.Evaluate(userAnswer);

            totalScore += scoreResult.EarnedPoints;

            if (!scoreResult.IsFullyCorrect)
            {
                incorrectIds.Add(question.Id);
            }
        }

        var allTopics = _repository.GetAllTopics();
        var test = allTopics.SelectMany(t => t.Tests).FirstOrDefault(t => t.Id == session.TestId);
        string title = test != null ? test.Title : "Невідомий тест";

        var result = new TestResult
        {
            SessionId = session.Id,
            TestTitle = title,
            Score = Math.Round(totalScore, 2),
            MaxScore = maxScore,
            CompletedAt = session.EndTime.Value,
            Duration = session.EndTime.Value - session.StartTime,
            IncorrectQuestionIds = incorrectIds
        };

        result.IsPassed = result.PercentageScore >= _config.PassingScorePercentage;

        _repository.SaveTestResult(result);
        OnLogMessage?.Invoke($"Завершено тест '{result.TestTitle}' | Результат: {result.Score}/{result.MaxScore} ({result.GradeDescription})");

        return result;
    }

    public void UpdateConfig(AppConfig newConfig)
    {
        _config = newConfig;
        _repository.SaveConfig(_config);
    }

    public AppConfig GetCurrentConfig() => _config;
}