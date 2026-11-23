namespace PasswordForge.Services;
using System.Linq;
public static class PasswordStrengthChecker
{
    public static string GetStrength(string password)
    {
        int score = 0;

        if (password.Length >= 8)
            score++;

        if (password.Length >= 12)
            score++;

        if (password.Any(char.IsUpper))
            score++;

        if (password.Any(char.IsDigit))
            score++;

        if (password.Any(c => !char.IsLetterOrDigit(c)))
            score++;

        return score switch
        {
            <= 2 => "Weak",
            <= 4 => "Medium",
            _ => "Strong"
        };
    }
}