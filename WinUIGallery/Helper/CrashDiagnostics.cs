using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.UI.Xaml;
using Windows.Storage;

namespace AppUIBasics.Helper
{
    internal static class CrashDiagnostics
    {
        private static readonly object LogLock = new object();

        public static void Install(Application application)
        {
            application.UnhandledException += (_, args) => Write(args.Message, args.Exception);
            AppDomain.CurrentDomain.UnhandledException += (_, args) =>
                Write("Unhandled process exception", args.ExceptionObject as Exception);
        }

        private static void Write(string message, Exception exception)
        {
            string entry = $"{DateTimeOffset.UtcNow:O} {message}{Environment.NewLine}{exception}{Environment.NewLine}";
            Trace.TraceError(entry);
            try
            {
                lock (LogLock)
                {
                    File.AppendAllText(Path.Combine(ApplicationData.Current.LocalFolder.Path, "Gallery-errors.log"), entry);
                }
            }
            catch (Exception error) when (error is IOException || error is UnauthorizedAccessException || error is COMException)
            {
                Trace.TraceError("Could not write the Gallery crash log: {0}", error);
            }
        }
    }
}
