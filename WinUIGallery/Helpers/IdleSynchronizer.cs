// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.UI.Dispatching;
using Windows.Foundation;
using Windows.Win32.Foundation;

namespace WinUIGallery.Helpers;

public partial class IdleSynchronizer
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

    private DispatcherQueue m_dispatcherQueue = null;

    private SafeHandle m_hasAnimationsHandle;
    private SafeHandle m_animationsCompleteHandle;
    private SafeHandle m_hasDeferredAnimationOperationsHandle;
    private SafeHandle m_deferredAnimationOperationsCompleteHandle;
    private SafeHandle m_rootVisualResetHandle;
    private SafeHandle m_imageDecodingIdleHandle;
    private SafeHandle m_fontDownloadsIdleHandle;

    private bool m_waitForAnimationsIsDisabled = false;
    private bool m_isRS2OrHigherInitialized = false;
    private bool m_isRS2OrHigher = false;

    private SafeHandle OpenNamedEvent(uint processId, uint threadId, string eventNamePrefix)
    {
        string eventName = string.Format("{0}.{1}.{2}", eventNamePrefix, processId, threadId);

        var handle = Windows.Win32.PInvoke.OpenEvent(Windows.Win32.System.Threading.SYNCHRONIZATION_ACCESS_RIGHTS.EVENT_MODIFY_STATE | Windows.Win32.System.Threading.SYNCHRONIZATION_ACCESS_RIGHTS.SYNCHRONIZATION_SYNCHRONIZE, false, eventName);
        if (handle.IsInvalid)
        {
            // Warning: Opening a session wide event handle, test may listen for events coming from the wrong process
            handle = Windows.Win32.PInvoke.OpenEvent(Windows.Win32.System.Threading.SYNCHRONIZATION_ACCESS_RIGHTS.EVENT_MODIFY_STATE | Windows.Win32.System.Threading.SYNCHRONIZATION_ACCESS_RIGHTS.SYNCHRONIZATION_SYNCHRONIZE, false, eventNamePrefix);
        }

        if (handle.IsInvalid)
        {
            throw new Exception("Failed to open " + eventName + " handle.");
        }

        return handle;
    }

    private SafeHandle OpenNamedEvent(DispatcherQueue dispatcherQueue, string eventNamePrefix)
    {
        return OpenNamedEvent(Windows.Win32.PInvoke.GetCurrentProcessId(), GetUIThreadId(dispatcherQueue), eventNamePrefix);
    }

    private uint GetUIThreadId(DispatcherQueue dispatcherQueue)
    {
        uint threadId = 0;
        if (dispatcherQueue.HasThreadAccess)
        {
            threadId = Windows.Win32.PInvoke.GetCurrentThreadId();
        }
        else
        {
            AutoResetEvent threadIdReceivedEvent = new AutoResetEvent(false);

            dispatcherQueue.TryEnqueue(
                DispatcherQueuePriority.Normal,
                new DispatcherQueueHandler(() =>
                {
                    threadId = Windows.Win32.PInvoke.GetCurrentThreadId();
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
        Wait(out _);
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
        return Instance.WaitInternal(out _);
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
        if (m_dispatcherQueue.HasThreadAccess)
        {
            return "Cannot wait for UI thread idle from the UI thread.";
        }

        Log = "LOG: ";
        TickCountBegin = Environment.TickCount;

        bool isIdle = false;
        while (!isIdle)
        {
            bool hadBuildTreeWork = false;

            var errorString = WaitForRootVisualReset();
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

            bool hadAnimations;
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

            bool hadDeferredAnimationOperations;
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
        var waitResult = Windows.Win32.PInvoke.WaitForSingleObject(m_rootVisualResetHandle, 5000);

        if (waitResult != WAIT_EVENT.WAIT_OBJECT_0 && waitResult != WAIT_EVENT.WAIT_TIMEOUT)
        {
            return "Waiting for root visual reset handle returned an invalid value.";
        }

        return string.Empty;
    }

    private string WaitForImageDecodingIdle()
    {
        var waitResult = Windows.Win32.PInvoke.WaitForSingleObject(m_imageDecodingIdleHandle, 5000);

        if (waitResult != WAIT_EVENT.WAIT_OBJECT_0 && waitResult != WAIT_EVENT.WAIT_TIMEOUT)
        {
            return "Waiting for image decoding idle handle returned an invalid value.";
        }
        return string.Empty;
    }

    string WaitForFontDownloadsIdle()
    {
        var waitResult = Windows.Win32.PInvoke.WaitForSingleObject(m_fontDownloadsIdleHandle, 5000);

        if (waitResult != WAIT_EVENT.WAIT_OBJECT_0 && waitResult != WAIT_EVENT.WAIT_TIMEOUT)
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

    string WaitForAnimationsComplete(out bool hadAnimations)
    {
        hadAnimations = false;
        if (!Windows.Win32.PInvoke.ResetEvent(m_animationsCompleteHandle))
        {
            return "Failed to reset AnimationsComplete handle.";
        }

        AddLog("WaitForAnimationsComplete: After ResetEvent");

        // This will be signaled if and only if XAML plans to at some point in the near
        // future set the animations complete event.
        var waitResult = Windows.Win32.PInvoke.WaitForSingleObject(m_hasAnimationsHandle, 0);

        if (waitResult != WAIT_EVENT.WAIT_OBJECT_0 && waitResult != WAIT_EVENT.WAIT_TIMEOUT)
        {
            return "HasAnimations handle wait returned an invalid value.";
        }
        
        AddLog("WaitForAnimationsComplete: After Wait(m_hasAnimationsHandle)");

        bool hasAnimations = (waitResult == WAIT_EVENT.WAIT_OBJECT_0);

        if (hasAnimations)
        {
            var animationCompleteWaitResult = Windows.Win32.PInvoke.WaitForSingleObject(m_animationsCompleteHandle, s_idleTimeoutMs);

            AddLog("WaitForAnimationsComplete: HasAnimations, After Wait(m_animationsCompleteHandle)");

            if (animationCompleteWaitResult != WAIT_EVENT.WAIT_OBJECT_0)
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
        if (!Windows.Win32.PInvoke.ResetEvent(m_deferredAnimationOperationsCompleteHandle))
        {
            return "Failed to reset DeferredAnimationOperations handle.";
        }

        // This will be signaled if and only if XAML plans to at some point in the near
        // future set the animations complete event.
        var waitResult = Windows.Win32.PInvoke.WaitForSingleObject(m_hasDeferredAnimationOperationsHandle, 0);

        if (waitResult != WAIT_EVENT.WAIT_OBJECT_0 && waitResult != WAIT_EVENT.WAIT_TIMEOUT)
        {
            return "HasDeferredAnimationOperations handle wait returned an invalid value.";
        }

        bool hasDeferredAnimationOperations = (waitResult == WAIT_EVENT.WAIT_OBJECT_0);

        if (hasDeferredAnimationOperations)
        {
            var animationCompleteWaitResult = Windows.Win32.PInvoke.WaitForSingleObject(m_deferredAnimationOperationsCompleteHandle, s_idleTimeoutMs);

            if (animationCompleteWaitResult != WAIT_EVENT.WAIT_OBJECT_0 && animationCompleteWaitResult != WAIT_EVENT.WAIT_TIMEOUT)
            {
                return "Deferred animation operations complete wait took longer than idle timeout.";
            }
        }

        hadDeferredAnimationOperations = hasDeferredAnimationOperations;
        return string.Empty;
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
        Windows.Win32.PInvoke.CloseHandle(new HANDLE(NativeHandle));
        NativeHandle = IntPtr.Zero;
    }
}

