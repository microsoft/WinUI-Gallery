using Microsoft.Graphics.Canvas;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Storage.Streams;
using Windows.Graphics.DirectX;
using Windows.Foundation;
using Windows.Devices.PointOfService;
using Windows.Graphics;
using ComputeSharp.D2D1.WinUI;
using Windows.Graphics.Effects;

namespace WinUIGallery.Shaders
{
    // A shader source helps with the two-phase conversion of buffers to an actual usable Win2D resource.
    internal class ShaderSourceHelper
    {
        public ShaderSourceHelper(int index)
        {
            m_sourceIndex = index;
        }

        public SizeInt32 InputSize => new SizeInt32((int)m_canvasBitmap.SizeInPixels.Width, (int)m_canvasBitmap.SizeInPixels.Height);

        public void SetPixelsFromTarget(CanvasBitmap bitmap, RectInt32? clip = null)
        {
            m_canvasBitmap = bitmap;

            // Invalidate so the next "EnsureResources" regenerates everything.
            // Note that this is importantly AFTER the await above. Otherwise if we attempt to
            // draw while still awaiting, we may drop a frame.
            m_dirty = true;
        }

        public void BindResources(CanvasDevice device, IList<IGraphicsEffectSource> sourceCollection)
        {
            if (!m_dirty)
            {
                // No change, so don't recreate anything
                return;
            }

            sourceCollection[m_sourceIndex] = m_canvasBitmap;

            m_dirty = false;
        }

        private CanvasBitmap m_canvasBitmap = null;
        private readonly int m_sourceIndex;
        private bool m_dirty = true;
    }
}
