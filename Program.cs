using System;
using System.Collections.Generic;
using System.Text;
using TestSimulator.Domain.Models;

namespace TestSimulator.UI;

class Program
{
    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        Console.WriteLine("=== ТРЕНАЖЕР ТЕСТІВ (Прототип V3 - Структурований) ===");

        var topic = new Topic
        {
            Name = "Програмування на платформі .NET",
            Description = "Основи синтаксису мови C# та базові концепції CLR"
        };

        var test = new Test
        {
            Title = "Вступний тест з C#",
            Description = "Перевірка базових знань синтаксису та ООП"
        };

        var q1 = new SingleChoiceQuestion
        {
            Text = "Який тип даних є посилальним (reference type) у C#?",
            Points = 2.0,
            Options = new List<string> { "int", "struct", "class", "double" },
            CorrectOptionIndex = 2
        };

        var q2 = new MultipleChoiceQuestion
        {
            Text = "Оберіть принципи ООП (виберіть кілька варіантів через кому):",
            Points = 4.0,
            Options = new List<string> { "Інкапсуляція", "Деструктуризація", "Поліморфізм", "Компіляція" },
            CorrectOptionIndices = new List<int> { 0, 2 }
        };

        var q3 = new OpenAnswerQuestion
        {
            Text = "Яке ключове слово використовується для визначення константи в C#?",
            Points = 3.0,
            AcceptableAnswers = new List<string> { "const", "CONST" }
        };

        test.Questions.Add(q1);
        test.Questions.Add(q2);
        test.Questions.Add(q3);
        topic.Tests.Add(test);

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n[📚 Тема]: {topic.Name}");
        Console.WriteLine($"   Опис: {topic.Description}");
        Console.WriteLine($"   |-- [📜 Тест]: {test.Title}");
        Console.WriteLine($"       Опис: {test.Description}");
        Console.WriteLine($"       Загальна кількість питань: {test.Questions.Count}");
        Console.WriteLine($"       Загальна сума балів за тест: {test.TotalPoints}");
        Console.ResetColor();

        Console.WriteLine("\nНатисніть Enter, щоб розпочати проходження цього тесту...");
        Console.ReadLine();

        double totalEarned = 0;

        foreach (var question in test.Questions)
        {
            Console.WriteLine($"\n------------------------------------------------");
            Console.WriteLine($"Запитання: {question.Text} ({question.Points} б.)");
            Console.WriteLine($"------------------------------------------------");

            object? userAnswer = null;

            if (question is SingleChoiceQuestion sc)
            {
                for (int i = 0; i < sc.Options.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {sc.Options[i]}");
                }
                Console.Write("Ваша відповідь: ");
                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    userAnswer = choice - 1;
                }
            }
            else if (question is MultipleChoiceQuestion mc)
            {
                for (int i = 0; i < mc.Options.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {mc.Options[i]}");
                }
                Console.Write("Ваші відповіді (через кому): ");
                string input = Console.ReadLine() ?? string.Empty;
                var parts = input.Split(',');
                var indices = new List<int>();
                foreach (var part in parts)
                {
                    if (int.TryParse(part.Trim(), out int val))
                    {
                        indices.Add(val - 1);
                    }
                }
                userAnswer = indices;
            }
            else if (question is OpenAnswerQuestion)
            {
                Console.Write("Введіть відповідь: ");
                userAnswer = Console.ReadLine() ?? string.Empty;
            }

            var score = question.Evaluate(userAnswer);
            totalEarned += score.EarnedPoints;

            if (score.IsFullyCorrect)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Правильно! (+{score.EarnedPoints} б.)");
            }
            else if (score.EarnedPoints > 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Частково правильно. (+{score.EarnedPoints} б.)");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Неправильно. (0 б.)");
            }
            Console.ResetColor();
        }

        Console.WriteLine("\n================================================");
        Console.WriteLine($"ТЕСТ ЗАВЕРШЕНО!");
        Console.WriteLine($"Ви набрали: {totalEarned} з {test.TotalPoints} балів.");
        Console.WriteLine($"Відсоток успішності: {Math.Round((totalEarned / test.TotalPoints) * 100, 1)}%");
        Console.WriteLine("================================================");

        Console.WriteLine("\nНатисніть будь-яку клавішу для виходу...");
        Console.ReadKey();
    }
}