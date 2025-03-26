using ComputeSharp;
using ComputeSharp.D2D1;

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
    float2 projectPointOntoRect(float2 v, float2 rectSize)
    {
        float radiusScaleToEdge = Hlsl.Min(rectSize.X * 0.5f / Hlsl.Abs(v.X), resolution.Y * 0.5f / Hlsl.Abs(v.Y));
        return v * radiusScaleToEdge;
    }

    float2 rotateVector(float2 v, float r)
    {
        float cos = Hlsl.Cos(r);
        float sin = Hlsl.Sin(r);
        return new(v.X * cos - v.Y * sin, v.X * sin + v.Y * cos);
    }

    /// <inheritdoc/>
    public float4 Execute()
    {
        // Pixel coordinate
        float2 xy = D2D.GetScenePosition().XY;

        // Time
        float duration = 0.9f;
        float f = time / duration;

        // Calculate offset from center and initial radial coordinate
        float2 xyOffset = xy - (float2)resolution * 0.5f;
        float radiansInitial = Hlsl.Atan2(xyOffset.Y, xyOffset.X);
        float initialRadius = Hlsl.Length(xyOffset);

        // Inscribed circle inside rect
        float inscribedCircleRadius = Hlsl.Min(resolution.X, resolution.Y) * 0.5f;

        // Rotation for inside and outside edges
        float insideRadians = 10 * f * f;
        float outsideRadians = insideRadians / 20.0f;
        if (insideRadians < 1.57f)
        {
            outsideRadians = insideRadians;
        }
        else
        {
            outsideRadians = 1.57f + ((insideRadians - 1.57f) / 20.0f);
        }
        // Measure of how far "out" the pixel is from center - 0 is center, 1 is perimeter of rect
        float fracOutFromCenterRect = initialRadius / (Hlsl.Length(resolution) * 0.5f);
        float radiansDelta = fracOutFromCenterRect * outsideRadians + (1 - fracOutFromCenterRect) * insideRadians;
        float radiansFinal = radiansInitial + radiansDelta;

        // When rotating, some of the image might be clipped, so take the vector that would be
        // clipped the most and shrink the image so it fits within the bounds.
        float2 worstVector = (float2)resolution * 0.5f;
        float2 worstVectorRotated = rotateVector(worstVector, radiansDelta);
        float scaling = Hlsl.Min(1, Hlsl.Length(projectPointOntoRect(worstVectorRotated, resolution)) / Hlsl.Length(worstVectorRotated));
        float maxSafeRadius = initialRadius / scaling;

        // After a quarter turn, the image will be shrunk enough to fit inside the inscribed circle
        if (radiansDelta >= 1.57f)
        {
            maxSafeRadius = inscribedCircleRadius;
        }

        float cos = Hlsl.Cos(radiansFinal);
        float sin = Hlsl.Sin(radiansFinal);

        // The pixel coordinate, rotated
        float2 offsetRotatedUnscaled = new float2(cos, sin) * initialRadius;

        // Project point onto the outside of the rectangle
        float2 offsetRotatedProjectedOntoRect = projectPointOntoRect(offsetRotatedUnscaled, resolution);
        float maxRadius = Hlsl.Length(offsetRotatedProjectedOntoRect);

        // Another measure of how far "out" the pixel is from center
        // 0 is center, 1 is the perimeter of the inscribed circle, 1+ is outside inscribed circle
        float fracOutFromCenterPolar = initialRadius / inscribedCircleRadius;

        // If the shader samples from a point at this radius, the rect will be morphed into a
        // perfect circle, inscribed within the rect
        float targetRadius = fracOutFromCenterPolar * maxRadius;

        // Slowly morph from rect to circle over first quarter turn
        float fracToQuarterTurn = Hlsl.Sin(Hlsl.Min(1.57f, radiansDelta));
        float morphedRadius = (targetRadius * fracToQuarterTurn) + (maxSafeRadius * (1 - fracToQuarterTurn));
        float finalRadius = morphedRadius;

        // Shrink as the animation goes on
        float size = Hlsl.Clamp(1.0f - (f * f), 0.0f, 1.0f);
        if (size == 0)
        {
            return new(0, 0, 0, 0);
        }
        finalRadius /= size;

        // Calculate the final polar/scene coordinate and sample.
        float2 offsetRotated = new float2(cos, sin) * finalRadius;
        float2 sceneRotated = offsetRotated + (float2)resolution * 0.5f;
        float4 tex = D2D.SampleInputAtPosition(0, sceneRotated);

        return new(tex.X, tex.Y, tex.Z, tex.A);
    }
}
