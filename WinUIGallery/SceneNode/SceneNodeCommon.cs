using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.Storage;
using WinRT;
using Microsoft.Graphics.Canvas.UI.Composition;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.DirectX;
using Microsoft.UI.Composition;
using Microsoft.UI.Composition.Scenes;
using Windows.Graphics;
using Windows.UI.Popups;

class SceneNodeCommon
{
    public static CompositionMipmapSurface LoadMipmapFromBitmap(
        CompositionGraphicsDevice graphicsDevice, CanvasBitmap canvasBitmap)
    {
        var size = new SizeInt32(2048, 2048);
        var mipmapSurface = graphicsDevice.CreateMipmapSurface(
            size,
            DirectXPixelFormat.B8G8R8A8UIntNormalized,
            DirectXAlphaMode.Premultiplied);

        var drawDestRect = new Rect(0, 0, size.Width, size.Height);
        var drawSourceRect = new Rect(0, 0, size.Width, size.Height);
        for (uint level = 0; level < mipmapSurface.LevelCount; ++level)
        {
            // Draw the image to the surface
            var drawingSurface = mipmapSurface.GetDrawingSurfaceForLevel(level);

            using (var session = CanvasComposition.CreateDrawingSession(drawingSurface))
            {
                session.Clear(Windows.UI.Color.FromArgb(0, 0, 0, 0));
                session.DrawImage(canvasBitmap, drawDestRect, drawSourceRect);
            }

            drawDestRect = new Rect(0, 0, drawDestRect.Width / 2, drawDestRect.Height / 2);
        }

        return mipmapSurface;
    }

    public static SceneSurfaceMaterialInput CreateMaterial(
        Compositor compositor, 
        CompositionMipmapSurface mipmap,
        SceneMeshRendererComponent rendererComponent,
        string mapping)
    {
        var materialInput = SceneSurfaceMaterialInput.Create(compositor);
        materialInput.Surface = mipmap;
        materialInput.BitmapInterpolationMode = 
            CompositionBitmapInterpolationMode.MagLinearMinLinearMipLinear;

        materialInput.WrappingUMode = SceneWrappingMode.Repeat;
        materialInput.WrappingVMode = SceneWrappingMode.Repeat;

        rendererComponent.UVMappings[mapping] = SceneAttributeSemantic.TexCoord0;

        return materialInput;
    }

    public static async Task<MemoryBuffer> LoadMemoryBufferFromUriAsync(Uri uri)
    {
        var file = await StorageFile.GetFileFromApplicationUriAsync(uri);
        var buffer = await FileIO.ReadBufferAsync(file);
        return CopyToMemoryBuffer(buffer);
    }

    public static MemoryBuffer CopyToMemoryBuffer(IBuffer buffer)
    {
        var dataReader = DataReader.FromBuffer(buffer);

        var memBuffer = new MemoryBuffer(buffer.Length);
        var memBufferRef = memBuffer.CreateReference();
        var memBufferByteAccess = memBufferRef.As<IMemoryBufferByteAccess>();

        unsafe
        {
            byte* bytes = null;
            uint capacity;
            memBufferByteAccess.GetBuffer(&bytes, &capacity);

            for (int i = 0; i < capacity; ++i)
            {
                bytes[i] = dataReader.ReadByte();
            }
        }

        return memBuffer;
    }


    public static MemoryBuffer CopyToMemoryBuffer(byte[] a)
    {
        MemoryBuffer mb = new MemoryBuffer((uint)a.Length);
        var mbr = mb.CreateReference();
        var mba = mbr.As<IMemoryBufferByteAccess>();
        unsafe
        {
            byte* bytes = null;
            uint capacity;
            mba.GetBuffer(&bytes, &capacity);
            for (int i = 0; i < capacity; ++i)
            {
                bytes[i] = a[i];
            }
        }

        return mb;
    }



    [ComImport,
    Guid("5b0d3235-4dba-4d44-865e-8f1d0e4fd04d"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMemoryBufferByteAccess
    {
        unsafe void GetBuffer(byte** bytes, uint* capacity);
    }
}
