# Agents.md - Developer Guide for Explorer Tab Utility

> This document provides essential information for AI agents and developers working on the Explorer Tab Utility project.

## Project Overview

Explorer Tab Utility is a Windows desktop application that automatically converts new File Explorer windows into tabs in Windows 11, providing a cleaner and more organized file management experience.

- **Repository**: https://github.com/iinvolvex/ExplorerTabUtility
- **License**: MIT
- **Platform**: Windows 11 (22H2 Build 22621 or later)

---

## Table of Contents

1. [Useful Commands](#useful-commands)
2. [Technologies](#technologies)
3. [Project Structure](#project-structure)
4. [Best Practices and Guidelines](#best-practices-and-guidelines)
5. [Architecture Overview](#architecture-overview)

---

## Useful Commands

### Build Commands

> **Note**: This project uses COM references (SHDocVw, Shell32) with an SDK-style project format. The build requires specific environment setup.

**Build with PowerShell Script (Recommended)**
```powershell
.\build.ps1
```

**Manual Command Line Build (PowerShell)**
```powershell
$env:MSBuildSDKsPath = "C:\Program Files\dotnet\sdk\10.0.302\Sdks"
& "I:\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe" ExplorerTabUtility.sln /t:Rebuild /p:Configuration=Release
```

**Build through Visual Studio IDE**
Open `ExplorerTabUtility.sln` in Visual Studio 2022 and build from there.

**Note**: If your VS installation is on a different drive, find MSBuild with:
```powershell
& "C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe" -latest -property installationPath
```

### Run Commands

```powershell
# Run the application (Debug mode)
dotnet run --project ExplorerTabUtility/ExplorerTabUtility.csproj

# Run in Release mode
dotnet run --project ExplorerTabUtility/ExplorerTabUtility.csproj -c Release

# Run with specific framework
dotnet run --project ExplorerTabUtility/ExplorerTabUtility.csproj -f net9.0-windows
```

### Publishing Commands

```powershell
# Publish self-contained for Windows x64
dotnet publish ExplorerTabUtility/ExplorerTabUtility.csproj -c Release -r win-x64 --self-contained

# Publish framework-dependent (requires .NET runtime installed)
dotnet publish ExplorerTabUtility/ExplorerTabUtility.csproj -c Release

# Publish with specific version and output directory
dotnet publish ExplorerTabUtility/ExplorerTabUtility.csproj -c Release -o ./publish -p:Version=2.5.1
```

### Testing Commands

```powershell
# Run tests (if test project exists)
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run tests in Release mode
dotnet test -c Release
```

### NuGet Commands

```powershell
# Restore packages
dotnet restore

# List installed packages
dotnet list ExplorerTabUtility/ExplorerTabUtility.csproj package

# Add a package
dotnet add ExplorerTabUtility/ExplorerTabUtility.csproj package PackageName

# Update packages
dotnet restore --force
```

### Git Commands

```powershell
# Check current status
git status

# View recent commits
git log --oneline -10

# Create a new branch
git checkout -b feature/your-feature-name

# Stage and commit changes
git add . && git commit -m "Your commit message"

# Push to remote
git push origin your-branch-name
```

---

## Technologies

### Core Technologies

| Technology | Version | Purpose |
|------------|---------|---------|
| .NET | 9.0 | Primary runtime |
| .NET Framework | 4.8.1 | Legacy support |
| C# | Latest (LangVersion) | Programming language |
| WPF | - | UI Framework |

### NuGet Packages

| Package | Version | Purpose |
|---------|---------|---------|
| H.Hooks | 1.7.0 | Keyboard hook implementation |
| Hardcodet.NotifyIcon.Wpf | 2.0.1 | System tray icon |
| Autoupdater.NET.Extended.Markdown | 1.9.5.2 | Auto-update functionality |
| ConfigureAwait.Fody | 3.3.2 | Async/await code weaving |
| Fody | 6.9.3 | IL code weaving |

### Windows COM Interfaces

| Interface | Purpose |
|-----------|---------|
| SHDocVw | Explorer window and tab management |
| Shell32 | Core shell functionality and file system operations |

### Development Tools

- **Visual Studio 2022** or **VS Code** with C# extension
- **.NET SDK 9.0**
- **Windows 11** for testing

---

## Project Structure

```
ExplorerTabUtility/
├── .github/                    # GitHub workflows and templates
│   ├── workflows/             # CI/CD pipelines
│   │   ├── build-release.yml  # Build and release workflow
│   │   ├── publish-winget.yml # Winget publishing
│   │   └── publish-chocolatey.yml
│   └── ISSUE_TEMPLATE/        # Issue templates
├── ExplorerTabUtility/         # Main project
│   ├── App.xaml               # Application entry point
│   ├── App.xaml.cs
│   ├── ExplorerTabUtility.csproj
│   ├── FodyWeavers.xml        # Fody configuration
│   ├── Helpers/               # Utility classes
│   │   ├── Constants.cs
│   │   ├── Helper.cs
│   │   ├── HotKeyActionJsonConverter.cs
│   │   ├── IntPtrConverter.cs
│   │   ├── KeyboardSimulator.cs
│   │   ├── ProcessWatcher.cs
│   │   ├── StaTaskScheduler.cs
│   ├── Hooks/                 # Input hooks
│   │   ├── ExplorerWatcher.cs
│   │   ├── IHook.cs
│   │   ├── Keyboard.cs
│   │   └── Mouse.cs
│   ├── Interop/               # COM interop
│   │   ├── IAccessible.cs
│   │   ├── IShellBrowser.cs
│   │   ├── IShellFolder.cs
│   │   ├── IServiceProvider.cs
│   │   └── ShellPathComparer.cs
│   ├── Managers/              # Core managers
│   │   ├── ClipboardManager.cs
│   │   ├── HookManager.cs
│   │   ├── ProfileManager.cs
│   │   ├── RegistryManager.cs
│   │   ├── SettingsManager.cs
│   │   └── UpdateManager.cs
│   ├── Models/                # Data models
│   │   ├── DualKeyDictionary.cs
│   │   ├── HotKeyAction.cs
│   │   ├── HotKeyEventArgs.cs
│   │   ├── HotKeyProfile.cs
│   │   ├── HotkeyScope.cs
│   │   ├── SupporterInfo.cs
│   │   ├── WindowInfo.cs
│   │   └── WindowRecord.cs
│   ├── Properties/            # Generated resources
│   │   ├── Resources.Designer.cs
│   │   ├── Resources.resx
│   │   ├── Settings.Designer.cs
│   │   └── Settings.settings
│   ├── UI/                    # User interface
│   │   ├── Behaviors/         # XAML behaviors
│   │   ├── Commands/          # Commands
│   │   ├── Converters/        # Value converters
│   │   ├── Themes/            # XAML themes/styles
│   │   └── Views/             # XAML views
│   ├── WinAPI/                # Windows API interop
│   │   ├── INPUT.cs
│   │   ├── RECT.cs
│   │   ├── VirtualKey.cs
│   │   ├── WinApi.cs
│   │   └── WinEventDelegate.cs
│   └── icon.ico               # Application icon
├── docs/                      # Documentation
│   └── README.md              # Full project documentation
├── packages/                  # Package templates
├── Assets/                    # Images and assets
├── LICENSE                    # MIT License
└── README.md                  # Project README
```

---

## Best Practices and Guidelines

### C# Development Guidelines

#### Code Style

1. **Use modern C# features** (targeting `LangVersion: latest`)
   - Pattern matching
   - Records (for immutable data)
   - Init-only setters
   - Target-typed `new` expressions

2. **Naming Conventions**
   - PascalCase for classes, methods, properties
   - camelCase for local variables and parameters
   - Prefix interfaces with `I` (e.g., `IHook`)
   - Use meaningful names (avoid single letters except in lambdas)

3. **Nullable Reference Types**
   - Enabled in project (`<Nullable>enable</Nullable>`)
   - Use `?` for nullable types
   - Use null-coalescing and null-conditional operators

#### Error Handling

1. **Use structured exception handling**
   ```csharp
   try
   {
       // Risky operation
   }
   catch (SpecificException ex)
   {
       // Handle specific exception
       Logger.Log(ex);
   }
   finally
   {
       // Cleanup
   }
   ```

2. **Avoid swallowing exceptions** - always log or handle appropriately
3. **Use `throw;` instead of `throw ex;`** to preserve stack trace

#### Async/Await Patterns

1. **Use ConfigureAwait.Fody** for automatic `ConfigureAwait(false)`
2. **Avoid blocking on async code** - use async all the way
3. **Use `ValueTask` for hot paths** where synchronous completion is likely

### WPF Development Guidelines

#### XAML Best Practices

1. **Separate concerns** - keep XAML for UI, code-behind for logic
2. **Use data binding** - avoid direct element manipulation
3. **Use MVVM pattern** - ViewModels for presentation logic
4. **Use resource dictionaries** - for shared styles and themes

#### Resource Organization

1. **Theme files** in `UI/Themes/`
2. **Converters** in `UI/Converters/`
3. **Behaviors** in `UI/Behaviors/`

#### UI Components

1. **Follow Windows 11 design language** - see theme files for styling
2. **Use built-in WPF components** when possible
3. **Handle DPI scaling** - use vector graphics when possible

### Windows API Guidelines

#### P/Invoke

1. **Use correct calling convention** (`CallingConvention.StdCall`)
2. **Use `Literal` for string parameters** in DllImport
3. **Handle `IntPtr` properly** - convert to/from managed types carefully
4. **Use `SafeHandle`** for resource handles

#### COM Interop

1. **Release COM objects** - use `Marshal.ReleaseComObject()`
2. **Use `using` statements** for disposable objects
3. **Handle COM exceptions** - `COMException`, `ExternalException`

### Performance Guidelines

1. **Use concurrent collections** - `ConcurrentDictionary`, `ConcurrentQueue`
2. **Cache expensive operations** - window handles, paths
3. **Minimize window recreation** - reuse existing windows
4. **Use STA scheduler** - for COM STA thread operations
5. **Profile before optimizing** - use Performance Profiler

### Security Guidelines

1. **Validate inputs** - especially from user/config
2. **Don't log sensitive data** - passwords, tokens
3. **Use secure storage** - for sensitive settings
4. **Validate file paths** - prevent path traversal

### Testing Guidelines

1. **Write unit tests** for utility functions
2. **Test edge cases** - null, empty, invalid inputs
3. **Use integration tests** for COM interop
4. **Mock external dependencies** - Windows API, COM objects

---

## Architecture Overview

### Application Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    Application Entry                        │
│                      (App.xaml.cs)                          │
└─────────────────────────────────────────────────────────────┘
                              │
         ┌────────────────────┼────────────────────┐
         ▼                    ▼                    ▼
┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐
│  UI Layer      │  │  Manager Layer  │  │  Hook Layer     │
│  (WPF Views)   │  │  (Core Logic)   │  │  (Input Hooks)  │
└─────────────────┘  └─────────────────┘  └─────────────────┘
         │                    │                    │
         └────────────────────┼────────────────────┘
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                  Interop/Platform Layer                     │
│         (Windows API + COM Interop)                        │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                  Windows Shell / Explorer                   │
└─────────────────────────────────────────────────────────────┘
```

### Key Components

| Component | Responsibility |
|-----------|----------------|
| **HookManager** | Manages keyboard and mouse hooks |
| **SettingsManager** | Handles application settings persistence |
| **ProfileManager** | Manages hotkey profiles |
| **WindowManager** | Tracks and manages Explorer windows |
| **ExplorerWatcher** | Monitors Explorer process events |

### Data Flow

1. **Hook Layer** captures keyboard/mouse input
2. **Manager Layer** processes input and determines action
3. **Interop Layer** communicates with Windows Shell
4. **UI Layer** displays settings, dialogs, notifications

### Settings Location

User settings are stored in:
```
%APPDATA%\ExplorerTabUtility\settings.json
```

---

## Common Tasks

### Adding a New Hotkey Action

1. Add action enum value in `Models/HotKeyAction.cs`
2. Implement action handler in `Managers/HookManager.cs`
3. Add UI support if needed in `UI/Views/`
4. Add tests if applicable

### Modifying Theme/Styles

1. Edit XAML files in `ExplorerTabUtility/UI/Themes/`
2. Update `Colors.xaml` for color changes
3. Add new styles in appropriate theme files
4. Test with Windows 11 dark/light modes

### Debugging Common Issues

1. **Window hook not working**: Check if Explorer process is running
2. **Hotkeys not firing**: Verify hook is registered, check scope (global vs explorer)
3. **Tab not opening**: Check COM connection to Shell
4. **Crash on startup**: Check settings file, try deleting to reset

---

## Troubleshooting

### Build Issues

| Error | Solution |
|-------|----------|
| Missing SDK | Install .NET 9.0 SDK |
| COM reference errors | Rebuild solution, check TLBIMP |
| Fody weaving errors | Check `FodyWeavers.xml` configuration |

### Runtime Issues

| Issue | Solution |
|-------|----------|
| App not starting | Check logs in `%TEMP%\ExplorerTabUtility` |
| Tray icon missing | Check if running, check notification settings |
| Hotkeys not working | Run as administrator, check hook status |

---

## Additional Resources

- [Project Documentation](docs/README.md)
- [Windows Shell Documentation](https://docs.microsoft.com/en-us/windows/win32/shell/shell-samples)
- [WPF Documentation](https://docs.microsoft.com/en-us/dotnet/wpf/)
- [.NET Documentation](https://docs.microsoft.com/en-us/dotnet/)

---

*Last Updated: 2025-05-14*