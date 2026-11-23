using System.Text;
using PasswordForge.Models;

namespace PasswordForge.Utils;

public static class ConsoleHelper
{
    // Prompts on the same screen share this width so they line up under each other.
    private const int FormWidth = 44;

    public static bool IsInteractive =>
        !Console.IsInputRedirected && !Console.IsOutputRedirected;

    public static int WindowWidth => GetWindowSize(() => Console.WindowWidth, 80);

    public static int WindowHeight => GetWindowSize(() => Console.WindowHeight, 25);

    public static int CenterLeft(int contentWidth) =>
        Math.Max(0, (WindowWidth - contentWidth) / 2);

    public static void PadTop(int contentHeight)
    {
        int lines = Math.Max(0, (WindowHeight - contentHeight) / 2);

        for (int i = 0; i < lines; i++)
            Console.WriteLine();
    }

    public static void WriteCentered(string text, ConsoleColor color = Theme.Text) =>
        WriteCenteredParts((text, color));

    public static void WriteCenteredParts(params (string Text, ConsoleColor Color)[] parts) =>
        WritePartsAt(CenterLeft(parts.Sum(part => part.Text.Length)), parts);

    public static void WritePartsAt(int left, params (string Text, ConsoleColor Color)[] parts)
    {
        Console.Write(new string(' ', left));

        foreach (var (text, color) in parts)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
        }

        Console.ForegroundColor = Theme.Text;
        Console.WriteLine();
    }

    public static void WriteHeader(string title)
    {
        ClearScreen();

        for (int i = 0; i < Math.Max(1, WindowHeight / 6); i++)
            Console.WriteLine();

        WriteCentered(title, Theme.Primary);
        WriteCentered(new string('─', title.Length + 4), Theme.Secondary);
        Console.WriteLine();
    }

    public static PasswordOptions ReadOptions()
    {
        int length = ReadNumber("Password length (1-128): ", 1, 128);

        while (true)
        {
            var options = new PasswordOptions
            {
                Length = length,
                UseLowercase = Ask("Use lowercase letters"),
                UseUppercase = Ask("Use uppercase letters"),
                UseNumbers = Ask("Use numbers"),
                UseSpecialCharacters = Ask("Use special characters")
            };

            if (options.UseLowercase || options.UseUppercase ||
                options.UseNumbers || options.UseSpecialCharacters)
            {
                return options;
            }

            WriteError("You must select at least one character type.");
        }
    }

    public static int ReadNumber(string prompt, int min, int max)
    {
        while (true)
        {
            if (int.TryParse(ReadLineCentered(prompt), out int value) &&
                value >= min && value <= max)
            {
                return value;
            }

            WriteError($"Enter a number between {min} and {max}.");
        }
    }

    public static string ReadRequired(string prompt)
    {
        while (true)
        {
            string value = ReadLineCentered(prompt).Trim();

            if (value.Length > 0)
                return value;

            WriteError("This field cannot be empty.");
        }
    }

    /// <summary>Asks a yes/no question where pressing Enter means yes.</summary>
    public static bool Ask(string question)
    {
        while (true)
        {
            switch (ReadLineCentered($"{question} (Enter = yes, n = no): ").Trim().ToLowerInvariant())
            {
                case "" or "y" or "yes": return true;
                case "n" or "no": return false;
            }

            WriteError("Press Enter for yes or type n for no.");
        }
    }

    /// <summary>
    /// Reads a line of input. Throws <see cref="OperationCanceledException"/> if the user presses Escape.
    /// </summary>
    public static string ReadLineCentered(string prompt) => ReadInput(prompt, hidden: false);

    /// <summary>Like <see cref="ReadLineCentered"/>, but shows '*' instead of the typed characters.</summary>
    public static string ReadHidden(string prompt) => ReadInput(prompt, hidden: true);

    private static string ReadInput(string prompt, bool hidden)
    {
        WritePrompt(prompt);
        Console.ForegroundColor = Theme.Primary;

        try
        {
            // Escape can only be detected with ReadKey, which needs a real console.
            if (Console.IsInputRedirected)
                return ReadLineOrExit();

            var input = new StringBuilder();

            while (true)
            {
                var key = Console.ReadKey(intercept: true);

                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        Console.WriteLine();
                        return input.ToString();
                    case ConsoleKey.Escape:
                        Console.WriteLine();
                        throw new OperationCanceledException();
                    case ConsoleKey.Backspace:
                        if (input.Length > 0)
                        {
                            input.Length--;
                            Console.Write("\b \b");
                        }
                        break;
                    default:
                        if (!char.IsControl(key.KeyChar))
                        {
                            input.Append(key.KeyChar);
                            Console.Write(hidden ? '*' : key.KeyChar);
                        }
                        break;
                }
            }
        }
        finally
        {
            Console.ForegroundColor = Theme.Text;
        }
    }

    public static string ReadLineOrExit()
    {
        // ReadLine returns null when input ends (e.g. Ctrl+Z), so exit cleanly instead of looping.
        string? line = Console.ReadLine();

        if (line is null)
        {
            Console.ResetColor();
            Environment.Exit(0);
        }

        return line;
    }

    public static void Pause(string message = "Press any key to return to the menu")
    {
        Console.WriteLine();
        WriteCentered(message, Theme.Muted);

        if (IsInteractive)
        {
            Console.CursorVisible = false;
            Console.ReadKey(intercept: true);
            Console.CursorVisible = true;
        }
        else
        {
            ReadLineOrExit();
        }
    }

    public static void ClearScreen()
    {
        if (Console.IsOutputRedirected)
            return;

        Console.ResetColor();
        Console.Clear();
        Console.ForegroundColor = Theme.Text;
    }

    public static void WriteError(string message) =>
        WriteCentered(message, Theme.Warning);

    public static void WriteSuccess(string message) =>
        WriteCentered(message, Theme.Success);

    public static void WriteStrength(string strength) =>
        WriteCenteredParts(("Strength: ", Theme.Text), (strength, Theme.ForStrength(strength)));

    private static void WritePrompt(string prompt)
    {
        int width = Math.Max(FormWidth, prompt.Length + 8);

        Console.Write(new string(' ', CenterLeft(width)));
        Console.ForegroundColor = Theme.Text;
        Console.Write(prompt);
    }

    private static int GetWindowSize(Func<int> read, int fallback)
    {
        if (Console.IsOutputRedirected)
            return fallback;

        try
        {
            int size = read();
            return size > 0 ? size : fallback;
        }
        catch (IOException)
        {
            return fallback;
        }
    }
}
