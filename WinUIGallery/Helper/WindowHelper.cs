using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml;

namespace AppUIBasics.Helper
{
    public static class WindowHelper
    {
        private static readonly List<Window> TrackedWindows = new List<Window>();

        public static IReadOnlyList<Window> Windows { get; } = TrackedWindows.AsReadOnly();

        public static Window TrackWindow(Window window)
        {
            ArgumentNullException.ThrowIfNull(window);
            if (!TrackedWindows.Contains(window))
            {
                TrackedWindows.Add(window);
                window.Closed += (_, _) => TrackedWindows.Remove(window);
                if (window.Content is FrameworkElement root)
                {
                    root.RequestedTheme = ThemeHelper.RootTheme;
                }
            }

            return window;
        }

        public static Window GetWindowForElement(FrameworkElement element)
        {
            ArgumentNullException.ThrowIfNull(element);
            if (element.XamlRoot == null)
            {
                throw new InvalidOperationException("The element must be attached to a window before using window interop.");
            }

            return TrackedWindows.FirstOrDefault(window =>
                window.Content is FrameworkElement root && root.XamlRoot == element.XamlRoot)
                ?? throw new InvalidOperationException("The element's owning window has not been registered.");
        }
    }
}
