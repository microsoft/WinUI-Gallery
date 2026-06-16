// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;

namespace WinUIGallery.Telemetry;

/// <summary>
/// Creates the singleton instance of <see cref="Telemetry"/>.
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
                if (telemetryInstance == null)
                {
                    telemetryInstance = new Telemetry();
                    telemetryInstance.AddWellKnownSensitiveStrings();
                }
            }
        }

        return telemetryInstance;
    }

    /// <summary>
    /// Gets a singleton instance of <see cref="ITelemetry"/>.
    /// </summary>
    public static T Get<T>()
        where T : ITelemetry
    {
        return (T)(object)GetTelemetryInstance();
    }
}
