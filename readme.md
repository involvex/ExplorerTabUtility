# Explorer Tab Utility

Force new File Explorer windows to open as tabs on Windows 11 — plus hotkeys, tab search, and session restore for a cleaner file management workflow.

> **Requirements:** Windows 11 (22H2 Build 22621+) · Full documentation: [`docs/README.md`](docs/README.md)

## Features

- **Window → tab conversion** — new Explorer windows automatically become tabs; existing tabs are reused when the path is already open
- **Close tab** — configurable hotkey (e.g. `Ctrl+W`) and middle-click on the tab bar to close the active tab
- **Duplicate / detach / reopen closed tabs** — with location and file selection preserved
- **Fuzzy tab search** — popup to find and switch tabs; tolerant to partial input (`dwnlds` → Downloads), multi-word queries, and typos
- **Hotkey profiles** — keyboard and mouse bindings with Global / File-Explorer scopes, import/export as JSON
- **Session persistence** — closed-tab history and previous-window restore across restarts and Explorer crashes
- **System tray** — quick toggles, per-profile menu, optional hidden icon with hotkey to reshow
- **Auto-update** — checks GitHub releases on launch (optional)

## Build

COM references (`SHDocVw`, `Shell32`) require MSBuild — `dotnet build` alone is not supported for the app project.

```powershell
.\build.ps1
```

Manual equivalent (adjust the VS path via `vswhere` if needed):

```powershell
& "<VS>\MSBuild\Current\Bin\MSBuild.exe" ExplorerTabUtility.sln /t:Rebuild /p:Configuration=Release
```

## Test

Pure-logic unit tests (fuzzy matcher, hotkey serialization, collections) run on .NET 9:

```powershell
dotnet test ExplorerTabUtility.Tests/ExplorerTabUtility.Tests.csproj -c Release
```

CI runs the same suite on every push (`test` job in `build-release.yml`); releases are built by MSBuild for `net481` / `net9.0-windows` across `x64` / `x86` / `arm64`.

## Project layout

```
ExplorerTabUtility/          # WPF app (net481)
  Helpers/  Hooks/  Interop/
  Managers/ Models/  UI/  WinAPI/
ExplorerTabUtility.Tests/    # xUnit tests (net9.0-windows, links pure-logic sources)
docs/README.md               # Full user documentation
suggestions.md               # Feature ideas and roadmap notes
```

## Settings

Stored in `%APPDATA%\ExplorerTabUtility\settings.json` (hotkey profiles, closed-tab history, window size). Delete it to reset to defaults.
