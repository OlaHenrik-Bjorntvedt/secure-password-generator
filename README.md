# 🔐 PasswordForge

A secure password generator built with C# and .NET.

PasswordForge allows you to generate strong, customizable passwords using cryptographically secure random number generation. The project was created to practice core C# concepts such as classes, methods, input validation, collections, and secure development practices.

---

## ✨ Features

- Custom password length
- Lowercase letters (a-z)
- Uppercase letters (A-Z)
- Numbers (0-9)
- Special characters (!@#$%^&*)
- Password strength assessment
- Cryptographically secure password generation
- Generate multiple passwords without restarting the application
- Vue web interface served by the C# application
- Clean and beginner-friendly code structure

---

## 📷 Example

```text
==========
PasswordForge
==========

Password length: 16

Use lowercase letters (y/n): y
Use uppercase letters (y/n): y
Use numbers (y/n): y
Use special characters (y/n): y

Password:
K@8sLm#2Qa!9Xp$T

Strength:
Strong
```

---

## 🏗️ Project Structure

```text
PasswordForge
│
├── Program.cs
│
├── Models
│   └── PasswordOptions.cs
│
├── Services
│   ├── PasswordGenerator.cs
│   └── PasswordStrengthChecker.cs
│
├── Utils
│   └── ConsoleHelper.cs
│
└── README.md
```

### Program.cs

Application entry point.

Responsibilities:

- Main program loop
- User interaction
- Calling services
- Displaying generated passwords

### Models

Contains data models used throughout the application.

#### PasswordOptions.cs

Stores user-selected password settings:

- Length
- Lowercase letters
- Uppercase letters
- Numbers
- Special characters

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

### Utils

Contains helper methods.

#### ConsoleHelper.cs

Responsible for:

- Reading user input
- Validation
- User prompts

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