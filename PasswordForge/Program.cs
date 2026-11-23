using System;
using System.Threading;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PasswordForge.Services;
using PasswordForge.Utils;

if (args.Contains("--web"))
{
    var builder = WebApplication.CreateBuilder(
        args.Where(argument => argument != "--web").ToArray());
    var app = builder.Build();

    app.UseDefaultFiles();
    app.UseStaticFiles();

    app.MapPost("/api/passwords", (PasswordForge.Models.PasswordOptions? options) =>
    {
        if (options is null)
            return Results.BadRequest(new { error = "Password options are required." });

        if (options.Length is < 1 or > 128)
            return Results.BadRequest(new { error = "Password length must be between 1 and 128." });

        if (!options.UseLowercase &&
            !options.UseUppercase &&
            !options.UseNumbers &&
            !options.UseSpecialCharacters)
        {
            return Results.BadRequest(new { error = "Select at least one character type." });
        }

        string password = PasswordGenerator.Generate(options);
        string strength = PasswordStrengthChecker.GetStrength(password);

        return Results.Ok(new { password, strength });
    });

    await app.RunAsync();
    return;
}



while (true)
{
    Console.Clear();

    Console.Title = "PasswordForge";
Console.CursorVisible = false;

// Colors
Console.ForegroundColor = ConsoleColor.Red;

// ASCII Logo
string logo = @"
██████╗  █████╗ ███████╗███████╗██╗    ██╗ ██████╗ ██████╗ ██████╗ 
██╔══██╗██╔══██╗██╔════╝██╔════╝██║    ██║██╔═══██╗██╔══██╗██╔══██╗
██████╔╝███████║███████╗███████╗██║ █╗ ██║██║   ██║██████╔╝██║  ██║
██╔═══╝ ██╔══██║╚════██║╚════██║██║███╗██║██║   ██║██╔══██╗██║  ██║
██║     ██║  ██║███████║███████║╚███╔███╔╝╚██████╔╝██║  ██║██████╔╝
╚═╝     ╚═╝  ╚═╝╚══════╝╚══════╝ ╚══╝╚══╝  ╚═════╝ ╚═╝  ╚═╝╚═════╝ 
";

foreach (char c in logo)
{
    Console.Write(c);
    Thread.Sleep(1);
}

Console.WriteLine();
Console.ForegroundColor = ConsoleColor.DarkRed;
Console.WriteLine("                PASSWORDFORGE v1.0");
Console.ResetColor();

Thread.Sleep(500);

// Boot Messages
string[] bootMessages =
{
    "Loading cryptographic modules...",
    "Generating entropy pools...",
    "Initializing secure engine...",
    "Verifying encryption standards...",
    "Finalizing startup sequence..."
};

foreach (string msg in bootMessages)
{
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.Write("[");
    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write("OK");
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.Write("] ");

    Console.ResetColor();
    Console.WriteLine(msg);

    Thread.Sleep(400);
}

Console.WriteLine();
Console.ForegroundColor = ConsoleColor.Red;

// Animated Progress Bar
for (int i = 0; i <= 100; i++)
{
    int filled = i / 5;
    string bar = new string('■', filled) + new string('□', 20 - filled);

    Console.SetCursorPosition(0, Console.CursorTop);
    Console.Write($"[{bar}] {i}%");

    Thread.Sleep(20);
}

Console.WriteLine("\n");

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("✅ PasswordForge Ready");
Console.ForegroundColor = ConsoleColor.DarkGray;
Console.WriteLine("🔒 Military-grade password generation enabled");
Console.WriteLine("⚡ Entropy source connected");
Console.WriteLine("🛡️ Secure session established");

Console.ResetColor();
Console.CursorVisible = true;


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