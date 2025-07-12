// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Win32.Foundation;

namespace WinUIGallery.Helpers;
internal partial class NativeMethods
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

    [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
    internal static extern int SetWindowLong32(IntPtr hWnd, WindowLongIndexFlags nIndex, WinProc newProc);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
    internal static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, WindowLongIndexFlags nIndex, WinProc newProc);

    [DllImport("user32.dll")]
    internal static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam);

    public unsafe static void SetWindowKeyHook()
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

    public static bool IsKeyDownHook(IntPtr lWord)
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
    public static bool IsAppPackaged { get; } = GetCurrentPackageName() != null;
    public static string GetCurrentPackageName()
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
