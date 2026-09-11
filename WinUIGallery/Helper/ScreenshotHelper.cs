using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.UI.Xaml;
using Windows.Foundation;

namespace AppUIBasics.Helper
{
    internal static class ScreenshotHelper
    {
        public static (byte[] Pixels, uint Width, uint Height, double Dpi) CaptureVisibleElement(FrameworkElement element)
        {
            var window = WindowHelper.GetWindowForElement(element);
            var origin = element.TransformToVisual(window.Content).TransformPoint(new Point());
            double scale = element.XamlRoot.RasterizationScale;
            int width = checked((int)Math.Ceiling(element.ActualWidth * scale));
            int height = checked((int)Math.Ceiling(element.ActualHeight * scale));
            var position = new NativePoint
            {
                X = checked((int)Math.Floor(origin.X * scale)),
                Y = checked((int)Math.Floor(origin.Y * scale))
            };
            if (!ClientToScreen(WinRT.Interop.WindowNative.GetWindowHandle(window), ref position))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            int screenLeft = GetSystemMetrics(76);
            int screenTop = GetSystemMetrics(77);
            if (width <= 0 || height <= 0 || position.X < screenLeft || position.Y < screenTop
                || (long)position.X + width > (long)screenLeft + GetSystemMetrics(78)
                || (long)position.Y + height > (long)screenTop + GetSystemMetrics(79))
            {
                throw new InvalidOperationException("Move the entire sample onto the screen before taking a delayed screenshot.");
            }

            IntPtr screen = GetDC(IntPtr.Zero);
            if (screen == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            IntPtr memory = IntPtr.Zero;
            IntPtr bitmap = IntPtr.Zero;
            IntPtr previous = IntPtr.Zero;
            try
            {
                memory = CreateCompatibleDC(screen);
                if (memory == IntPtr.Zero)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                var info = new BitmapInfo
                {
                    Size = 40,
                    Width = width,
                    Height = -height,
                    Planes = 1,
                    BitCount = 32
                };
                bitmap = CreateDIBSection(screen, ref info, 0, out IntPtr bits, IntPtr.Zero, 0);
                if (bitmap == IntPtr.Zero)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                previous = SelectObject(memory, bitmap);
                if (previous == IntPtr.Zero || previous == new IntPtr(-1))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                // Screen copy includes the flyouts rendered in separate desktop windows.
                if (!BitBlt(memory, 0, 0, width, height, screen, position.X, position.Y, 0x40CC0020))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                var pixels = new byte[checked(width * height * 4)];
                Marshal.Copy(bits, pixels, 0, pixels.Length);
                for (int i = 3; i < pixels.Length; i += 4)
                {
                    pixels[i] = 255;
                }
                return (pixels, (uint)width, (uint)height, 96 * scale);
            }
            finally
            {
                if (previous != IntPtr.Zero && previous != new IntPtr(-1))
                {
                    SelectObject(memory, previous);
                }
                if (bitmap != IntPtr.Zero && !DeleteObject(bitmap))
                {
                    Trace.TraceWarning("Could not release the screenshot bitmap.");
                }
                if (memory != IntPtr.Zero && !DeleteDC(memory))
                {
                    Trace.TraceWarning("Could not release the screenshot device context.");
                }
                if (ReleaseDC(IntPtr.Zero, screen) == 0)
                {
                    Trace.TraceWarning("Could not release the screen device context.");
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct NativePoint { public int X; public int Y; }

        [StructLayout(LayoutKind.Sequential)]
        private struct BitmapInfo
        {
            public uint Size;
            public int Width;
            public int Height;
            public ushort Planes;
            public ushort BitCount;
            public uint Compression;
            public uint SizeImage;
            public int XPelsPerMeter;
            public int YPelsPerMeter;
            public uint ColorsUsed;
            public uint ColorsImportant;
            public uint Colors;
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ClientToScreen(IntPtr window, ref NativePoint point);
        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int index);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetDC(IntPtr window);
        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr window, IntPtr context);
        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern IntPtr CreateCompatibleDC(IntPtr context);
        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern IntPtr CreateDIBSection(IntPtr context, ref BitmapInfo info,
            uint usage, out IntPtr bits, IntPtr section, uint offset);
        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern IntPtr SelectObject(IntPtr context, IntPtr value);
        [DllImport("gdi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool BitBlt(IntPtr target, int x, int y, int width, int height,
            IntPtr source, int sourceX, int sourceY, uint operation);
        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteObject(IntPtr value);
        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteDC(IntPtr context);
    }
}
