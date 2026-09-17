// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using Microsoft.UI.Xaml;
using Windows.System;

namespace WinUIGallery;

public static class Program
{
    [System.STAThread]
    public static void Main(string[] args)
    {
        WinRT.ComWrappersSupport.InitializeComWrappers();
        Application.Start(_ =>
        {
            DispatcherQueue queue = DispatcherQueue.GetForCurrentThread()
                ?? throw new InvalidOperationException("No system DispatcherQueue is available on the UI thread.");
            SynchronizationContext.SetSynchronizationContext(new SystemDispatcherQueueSynchronizationContext(queue));
            App.XamlGeneratedCreateApplicationInstance();
        });
    }
}

internal sealed class SystemDispatcherQueueSynchronizationContext(DispatcherQueue queue) : SynchronizationContext
{
    public override void Post(SendOrPostCallback callback, object? state)
    {
        if (!queue.TryEnqueue(() => callback(state)))
        {
            throw new InvalidOperationException("Failed to enqueue the synchronization-context callback.");
        }
    }

    public override SynchronizationContext CreateCopy() => new SystemDispatcherQueueSynchronizationContext(queue);
}
