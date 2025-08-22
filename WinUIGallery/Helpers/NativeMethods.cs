// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Win32.Foundation;

namespace WinUIGallery.Helpers;
internal partial class NativeMethods
{
    [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
    internal static extern int SetWindowLong32(IntPtr hWnd, WindowLongIndexFlags nIndex, WinProc newProc);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
    internal static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, WindowLongIndexFlags nIndex, WinProc newProc);

    [DllImport("User32.dll", CharSet = CharSet.Auto, EntryPoint = "SetWindowLongPtr")]
    internal static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    [DllImport("User32.dll", CharSet = CharSet.Auto, EntryPoint = "SetWindowLong")]
    internal static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);


    [DllImport("user32.dll")]
    internal static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam);
    
    internal unsafe static void SetWindowKeyHook()
    {
        delegate* unmanaged[Stdcall]<int, WPARAM, LPARAM, LRESULT> callback = &HookCallback;

        var moduleHandle = Windows.Win32.PInvoke.GetModuleHandle(string.Empty);
        var threadId = Windows.Win32.PInvoke.GetCurrentThreadId();

        var res = Windows.Win32.PInvoke.SetWindowsHookEx(Windows.Win32.UI.WindowsAndMessaging.WINDOWS_HOOK_ID.WH_KEYBOARD, callback, moduleHandle, threadId);

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        static LRESULT HookCallback(int msg, WPARAM wPARAM, LPARAM lPARAM)
        {
            if (msg >= 0 && IsKeyDownHook(lPARAM))
            {
                RootFrameNavigationHelper.RaiseKeyPressed((uint)wPARAM);
            }
            return Windows.Win32.PInvoke.CallNextHookEx(null, msg, wPARAM, lPARAM);
        }
    }

    internal static bool IsKeyDownHook(IntPtr lWord)
    {
        // The 30th bit tells what the previous key state is with 0 being the "UP" state
        // For more info see https://learn.microsoft.com/windows/win32/winmsg/keyboardproc#lparam-in
        return (lWord >> 30 & 1) == 0;
    }

    internal delegate IntPtr WinProc(IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam);

    [Flags]
    internal enum WindowLongIndexFlags : int
    {
        GWL_WNDPROC = -4,
    }

    internal enum WindowMessage : int
    {
        WM_GETMINMAXINFO = 0x0024,
    }

    internal static bool IsAppPackaged { get; } = GetCurrentPackageName() != null;
    internal static string GetCurrentPackageName()
    {
        unsafe
        {
            uint packageFullNameLength = 0;

            var result = Windows.Win32.PInvoke.GetCurrentPackageFullName(&packageFullNameLength, null);

            if (result == WIN32_ERROR.ERROR_INSUFFICIENT_BUFFER)
            {
                char* packageFullName = stackalloc char[(int)packageFullNameLength];

                result = Windows.Win32.PInvoke.GetCurrentPackageFullName(&packageFullNameLength, packageFullName);

                if (result == 0) // S_OK or ERROR_SUCCESS
                {
                    return new string(packageFullName, 0, (int)packageFullNameLength);
                }
            }
        }

        return null;
    }
}
