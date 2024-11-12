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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUIGallery.Shaders
{
    /// <summary>
    /// Simple wrapper around CanvasAnimatedControl to draw a ComputeSharp shader
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

        private PixelShaderRenderer Renderer { get; } = new();

        private bool m_firstRender = false;
        private DispatcherTimer timer;
        private DateTime startTime;

        private CanvasDevice CanvasDevice { get; set; }

        public async Task SetShaderInputAsync(RenderTargetBitmap renderTargetBitmap, Rect? clip = null)
        {
            // It's ok if CanvasDevice is null here, the function can handle it
            await Renderer.SetSourceBitmap(0, renderTargetBitmap, CanvasDevice, clip);
        }

        public void InitializeForShader<T>()
            where T : unmanaged, ID2D1PixelShader, ID2D1PixelShaderDescriptor<T>
        {
            Renderer.InitializeForShader<T>();
        }

        private void CanvasAnimatedControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (CanvasDevice == null)
            {
                CanvasDevice = canvasAnimatedControl.Device;
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

        private void CanvasAnimatedControl_Loaded(object sender, RoutedEventArgs e)
        {
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
            canvasAnimatedControl.Invalidate(); // Request a redraw
        }

        private void canvasAnimatedControl_Unloaded(object sender, RoutedEventArgs e)
        {
            timer.Stop();
        }
    }
}
