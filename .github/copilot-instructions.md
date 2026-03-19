# Copilot instructions for XAML Controls Gallery

## What this repo is

This is the **XAML Controls Gallery** — a UWP sample app that demonstrates WinUI 2 controls (`Microsoft.UI.Xaml` NuGet package). It targets the Universal Windows Platform (UWP), not the Windows App SDK.

## Important: this is NOT a WinUI 3 app

| This repo | For new apps |
|---|---|
| WinUI 2 (`Microsoft.UI.Xaml` NuGet) | WinUI 3 (Windows App SDK) |
| UWP app model | Windows App SDK app model |
| `package.appxmanifest` + UWP project | Single-project MSIX or unpackaged |
| `Windows.UI.Xaml` + `Microsoft.UI.Xaml` | `Microsoft.UI.Xaml` only |

**If the user is building a new Windows app**, do not use this repo's patterns as the template. Direct them to WinUI 3 instead.

## When helping with code in this repo

- This app uses UWP APIs (`Windows.UI.*`) — that is correct for this codebase
- `Window.Current` is valid here (UWP) — it is NOT valid in WinUI 3
- `CoreDispatcher` is used here — WinUI 3 uses `DispatcherQueue` instead
- The project type is UWP, so MSIX packaging works differently than in Windows App SDK apps
- Do not suggest migrating this app to WinUI 3 unless the user explicitly asks

## For new Windows app development

If the user is starting a new Windows desktop app, the correct stack is:

```
Framework:        WinUI 3
SDK:              Windows App SDK (NuGet: Microsoft.WindowsAppSDK)
Target framework: net10.0-windows10.0.19041.0
IDE:              Visual Studio 2022+ with WinUI workload
CLI alternative:  winapp new winui (dotnet tool: Microsoft.WindowsAppSDK.Cli)
```

Reference: https://learn.microsoft.com/windows/apps/get-started/start-here
