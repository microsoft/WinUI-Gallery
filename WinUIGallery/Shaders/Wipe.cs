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
internal readonly partial struct Wipe(float t, int2 resolution, float2 wipeDirection) : ID2D1PixelShader
{
    /// <inheritdoc/>
    public float4 Execute()
    {
        // Constants
        float delay = 0.25f;
        float duration = 1.0f;
        float borderWidth = 200.0f;

        // Properties of the wipe
        // Could be moved out of the shader as an optimization
        float2 center = (float2)resolution * 0.5f;
        float2 d = Hlsl.Normalize(wipeDirection);
        float2 wipeStart = Hlsl.Clamp(center - (d * resolution), new float2(0, 0), resolution) - (d * borderWidth);
        float2 wipeEnd = Hlsl.Clamp(center + (d * resolution), new float2(0, 0), resolution);
        float2 wipeVector = wipeEnd - wipeStart;
        float wipeLength = Hlsl.Length(wipeVector);

        // Progress of the wipe
        float f = Hlsl.Clamp((t-delay) / duration, 0.0f, 1.0f);
        float2 xy = D2D.GetScenePosition().XY;
        float borderMin = wipeLength * f;
        float borderMax = borderMin + borderWidth;

        float2 offsetFromWipeStart = xy - wipeStart;
        float projectionAlongWipe = Hlsl.Dot(offsetFromWipeStart, d);
        float c = projectionAlongWipe / wipeLength;

        // Left side
        if (projectionAlongWipe < borderMin)
        {
            return new(0, 0, 0, 0);
        }

        // Right side
        float4 tex = D2D.SampleInput(0, xy / resolution);
        if (projectionAlongWipe > borderMax)
        {
            return new(tex.RGB, 1.0f);
        }

        // Blend in the middle
        float blend = (projectionAlongWipe - borderMin) / borderWidth;

        return tex * blend;
    }
}
