namespace PasswordForge.Models;

public class PasswordOptions
{
    public int Length { get; set; }

    public bool UseLowercase { get; set; }

    public bool UseUppercase { get; set; }

    public bool UseNumbers { get; set; }

    public bool UseSpecialCharacters { get; set; }
}