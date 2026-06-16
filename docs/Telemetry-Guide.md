# WinUI Gallery Telemetry Guide

This guide summarizes the telemetry implementation in this repo, the event catalog and how to view it locally.

---

## Overview

- **Provider name**: `Microsoft.Windows.WinUIGallery`
- **Implementation**: `EventSource` with Windows ETW TraceLogging
- **Code locations**:
  - `WinUIGallery/Telemetry/Telemetry.cs` (core logic)
  - `WinUIGallery/Telemetry/TelemetryFactory.cs` (singleton factory)
  - `WinUIGallery/Telemetry/ITelemetry.cs`, `WinUIGallery/Telemetry/LogLevel.cs`
  - `WinUIGallery/Telemetry/TelemetryEventSource.cs` (keywords/tags)
  - `WinUIGallery/Telemetry/PrivacyConsentHelpers.cs` (privacy-sensitive regions)
  - `WinUIGallery/Telemetry/Events/*.cs` (event data contracts and logging entry points)

---

## Collection and write flow

1. Product code calls static `Log(...)` methods defined under `Telemetry/Events/*.cs`; parameters become the public properties on the event type.
2. Calls go through the `ITelemetry` implementation (`Telemetry.cs`), which performs sensitive string replacement (see "Sensitive information handling").
3. Based on `LogLevel` and `IsDiagnosticTelemetryOn`, events may upload or be logged locally only:
   - When `IsDiagnosticTelemetryOn == false`, non-error `Info`/`Measure` events are downgraded to `Local` (local-only).
   - `Critical` and error-level events are not downgraded.
4. Finally, events are written via `EventSource.Write(...)` using TraceLogging:
   - Provider: `Microsoft.Windows.WinUIGallery`
   - Keywords: `TelemetryKeyword` / `MeasuresKeyword` / `CriticalDataKeyword`

---

## View telemetry locally (debug/verification)

### Method 1: PerfView (recommended)

1. Run PerfView as Administrator.
2. Collect → Collect:
   - Uncheck Thread Time, CPU, .NET/CLR, Kernel (set Kernel Events to None under Advanced).
   - In Additional Providers, manually enter:
     - `Microsoft.Windows.WinUIGallery:0xFFFFFFFFFFFFFFFF:Verbose`
3. Start → exercise the app → Stop.
4. Open the produced etl.zip → Events; filter by Provider `Microsoft.Windows.WinUIGallery` to inspect events and their public properties.

Tip: If the provider is not in the dropdown, manually type the provider string above.

### Method 2: PerfView Listen (live)

Run → Listen → Providers `Microsoft.Windows.WinUIGallery:0xFFFFFFFFFFFFFFFF:Verbose` → Start → exercise the app to see live events.

### Method 3: logman (command line, minimal)

```bat
logman start WinUIGalleryOnly -p Microsoft.Windows.WinUIGallery 0xFFFFFFFFFFFFFFFF 5 -ets
rem After exercising the app
logman stop WinUIGalleryOnly -ets
```

The resulting `.etl` can be opened with PerfView/WPA; or convert to CSV via `tracerpt`.

### Method 4: dotnet-trace (attach to process)

```bat
dotnet-trace collect --process-id <PID> --providers Microsoft.Windows.WinUIGallery:0xFFFFFFFFFFFFFFFF:Verbose
```

---

## Event catalog

- Navigation (Log)
  - `NavigatedToPage_Event` — every top-level page navigation
  - `NavigatedToSample_Event` — when an individual control sample is loaded inside `ItemPage`
- Search (Log)
  - `SearchSample_Event` — when the user submits a query in the search box
- Activation (Log)
  - `ProtocolActivated_Event` — when the app is launched via a `winui3gallery://` deep link
- Interaction (Log)
  - `ButtonClicked_Event` — generic button-click event; opt-in per call site
- Exceptions (LogException → event name `ExceptionThrown`)
  - Wired into `App.HandleExceptions`; includes exception type name, message, and stack trace with PII scrubbed.

---

## Event data & sensitive information handling

- Event "public properties" are the fields written; they are defined on `[EventData]` classes under `Telemetry/Events/*.cs`.
- On singleton initialization, "well-known sensitive strings" are added for replacement:
  - User directory (e.g., `C:\Users\<name>` → `<UserDirectory>`)
  - User name / current user (→ `<UserName>` / `<CurrentUserName>`)
- `Log/LogError` call `ReplaceSensitiveStrings(...)` on the event. Most events implement a no-op replacement; free-form input fields such as `Query` and `Uri` are scrubbed for known sensitive substrings.
- `LogException` replaces sensitive substrings in `Message` and `InnerException.Message`; type names and stack traces are recorded as-is for diagnostics.

---

## Region and diagnostic switch

- `SettingsHelper.Current.IsDiagnosticDataEnabled` default: `true` for non-privacy-sensitive regions; `false` for privacy-sensitive regions (see `PrivacyConsentHelpers.IsPrivacySensitiveRegion()`).
- On app start, its value is assigned to `Telemetry.IsDiagnosticTelemetryOn`:
  - Non-error `Info`/`Measure` events downgrade to `Local` when `false`.
  - `Critical` and error events are not downgraded.
- Users in privacy-sensitive regions see a one-time consent `InfoBar` on the HomePage; they can also toggle the setting at any time from the Settings page.

---

## FAQ

- Q: The PerfView provider list does not show `Microsoft.Windows.WinUIGallery`.
  - A: This is expected. EventSource-based custom providers often don't appear in the dropdown; manually type `Microsoft.Windows.WinUIGallery:0xFFFFFFFFFFFFFFFF:Verbose` in Additional Providers.
- Q: I see other providers' events in the etl.
  - A: Additional Providers is an "add" subscription; PerfView also captures Kernel/CLR by default. Disable defaults (Kernel=None) before collection, or filter by Provider while viewing.

---

## Change and maintenance

- Adding a new event: create a type under `Telemetry/Events/` with `[EventData]`; public properties become event fields. Provide `public static void Log(...)` and use `TelemetryFactory.Get<ITelemetry>()` to write.
- Changing event fields can impact downstream processing/queries; avoid when possible. If required, update release notes and this document accordingly.
