using ComputeSharp;
using ComputeSharp.D2D1;
using System;
using Windows.UI.ViewManagement.Core;

namespace WinUIGallery.Shaders;

/// <summary>
/// A shader for creating a ripple and fade out effect.
/// Ported from <see href="https://www.shadertoy.com/view/WtjyzR"/>.
/// <para>Created by Geoffrey Trousdale.</para>
/// </summary>
/// <param name="time">The current time since the start of the application.</param>
/// <param name="dispatchSize">The dispatch size for the current output.</param>
[D2DInputCount(1)]
[D2DRequiresScenePosition]
[D2DShaderProfile(D2D1ShaderProfile.PixelShader50)]
[D2DGeneratedPixelShaderDescriptor]
internal readonly partial struct RippleFade(float t, int2 resolution) : ID2D1PixelShader
{
    /// <inheritdoc/>
    public float4 Execute()
    {
        int2 xy = (int2)D2D.GetScenePosition().XY;
        float resolution1D = Hlsl.Max(resolution.X, resolution.Y);
        float2 uv = (xy - ((float2)resolution * 0.5f)) / resolution1D; // Normalized to 0-1;

        // Wave math
        float amplitude = 30.0f*t;
        float wavelength = .2f;// 1.0f / (6*t + 1.5f) + 0.1f;

        // Distance from center
        float dist = Hlsl.Length(uv);

        // Wave height
        float d = Hlsl.Max(0, (t-dist) / wavelength);
        float h = (float)-Hlsl.Sin(d * 3.14f * 2.0f);

        // Wave direction
        float2 waveDir = new(0, 0);
        if (uv.X != 0 || uv.Y != 0)
        {
            waveDir = Hlsl.Normalize(uv);
        }

        // Offset to sample from
        float2 sampleOffset = waveDir * h * amplitude;

        // Chromatic aberration
        float2 uvRed = (sampleOffset * 0.7f);
        float2 uvGreen = (sampleOffset * 1.3f);
        float2 uvBlue = (sampleOffset * 1.6f);

        float red = D2D.SampleInputAtOffset(0, uvRed).R;
        float green = D2D.SampleInputAtOffset(0, uvGreen).G;
        float blue = D2D.SampleInputAtOffset(0, uvBlue).B;

        return new(red, green, blue, 1);
    }
}
