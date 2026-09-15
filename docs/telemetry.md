# WinUI Gallery telemetry

Official releases of WinUI Gallery collect limited usage data to help Microsoft understand which Windows App SDK samples are most useful.

## Data collected

When you open a sample page, WinUI Gallery records:

- The sample identifier from the catalog included with the app.
- The WinUI Gallery app version.

If WinUI Gallery encounters an unhandled exception, it may also record:

- The active sample identifier, when the exception occurred on a sample page.
- The exception type and error code.
- A sanitized stack trace limited to WinUI Gallery, WinUI, Windows App SDK, Windows projection, and WinRT interop code, without source-file paths.
- The app version and time of the exception.

The app does not include exception messages, search text, sample titles or descriptions, file paths, navigation history, account information, or app-generated user, device, installation, or session identifiers in these events. Navigation values are recorded only when they exactly match a sample identifier in the bundled catalog.

Home, category, All Controls, and Settings page visits are not recorded.

## Turn telemetry off

Telemetry is enabled by default in official releases, except in regions where WinUI Gallery asks for permission before enabling it. In those regions, the app displays an **Allow** or **Don't allow** choice on the Home page and does not send app telemetry unless **Allow** is selected.

To turn telemetry off:

1. Open **Settings** in WinUI Gallery.
2. Turn off **Optional diagnostic data**.

The change takes effect immediately and is remembered for future sessions.
Pending crash diagnostics are deleted when telemetry is turned off.

Crash diagnostics are first saved locally and then submitted to the Windows telemetry pipeline. If that pipeline is unavailable, WinUI Gallery retries the pending record on the next launch. The local record expires after seven days and is deleted after it is submitted.

## Developer builds

Versions built from source do not configure the production telemetry channel and do not upload this app telemetry.

For more information about how Microsoft handles data, see the [Microsoft Privacy Statement](https://go.microsoft.com/fwlink/?LinkId=521839).