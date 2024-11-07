using ComputeSharp;
using ComputeSharp.D2D1;
using System;
using System.Numerics;

namespace WinUIGallery.Shaders;

/// <summary>
/// A shader creating an abstract and colorful animation.
/// Ported from <see href="https://www.shadertoy.com/view/WtjyzR"/>.
/// <para>Created by Benoit Marini.</para>
/// <para>License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.</para>
/// </summary>
/// <param name="time">The current time since the start of the application.</param>
/// <param name="dispatchSize">The dispatch size for the current output.</param>
[D2DInputCount(1)]
[D2DInputSimple(0)]
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
        //float insideRadians = time * 3.14f * 3;
        float outsideRadians = insideRadians / 2.0f;
        float size = Hlsl.Max(0.0f, 1.0f - (f * f));

        //float cos = Hlsl.Cos(insideRadians);
        //float sin = Hlsl.Sin(insideRadians);

        //float xNew = uv.X * cos - uv.Y * sin;
        //float yNew = uv.X * sin + uv.Y * cos;

        //float2 uvFinal = new float2(xNew, yNew);

        float radiansInitial = Hlsl.Atan2(xyOffset.Y, xyOffset.X);
        float radiansDelta =  percentOut * outsideRadians + (1-percentOut) * insideRadians;
        float radiansFinal = radiansInitial + radiansDelta;


        float cos = Hlsl.Cos(radiansFinal);
        float sin = Hlsl.Sin(radiansFinal);
        if (size == 0)
        {
            return new(0, 0, 0, 0);
        }
        float2 offsetRotated = new float2(cos, sin) * radius / size;

        //float cos = Hlsl.Cos(radiansDelta);
        //float sin = Hlsl.Sin(radiansDelta);
        //float xNew = xyOffset.X * cos - xyOffset.Y * sin;
        //float yNew = xyOffset.X * sin + xyOffset.Y * cos;
        //float2 offsetRotated = new float2(xNew, yNew) * size;

        float2 uvFinal = offsetRotated / resolution + new float2(0.5f, 0.5f);

        if (uvFinal.X < 0.0f || uvFinal.Y < 0.0f || uvFinal.X > 1.0f || uvFinal.Y > 1.0f)
        {
            return new(0, 0, 0, 0);
        }

        float3 tex = D2D.SampleInput(0, uvFinal).RGB;

        return new(tex.X, tex.Y, tex.Z, 1.0f);
    }
}
