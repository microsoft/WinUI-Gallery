using ComputeSharp;
using ComputeSharp.D2D1;
using Microsoft.UI.Composition.Interactions;
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
internal readonly partial struct RippleFade(float time, int2 resolution) : ID2D1PixelShader
{
    /// <inheritdoc/>
    public float4 Execute()
    {
        int2 xy = (int2)D2D.GetScenePosition().XY;
        float resolution1D = Hlsl.Max(resolution.X, resolution.Y);
        float2 uv = (xy - ((float2)resolution * 0.5f)) / resolution1D; // Normalized to -0.5 to 0.5;

        float duration = 2.0f;
        // function that is linear with time
        float t = time / duration;
        // easing function that starts at 0 and asymptotes at 1 as x->infinity
        float f = Hlsl.Min(1.0f, -(t - 1) * (t - 1) + 1);

        // Wave math
        float startAmplitude = 12;
        float endAmplitude = 3;
        float amplitude = startAmplitude * (1 - f) + endAmplitude * f;
        float startWavelength = .2f;
        float endWavelength = .4f;
        float wavelength = startWavelength * (1 - t) + endWavelength * t;

        // Distance from center
        float dist = Hlsl.Length(uv);

        // Wave distance and height
        float waveSpeed = 1.75f;
        float d = Hlsl.Max(0, (t*waveSpeed-dist) / wavelength);
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
        float2 uvGreen = (sampleOffset * 1.0f);
        float2 uvBlue = (sampleOffset * 1.4f);

        // Clamp to the image size, so we don't sample outside the image
        uvRed = Hlsl.Clamp(xy+uvRed, new float2(0, 0), resolution - new float2(1,1));
        uvGreen = Hlsl.Clamp(xy+uvGreen, new float2(0, 0), resolution - new float2(1, 1));
        uvBlue = Hlsl.Clamp(xy+uvBlue, new float2(0, 0), resolution - new float2(1, 1));

        float2 halvesies = new float2(0.5f, 0.5f);
        float4 red = D2D.SampleInputAtPosition(0, uvRed + halvesies);
        float4 green = D2D.SampleInputAtPosition(0, uvGreen + halvesies);
        float4 blue = D2D.SampleInputAtPosition(0, uvBlue + halvesies);

        return new float4(red.R, green.G, blue.B, 1);
    }
}
