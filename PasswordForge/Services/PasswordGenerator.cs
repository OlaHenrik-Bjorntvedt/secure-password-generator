using System.Security.Cryptography;
using PasswordForge.Models;

namespace PasswordForge.Services;

public static class PasswordGenerator
{
    private const string Lowercase =
        "abcdefghijklmnopqrstuvwxyz";

    private const string Uppercase =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    private const string Numbers =
        "0123456789";

    private const string SpecialCharacters =
        "!@#$%^&*()-_=+[]{}";

    public static string Generate(PasswordOptions options)
    {
        var availableCharacters = "";

        if (options.UseLowercase)
            availableCharacters += Lowercase;

        if (options.UseUppercase)
            availableCharacters += Uppercase;

        if (options.UseNumbers)
            availableCharacters += Numbers;

        if (options.UseSpecialCharacters)
            availableCharacters += SpecialCharacters;

        var password = new char[options.Length];

        for (int i = 0; i < options.Length; i++)
        {
            int index =
                RandomNumberGenerator.GetInt32(
                    availableCharacters.Length);

            password[i] = availableCharacters[index];
        }

        return new string(password);
    }
}