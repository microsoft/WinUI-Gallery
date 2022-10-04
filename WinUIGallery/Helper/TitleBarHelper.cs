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

namespace WinUIGallery.DesktopWap.Helper
{
    internal class TitleBarHelper
    {

        public static void triggerTitleBarRepaint(Window window)
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

    }
}
