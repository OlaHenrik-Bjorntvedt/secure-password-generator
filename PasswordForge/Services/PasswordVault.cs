using System.Security.Cryptography;
using System.Text.Json;
using PasswordForge.Models;

namespace PasswordForge.Services;

/// <summary>
/// Stores saved passwords in a file encrypted with Windows DPAPI,
/// so only the current Windows user can decrypt it.
/// </summary>
public class PasswordVault
{
    private static readonly byte[] Entropy = "PasswordForge.Vault.v1"u8.ToArray();

    private readonly string filePath;

    public PasswordVault(string? filePath = null)
    {
        this.filePath = filePath ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "PasswordForge",
            "vault.dat");
    }

    public string FilePath => filePath;

    public List<SavedPassword> Load()
    {
        if (!OperatingSystem.IsWindows())
            throw new PlatformNotSupportedException("Saved passwords are only supported on Windows.");

        if (!File.Exists(filePath))
            return [];

        byte[] json = ProtectedData.Unprotect(
            File.ReadAllBytes(filePath), Entropy, DataProtectionScope.CurrentUser);

        try
        {
            return JsonSerializer.Deserialize<List<SavedPassword>>(json) ?? [];
        }
        finally
        {
            CryptographicOperations.ZeroMemory(json);
        }
    }

    public void Add(SavedPassword entry)
    {
        var entries = Load();
        entries.Add(entry);
        Save(entries);
    }

    private void Save(List<SavedPassword> entries)
    {
        if (!OperatingSystem.IsWindows())
            throw new PlatformNotSupportedException("Saved passwords are only supported on Windows.");

        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

        byte[] json = JsonSerializer.SerializeToUtf8Bytes(entries);

        try
        {
            byte[] encrypted = ProtectedData.Protect(
                json, Entropy, DataProtectionScope.CurrentUser);

            // Write to a temp file first so a crash can't leave a half-written vault.
            string tempPath = filePath + ".tmp";
            File.WriteAllBytes(tempPath, encrypted);
            File.Move(tempPath, filePath, overwrite: true);
        }
        finally
        {
            CryptographicOperations.ZeroMemory(json);
        }
    }
}
