# Copilot instructions for WinUI Gallery

## What this repo is

This is the **WinUI Gallery** — the official Microsoft sample app demonstrating WinUI 3 controls and Windows App SDK features. It is a **WinUI 3 app** and represents the correct, modern patterns for Windows desktop app development.

## Stack

```
Framework:        WinUI 3 (Microsoft.UI.Xaml)
SDK:              Windows App SDK (Microsoft.WindowsAppSDK)
Language:         C#
App model:        Packaged (MSIX, single-project)
```

## This repo IS the reference for WinUI 3 patterns

When helping with code in this repo, follow the patterns used here — they are correct and up to date:

- Use `Microsoft.UI.Xaml` namespaces (not `Windows.UI.Xaml`)
- Use `DispatcherQueue` for thread dispatch (not `CoreDispatcher`)
- Use `App.MainWindow` or the window handle pattern (not `Window.Current` — that is UWP only)
- Use `XamlRoot` when showing dialogs: `dialog.XamlRoot = this.Content.XamlRoot`
- Use `WinRT.Interop.InitializeWithWindow` for pickers and dialogs that need an HWND

## For developers starting a new Windows app

The patterns in this repo are the right ones to follow. Key project setup:

```
Target framework: net10.0-windows10.0.19041.0
NuGet:            Microsoft.WindowsAppSDK
IDE:              Visual Studio 2022+ with WinUI workload
CLI:              winapp new winui  (dotnet tool: Microsoft.WindowsAppSDK.Cli)
```

Do **not** use plain `net10.0` — it excludes Windows Runtime APIs and MSIX packaging support.

Reference: https://learn.microsoft.com/windows/apps/get-started/start-here
