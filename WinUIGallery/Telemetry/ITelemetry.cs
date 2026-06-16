// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using WinUIGallery.Telemetry.Events;

namespace WinUIGallery.Telemetry;

internal interface ITelemetry
{
    /// <summary>
    /// Add a string that we should try stripping out of some of our telemetry for sensitivity reasons (ex. VM name, etc.).
    /// We can never be 100% sure we can remove every string, but this should greatly reduce us collecting PII.
    /// Note that the order in which AddSensitive is called matters, as later when we call ReplaceSensitiveStrings, it will try
    /// finding and replacing the earlier strings first.  This can be helpful, since we can target specific
    /// strings (like username) first, which should help preserve more information helpful for diagnosis.
    /// </summary>
    /// <param name="name">Sensitive string to add (ex. "c:\xyz")</param>
    /// <param name="replaceWith">string to replace it with (ex. "-path-")</param>
    public void AddSensitiveString(string name, string replaceWith);

    /// <summary>
    /// Gets a value indicating whether telemetry is on.
    /// For future use if we add a registry key or some other setting to check if telemetry is turned on.
    /// </summary>
    public bool IsTelemetryOn { get; }

    /// <summary>
    /// Gets or sets a value indicating whether diagnostic telemetry is on.
    /// </summary>
    public bool IsDiagnosticTelemetryOn { get; set; }

    /// <summary>
    /// Logs an exception at Critical level.
    /// </summary>
    /// <param name="action">What we were trying to do when the exception occurred.</param>
    /// <param name="e">Exception object.</param>
    /// <param name="relatedActivityId">Optional relatedActivityId which allows correlating this telemetry with other telemetry in the same action/activity.</param>
    public void LogException(string action, Exception e, Guid? relatedActivityId = null);

    /// <summary>
    /// Logs an event with no additional data at Critical level.
    /// </summary>
    public void LogCritical(string eventName, bool isError = false, Guid? relatedActivityId = null);

    /// <summary>
    /// Logs an informational event.
    /// </summary>
    public void Log<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(string eventName, LogLevel level, T data, Guid? relatedActivityId = null)
        where T : EventBase;

    /// <summary>
    /// Logs an error event.
    /// </summary>
    public void LogError<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(string eventName, LogLevel level, T data, Guid? relatedActivityId = null)
        where T : EventBase;
}
