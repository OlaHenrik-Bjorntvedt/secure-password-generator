namespace PasswordForge.Utils;

/// <summary>The color palette used by every CLI screen, so colors stay consistent.</summary>
public static class Theme
{
    public const ConsoleColor Primary = ConsoleColor.Red;
    public const ConsoleColor Secondary = ConsoleColor.DarkRed;
    public const ConsoleColor Text = ConsoleColor.Gray;
    public const ConsoleColor Muted = ConsoleColor.DarkGray;
    public const ConsoleColor Success = ConsoleColor.Green;
    public const ConsoleColor Warning = ConsoleColor.Yellow;
    public const ConsoleColor SelectedText = ConsoleColor.Black;

    public static ConsoleColor ForStrength(string strength) => strength switch
    {
        "Strong" => Success,
        "Medium" => Warning,
        _ => Primary
    };
}
