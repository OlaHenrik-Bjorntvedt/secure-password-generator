# 🔐 PasswordForge

A secure password generator built with C# and .NET.

PasswordForge allows you to generate strong, customizable passwords using cryptographically secure random number generation. The project was created to practice core C# concepts such as classes, methods, input validation, collections, and secure development practices.

---

## ✨ Features

- Start menu: view saved passwords, generate a new password, or test a password
- Centered layout with arrow-key navigation (↑/↓, Enter, Esc) and a consistent color theme
- Custom password length
- Lowercase letters (a-z)
- Uppercase letters (A-Z)
- Numbers (0-9)
- Special characters (!@#$%^&*)
- Password strength assessment
- Cryptographically secure password generation
- Save generated passwords in an encrypted vault (Windows DPAPI)
- Test any password's strength with hidden input
- Generate multiple passwords without restarting the application
- Vue web interface served by the C# application
- Clean and beginner-friendly code structure

---

## 📷 Example

```text
                 PASSWORDFORGE v1.0

             View saved passwords
          ▸  Generate a new password
             Test a password
             Exit

         ↑/↓ Move   Enter Select   Esc Exit

Password length (1-128): 16

Use lowercase letters (Enter = yes, n = no):
Use uppercase letters (Enter = yes, n = no):
Use numbers (Enter = yes, n = no):
Use special characters (Enter = yes, n = no): n

Password: K@8sLm#2Qa!9Xp$T
Strength: Strong

          ▸  Save password
             Generate another
             Back to main menu

Name (e.g. Email, Netflix): Email
Saved "Email".
```

Press **Esc** at any prompt to go back to the main menu.

---

## 🏗️ Project Structure

```text
PasswordForge
│
├── Program.cs
│
├── Models
│   ├── PasswordOptions.cs
│   └── SavedPassword.cs
│
├── Services
│   ├── PasswordGenerator.cs
│   ├── PasswordStrengthChecker.cs
│   └── PasswordVault.cs
│
├── Utils
│   ├── ConsoleHelper.cs
│   ├── ConsoleMenu.cs
│   └── Theme.cs
│
└── README.md
```

### Program.cs

Application entry point.

Responsibilities:

- Start menu loop
- User interaction
- Calling services
- Displaying, saving and testing passwords

### Models

Contains data models used throughout the application.

#### PasswordOptions.cs

Stores user-selected password settings:

- Length
- Lowercase letters
- Uppercase letters
- Numbers
- Special characters

#### SavedPassword.cs

A saved vault entry: name, password and the date it was saved.

### Services

Contains the application's business logic.

#### PasswordGenerator.cs

Responsible for:

- Building character pools
- Generating passwords
- Using `RandomNumberGenerator`
- Returning secure passwords

#### PasswordStrengthChecker.cs

Responsible for:

- Evaluating password strength
- Checking complexity
- Returning:
  - Weak
  - Medium
  - Strong

#### PasswordVault.cs

Responsible for:

- Loading and saving passwords to `%LOCALAPPDATA%\PasswordForge\vault.dat`
- Encrypting the vault with Windows DPAPI (`ProtectedData`), so only your
  Windows user account can read it

### Utils

Contains helper methods.

#### ConsoleHelper.cs

Responsible for:

- Reading user input
- Validation
- User prompts
- Centering text in the console window

#### ConsoleMenu.cs

Arrow-key menu (↑/↓, Home/End, Enter, Esc). Falls back to a numbered list when
input is redirected.

#### Theme.cs

The shared color palette, so every screen uses the same colors.

---

## 🔒 Security

PasswordForge uses:

```csharp
RandomNumberGenerator.GetInt32()
```

instead of:

```csharp
Random
```

This provides cryptographically secure random values suitable for password generation.

---

## 🚀 Getting Started

### Requirements

Install:

- .NET 9 SDK (or newer)

Verify installation:

```bash
dotnet --version
```

---

### Clone Repository

```bash
git clone https://github.com/YOUR_USERNAME/PasswordForge.git
```

Enter the project:

```bash
cd PasswordForge
```

---

### Run Application

```bash
dotnet run
```

The application will build automatically and start in the terminal.

### Run the web interface

Start the ASP.NET Core API and Vue interface:

```bash
dotnet run --project PasswordForge -- --web
```

Run this command from the repository folder (or use `dotnet run -- --web` from
inside the `PasswordForge` project folder). Open the URL printed in the terminal
(usually `http://localhost:5000`). Do not use VS Code Live Server or open
`index.html` directly: these can preview the page, but cannot run the C# API
needed to generate passwords.

The web interface uses the same secure C# password
generator and strength checker as the CLI. Vue is loaded from a CDN, so the
browser needs an internet connection to load the interface.

`PasswordForge/wwwroot` is ASP.NET Core's conventional public web folder.
The app serves its HTML, CSS, and JavaScript from that folder; the C# source
files remain outside it. A separate frontend project is not needed for this
Vue setup.

---

### Build Application

```bash
dotnet build
```

Output:

```text
bin/
└── Debug/
```

---

### Publish Executable

Create a standalone executable:

```bash
dotnet publish -c Release
```

Output:

```text
bin/Release/net9.0/publish/
```

---

## 🧠 What I Learned

This project helped me practice:

- C#
- .NET
- Methods
- Classes
- Properties
- Loops
- Input validation
- Arrays
- Strings
- Namespaces
- Secure random number generation
- Project structure and organization

---

## 🛣️ Future Improvements

Planned features:

- Clipboard support
- Password history
- Entropy calculation
- Passphrase generator
- Export passwords to file
- Colorized console output
- WinForms user interface
- WPF desktop application
- Blazor web version
- Unit tests with xUnit

---

## 📄 License

This project is licensed under the MIT License.

---