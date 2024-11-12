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

        public SizeInt32 BufferSize => m_bufferSize;

        public async Task SetPixelsFromTarget(RenderTargetBitmap bitmap, Rect? clip = null)
        {
            SizeInt32 bitmapSize = new SizeInt32(bitmap.PixelWidth, bitmap.PixelHeight);
            var pixelBuffer = await bitmap.GetPixelsAsync();

            if (clip != null)
            {
                m_bitmapBuffer = ClipBuffer(pixelBuffer, bitmapSize, clip.Value, out SizeInt32 newBufferSize);
                m_bufferSize = newBufferSize;
            }
            else
            {
                m_bitmapBuffer = pixelBuffer;
                m_bufferSize = bitmapSize;
            }

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

            if (m_bitmapBuffer == null)
            {
                // We don't have pixels yet, so can't create anything.
                return;
            }

            var canvasBitmap = CanvasBitmap.CreateFromBytes(
                device,
                m_bitmapBuffer.ToArray(),
                m_bufferSize.Width,
                m_bufferSize.Height,
                DirectXPixelFormat.B8G8R8A8UIntNormalized);

            sourceCollection[m_sourceIndex] = canvasBitmap;

            m_dirty = false;
        }

        private IBuffer ClipBuffer(IBuffer buffer, SizeInt32 bufferSize, Rect clip, out SizeInt32 newBufferSize)
        {
            byte[] pixels = buffer.ToArray();
            int stride = bufferSize.Width * 4; // Assuming 4 bytes per pixel (BGRA)

            // Extract the cropped pixels
            int cropX = (int)clip.X;
            int cropY = (int)clip.Y;
            int cropWidth = Math.Min((int)clip.Width, bufferSize.Width - cropX);
            int cropHeight = Math.Min((int)clip.Height, bufferSize.Height - cropY);
            byte[] croppedPixels = new byte[cropWidth * cropHeight * 4];
            for (int y = 0; y < cropHeight; y++)
            {
                Array.Copy(
                    pixels,
                    ((cropY + y) * stride) + (cropX * 4), // Source start index
                    croppedPixels,
                    y * cropWidth * 4,                    // Destination start index
                    cropWidth * 4                         // Number of bytes to copy
                );
            }

            newBufferSize = new SizeInt32(cropWidth, cropHeight);
            return croppedPixels.AsBuffer();
        }

        private IBuffer m_bitmapBuffer = null;
        private SizeInt32 m_bufferSize = new();
        private readonly int m_sourceIndex;
        private bool m_dirty = true;
    }
}
