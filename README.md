# PasswordForge

A simple C# console application that generates passwords using a
cryptographically secure random number generator.

## Run

Install the .NET 10 SDK, then run:

```sh
dotnet run --project PasswordForge
```

Enter a password length between 1 and 1024. The password is randomly drawn
from uppercase and lowercase letters, numbers, and special characters; it does
not guarantee that every character type appears.