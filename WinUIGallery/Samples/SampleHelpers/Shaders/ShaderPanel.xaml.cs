using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using ComputeSharp.D2D1.Descriptors;
using ComputeSharp.D2D1;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Graphics.Canvas;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Diagnostics;
using Windows.UI;
using System.Numerics;
using Windows.Graphics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUIGallery.Shaders
{
    /// <summary>
    /// Simple wrapper around CanvasControl to draw a ComputeSharp shader
    /// </summary>
    public sealed partial class ShaderPanel : UserControl
    {
        public ShaderPanel()
        {
            this.InitializeComponent();
        }

        public event EventHandler FirstRender;
        public event EventHandler ShaderCompleted;

        // Variable to be passed through to the shader
        public Vector2 WipeDirection { get; set; } = new Vector2(1,0);

        public TimeSpan Duration => Renderer.Duration;

        public double DpiScale { get; private set; } = 1.0f;

        private PixelShaderRenderer Renderer { get; } = new();

        private bool m_firstRender = false;
        private DispatcherTimer timer;
        private DateTime startTime;

        private CanvasDevice CanvasDevice { get; set; }

        public void SetShaderInputAsync(CanvasBitmap renderTargetBitmap, Rect? clip = null)
        {
            // Clip comes in as DIPs and we want it as pixels
            RectInt32? pixelClip;
            if (clip.HasValue)
            {
                RectInt32 scaledClip = new RectInt32();
                scaledClip.X = (int)(clip.Value.X * DpiScale);
                scaledClip.Y = (int)(clip.Value.Y * DpiScale);
                scaledClip.Width = (int)(clip.Value.Width * DpiScale);
                scaledClip.Height = (int)(clip.Value.Height * DpiScale);
                pixelClip = scaledClip;

                // Make our Win2D canvas match exactly the pixels we're drawing
                canvasControl.Width = scaledClip.Width;
                canvasControl.Height = scaledClip.Height;
            }
            else
            {
                pixelClip = null;

                // Make our Win2D canvas match exactly the pixels we're drawing
                canvasControl.Width = renderTargetBitmap.Size.Width;
                canvasControl.Height = renderTargetBitmap.Size.Height;
            }

            // It's ok if CanvasDevice is null here, the function can handle it
            Renderer.SetSourceBitmap(0, renderTargetBitmap, CanvasDevice, pixelClip);
        }

        public void SetShaderInput(CanvasRenderTarget renderTargetBitmap)
        {
            // It's ok if CanvasDevice is null here, the function can handle it
            Renderer.SetSourceBitmap(0, renderTargetBitmap, CanvasDevice, null);
        }

        public void InitializeForShader<T>()
            where T : unmanaged, ID2D1PixelShader, ID2D1PixelShaderDescriptor<T>
        {
            Renderer.InitializeForShader<T>();
        }

        private void CanvasControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (CanvasDevice == null)
            {
                CanvasDevice = canvasControl.Device;
            }

            ShaderDrawData drawData = new()
            {
                CanvasDevice = CanvasDevice,
                CanvasSize = sender.Size,
                EventArgs = args,
                Duration = DateTime.Now - startTime,
                Dpi = CaptureHelper.GetDpi(this),
                WipeDirection = WipeDirection
            };

            bool lastDraw = false;
            if (drawData.Duration > Renderer.Duration)
            {
                drawData.Duration = Renderer.Duration;
                lastDraw = true;
            }

            Renderer.Draw(drawData);

            if (!m_firstRender)
            {
                if (FirstRender != null)
                {
                    FirstRender(null, null);
                }
                m_firstRender = true;
            }

            if (lastDraw)
            {
                ShaderCompleted?.Invoke(null, null);
                ShaderCompleted = null;
            }
        }

        private void CanvasControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Change the canvas to be in pixels - we've already undone the DPI scale in "AdjustForDpi" below.
            canvasControl.DpiScale = 1.0f; // 1.0 after the scale and everything is applied
            canvasControl.RasterizationScale = 1.0f;

            // Initialize and start the timer
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16) // Roughly 60 FPS
            };
            timer.Tick += OnTick;
            startTime = DateTime.Now;
            timer.Start();
        }

        private void OnTick(object sender, object e)
        {
            canvasControl.Invalidate(); // Request a redraw
        }

        private void canvasControl_Unloaded(object sender, RoutedEventArgs e)
        {
            // Unload can happen without a Load in the right circumstances.
            if (timer != null)
            {
                timer.Stop();
            }
        }
    }
}
