using System.Security.Cryptography;

namespace PasswordForge;

class Program
{
    static void Main(string[] args)
    {
        int? length = ReadPasswordLength();
        if (length.HasValue)
        {
            Console.WriteLine($"Password: {GeneratePassword(length.Value)}");
        }
    }

    static int? ReadPasswordLength()
    {
        while (true)
        {
            Console.Write("Enter a password length (1-1024): ");
            string? input = Console.ReadLine();
            if (input == null)
            {
                return null;
            }

            if (int.TryParse(input, out int length) && length >= 1 && length <= 1024)
            {
                return length;
            }

            Console.WriteLine("Please enter a whole number between 1 and 1024.");
        }
    }

    static string GeneratePassword(int length)
    {
        const string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()-_=+";
        char[] password = new char[length];

        for (int i = 0; i < password.Length; i++)
        {
            password[i] = characters[RandomNumberGenerator.GetInt32(characters.Length)];
        }

        return new string(password);
    }
}
