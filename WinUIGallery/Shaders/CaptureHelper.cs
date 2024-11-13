using Microsoft.Graphics.Display;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using WinUIGallery.Helper;

namespace WinUIGallery.Shaders
{
    internal static class CaptureHelper
    {
        public static async Task CaptureTo(this Window window, RenderTargetBitmap renderTarget)
        {
            var root = GetRoot(window.Content);
            await renderTarget.RenderAsync(root);
        }
        
        public static async Task CaptureTo(this UIElement uiElement, RenderTargetBitmap renderTarget)
        {
            //var root = GetRoot(uiElement);

            //var dpi = GetDpi(uiElement);

            //await renderTarget.RenderAsync(root, (int)(uiElement.RenderSize.Width * 96.0f / dpi), (int)(uiElement.RenderSize.Height * 96.0f / dpi));
            await renderTarget.RenderAsync(uiElement);
        }

        public static async Task<Rect> CaptureTo(this ContentDialog dialog, RenderTargetBitmap renderTarget)
        {
            // The dialog is actually a full window element because it darkens the screen
            // when it appears. Drill in a bit to get to the actual element that contains
            // what we think of as the "dialog".
            var child1 = VisualTreeHelper.GetChild(dialog, 0);
            var child2 = VisualTreeHelper.GetChild(child1, 0);
            var child3 = VisualTreeHelper.GetChild(child2, 0);

            var dialogContent = child3 as Border;
            var dpi = GetDpi(dialog) / 96.0f;

            // Get transform from dialog content to the window wide dialog.
            // This will let us position things later.
            var transform = dialogContent.TransformToVisual(dialog);
            Rect originalBounds = new()
            {
                X = 0,
                Y = 0,
                Width = dialogContent.RenderSize.Width,
                Height = dialogContent.RenderSize.Height
            };

            var transformedBounds = transform.TransformBounds(originalBounds);

            await renderTarget.RenderAsync(dialogContent, (int)(transformedBounds.Width / dpi), (int)(transformedBounds.Height / dpi));

            return transformedBounds;
        }

        public static float GetDpi(UIElement element)
        {
            var window = WindowHelper.GetWindowForElement(element);
            if (window == null)
            {
                return 1.0f;
            }

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            return Win32.GetDpiForWindow(hwnd);
        }

        private static UIElement GetRoot(UIElement element)
        {
            UIElement root = element;
            UIElement parent = VisualTreeHelper.GetParent(root) as UIElement;
            while (parent != null)
            {
                root = parent;
                parent = VisualTreeHelper.GetParent(root) as UIElement;
            }

            return root;
        }
    }
}
