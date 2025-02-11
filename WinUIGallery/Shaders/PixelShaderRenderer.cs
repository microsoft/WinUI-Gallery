using ComputeSharp.D2D1;
using ComputeSharp.D2D1.Descriptors;
using ComputeSharp.D2D1.Interop;
using ComputeSharp.D2D1.WinUI;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics;
using Windows.Graphics.Effects;

using EffectSourceList = System.Collections.Generic.IList<Windows.Graphics.Effects.IGraphicsEffectSource>;

namespace WinUIGallery.Shaders
{
    class ShaderDrawData
    {
        public CanvasDevice CanvasDevice;
        public Windows.Foundation.Size CanvasSize;
        public CanvasDrawEventArgs EventArgs;
        public TimeSpan Duration;
        public SizeInt32 InputSize;
        public float Dpi;
        public Vector2 WipeDirection;

        public int2 CanvasSizeInt2 => new int2((int)CanvasSize.Width, (int)CanvasSize.Height);

        public int2 InputSizeInt2 => new int2(InputSize.Width, InputSize.Height);
    }

    class PixelShaderRenderer
    {
        public PixelShaderRenderer() { }

        public TimeSpan Duration => m_impl.Duration;

        private IReadOnlyList<ShaderSourceHelper> Sources => m_shaderSources;

#nullable enable
        public void SetSourceBitmap(int index, CanvasBitmap renderTargetBitmap, CanvasDevice? canvasDevice, RectInt32? clip = null)
        {
            var source = Sources[index];

            source.SetPixelsFromTarget(renderTargetBitmap, clip);

            if (canvasDevice != null && m_impl != null)
            {
                source.BindResources(canvasDevice, m_impl.Sources);
            }
        }

        public void Draw(ShaderDrawData data)
        {
            if (m_impl != null)
            {
                foreach (var sourceHelper in m_shaderSources)
                {
                    sourceHelper.BindResources(data.CanvasDevice, m_impl.Sources);
                }

                foreach (var source in m_impl.Sources)
                {
                    if (source == null)
                    {
                        // One of our sources isn't bound. Bail.
                        return;
                    }
                }

                if (m_shaderSources.Count > 0)
                {
                    data.InputSize = m_shaderSources[0].InputSize;
                }

                m_impl.DrawAction(data);

                data.EventArgs.DrawingSession.DrawImage(m_impl.PixelShader);
            }
        }

        public void InitializeForShader<TPixelShader>()
            where TPixelShader : unmanaged, ID2D1PixelShader, ID2D1PixelShaderDescriptor<TPixelShader>
        {
            m_impl = PixelShaderRenderImpl.GetRenderImplForShader<TPixelShader>();

            var sourcesCount = m_impl.Sources.Count;
            m_shaderSources = new List<ShaderSourceHelper>(sourcesCount);
            for (int i = 0; i < sourcesCount; i++)
            {
                m_shaderSources.Add(new ShaderSourceHelper(i));
            }
        }

        private List<ShaderSourceHelper> m_shaderSources = new List<ShaderSourceHelper>(0);

        private PixelShaderRenderImpl? m_impl = null;
    }

    class PixelShaderRenderImpl
    {
        public static PixelShaderRenderImpl GetRenderImplForShader<TPixelShader>()
        {
            var shaderFactory = s_shaderFactoryMap[typeof(TPixelShader)];
            return shaderFactory();
        }

        private PixelShaderRenderImpl() { }

        public required ICanvasImage PixelShader { get; set; }
        public required Action<ShaderDrawData> DrawAction { get; set; }
        public required EffectSourceList Sources { get; set; }
        public required TimeSpan Duration { get; set; }

        private delegate PixelShaderRenderImpl ShaderEffectFactory();

        private static readonly Dictionary<Type, ShaderEffectFactory> s_shaderFactoryMap = new()
        {
            { typeof(RippleFade), RippleFadeFactory },
            { typeof(TwirlDismiss), TwirlDismissFactory },
            { typeof(Wipe), WipeEffectFactory },
            { typeof(IdentityShader), IdentityShaderFactory },
        };

        private static PixelShaderRenderImpl RippleFadeFactory()
        {
            PixelShaderEffect<RippleFade> effect = new PixelShaderEffect<RippleFade>();
            var s = D2D1ReflectionServices.GetShaderInfo<RippleFade>().HlslSource;
            Debug.WriteLine(s);

            var drawAction = (ShaderDrawData drawData) =>
            {
                effect.ConstantBuffer = new RippleFade((float)drawData.Duration.TotalSeconds, drawData.InputSizeInt2);
            };

            return new PixelShaderRenderImpl()
            {
                PixelShader = effect,
                Sources = effect.Sources,
                Duration = TimeSpan.FromSeconds(2.0),
                DrawAction = drawAction
            };
        }

        private static PixelShaderRenderImpl TwirlDismissFactory()
        {
            PixelShaderEffect<TwirlDismiss> effect = new PixelShaderEffect<TwirlDismiss>();

            var drawAction = (ShaderDrawData drawData) =>
            {
                // Multiply size by DPI since we don't bother capturing TwirlDismiss at full resolution
                effect.ConstantBuffer = new TwirlDismiss((float)drawData.Duration.TotalSeconds, new ComputeSharp.Int2((int)(drawData.InputSizeInt2.X * drawData.Dpi / 96.0f), (int)(drawData.InputSizeInt2.Y * drawData.Dpi / 96.0f)));
            };

            return new PixelShaderRenderImpl()
            {
                PixelShader = effect,
                Sources = effect.Sources,
                Duration = TimeSpan.FromSeconds(1.0),
                DrawAction = drawAction
            };
        }

        private static PixelShaderRenderImpl WipeEffectFactory()
        {
            PixelShaderEffect<Wipe> effect = new PixelShaderEffect<Wipe>();

            var drawAction = (ShaderDrawData drawData) =>
            {
                effect.ConstantBuffer = new Wipe((float)drawData.Duration.TotalSeconds, drawData.InputSizeInt2, drawData.WipeDirection);
            };

            return new PixelShaderRenderImpl()
            {
                PixelShader = effect,
                Sources = effect.Sources,
                Duration = TimeSpan.FromSeconds(2.0),
                DrawAction = drawAction
            };
        }

        private static PixelShaderRenderImpl IdentityShaderFactory()
        {
            PixelShaderEffect<IdentityShader> effect = new PixelShaderEffect<IdentityShader>();

            var drawAction = (ShaderDrawData drawData) =>
            {
                effect.ConstantBuffer = new IdentityShader();
            };

            return new PixelShaderRenderImpl()
            {
                PixelShader = effect,
                Sources = effect.Sources,
                Duration = TimeSpan.FromSeconds(5.0), // extra long to help with debugging
                DrawAction = drawAction
            };
        }
    }

}
