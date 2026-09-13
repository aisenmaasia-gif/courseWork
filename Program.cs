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

        Console.WriteLine("=== ТРЕНАЖЕР ТЕСТІВ ===");

        var question = new SingleChoiceQuestion
        {
            Text = "Яка мова програмування є основною для платформи .NET?",
            Points = 5.0,
            Options = new List<string> { "Java", "C++", "C#", "Python" },
            CorrectOptionIndex = 2 
        };

        Console.WriteLine($"\nПитання: {question.Text}");
        for (int i = 0; i < question.Options.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {question.Options[i]}");
        }

        Console.Write("\nВаша відповідь (номер варіанту): ");
        string input = Console.ReadLine() ?? string.Empty;

        if (int.TryParse(input, out int userChoice))
        {
            int index = userChoice - 1;
            bool isCorrect = question.CheckAnswer(index);

            Console.WriteLine();
            if (isCorrect)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Правильно! Ви отримали {question.Points} балів.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Неправильно. Правильний варіант: {question.Options[question.CorrectOptionIndex]}");
            }
            Console.ResetColor();
        }
        else
        {
            Console.WriteLine("Некоректне введення значення.");
        }

        Console.WriteLine("\nНатисніть будь-яку клавішу для завершення роботи...");
        Console.ReadKey();
    }
}