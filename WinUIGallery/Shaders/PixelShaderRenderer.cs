using ComputeSharp.D2D1;
using ComputeSharp.D2D1.Descriptors;
using ComputeSharp.D2D1.WinUI;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
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

        public int2 CanvasSizeInt2 => new int2((int)CanvasSize.Width, (int)CanvasSize.Height);
    }

    // Unfortunately we need to use type erasure to instantiate through generics...
    delegate object ConstantBufferFactory(ShaderDrawData drawData);

    class PixelShaderRenderer
    {
        public PixelShaderRenderer() { }

        private IReadOnlyList<ShaderSourceHelper> Sources => m_shaderSources;

#nullable enable
        public async Task SetSourceBitmap(int index,RenderTargetBitmap renderTargetBitmap, CanvasDevice? canvasDevice, Rect? clip = null)
        {
            var source = Sources[index];

            await source.SetPixelsFromTarget(renderTargetBitmap, clip);

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

                m_impl.DrawAction(data);

                data.EventArgs.DrawingSession.DrawImage(m_impl.PixelShader);
            }
        }

        public void InitializeForShader<TPixelShader>()
            where TPixelShader : unmanaged, ID2D1PixelShader, ID2D1PixelShaderDescriptor<TPixelShader>
        {
            m_impl = new PixelShaderRenderImpl(typeof(TPixelShader));

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
        public PixelShaderRenderImpl(Type type)
        {
            var drawDelegate = s_shaderDrawMap[type];
            PixelShader = drawDelegate(out Action<ShaderDrawData> drawAction, out EffectSourceList sources);
            DrawAction = drawAction;
            Sources = sources;
        }

        public ICanvasImage PixelShader { get; }
        public Action<ShaderDrawData> DrawAction { get; }
        public EffectSourceList Sources { get; }

        private delegate ICanvasImage ShaderDrawDelegate(out Action<ShaderDrawData> drawFunc, out EffectSourceList sources);

        private static readonly Dictionary<Type, ShaderDrawDelegate> s_shaderDrawMap = new()
        {
            //{ typeof(ColorfulInfinity), DrawColorfulInfinity },
            { typeof(RippleFade), DrawRippleFade },
            //{ typeof(ColorfulInfinitySolo), DrawColorfulInfinitySolo },
            { typeof(TwirlDismiss), DrawTwirlDismiss },
        };

        //private static PixelShaderEffect<ColorfulInfinity> DrawColorfulInfinity(out Action<ShaderDrawData> drawFunc, out EffectSourceList sources)
        //{
        //    PixelShaderEffect<ColorfulInfinity> colorfulInfinity = new PixelShaderEffect<ColorfulInfinity>();
        //    sources = colorfulInfinity.Sources;

        //    drawFunc = (ShaderDrawData drawData) =>
        //    {
        //        colorfulInfinity.ConstantBuffer = new ColorfulInfinity((float)drawData.Duration.TotalSeconds, drawData.CanvasSizeInt2);
        //    };

        //    return colorfulInfinity;
        //}

        private static PixelShaderEffect<RippleFade> DrawRippleFade(out Action<ShaderDrawData> drawFunc, out EffectSourceList sources)
        {
            PixelShaderEffect<RippleFade> effect = new PixelShaderEffect<RippleFade>();
            sources = effect.Sources;

            drawFunc = (ShaderDrawData drawData) =>
            {
                effect.ConstantBuffer = new RippleFade((float)drawData.Duration.TotalSeconds, drawData.CanvasSizeInt2);
            };

            return effect;
        }

        //private static PixelShaderEffect<ColorfulInfinitySolo> DrawColorfulInfinitySolo(out Action<ShaderDrawData> drawFunc, out EffectSourceList sources)
        //{
        //    PixelShaderEffect<ColorfulInfinitySolo> rippleFade = new PixelShaderEffect<ColorfulInfinitySolo>();
        //    sources = rippleFade.Sources;

        //    drawFunc = (ShaderDrawData drawData) =>
        //    {
        //        rippleFade.ConstantBuffer = new ColorfulInfinitySolo((float)drawData.Duration.TotalSeconds, drawData.CanvasSizeInt2);
        //    };

        //    return rippleFade;
        //}

        private static PixelShaderEffect<TwirlDismiss> DrawTwirlDismiss(out Action<ShaderDrawData> drawFunc, out EffectSourceList sources)
        {
            PixelShaderEffect<TwirlDismiss> effect = new PixelShaderEffect<TwirlDismiss>();
            sources = effect.Sources;

            drawFunc = (ShaderDrawData drawData) =>
            {
                effect.ConstantBuffer = new TwirlDismiss((float)drawData.Duration.TotalSeconds, drawData.CanvasSizeInt2);
            };

            return effect;
        }
    }

}
