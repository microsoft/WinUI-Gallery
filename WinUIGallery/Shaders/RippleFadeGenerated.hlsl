#define D2D_INPUT_COUNT 1
#define D2D_REQUIRES_SCENE_POSITION

#include "d2d1effecthelpers.hlsli"

float time;
int2 resolution;

D2D_PS_ENTRY(Execute)
{
    float2 xy = D2DGetScenePosition().xy;
    float resolution1D = max(resolution.x, resolution.y);
    float2 uv = (xy - ((float2)resolution * asfloat(1056964608U))) / resolution1D;
    float duration = asfloat(1073741824U);
    float t = time / duration;
    float f = min(asfloat(1065353216U), -(t - 1) * (t - 1) + 1);
    float startAmplitude = 12;
    float endAmplitude = 3;
    float amplitude = startAmplitude * (1 - f) + endAmplitude * f;
    float startWavelength = asfloat(1045220557U);
    float endWavelength = asfloat(1053609165U);
    float wavelength = startWavelength * (1 - t) + endWavelength * t;
    float dist = length(uv);
    float waveSpeed = asfloat(1071644672U);
    float d = max(0, (t * waveSpeed - dist) / wavelength);
    float h = (float)-sin(d * asfloat(1078523331U) * asfloat(1073741824U));
    float2 waveDir = float2(0, 0);
    if (uv.x != 0 || uv.y != 0)
    {
        waveDir = normalize(uv);
    }

    float2 sampleOffset = waveDir * h * amplitude;
    float2 uvRed = (sampleOffset * asfloat(1060320051U));
    float2 uvGreen = (sampleOffset * asfloat(1065353216U));
    float2 uvBlue = (sampleOffset * asfloat(1068708659U));
    uvRed = clamp(xy + uvRed, float2(asfloat(1056964608U), asfloat(1056964608U)), resolution - float2(asfloat(1056964608U), asfloat(1056964608U)));
    uvGreen = clamp(xy + uvGreen, float2(asfloat(1056964608U), asfloat(1056964608U)), resolution - float2(asfloat(1056964608U), asfloat(1056964608U)));
    uvBlue = clamp(xy + uvBlue, float2(asfloat(1056964608U), asfloat(1056964608U)), resolution - float2(asfloat(1056964608U), asfloat(1056964608U)));
    float4 red = D2DSampleInputAtPosition(0, uvRed);
    float4 green = D2DSampleInputAtPosition(0, uvGreen);
    float4 blue = D2DSampleInputAtPosition(0, uvBlue);
    return float4(red.r, green.g, blue.b, 1);
}