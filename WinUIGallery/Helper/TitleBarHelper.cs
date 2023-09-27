using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AppUIBasics.Helper;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using WinRT;
using System.Runtime.InteropServices;
using AppUIBasics;

namespace WinUIGallery.DesktopWap.Helper
{
    internal class TitleBarHelper
    {

        private static void triggerTitleBarRepaint(Window window)
        {
            // to trigger repaint tracking task id 38044406
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            var activeWindow = AppUIBasics.Win32.GetActiveWindow();
            if (hwnd == activeWindow)
            {
                AppUIBasics.Win32.SendMessage(hwnd, AppUIBasics.Win32.WM_ACTIVATE, AppUIBasics.Win32.WA_INACTIVE, IntPtr.Zero);
                AppUIBasics.Win32.SendMessage(hwnd, AppUIBasics.Win32.WM_ACTIVATE, AppUIBasics.Win32.WA_ACTIVE, IntPtr.Zero);
            }
            else
            {
                AppUIBasics.Win32.SendMessage(hwnd, AppUIBasics.Win32.WM_ACTIVATE, AppUIBasics.Win32.WA_ACTIVE, IntPtr.Zero);
                AppUIBasics.Win32.SendMessage(hwnd, AppUIBasics.Win32.WM_ACTIVATE, AppUIBasics.Win32.WA_INACTIVE, IntPtr.Zero);
            }

        }

        public static Windows.UI.Color ApplySystemThemeToCaptionButtons(Window window)
        {
            var res = Application.Current.Resources;
            var frame = (Application.Current as AppUIBasics.App).GetRootFrame() as FrameworkElement;
            Windows.UI.Color color;
            if (frame.ActualTheme == ElementTheme.Dark)
            {
                color = Colors.White;
            }
            else
            {
                color = Colors.Black;
            }
            SetCaptionButtonColors(window,color);
            return color;
        }

        public static void SetCaptionButtonColors(Window window, Windows.UI.Color color)
        {
            var res = Application.Current.Resources;
            res["WindowCaptionForeground"] = color;
            triggerTitleBarRepaint(window);
        }

        public static void SetCaptionButtonBackgroundColors(Window window, Windows.UI.Color? color)
        {
            var titleBar = window.AppWindow.TitleBar;
            titleBar.ButtonBackgroundColor = color;
            triggerTitleBarRepaint(window);
        }

        public static void SetForegroundColor(Window window, Windows.UI.Color? color)
        {
            var titleBar = window.AppWindow.TitleBar;
            titleBar.ForegroundColor = color;
            triggerTitleBarRepaint(window);
        }

        public static void SetBackgroundColor(Window window, Windows.UI.Color? color)
        {
            var titleBar = window.AppWindow.TitleBar;
            titleBar.BackgroundColor = color;
            triggerTitleBarRepaint(window);
        }
    }
}
