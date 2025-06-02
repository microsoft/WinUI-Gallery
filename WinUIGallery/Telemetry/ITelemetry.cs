// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;

namespace AIDevGallery.Telemetry;

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
    /// Gets a value indicating whether telemetry is on
    /// For future use if we add a registry key or some other setting to check if telemetry is turned on.
    /// </summary>
    public bool IsTelemetryOn { get; }

    /// <summary>
    /// Gets or sets a value indicating whether diagnostic telemetry is on.
    /// </summary>
    public bool IsDiagnosticTelemetryOn { get; set; }

    /// <summary>
    /// Logs an exception at Measure level. To log at Critical level, the event name needs approval.
    /// </summary>
    /// <param name="action">What we trying to do when the exception occurred.</param>
    /// <param name="e">Exception object</param>
    /// <param name="relatedActivityId">Optional relatedActivityId which will allow to correlate this telemetry with other telemetry in the same action/activity or thread and correlate them</param>
    public void LogException(string action, Exception e, Guid? relatedActivityId = null);

    /// <summary>
    /// Log the time an action took (ex. time spent on a tool).
    /// </summary>
    /// <param name="eventName">The measurement we're performing (ex. "DeployTime").</param>
    /// <param name="timeTakenMilliseconds">How long the action took in milliseconds.</param>
    /// <param name="relatedActivityId">Optional relatedActivityId which will allow to correlate this telemetry with other telemetry in the same action/activity or thread and correlate them</param>
    public void LogTimeTaken(string eventName, uint timeTakenMilliseconds, Guid? relatedActivityId = null);

    /// <summary>
    /// Log an event with no additional data.
    /// </summary>
    /// <param name="eventName">The name of the event to log</param>
    /// <param name="isError">Set to true if an error condition raised this event.</param>
    /// <param name="relatedActivityId">Optional relatedActivityId which will allow to correlate this telemetry with other telemetry in the same action/activity or thread and correlate them</param>
    public void LogCritical(string eventName, bool isError = false, Guid? relatedActivityId = null);

    /// <summary>
    /// Log an informational event. Typically used for just a single event that's only called one place in the code.
    /// If you are logging the same event multiple times, it's best to add a helper method in Telemetry
    /// </summary>
    /// <param name="eventName">Name of the error event</param>
    /// <param name="level">Determines whether to upload the data to our servers, and on how many machines.</param>
    /// <param name="data">Values to send to the telemetry system.</param>
    /// <param name="relatedActivityId">Optional relatedActivityId which will allow to correlate this telemetry with other telemetry in the same action/activity or thread and correlate them</param>
    /// <typeparam name="T">Anonymous type.</typeparam>
    public void Log<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(string eventName, LogLevel level, T data, Guid? relatedActivityId = null)
        where T : EventBase;

    /// <summary>
    /// Log an error event. Typically used for just a single event that's only called one place in the code.
    /// If you are logging the same event multiple times, it's best to add a helper method in Telemetry
    /// </summary>
    /// <param name="eventName">Name of the error event</param>
    /// <param name="level">Determines whether to upload the data to our servers, and on how many machines.</param>
    /// <param name="data">Values to send to the telemetry system.</param>
    /// <param name="relatedActivityId">Optional relatedActivityId which will allow to correlate this telemetry with other telemetry in the same action/activity or thread and correlate them</param>
    /// <typeparam name="T">Anonymous type.</typeparam>
    public void LogError<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(string eventName, LogLevel level, T data, Guid? relatedActivityId = null)
        where T : EventBase;
}