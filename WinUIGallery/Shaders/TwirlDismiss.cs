using ComputeSharp;
using ComputeSharp.D2D1;
using System;
using System.Numerics;

namespace WinUIGallery.Shaders;

/// <summary>
/// A shader to make an image spin and shrink.
/// Ported from <see href="https://www.shadertoy.com/view/WtjyzR"/>.
/// <para>Created by Geoffrey Trousdale.</para>
/// </summary>
/// <param name="time">The current time since the start of the application.</param>
/// <param name="dispatchSize">The dispatch size for the current output.</param>
[D2DInputCount(1)]
[D2DRequiresScenePosition]
[D2DShaderProfile(D2D1ShaderProfile.PixelShader50)]
[D2DGeneratedPixelShaderDescriptor]
internal readonly partial struct TwirlDismiss(float time, int2 resolution) : ID2D1PixelShader
{
    /// <inheritdoc/>
    public float4 Execute()
    {
        int2 xy = (int2)D2D.GetScenePosition().XY;

        float2 xyOffset = xy - (float2)resolution * 0.5f;
        float radius = Hlsl.Length(xyOffset);
        float maxDist = Hlsl.Length((float2)resolution * 0.5f);
        float percentOut = radius / maxDist;

        float duration = 1.0f;
        float f = time / duration;
        float insideRadians = 20 * f * f;
        float outsideRadians = insideRadians / 2.0f;
        float size = Hlsl.Max(0.0f, 1.0f - (f * f));
        float radiansInitial = Hlsl.Atan2(xyOffset.Y, xyOffset.X);
        float radiansDelta = percentOut * outsideRadians + (1 - percentOut) * insideRadians;
        float radiansFinal = radiansInitial + radiansDelta;

        float cos = Hlsl.Cos(radiansFinal);
        float sin = Hlsl.Sin(radiansFinal);
        if (size == 0)
        {
            return new(0, 0, 0, 0);
        }
        float2 offsetRotated = new float2(cos, sin) * radius / size;
        float2 offsetScene = offsetRotated + (float2)resolution * 0.5f;

        float4 tex = D2D.SampleInputAtPosition(0, offsetScene);

        if (tex.A < 0.5f)
        {
            return new(0, 0, 0, 0);
        }

        return new(tex.X, tex.Y, tex.Z, 1.0f);
    }
}
