namespace PasswordForge.Models;

public class SavedPassword
{
    public string Name { get; set; } = "";

    public string Password { get; set; } = "";

    public DateTime CreatedAt { get; set; }
}
