using PasswordForge.Models;

namespace PasswordForge.Utils;

public static class ConsoleHelper
{
    public static PasswordOptions ReadOptions()
    {
        Console.Write("Password length: ");
        int length = int.Parse(Console.ReadLine()!);

        return new PasswordOptions
        {
            Length = length,
            UseLowercase = Ask("Use lowercase letters"),
            UseUppercase = Ask("Use uppercase letters"),
            UseNumbers = Ask("Use numbers"),
            UseSpecialCharacters = Ask("Use special characters")
        };
    }

    private static bool Ask(string question)
    {
        Console.Write($"{question} (y/n): ");

        return Console.ReadLine()?
            .ToLower() == "y";
    }
}