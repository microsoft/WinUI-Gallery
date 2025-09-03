// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;

namespace AIDevGallery.Telemetry;

/// <summary>
/// Creates instance of Telemetry
/// This would be useful for the future when interfaces have been updated for logger like ITelemetry2, ITelemetry3 and so on
/// </summary>
internal class TelemetryFactory
{
    private static readonly Lock LockObj = new();

    private static Telemetry? telemetryInstance;

    private static Telemetry GetTelemetryInstance()
    {
        if (telemetryInstance == null)
        {
            lock (LockObj)
            {
                telemetryInstance ??= new Telemetry();
                telemetryInstance.AddWellKnownSensitiveStrings();
            }
        }

        return telemetryInstance;
    }

    /// <summary>
    /// Gets a singleton instance of Telemetry
    /// This would be useful for the future when interfaces have been updated for logger like ITelemetry2, ITelemetry3 and so on
    /// </summary>
    /// <typeparam name="T">The type of telemetry interface.</typeparam>
    /// <returns>A singleton instance of the specified telemetry interface.</returns>
    public static T Get<T>()
        where T : ITelemetry
    {
        return (T)(object)GetTelemetryInstance();
    }
}