using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using PasswordForge.Models;
using PasswordForge.Services;
using PasswordForge.Utils;

if (args.Contains("--web"))
{
    var builder = WebApplication.CreateBuilder(
        args.Where(argument => argument != "--web").ToArray());
    var app = builder.Build();

    app.UseDefaultFiles();
    app.UseStaticFiles();

    app.MapPost("/api/passwords", (PasswordOptions? options) =>
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

const string Version = "PASSWORDFORGE v1.0";
const string MenuHint = "↑/↓ Move   Enter Select   Esc Back";
const int NameColumnWidth = 24;

string[] logoLines =
[
    "██████╗  █████╗ ███████╗███████╗██╗    ██╗ ██████╗ ██████╗ ██████╗ ",
    "██╔══██╗██╔══██╗██╔════╝██╔════╝██║    ██║██╔═══██╗██╔══██╗██╔══██╗",
    "██████╔╝███████║███████╗███████╗██║ █╗ ██║██║   ██║██████╔╝██║  ██║",
    "██╔═══╝ ██╔══██║╚════██║╚════██║██║███╗██║██║   ██║██╔══██╗██║  ██║",
    "██║     ██║  ██║███████║███████║╚███╔███╔╝╚██████╔╝██║  ██║██████╔╝",
    "╚═╝     ╚═╝  ╚═╝╚══════╝╚══════╝ ╚══╝╚══╝  ╚═════╝ ╚═╝  ╚═╝╚═════╝ "
];

string[] mainMenuItems =
[
    "View saved passwords",
    "Generate a new password",
    "Test a password",
    "Exit"
];

if (!Console.IsOutputRedirected)
{
    Console.OutputEncoding = Encoding.UTF8;
    Console.Title = "PasswordForge";

    // Restore the terminal if the user quits with Ctrl+C while the cursor is hidden.
    Console.CancelKeyPress += (_, _) =>
    {
        Console.ResetColor();
        Console.CursorVisible = true;
    };
}

var vault = new PasswordVault();

ShowStartupScreen();

int mainSelection = 0;

while (true)
{
    int choice = ConsoleMenu.Select(
        DrawMainMenuHeader, mainMenuItems, "↑/↓ Move   Enter Select   Esc Exit", mainSelection);

    if (choice is -1 || mainMenuItems[choice] == "Exit")
    {
        ConsoleHelper.ClearScreen();
        ConsoleHelper.PadTop(1);
        ConsoleHelper.WriteCentered("Goodbye!", Theme.Primary);
        Console.ResetColor();
        return;
    }

    mainSelection = choice;

    switch (choice)
    {
        case 0:
            ShowSavedPasswords();
            break;
        case 1:
            CreatePassword();
            break;
        case 2:
            TestPassword();
            break;
    }
}

void DrawMainMenuHeader()
{
    ConsoleHelper.ClearScreen();

    // Logo, blank, version, blank, menu items, blank, hint.
    ConsoleHelper.PadTop(LogoHeight() + 3 + mainMenuItems.Length + 2);

    DrawLogo(animated: false);
    Console.WriteLine();
    ConsoleHelper.WriteCentered(Version, Theme.Secondary);
    Console.WriteLine();
}

void ShowSavedPasswords()
{
    int selection = 0;

    while (true)
    {
        List<SavedPassword> entries;

        try
        {
            entries = vault.Load();
        }
        catch (Exception ex) when (IsVaultError(ex))
        {
            ConsoleHelper.WriteHeader("SAVED PASSWORDS");
            ConsoleHelper.WriteError($"Could not open saved passwords: {ex.Message}");
            ConsoleHelper.WriteCentered($"Vault file: {vault.FilePath}", Theme.Muted);
            ConsoleHelper.Pause();
            return;
        }

        if (entries.Count == 0)
        {
            ConsoleHelper.WriteHeader("SAVED PASSWORDS");
            ConsoleHelper.WriteCentered("You have no saved passwords yet.");
            ConsoleHelper.WriteCentered("Generate one from the main menu and choose to save it.", Theme.Muted);
            ConsoleHelper.Pause();
            return;
        }

        var items = entries.Select(FormatEntry).Append("← Back").ToList();
        string columns =
            $"{"NAME",-NameColumnWidth}  {"PASSWORD",-10}  {"STRENGTH",-8}  SAVED";

        void DrawHeader()
        {
            ConsoleHelper.WriteHeader("SAVED PASSWORDS");
            ConsoleHelper.WritePartsAt(
                ConsoleHelper.CenterLeft(ConsoleMenu.GetWidth(items)),
                ("   " + columns, Theme.Muted));
        }

        int choice = ConsoleMenu.Select(DrawHeader, items, MenuHint, selection);

        if (choice == -1 || choice == entries.Count)
            return;

        selection = choice;
        ShowPasswordDetails(entries[choice]);
    }
}

string FormatEntry(SavedPassword entry)
{
    string name = entry.Name.Length > NameColumnWidth
        ? entry.Name[..(NameColumnWidth - 1)] + "…"
        : entry.Name;
    string strength = PasswordStrengthChecker.GetStrength(entry.Password);

    return $"{name,-NameColumnWidth}  {new string('*', 10)}  {strength,-8}  {entry.CreatedAt:yyyy-MM-dd HH:mm}";
}

void ShowPasswordDetails(SavedPassword entry)
{
    ConsoleHelper.WriteHeader(entry.Name);
    ConsoleHelper.WriteCentered(entry.Password, Theme.Primary);
    Console.WriteLine();
    ConsoleHelper.WriteStrength(PasswordStrengthChecker.GetStrength(entry.Password));
    ConsoleHelper.WriteCenteredParts(
        ("Saved: ", Theme.Text), ($"{entry.CreatedAt:yyyy-MM-dd HH:mm}", Theme.Muted));
    ConsoleHelper.Pause("Press any key to go back");
}

void CreatePassword()
{
    const string EscHint = "Press Esc at any time to go back to the main menu";

    while (true)
    {
        string password;

        try
        {
            ConsoleHelper.WriteHeader("GENERATE A NEW PASSWORD");
            ConsoleHelper.WriteCentered(EscHint, Theme.Muted);
            Console.WriteLine();

            password = PasswordGenerator.Generate(ConsoleHelper.ReadOptions());
        }
        catch (OperationCanceledException)
        {
            return;
        }

        string? status = null;
        var statusColor = Theme.Success;

        void DrawResult()
        {
            ConsoleHelper.WriteHeader("GENERATE A NEW PASSWORD");

            ConsoleHelper.WriteCentered("╔══════════════════════════════════════╗", Theme.Primary);
            ConsoleHelper.WriteCentered("║        ⚒ PASSWORD FORGED ⚒         ║", Theme.Muted);
            ConsoleHelper.WriteCentered("╔════════════════════════════════════════════════════════════════════════════╗", Theme.Primary);
            ConsoleHelper.WriteCentered("║                                      ╝╚                                   ║", Theme.Muted);
            ConsoleHelper.WriteCentered($" {password.PadRight(34)} ", Theme.Success);                                
            ConsoleHelper.WriteCentered("║                                      ╗╔                                   ║", Theme.Muted);
            ConsoleHelper.WriteCentered("╚════════════════════════════════════════════════════════════════════════════╝", Theme.Primary);

            ConsoleHelper.WriteStrength(PasswordStrengthChecker.GetStrength(password));

            Console.WriteLine();

            if (status is not null)
            {
                ConsoleHelper.WriteCentered(status, statusColor);
                Console.WriteLine();
            }
        }

        bool saved = false;

        while (true)
        {
            var items = new List<string>();
            if (!saved)
                items.Add("Save password");
            items.Add("Generate another");
            items.Add("Back to main menu");

            int choice = ConsoleMenu.Select(DrawResult, items, MenuHint);
            string selected = choice == -1 ? "Back to main menu" : items[choice];

            if (selected == "Back to main menu")
                return;

            if (selected == "Generate another")
                break;

            DrawResult();
            (saved, status) = SavePassword(password);
            statusColor = saved ? Theme.Success : Theme.Warning;
        }
    }
}

(bool Saved, string? Status) SavePassword(string password)
{
    string name;

    try
    {
        name = ConsoleHelper.ReadRequired("Name (e.g. Email, Netflix): ");
    }
    catch (OperationCanceledException)
    {
        return (false, null);
    }

    try
    {
        vault.Add(new SavedPassword
        {
            Name = name,
            Password = password,
            CreatedAt = DateTime.Now
        });

        return (true, $"Saved \"{name}\".");
    }
    catch (Exception ex) when (IsVaultError(ex))
    {
        return (false, $"Could not save the password: {ex.Message}");
    }
}

void TestPassword()
{
    ConsoleHelper.WriteHeader("TEST A PASSWORD");
    ConsoleHelper.WriteCentered("Press Esc to go back to the main menu", Theme.Muted);
    Console.WriteLine();

    string password;

    try
    {
        password = ConsoleHelper.ReadHidden("Password to test (hidden): ");
    }
    catch (OperationCanceledException)
    {
        return;
    }

    Console.WriteLine();

    if (password.Length == 0)
    {
        ConsoleHelper.WriteError("No password entered.");
    }
    else
    {
        ConsoleHelper.WriteCenteredParts(
            ("Length: ", Theme.Text), ($"{password.Length} characters", Theme.Primary));
        ConsoleHelper.WriteStrength(PasswordStrengthChecker.GetStrength(password));
    }

    ConsoleHelper.Pause();
}

static bool IsVaultError(Exception ex) =>
    ex is CryptographicException or IOException or UnauthorizedAccessException
        or PlatformNotSupportedException or System.Text.Json.JsonException;

bool LogoFits() => ConsoleHelper.WindowWidth > logoLines.Max(line => line.Length);

int LogoHeight() => LogoFits() ? logoLines.Length : 1;

void DrawLogo(bool animated)
{
    // Narrow windows would wrap the ASCII art, so show a plain title instead.
    if (!LogoFits())
    {
        ConsoleHelper.WriteCentered("P A S S W O R D F O R G E", Theme.Primary);
        return;
    }

    int left = ConsoleHelper.CenterLeft(logoLines.Max(line => line.Length));
    Console.ForegroundColor = Theme.Primary;

    foreach (string line in logoLines)
    {
        Console.Write(new string(' ', left));

        if (animated)
        {
            foreach (char c in line)
            {
                Console.Write(c);
                Thread.Sleep(1);
            }
        }
        else
        {
            Console.Write(line);
        }

        Console.WriteLine();
    }

    Console.ForegroundColor = Theme.Text;
}

void ShowStartupScreen()
{
    ConsoleHelper.ClearScreen();

    // Animations need a real console; skip them when output is redirected.
    if (Console.IsOutputRedirected)
        return;

    string[] bootMessages =
    [
        "Loading cryptographic modules...",
        "Generating entropy pools...",
        "Initializing secure engine...",
        "Verifying encryption standards...",
        "Finalizing startup sequence..."
    ];

    string[] readyLines =
    [
        "✅ PasswordForge Ready",
        "🔒 Military-grade password generation enabled",
        "⚡ Entropy source connected",
        "🛡️ Secure session established"
    ];

    const int barSegments = 20;
    int barWidth = $"[{new string('■', barSegments)}] 100%".Length;

    Console.CursorVisible = false;

    // Logo, blank, version, blank, boot messages, blank, bar, blank, ready lines.
    ConsoleHelper.PadTop(LogoHeight() + 3 + bootMessages.Length + 3 + readyLines.Length);

    DrawLogo(animated: true);
    Console.WriteLine();
    ConsoleHelper.WriteCentered(Version, Theme.Secondary);
    Console.WriteLine();

    Thread.Sleep(500);

    int bootLeft = ConsoleHelper.CenterLeft("[OK] ".Length + bootMessages.Max(msg => msg.Length));

    foreach (string msg in bootMessages)
    {
        ConsoleHelper.WritePartsAt(
            bootLeft,
            ("[", Theme.Muted), ("OK", Theme.Success), ("] ", Theme.Muted), (msg, Theme.Text));

        Thread.Sleep(400);
    }

    Console.WriteLine();

    int barLeft = ConsoleHelper.CenterLeft(barWidth);
    int barRow = Console.CursorTop;
    Console.ForegroundColor = Theme.Primary;

    for (int i = 0; i <= 100; i++)
    {
        int filled = i * barSegments / 100;
        string bar = new string('■', filled) + new string('□', barSegments - filled);

        Console.SetCursorPosition(barLeft, barRow);
        Console.Write($"[{bar}] {i,3}%");

        Thread.Sleep(20);
    }

    Console.WriteLine();
    Console.WriteLine();

    int readyLeft = ConsoleHelper.CenterLeft(readyLines.Max(line => line.Length));
    ConsoleHelper.WritePartsAt(readyLeft, (readyLines[0], Theme.Success));

    foreach (string line in readyLines.Skip(1))
        ConsoleHelper.WritePartsAt(readyLeft, (line, Theme.Muted));

    Thread.Sleep(1200);

    // Drop keys pressed during the animation so they don't trigger a menu choice.
    while (!Console.IsInputRedirected && Console.KeyAvailable)
        Console.ReadKey(intercept: true);

    Console.CursorVisible = true;
}
