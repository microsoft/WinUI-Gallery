// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#pragma warning disable CA1707 // Identifiers should not contain underscores
#pragma warning disable SA1604 // Element documentation should have summary
#pragma warning disable SA1642 // Constructor summary documentation should begin with standard text
#pragma warning disable SA1625 // Element documentation should not be copied and pasted

#if TELEMETRYEVENTSOURCE_USE_NUGET
using Microsoft.Diagnostics.Tracing;
#else
using System.Diagnostics.Tracing;
#endif
using System;
using SuppressMessageAttribute = System.Diagnostics.CodeAnalysis.SuppressMessageAttribute;

#pragma warning disable 3021 // 'type' does not need a CLSCompliant attribute

namespace Microsoft.Diagnostics.Telemetry
{
    /// <summary>
    /// <para>
    /// An EventSource with extra methods and constants commonly used in Microsoft's
    /// TraceLogging-based ETW. This class inherits from EventSource, and is exactly
    /// the same as EventSource except that it always enables
    /// EtwSelfDescribingEventFormat and never uses traits. It also provides several
    /// constants and helpers commonly used by Microsoft code.
    /// </para>
    /// <para>
    /// Different versions of this class use different provider traits. The provider
    /// traits in this class are empty. As a result, providers using this class will
    /// not join any ETW Provider Groups and will not be given any special treatment
    /// by group-sensitive ETW listeners.
    /// </para>
    /// <para>
    /// When including this class in your project, you may define the following
    /// conditional-compilation symbols to adjust the default behaviors:
    /// </para>
    /// <para>
    /// TELEMETRYEVENTSOURCE_USE_NUGET - use Microsoft.Diagnostics.Tracing instead
    /// of System.Diagnostics.Tracing.
    /// </para>
    /// <para>
    /// TELEMETRYEVENTSOURCE_PUBLIC - define TelemetryEventSource as public instead
    /// of internal.
    /// </para>
    /// </summary>
#if TELEMETRYEVENTSOURCE_PUBLIC
    public
#else
    internal
#endif
        class TelemetryEventSource
        : EventSource
    {
        /// <summary>
        /// Keyword 0x0000100000000000 is reserved for future definition. Do
        /// not use keyword 0x0000100000000000 in Microsoft-style ETW.
        /// </summary>
        public const EventKeywords Reserved44Keyword = (EventKeywords)0x0000100000000000;

        /// <summary>
        /// Add TelemetryKeyword to eventSourceOptions.Keywords to indicate that
        /// an event is for general-purpose telemetry.
        /// This keyword should not be combined with MeasuresKeyword or
        /// CriticalDataKeyword.
        /// </summary>
        public const EventKeywords TelemetryKeyword = (EventKeywords)0x0000200000000000;

        /// <summary>
        /// Add MeasuresKeyword to eventSourceOptions.Keywords to indicate that
        /// an event is for understanding measures and reporting scenarios.
        /// This keyword should not be combined with TelemetryKeyword or
        /// CriticalDataKeyword.
        /// </summary>
        public const EventKeywords MeasuresKeyword = (EventKeywords)0x0000400000000000;

        /// <summary>
        /// Add CriticalDataKeyword to eventSourceOptions.Keywords to indicate that
        /// an event powers user experiences or is critical to business intelligence.
        /// This keyword should not be combined with TelemetryKeyword or
        /// MeasuresKeyword.
        /// </summary>
        public const EventKeywords CriticalDataKeyword = (EventKeywords)0x0000800000000000;

        /// <summary>
        /// Add CostDeferredLatency to eventSourceOptions.Tags to indicate that an event
        /// should try to upload over free networks for a period of time before resorting
        /// to upload over costed networks.
        /// </summary>
        public const EventTags CostDeferredLatency = (EventTags)0x040000;

        /// <summary>
        /// Add CoreData to eventSourceOptions.Tags to indicate that an event
        /// contains high priority "core data".
        /// </summary>
        public const EventTags CoreData = (EventTags)0x00080000;

        /// <summary>
        /// Add InjectXToken to eventSourceOptions.Tags to indicate that an XBOX
        /// identity token should be injected into the event before the event is
        /// uploaded.
        /// </summary>
        public const EventTags InjectXToken = (EventTags)0x00100000;

        /// <summary>
        /// Add RealtimeLatency to eventSourceOptions.Tags to indicate that an event
        /// should be transmitted in real time (via any available connection).
        /// </summary>
        public const EventTags RealtimeLatency = (EventTags)0x0200000;

        /// <summary>
        /// Add NormalLatency to eventSourceOptions.Tags to indicate that an event
        /// should be transmitted via the preferred connection based on device policy.
        /// </summary>
        public const EventTags NormalLatency = (EventTags)0x0400000;

        /// <summary>
        /// Add CriticalPersistence to eventSourceOptions.Tags to indicate that an
        /// event should be deleted last when low on spool space.
        /// </summary>
        public const EventTags CriticalPersistence = (EventTags)0x0800000;

        /// <summary>
        /// Add NormalPersistence to eventSourceOptions.Tags to indicate that an event
        /// should be deleted first when low on spool space.
        /// </summary>
        public const EventTags NormalPersistence = (EventTags)0x1000000;

        /// <summary>
        /// Add DropPii to eventSourceOptions.Tags to indicate that an event contains
        /// PII and should be anonymized by the telemetry client. If this tag is
        /// present, PartA fields that might allow identification or cross-event
        /// correlation will be removed from the event.
        /// </summary>
        public const EventTags DropPii = (EventTags)0x02000000;

        /// <summary>
        /// Add HashPii to eventSourceOptions.Tags to indicate that an event contains
        /// PII and should be anonymized by the telemetry client. If this tag is
        /// present, PartA fields that might allow identification or cross-event
        /// correlation will be hashed (obfuscated).
        /// </summary>
        public const EventTags HashPii = (EventTags)0x04000000;

        /// <summary>
        /// Add MarkPii to eventSourceOptions.Tags to indicate that an event contains
        /// PII but may be uploaded as-is. If this tag is present, the event will be
        /// marked so that it will only appear on the private stream.
        /// </summary>
        public const EventTags MarkPii = (EventTags)0x08000000;

        /// <summary>
        /// Add DropPiiField to eventFieldAttribute.Tags to indicate that a field
        /// contains PII and should be dropped by the telemetry client.
        /// </summary>
        public const EventFieldTags DropPiiField = (EventFieldTags)0x04000000;

        /// <summary>
        /// Add HashPiiField to eventFieldAttribute.Tags to indicate that a field
        /// contains PII and should be hashed (obfuscated) prior to uploading.
        /// </summary>
        public const EventFieldTags HashPiiField = (EventFieldTags)0x08000000;

        /// <summary>
        /// Constructs a new instance of the TelemetryEventSource class with the
        /// specified name. Sets the EtwSelfDescribingEventFormat option.
        /// </summary>
        /// <param name="eventSourceName">The name of the event source.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811", Justification = "Shared class with tiny helper methods - not all constructors/methods are used by all consumers")]
        public TelemetryEventSource(
            string eventSourceName)
            : base(
            eventSourceName,
            EventSourceSettings.EtwSelfDescribingEventFormat)
        {
            return;
        }

        /// <summary>
        /// For use by derived classes that set the eventSourceName via EventSourceAttribute.
        /// Sets the EtwSelfDescribingEventFormat option.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811", Justification = "Shared class with tiny helper methods - not all constructors/methods are used by all consumers")]
        protected TelemetryEventSource()
            : base(
            EventSourceSettings.EtwSelfDescribingEventFormat)
        {
            return;
        }

        /// <summary>
        /// Constructs a new instance of the TelemetryEventSource class with the
        /// specified name. Sets the EtwSelfDescribingEventFormat option.
        /// </summary>
        /// <param name="eventSourceName">The name of the event source.</param>
        /// <param name="telemetryGroup">The parameter is not used.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811", Justification = "Shared class with tiny helper methods - not all constructors/methods are used by all consumers")]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "API compatibility")]
        public TelemetryEventSource(
            string eventSourceName,
            TelemetryGroup telemetryGroup)
            : base(
            eventSourceName,
            EventSourceSettings.EtwSelfDescribingEventFormat)
        {
            return;
        }

        /// <summary>
        /// Returns an instance of EventSourceOptions with the TelemetryKeyword set.
        /// </summary>
        /// <returns>Returns an instance of EventSourceOptions with the TelemetryKeyword set.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1811", Justification = "Shared class with tiny helper methods - not all constructors/methods are used by all consumers")]
        public static EventSourceOptions TelemetryOptions()
        {
            return new EventSourceOptions { Keywords = TelemetryKeyword };
        }

        /// <summary>
        /// Returns an instance of EventSourceOptions with the MeasuresKeyword set.
        /// </summary>
        /// <returns>Returns an instance of EventSourceOptions with the MeasuresKeyword set.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1811", Justification = "Shared class with tiny helper methods - not all constructors/methods are used by all consumers")]
        public static EventSourceOptions MeasuresOptions()
        {
            return new EventSourceOptions { Keywords = MeasuresKeyword };
        }
    }

    /// <summary>
    /// <para>
    /// The PrivTags class defines privacy tags that can be used to specify the privacy
    /// category of an event. Add a privacy tag as a field with name "PartA_PrivTags".
    /// As a shortcut, you can use _1 as the field name, which will automatically be
    /// expanded to "PartA_PrivTags" at runtime.
    /// </para>
    /// <para>
    /// Multiple tags can be OR'ed together if necessary (rarely needed).
    /// </para>
    /// </summary>
    /// <example>
    /// Typical usage:
    /// <code>
    /// es.Write("UsageEvent", new
    /// {
    ///     _1 = PrivTags.ProductAndServiceUsage,
    ///     field1 = fieldValue1,
    ///     field2 = fieldValue2
    /// });
    /// </code>
    /// </example>
#if TELEMETRYEVENTSOURCE_PUBLIC
    [CLSCompliant(false)]
    public
#else
    internal
#endif
        static class PrivTags
    {
        /// <nodoc/>
        public const Internal.PartA_PrivTags BrowsingHistory = Internal.PartA_PrivTags.BrowsingHistory;

        /// <nodoc/>
        public const Internal.PartA_PrivTags DeviceConnectivityAndConfiguration = Internal.PartA_PrivTags.DeviceConnectivityAndConfiguration;

        /// <nodoc/>
        public const Internal.PartA_PrivTags InkingTypingAndSpeechUtterance = Internal.PartA_PrivTags.InkingTypingAndSpeechUtterance;

        /// <nodoc/>
        public const Internal.PartA_PrivTags ProductAndServicePerformance = Internal.PartA_PrivTags.ProductAndServicePerformance;

        /// <nodoc/>
        public const Internal.PartA_PrivTags ProductAndServiceUsage = Internal.PartA_PrivTags.ProductAndServiceUsage;

        /// <nodoc/>
        public const Internal.PartA_PrivTags SoftwareSetupAndInventory = Internal.PartA_PrivTags.SoftwareSetupAndInventory;
    }

    /// <summary>
    /// Pass a TelemetryGroup value to the constructor of TelemetryEventSource
    /// to control which telemetry group should be joined.
    /// Note: has no effect in this version of TelemetryEventSource.
    /// </summary>
#if TELEMETRYEVENTSOURCE_PUBLIC
    public
#else
    internal
#endif
    enum TelemetryGroup
    {
        /// <summary>
        /// The default group. Join this group to log normal, non-critical, non-coredata
        /// events.
        /// </summary>
        MicrosoftTelemetry,

        /// <summary>
        /// Join this group to log CriticalData, CoreData, or other specially approved
        /// events.
        /// </summary>
        WindowsCoreTelemetry
    }

#pragma warning disable SA1403 // File may only contain a single namespace
    namespace Internal
#pragma warning restore SA1403 // File may only contain a single namespace
    {
        /// <summary>
        /// The complete list of privacy tags supported for events.
        /// Multiple tags can be OR'ed together if an event belongs in multiple
        /// categories.
        /// Note that the PartA_PrivTags enum should not be used directly.
        /// Instead, use values from the PrivTags class.
        /// </summary>
        [Flags]
#if TELEMETRYEVENTSOURCE_PUBLIC
        [CLSCompliant(false)]
        public
#else
        internal
#endif
            enum PartA_PrivTags
            : ulong
        {
            /// <nodoc/>
            None = 0,

            /// <nodoc/>
            BrowsingHistory = 0x0000000000000002u,

            /// <nodoc/>
            DeviceConnectivityAndConfiguration = 0x0000000000000800u,

            /// <nodoc/>
            InkingTypingAndSpeechUtterance = 0x0000000000020000u,

            /// <nodoc/>
            ProductAndServicePerformance = 0x0000000001000000u,

            /// <nodoc/>
            ProductAndServiceUsage = 0x0000000002000000u,

            /// <nodoc/>
            SoftwareSetupAndInventory = 0x0000000080000000u,
        }
    }
}

#pragma warning restore SA1625 // Element documentation should not be copied and pasted
#pragma warning restore SA1642 // Constructor summary documentation should begin with standard text
#pragma warning restore SA1604 // Element documentation should have summary
#pragma warning restore CA1707 // Identifiers should not contain underscores