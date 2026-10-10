using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TestSimulator.Domain.Config;
using TestSimulator.Domain.Models;

namespace TestSimulator.UI;

class Program
{
    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        Console.WriteLine("=== ТРЕНАЖЕР ТЕСТІВ (Прототип V4 - Конфігурація та Результати) ===");

        var config = new AppConfig
        {
            PassingScorePercentage = 70.0, 
            SessionDurationMinutes = 15
        };

        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine($"[Налаштування]: Прохідний поріг: {config.PassingScorePercentage}% | Час ліміту: {config.SessionDurationMinutes} хв.");
        Console.ResetColor();

        var topic = new Topic { Name = "Базовий C#" };
        var test = new Test { Title = "Експрес-контроль знань" };
        
        var q1 = new SingleChoiceQuestion
        {
            Text = "Чи підтримує C# множинне наслідування класів?",
            Points = 5.0,
            Options = new List<string> { "Так", "Ні" },
            CorrectOptionIndex = 1 // Ні
        };

        var q2 = new OpenAnswerQuestion
        {
            Text = "Введіть назву методу, який є точкою входу в консольну програму C#:",
            Points = 5.0,
            AcceptableAnswers = new List<string> { "Main", "main" }
        };

        test.Questions.Add(q1);
        test.Questions.Add(q2);
        topic.Tests.Add(test);

        Console.WriteLine($"\nТест: {test.Title} | Макс. балів: {test.TotalPoints}. Починаємо таймер...");
        
        var stopwatch = Stopwatch.StartNew();
        
        double totalEarned = 0;
        var incorrectIds = new List<Guid>();

        foreach (var question in test.Questions)
        {
            Console.WriteLine($"\nЗапитання: {question.Text}");
            object? userAnswer = null;

            if (question is SingleChoiceQuestion sc)
            {
                for (int i = 0; i < sc.Options.Count; i++)
                    Console.WriteLine($"{i + 1}. {sc.Options[i]}");
                
                Console.Write("Відповідь: ");
                if (int.TryParse(Console.ReadLine(), out int choice))
                    userAnswer = choice - 1;
            }
            else if (question is OpenAnswerQuestion)
            {
                Console.Write("Відповідь: ");
                userAnswer = Console.ReadLine() ?? string.Empty;
            }

            var score = question.Evaluate(userAnswer);
            totalEarned += score.EarnedPoints;

            if (!score.IsFullyCorrect)
            {
                incorrectIds.Add(question.Id);
            }
        }

        stopwatch.Stop();

        var result = new TestResult
        {
            SessionId = Guid.NewGuid(),
            TestTitle = test.Title,
            Score = Math.Round(totalEarned, 2),
            MaxScore = test.TotalPoints,
            CompletedAt = DateTime.Now,
            Duration = stopwatch.Elapsed,
            IncorrectQuestionIds = incorrectIds
        };

        result.IsPassed = result.PercentageScore >= config.PassingScorePercentage;

        Console.WriteLine("\n================================================");
        Console.ForegroundColor = result.IsPassed ? ConsoleColor.Green : ConsoleColor.Red;
        Console.WriteLine($"РЕЗУЛЬТАТ: {(result.IsPassed ? "УСПІШНО СКЛАДЕНО" : "НЕ СКЛАДЕНО")}");
        Console.ResetColor();
        Console.WriteLine($"Оцінка: {result.GradeDescription} ({result.PercentageScore}%)");
        Console.WriteLine($"Набрано балів: {result.Score} з {result.MaxScore}");
        Console.WriteLine($"Час проходження: {result.Duration.Minutes} хв. {result.Duration.Seconds} сек.");
        Console.WriteLine($"Кількість помилок: {result.IncorrectQuestionIds.Count}");
        Console.WriteLine("================================================");

        Console.WriteLine("\nНатисніть будь-яку клавішу для виходу...");
        Console.ReadKey();
    }
}