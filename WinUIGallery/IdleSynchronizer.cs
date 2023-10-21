// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using AppUIBasics.Helper;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace AppUIBasics
{
    class IdleSynchronizer
    {
        const uint s_idleTimeoutMs = 100000;
        const int s_defaultWaitForEventMs = 10000;

        const string s_hasAnimationsHandleName = "HasAnimations";
        const string s_animationsCompleteHandleName = "AnimationsComplete";
        const string s_hasDeferredAnimationOperationsHandleName = "HasDeferredAnimationOperations";
        const string s_deferredAnimationOperationsCompleteHandleName = "DeferredAnimationOperationsComplete";
        const string s_rootVisualResetHandleName = "RootVisualReset";
        const string s_imageDecodingIdleHandleName = "ImageDecodingIdle";
        const string s_fontDownloadsIdleHandleName = "FontDownloadsIdle";
        const string s_hasBuildTreeWorksHandleName = "HasBuildTreeWorks";
        const string s_buildTreeServiceDrainedHandleName = "BuildTreeServiceDrained";

        private DispatcherQueue m_dispatcherQueue = null;

        private Handle m_hasAnimationsHandle;
        private Handle m_animationsCompleteHandle;
        private Handle m_hasDeferredAnimationOperationsHandle;
        private Handle m_deferredAnimationOperationsCompleteHandle;
        private Handle m_rootVisualResetHandle;
        private Handle m_imageDecodingIdleHandle;
        private Handle m_fontDownloadsIdleHandle;
        private Handle m_hasBuildTreeWorksHandle;
        private Handle m_buildTreeServiceDrainedHandle;

        private bool m_waitForAnimationsIsDisabled = false;
        private bool m_isRS2OrHigherInitialized = false;
        private bool m_isRS2OrHigher = false;

        private Handle OpenNamedEvent(uint processId, uint threadId, string eventNamePrefix)
        {
            string eventName = string.Format("{0}.{1}.{2}", eventNamePrefix, processId, threadId);
            Handle handle = new Handle(
                NativeMethods.OpenEvent(
                    (uint)(SyncObjectAccess.EVENT_MODIFY_STATE | SyncObjectAccess.SYNCHRONIZE),
                    false /* inherit handle */,
                    eventName));

            if (!handle.IsValid)
            {
                // Warning: Opening a session wide event handle, test may listen for events coming from the wrong process
                handle = new Handle(
                    NativeMethods.OpenEvent(
                        (uint)(SyncObjectAccess.EVENT_MODIFY_STATE | SyncObjectAccess.SYNCHRONIZE),
                        false /* inherit handle */,
                        eventNamePrefix));
            }

            if (!handle.IsValid)
            {
                throw new Exception("Failed to open " + eventName + " handle.");
            }

            return handle;
        }

        private Handle OpenNamedEvent(uint threadId, string eventNamePrefix)
        {
            return OpenNamedEvent(NativeMethods.GetCurrentProcessId(), threadId, eventNamePrefix);
        }

        private Handle OpenNamedEvent(DispatcherQueue dispatcherQueue, string eventNamePrefix)
        {
            return OpenNamedEvent(NativeMethods.GetCurrentProcessId(), GetUIThreadId(dispatcherQueue), eventNamePrefix);
        }

        private uint GetUIThreadId(DispatcherQueue dispatcherQueue)
        {
            uint threadId = 0;
            if (dispatcherQueue.HasThreadAccess)
            {
                threadId = NativeMethods.GetCurrentThreadId();
            }
            else
            {
                AutoResetEvent threadIdReceivedEvent = new AutoResetEvent(false);

                dispatcherQueue.TryEnqueue(
                    DispatcherQueuePriority.Normal,
                    new DispatcherQueueHandler(() =>
                    {
                        threadId = NativeMethods.GetCurrentThreadId();
                        threadIdReceivedEvent.Set();
                    }));

                threadIdReceivedEvent.WaitOne(s_defaultWaitForEventMs);
            }

            return threadId;
        }

        private static IdleSynchronizer instance = null;

        public static IdleSynchronizer Instance
        {
            get
            {
                if (instance == null)
                {
                    throw new Exception("Init() must be called on the UI thread before retrieving Instance.");
                }

                return instance;
            }
        }

        public string Log { get; set; }
        public int TickCountBegin { get; set; }

        private IdleSynchronizer(DispatcherQueue dispatcherQueue)
        {
            m_dispatcherQueue = dispatcherQueue;
            m_hasAnimationsHandle = OpenNamedEvent(m_dispatcherQueue, s_hasAnimationsHandleName);
            m_animationsCompleteHandle = OpenNamedEvent(m_dispatcherQueue, s_animationsCompleteHandleName);
            m_hasDeferredAnimationOperationsHandle = OpenNamedEvent(m_dispatcherQueue, s_hasDeferredAnimationOperationsHandleName);
            m_deferredAnimationOperationsCompleteHandle = OpenNamedEvent(m_dispatcherQueue, s_deferredAnimationOperationsCompleteHandleName);
            m_rootVisualResetHandle = OpenNamedEvent(m_dispatcherQueue, s_rootVisualResetHandleName);
            m_imageDecodingIdleHandle = OpenNamedEvent(m_dispatcherQueue, s_imageDecodingIdleHandleName);
            m_fontDownloadsIdleHandle = OpenNamedEvent(m_dispatcherQueue, s_fontDownloadsIdleHandleName);
            m_hasBuildTreeWorksHandle = OpenNamedEvent(m_dispatcherQueue, s_hasBuildTreeWorksHandleName);
            m_buildTreeServiceDrainedHandle = OpenNamedEvent(m_dispatcherQueue, s_buildTreeServiceDrainedHandleName);
        }

        public static void Init()
        {
            DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

            if (dispatcherQueue == null)
            {
                throw new Exception("Init() must be called on the UI thread.");
            }

            instance = new IdleSynchronizer(dispatcherQueue);
        }

        public static void Wait()
        {
            string logMessage;
            Wait(out logMessage);
        }

        public static void Wait(out string logMessage)
        {
            string errorString = Instance.WaitInternal(out logMessage);

            if (errorString.Length > 0)
            {
                throw new Exception(errorString);
            }
        }

        public static string TryWait()
        {
            string logMessage;
            return Instance.WaitInternal(out logMessage);
        }

        public static string TryWait(out string logMessage)
        {
            return Instance.WaitInternal(out logMessage);
        }

        public void AddLog(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);

            if (Log != null && Log != "LOG: ")
            {
                Log += "; ";
            }

            Log += (Environment.TickCount - TickCountBegin).ToString() + ": ";
            Log += message;
        }

        private string WaitInternal(out string logMessage)
        {
            logMessage = string.Empty;
            string errorString = string.Empty;

            if (m_dispatcherQueue.HasThreadAccess)
            {
                return "Cannot wait for UI thread idle from the UI thread.";
            }

            Log = "LOG: ";
            TickCountBegin = Environment.TickCount;

            bool isIdle = false;
            while (!isIdle)
            {
                bool hadAnimations = true;
                bool hadDeferredAnimationOperations = true;
                bool hadBuildTreeWork = false;

                errorString = WaitForRootVisualReset();
                if (errorString.Length > 0) { return errorString; }
                AddLog("After WaitForRootVisualReset");

                errorString = WaitForImageDecodingIdle();
                if (errorString.Length > 0) { return errorString; }
                AddLog("After WaitForImageDecodingIdle");

                // SynchronouslyTickUIThread(1);
                // AddLog("After SynchronouslyTickUIThread(1)");

                errorString = WaitForFontDownloadsIdle();
                if (errorString.Length > 0) { return errorString; }
                AddLog("After WaitForFontDownloadsIdle");

                WaitForIdleDispatcher();
                AddLog("After WaitForIdleDispatcher");

                // At this point, we know that the UI thread is idle - now we need to make sure
                // that XAML isn't animating anything.
                // TODO 27870237: Remove this #if once BuildTreeServiceDrained is properly signaled in WinUI desktop apps.

                // The AnimationsComplete handle sometimes is never set in RS1,
                // so we'll skip waiting for animations to complete
                // if we've timed out once while waiting for animations in RS1.
                if (!m_waitForAnimationsIsDisabled)
                {
                    errorString = WaitForAnimationsComplete(out hadAnimations);
                    if (errorString.Length > 0) { return errorString; }
                    AddLog("After WaitForAnimationsComplete");
                }
                else
                {
                    hadAnimations = false;
                }

                errorString = WaitForDeferredAnimationOperationsComplete(out hadDeferredAnimationOperations);
                if (errorString.Length > 0) { return errorString; }
                AddLog("After WaitForDeferredAnimationOperationsComplete");

                // In the case where we waited for an animation to complete there's a possibility that
                // XAML, at the completion of the animation, scheduled a new tick. We will loop
                // for as long as needed until we complete an idle dispatcher callback without
                // waiting for a pending animation to complete.
                isIdle = !hadAnimations && !hadDeferredAnimationOperations && !hadBuildTreeWork;

                AddLog("IsIdle? " + isIdle);
            }

            AddLog("End");

            logMessage = Log;
            return string.Empty;
        }

        private string WaitForRootVisualReset()
        {
            uint waitResult = NativeMethods.WaitForSingleObject(m_rootVisualResetHandle.NativeHandle, 5000);

            if (waitResult != NativeMethods.WAIT_OBJECT_0 && waitResult != NativeMethods.WAIT_TIMEOUT)
            {
                return "Waiting for root visual reset handle returned an invalid value.";
            }

            return string.Empty;
        }

        private string WaitForImageDecodingIdle()
        {
            uint waitResult = NativeMethods.WaitForSingleObject(m_imageDecodingIdleHandle.NativeHandle, 5000);

            if (waitResult != NativeMethods.WAIT_OBJECT_0 && waitResult != NativeMethods.WAIT_TIMEOUT)
            {
                return "Waiting for image decoding idle handle returned an invalid value.";
            }

            return string.Empty;
        }

        string WaitForFontDownloadsIdle()
        {
            uint waitResult = NativeMethods.WaitForSingleObject(m_fontDownloadsIdleHandle.NativeHandle, 5000);

            if (waitResult != NativeMethods.WAIT_OBJECT_0 && waitResult != NativeMethods.WAIT_TIMEOUT)
            {
                return "Waiting for font downloads handle returned an invalid value.";
            }

            return string.Empty;
        }

        void WaitForIdleDispatcher()
        {
            AutoResetEvent shouldContinueEvent = new AutoResetEvent(false);

            // DispatcherQueueTimer runs at below idle priority, so we can use it to ensure that we only raise the event when we're idle.
            var timer = m_dispatcherQueue.CreateTimer();
            timer.Interval = TimeSpan.FromMilliseconds(0);
            timer.IsRepeating = false;

            TypedEventHandler<DispatcherQueueTimer,object> tickHandler = null;

            tickHandler = (sender, args) =>
            {
                timer.Tick -= tickHandler;
                shouldContinueEvent.Set();
            };

            timer.Tick += tickHandler;

            timer.Start();
            shouldContinueEvent.WaitOne(s_defaultWaitForEventMs);
        }

        string WaitForBuildTreeServiceWork(out bool hadBuildTreeWork)
        {
            hadBuildTreeWork = false;
            bool hasBuildTreeWork = true;

            // We want to avoid an infinite loop, so we'll iterate 20 times before concluding that
            // we probably are never going to become idle.
            int waitCount = 20;

            while (hasBuildTreeWork && waitCount-- > 0)
            {
                if (!NativeMethods.ResetEvent(m_buildTreeServiceDrainedHandle.NativeHandle))
                {
                    return "Failed to reset BuildTreeServiceDrained handle.";
                }

                AutoResetEvent layoutUpdatedEvent = new AutoResetEvent(false);

                m_dispatcherQueue.TryEnqueue(
                    DispatcherQueuePriority.Normal,
                    new DispatcherQueueHandler(() =>
                    {
                        foreach (Window window in WindowHelper.ActiveWindows)
                        {
                            if (window.Content != null)
                            {
                                window.Content.UpdateLayout();
                            }
                        }

                        layoutUpdatedEvent.Set();
                    }));

                layoutUpdatedEvent.WaitOne(s_defaultWaitForEventMs);

                // This will be signaled if and only if Jupiter plans to at some point in the near
                // future set the BuildTreeServiceDrained event.
                uint waitResult = NativeMethods.WaitForSingleObject(m_hasBuildTreeWorksHandle.NativeHandle, 0);

                if (waitResult != NativeMethods.WAIT_OBJECT_0 && waitResult != NativeMethods.WAIT_TIMEOUT)
                {
                    return "HasBuildTreeWork handle wait returned an invalid value.";
                }

                hasBuildTreeWork = (waitResult == NativeMethods.WAIT_OBJECT_0);
                AddLog("HasBuildTreeWork? " + hasBuildTreeWork);

                if (hasBuildTreeWork)
                {
                    AddLog("Waiting for BuildTreeService to finish...");
                    waitResult = NativeMethods.WaitForSingleObject(m_buildTreeServiceDrainedHandle.NativeHandle, 10000);

                    if (waitResult != NativeMethods.WAIT_OBJECT_0 && waitResult != NativeMethods.WAIT_TIMEOUT)
                    {
                        return "Wait for build tree service failed";
                    }
                    AddLog("BuildTreeService drained");
                }
            }

            hadBuildTreeWork = hasBuildTreeWork;
            return string.Empty;
        }

        string WaitForAnimationsComplete(out bool hadAnimations)
        {
            hadAnimations = false;

            if (!NativeMethods.ResetEvent(m_animationsCompleteHandle.NativeHandle))
            {
                return "Failed to reset AnimationsComplete handle.";
            }

            AddLog("WaitForAnimationsComplete: After ResetEvent");

            // This will be signaled if and only if XAML plans to at some point in the near
            // future set the animations complete event.
            uint waitResult = NativeMethods.WaitForSingleObject(m_hasAnimationsHandle.NativeHandle, 0);

            if (waitResult != NativeMethods.WAIT_OBJECT_0 && waitResult != NativeMethods.WAIT_TIMEOUT)
            {
                return "HasAnimations handle wait returned an invalid value.";
            }

            AddLog("WaitForAnimationsComplete: After Wait(m_hasAnimationsHandle)");

            bool hasAnimations = (waitResult == NativeMethods.WAIT_OBJECT_0);

            if (hasAnimations)
            {
                uint animationCompleteWaitResult = NativeMethods.WaitForSingleObject(m_animationsCompleteHandle.NativeHandle, s_idleTimeoutMs);

                AddLog("WaitForAnimationsComplete: HasAnimations, After Wait(m_animationsCompleteHandle)");

                if (animationCompleteWaitResult != NativeMethods.WAIT_OBJECT_0)
                {
                    if (!IsRS2OrHigher())
                    {
                        // The AnimationsComplete handle is sometimes just never signaled on RS1, ever.
                        // If we run into this problem, we'll just disable waiting for animations to complete
                        // and continue execution.  When the current test completes, we'll then close and reopen
                        // the test app to minimize the effects of this problem.
                        m_waitForAnimationsIsDisabled = true;

                        hadAnimations = false;
                    }

                    return "Animation complete wait took longer than idle timeout.";
                }
            }

            hadAnimations = hasAnimations;
            return string.Empty;
        }

        string WaitForDeferredAnimationOperationsComplete(out bool hadDeferredAnimationOperations)
        {
            hadDeferredAnimationOperations = false;

            if (!NativeMethods.ResetEvent(m_deferredAnimationOperationsCompleteHandle.NativeHandle))
            {
                return "Failed to reset DeferredAnimationOperations handle.";
            }

            // This will be signaled if and only if XAML plans to at some point in the near
            // future set the animations complete event.
            uint waitResult = NativeMethods.WaitForSingleObject(m_hasDeferredAnimationOperationsHandle.NativeHandle, 0);

            if (waitResult != NativeMethods.WAIT_OBJECT_0 && waitResult != NativeMethods.WAIT_TIMEOUT)
            {
                return "HasDeferredAnimationOperations handle wait returned an invalid value.";
            }

            bool hasDeferredAnimationOperations = (waitResult == NativeMethods.WAIT_OBJECT_0);

            if (hasDeferredAnimationOperations)
            {
                uint animationCompleteWaitResult = NativeMethods.WaitForSingleObject(m_deferredAnimationOperationsCompleteHandle.NativeHandle, s_idleTimeoutMs);

                if (animationCompleteWaitResult != NativeMethods.WAIT_OBJECT_0 && animationCompleteWaitResult != NativeMethods.WAIT_TIMEOUT)
                {
                    return "Deferred animation operations complete wait took longer than idle timeout.";
                }
            }

            hadDeferredAnimationOperations = hasDeferredAnimationOperations;
            return string.Empty;
        }

        private void SynchronouslyTickUIThread(uint ticks)
        {
            for (uint i = 0; i < ticks; i++)
            {
                AutoResetEvent tickCompleteEvent = new AutoResetEvent(false);

                m_dispatcherQueue.TryEnqueue(
                    DispatcherQueuePriority.Normal,
                    new DispatcherQueueHandler(() =>
                    {
                        EventHandler<object> renderingHandler = null;

                        renderingHandler = (object sender, object args) =>
                        {
                            CompositionTarget.Rendering -= renderingHandler;
                            tickCompleteEvent.Set();
                        };

                        CompositionTarget.Rendering += renderingHandler;
                    }));

                tickCompleteEvent.WaitOne(s_defaultWaitForEventMs);
            }
        }

        private bool IsRS2OrHigher()
        {
            if (!m_isRS2OrHigherInitialized)
            {
                m_isRS2OrHigherInitialized = true;
                m_isRS2OrHigher = Windows.Foundation.Metadata.ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 4);
            }

            return m_isRS2OrHigher;
        }
    }

    internal class Handle
    {
        public IntPtr NativeHandle { get; private set; }

        public bool IsValid
        {
            get
            {
                return NativeHandle != IntPtr.Zero;
            }
        }

        public Handle(IntPtr nativeHandle)
        {
            Attach(nativeHandle);
        }

        ~Handle()
        {
            Release();
        }

        public void Attach(IntPtr nativeHandle)
        {
            Release();
            NativeHandle = nativeHandle;
        }

        public IntPtr Detach()
        {
            IntPtr returnValue = NativeHandle;
            NativeHandle = IntPtr.Zero;
            return returnValue;
        }

        public void Release()
        {
            NativeMethods.CloseHandle(NativeHandle);
            NativeHandle = IntPtr.Zero;
        }
    }

    internal static class NativeMethods
    {
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenEvent(uint dwDesiredAccess, bool bInheritHandle, string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);

        public const UInt32 INFINITE = 0xFFFFFFFF;
        public const UInt32 WAIT_ABANDONED = 0x00000080;
        public const UInt32 WAIT_OBJECT_0 = 0x00000000;
        public const UInt32 WAIT_TIMEOUT = 0x00000102;

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ResetEvent(IntPtr hEvent);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentProcessId();

        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();
    }

    [Flags]
    public enum SyncObjectAccess : uint
    {
        DELETE = 0x00010000,
        READ_CONTROL = 0x00020000,
        WRITE_DAC = 0x00040000,
        WRITE_OWNER = 0x00080000,
        SYNCHRONIZE = 0x00100000,
        EVENT_ALL_ACCESS = 0x001F0003,
        EVENT_MODIFY_STATE = 0x00000002,
        MUTEX_ALL_ACCESS = 0x001F0001,
        MUTEX_MODIFY_STATE = 0x00000001,
        SEMAPHORE_ALL_ACCESS = 0x001F0003,
        SEMAPHORE_MODIFY_STATE = 0x00000002,
        TIMER_ALL_ACCESS = 0x001F0003,
        TIMER_MODIFY_STATE = 0x00000002,
        TIMER_QUERY_STATE = 0x00000001
    }
}