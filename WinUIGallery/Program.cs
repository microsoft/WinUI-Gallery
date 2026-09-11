using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using Windows.ApplicationModel.Activation;

namespace AppUIBasics
{
    internal static class Program
    {
        private static readonly object ActivationLock = new object();
        private static readonly Queue<ActivationRequest> PendingActivations = new Queue<ActivationRequest>();
        private static DispatcherQueue _dispatcher;
        private static Action<ActivationRequest> _activationHandler;

        public static ActivationRequest InitialActivation { get; private set; }

        [STAThread]
        private static void Main()
        {
            WinRT.ComWrappersSupport.InitializeComWrappers();
            var activation = AppInstance.GetCurrent().GetActivatedEventArgs();
            InitialActivation = ActivationRequest.Capture(activation);
            var instance = AppInstance.FindOrRegisterForKey("GalleryMainWindow");
            if (!instance.IsCurrent)
            {
                RedirectActivation(instance, activation);
                return;
            }

            instance.Activated += OnActivated;
            Application.Start(initialization =>
            {
                SynchronizationContext.SetSynchronizationContext(
                    new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread()));
                _ = new App();
            });
        }

        public static void RegisterActivationHandler(DispatcherQueue dispatcher, Action<ActivationRequest> handler)
        {
            lock (ActivationLock)
            {
                _dispatcher = dispatcher;
                _activationHandler = handler;
                while (PendingActivations.Count > 0)
                {
                    DispatchActivation(PendingActivations.Dequeue());
                }
            }
        }

        private static void OnActivated(object sender, AppActivationArguments args)
        {
            // Redirected WinRT data belongs to the calling process, which can exit as soon as this callback returns.
            var request = ActivationRequest.Capture(args);
            lock (ActivationLock)
            {
                if (_dispatcher == null)
                {
                    PendingActivations.Enqueue(request);
                    return;
                }

                DispatchActivation(request);
            }
        }

        private static void DispatchActivation(ActivationRequest args)
        {
            if (!_dispatcher.TryEnqueue(() => _activationHandler(args)))
            {
                Trace.TraceError("The Gallery dispatcher rejected a redirected activation during shutdown.");
            }
        }

        private static void RedirectActivation(AppInstance instance, AppActivationArguments activation)
        {
            if (!AllowSetForegroundWindow(instance.ProcessId))
            {
                Trace.TraceWarning("Windows did not grant foreground permission to the existing Gallery window.");
            }

            using var completed = new EventWaitHandle(false, EventResetMode.ManualReset);
            var redirect = Task.Run(async () =>
            {
                try
                {
                    await instance.RedirectActivationToAsync(activation);
                }
                finally
                {
                    completed.Set();
                }
            });

            // Pump COM calls on the STA while the other process accepts the activation.
            Marshal.ThrowExceptionForHR(CoWaitForMultipleHandles(
                0, uint.MaxValue, 1, new[] { completed.SafeWaitHandle.DangerousGetHandle() }, out _));
            redirect.GetAwaiter().GetResult();
        }

        [DllImport("ole32.dll")]
        private static extern int CoWaitForMultipleHandles(
            uint flags, uint timeout, uint count, IntPtr[] handles, out uint index);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllowSetForegroundWindow(uint processId);
    }

    internal sealed record ActivationRequest(ExtendedActivationKind Kind, string LaunchArguments, Uri ProtocolUri)
    {
        public static ActivationRequest Capture(AppActivationArguments args)
        {
            return args.Kind switch
            {
                ExtendedActivationKind.Launch => new ActivationRequest(args.Kind,
                    ((ILaunchActivatedEventArgs)args.Data).Arguments, null),
                ExtendedActivationKind.Protocol => new ActivationRequest(args.Kind, string.Empty,
                    new Uri(((IProtocolActivatedEventArgs)args.Data).Uri.AbsoluteUri)),
                _ => new ActivationRequest(args.Kind, string.Empty, null)
            };
        }
    }
}
