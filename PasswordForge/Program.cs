using PasswordForge.Services;
using PasswordForge.Utils;

while (true)
{
    Console.Clear();

    Console.WriteLine("==========");
    Console.WriteLine("PasswordForge");
    Console.WriteLine("==========");
    Console.WriteLine();

    var options = ConsoleHelper.ReadOptions();

    if (!options.UseLowercase &&
        !options.UseUppercase &&
        !options.UseNumbers &&
        !options.UseSpecialCharacters)
    {
        Console.WriteLine();
        Console.WriteLine("You must select at least one character type.");

        Console.ReadKey();
        continue;
    }

    Console.WriteLine();

    string password =
        PasswordGenerator.Generate(options);

    string strength =
        PasswordStrengthChecker.GetStrength(password);

    Console.WriteLine($"Password: {password}");

    Console.WriteLine($"Strength: {strength}");

    Console.WriteLine();
    Console.Write("Generate another password? (y/n): ");

    if (Console.ReadLine()?.ToLower() != "y")
        break;
}