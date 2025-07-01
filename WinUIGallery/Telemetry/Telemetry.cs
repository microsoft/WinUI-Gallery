// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Diagnostics.Telemetry;
using Microsoft.Diagnostics.Telemetry.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;

namespace AIDevGallery.Telemetry;

/// <summary>
/// To create an instance call TelemetryFactory.Get&lt;ITelemetry&gt;()
/// </summary>
internal sealed class Telemetry : ITelemetry
{
    private const string ProviderName = "Microsoft.Windows.AIDevGallery";

    /// <summary>
    /// Time Taken Event Name
    /// </summary>
    private const string TimeTakenEventName = "TimeTaken";

    /// <summary>
    /// Exception Thrown Event Name
    /// </summary>
    private const string ExceptionThrownEventName = "ExceptionThrown";

    private static readonly Guid DefaultRelatedActivityId = Guid.Empty;

    /// <summary>
    /// Can only have one EventSource alive per process, so just create one statically.
    /// </summary>
    private static readonly EventSource TelemetryEventSourceInstance = new TelemetryEventSource(ProviderName);

    /// <summary>
    /// Logs telemetry locally, but shouldn't upload it.  Similar to an ETW event.
    /// Should be the same as EventSourceOptions(), as Verbose is the default level.
    /// </summary>
    private static readonly EventSourceOptions LocalOption = new() { Level = EventLevel.Verbose };

    /// <summary>
    /// Logs error telemetry locally, but shouldn't upload it.  Similar to an ETW event.
    /// </summary>
    private static readonly EventSourceOptions LocalErrorOption = new() { Level = EventLevel.Error };

    /// <summary>
    /// Logs telemetry.
    /// Currently this is at 0% sampling for both internal and external retail devices.
    /// </summary>
    private static readonly EventSourceOptions InfoOption = new() { Keywords = TelemetryEventSource.TelemetryKeyword };

    /// <summary>
    /// Logs error telemetry.
    /// Currently this is at 0% sampling for both internal and external retail devices.
    /// </summary>
    private static readonly EventSourceOptions InfoErrorOption = new() { Level = EventLevel.Error, Keywords = TelemetryEventSource.TelemetryKeyword };

    /// <summary>
    /// Logs measure telemetry.
    /// This should be sent back on internal devices, and a small, sampled % of external retail devices.
    /// </summary>
    private static readonly EventSourceOptions MeasureOption = new() { Keywords = TelemetryEventSource.MeasuresKeyword };

    /// <summary>
    /// Logs measure error telemetry.
    /// This should be sent back on internal devices, and a small, sampled % of external retail devices.
    /// </summary>
    private static readonly EventSourceOptions MeasureErrorOption = new() { Level = EventLevel.Error, Keywords = TelemetryEventSource.MeasuresKeyword };

    /// <summary>
    /// Logs critical telemetry.
    /// This should be sent back on all devices sampled at 100%.
    /// </summary>
    private static readonly EventSourceOptions CriticalDataOption = new() { Keywords = TelemetryEventSource.CriticalDataKeyword };

    /// <summary>
    /// Logs critical error telemetry.
    /// This should be sent back on all devices sampled at 100%.
    /// </summary>
    private static readonly EventSourceOptions CriticalDataErrorOption = new() { Level = EventLevel.Error, Keywords = TelemetryEventSource.CriticalDataKeyword };

    /// <summary>
    /// ActivityId so we can correlate all events in the same run
    /// </summary>
    private static Guid activityId = Guid.NewGuid();

    /// <summary>
    /// List of strings we should try removing for sensitivity reasons.
    /// </summary>
    private readonly List<KeyValuePair<string, string>> sensitiveStrings = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="Telemetry"/> class.
    /// Prevents a default instance of the Telemetry class from being created.
    /// </summary>
    internal Telemetry()
    {
    }

    /// <summary>
    /// Gets a value indicating whether telemetry is on
    /// For future use if we add a registry key or some other setting to check if telemetry is turned on.
    /// </summary>
    public bool IsTelemetryOn { get; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether diagnostic telemetry is on.
    /// </summary>
    public bool IsDiagnosticTelemetryOn { get; set; }

    /// <summary>
    /// Add a string that we should try stripping out of some of our telemetry for sensitivity reasons (ex. VM name, etc.).
    /// We can never be 100% sure we can remove every string, but this should greatly reduce us collecting PII.
    /// Note that the order in which AddSensitive is called matters, as later when we call ReplaceSensitiveStrings, it will try
    /// finding and replacing the earlier strings first.  This can be helpful, since we can target specific
    /// strings (like username) first, which should help preserve more information helpful for diagnosis.
    /// </summary>
    /// <param name="name">Sensitive string to add (ex. "c:\xyz")</param>
    /// <param name="replaceWith">string to replace it with (ex. "-path-")</param>
    public void AddSensitiveString(string name, string replaceWith)
    {
        // Make sure the name isn't blank, hasn't already been added, and is greater than three characters.
        // Otherwise they could name their VM "a", and then we would end up replacing every "a" with another string.
        if (!string.IsNullOrWhiteSpace(name) && name.Length > 3 && !this.sensitiveStrings.Exists(item => name.Equals(item.Key, StringComparison.Ordinal)))
        {
            this.sensitiveStrings.Add(new KeyValuePair<string, string>(name, replaceWith ?? string.Empty));
        }
    }

    /// <summary>
    /// Logs an exception at Measure level. To log at Critical level, the event name needs approval.
    /// </summary>
    /// <param name="action">What we trying to do when the exception occurred.</param>
    /// <param name="e">Exception object</param>
    /// <param name="relatedActivityId">Optional relatedActivityId which will allow to correlate this telemetry with other telemetry in the same action/activity or thread and corelate them</param>
    public void LogException(string action, Exception e, Guid? relatedActivityId = null)
    {
        var innerMessage = this.ReplaceSensitiveStrings(e.InnerException?.Message);
        StringBuilder innerStackTrace = new();
        Exception? innerException = e.InnerException;
        while (innerException != null)
        {
            innerStackTrace.Append(innerException.StackTrace);

            // Separating by 2 new lines to distinguish between different exceptions.
            innerStackTrace.AppendLine();
            innerStackTrace.AppendLine();
            innerException = innerException.InnerException;
        }

        this.LogInternal(
            ExceptionThrownEventName,
            LogLevel.Critical,
            new
            {
                action,
                name = e.GetType().Name,
                stackTrace = e.StackTrace,
                innerName = e.InnerException?.GetType().Name,
                innerMessage,
                innerStackTrace = innerStackTrace.ToString(),
                message = this.ReplaceSensitiveStrings(e.Message),
                PartA_PrivTags = PartA_PrivTags.ProductAndServicePerformance,
            },
            relatedActivityId,
            isError: true);
    }

    /// <summary>
    /// Log the time an action took (ex. deploy time).
    /// </summary>
    /// <param name="eventName">The measurement we're performing (ex. "DeployTime").</param>
    /// <param name="timeTakenMilliseconds">How long the action took in milliseconds.</param>
    /// <param name="relatedActivityId">Optional relatedActivityId which will allow to correlate this telemetry with other telemetry in the same action/activity or thread and corelate them</param>
    public void LogTimeTaken(string eventName, uint timeTakenMilliseconds, Guid? relatedActivityId = null)
    {
        this.LogInternal(
            TimeTakenEventName,
            LogLevel.Critical,
            new
            {
                eventName,
                timeTakenMilliseconds,
                PartA_PrivTags = PartA_PrivTags.ProductAndServicePerformance,
            },
            relatedActivityId,
            isError: false);
    }

    /// <summary>
    /// Log an informal event with no additional data at log level measure.
    /// </summary>
    /// <param name="eventName">The name of the event to log</param>
    /// <param name="isError">Set to true if an error condition raised this event.</param>
    /// <param name="relatedActivityId">GUID to correlate activities.</param>
    public void LogCritical(string eventName, bool isError = false, Guid? relatedActivityId = null)
    {
        this.LogInternal(eventName, LogLevel.Critical, new EmptyEvent(PartA_PrivTags.ProductAndServiceUsage), relatedActivityId, isError);
    }

    /// <summary>
    /// Log an informational event. Typically used for just a single event that's only called one place in the code.
    /// </summary>
    /// <param name="eventName">Name of the error event</param>
    /// <param name="level">Determines whether to upload the data to our servers, and on how many machines.</param>
    /// <param name="data">Values to send to the telemetry system.</param>
    /// <param name="relatedActivityId">Optional relatedActivityId which will allow to correlate this telemetry with other telemetry in the same action/activity or thread and correlate them</param>
    /// <typeparam name="T">Anonymous type.</typeparam>
    public void Log<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(string eventName, LogLevel level, T data, Guid? relatedActivityId = null)
        where T : EventBase
    {
        data.ReplaceSensitiveStrings(this.ReplaceSensitiveStrings);
        this.LogInternal(eventName, level, data, relatedActivityId, isError: false);
    }

    /// <summary>
    /// Log an error event. Typically used for just a single event that's only called one place in the code.
    /// </summary>
    /// <param name="eventName">Name of the error event</param>
    /// <param name="level">Determines whether to upload the data to our servers, and on how many machines.</param>
    /// <param name="data">Values to send to the telemetry system.</param>
    /// <param name="relatedActivityId">Optional relatedActivityId which will allow to correlate this telemetry with other telemetry in the same action/activity or thread and correlate them</param>
    /// <typeparam name="T">Anonymous type.</typeparam>
    public void LogError<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(string eventName, LogLevel level, T data, Guid? relatedActivityId = null)
        where T : EventBase
    {
        data.ReplaceSensitiveStrings(this.ReplaceSensitiveStrings);
        this.LogInternal(eventName, level, data, relatedActivityId, isError: true);
    }

    private void LogInternal<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(string eventName, LogLevel level, T data, Guid? relatedActivityId, bool isError)
    {
        this.WriteTelemetryEvent(eventName, level, relatedActivityId ?? DefaultRelatedActivityId, isError, data);
    }

    /// <summary>
    /// Replaces sensitive strings in a string with non sensitive strings.
    /// </summary>
    /// <param name="message">Before, unstripped string.</param>
    /// <returns>After, stripped string</returns>
    private string? ReplaceSensitiveStrings(string? message)
    {
        if (message != null)
        {
            foreach (KeyValuePair<string, string> pair in this.sensitiveStrings)
            {
                // There's no String.Replace() with case insensitivity.
                // We could use Regular Expressions here for searching for case-insensitive string matches,
                // but it's not easy to specify the RegEx timeout value for .net 4.0.  And we were worried
                // about rare cases where the user could accidentally lock us up with RegEx, since we're using strings
                // provided by the user, so just use a simple non-RegEx replacement algorithm instead.
                var sb = new StringBuilder();
                var i = 0;
                while (true)
                {
                    // Find the string to strip out.
                    var foundPosition = message.IndexOf(pair.Key, i, StringComparison.OrdinalIgnoreCase);
                    if (foundPosition < 0)
                    {
                        sb.Append(message, i, message.Length - i);
                        message = sb.ToString();
                        break;
                    }

                    // Replace the string.
                    sb.Append(message, i, foundPosition - i);
                    sb.Append(pair.Value);
                    i = foundPosition + pair.Key.Length;
                }
            }
        }

        return message;
    }

    /// <summary>
    /// Writes the telemetry event info using the TraceLogging API.
    /// </summary>
    /// <typeparam name="T">Anonymous type.</typeparam>
    /// <param name="eventName">Name of the event.</param>
    /// <param name="level">Determines whether to upload the data to our servers, and the sample set of host machines.</param>
    /// <param name="relatedActivityId">GUID to correlate activities.</param>
    /// <param name="isError">Set to true if an error condition raised this event.</param>
    /// <param name="data">Values to send to the telemetry system.</param>
    private void WriteTelemetryEvent<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(string eventName, LogLevel level, Guid relatedActivityId, bool isError, T data)
    {
        EventSourceOptions telemetryOptions;
        if (this.IsTelemetryOn)
        {
            if (!IsDiagnosticTelemetryOn)
            {
                if (!isError && (level == LogLevel.Measure || level == LogLevel.Info))
                {
                    level = LogLevel.Local;
                }
            }

            switch (level)
            {
                case LogLevel.Critical:
                    telemetryOptions = isError ? Telemetry.CriticalDataErrorOption : Telemetry.CriticalDataOption;
                    break;
                case LogLevel.Measure:
                    telemetryOptions = isError ? Telemetry.MeasureErrorOption : Telemetry.MeasureOption;
                    break;
                case LogLevel.Info:
                    telemetryOptions = isError ? Telemetry.InfoErrorOption : Telemetry.InfoOption;
                    break;
                case LogLevel.Local:
                default:
                    telemetryOptions = isError ? Telemetry.LocalErrorOption : Telemetry.LocalOption;
                    break;
            }
        }
        else
        {
            // The telemetry is not turned on, downgrade to local telemetry
            telemetryOptions = isError ? Telemetry.LocalErrorOption : Telemetry.LocalOption;
        }
#pragma warning disable IL2026
        TelemetryEventSourceInstance.Write(eventName, ref telemetryOptions, ref activityId, ref relatedActivityId, ref data);
#pragma warning restore IL2026
    }

    internal void AddWellKnownSensitiveStrings()
    {
        try
        {
            // This should convert "c:\users\johndoe" to "<UserProfile>".
            var userDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            this.AddSensitiveString(userDirectory.ToString(), "<UserDirectory>");

            // Include both these names, since they should cover the logged on user, and the user who is running the tools built on top of these API's
            // These names should almost always be the same, but technically could be different.
            this.AddSensitiveString(Environment.UserName, "<UserName>");
            var currentUserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\').Last();
            this.AddSensitiveString(currentUserName, "<CurrentUserName>");
        }
        catch (Exception e)
        {
            // Catch and log exception
            this.LogException("AddSensitiveStrings", e);
        }
    }
}