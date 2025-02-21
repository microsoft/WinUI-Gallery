using ComputeSharp;
using ComputeSharp.D2D1;
using System;
using Windows.UI.ViewManagement.Core;

namespace WinUIGallery.Shaders;

/// <summary>
/// A shader that displays the given input with no changes.
/// This is most useful as a debugging tool for looking at the inputs
/// passed into other shaders.
/// Ported from <see href="https://www.shadertoy.com/view/WtjyzR"/>.
/// <para>Created by Geoffrey Trousdale.</para>
/// </summary>
[D2DInputCount(1)]
[D2DShaderProfile(D2D1ShaderProfile.PixelShader50)]
[D2DGeneratedPixelShaderDescriptor]
internal readonly partial struct IdentityShader() : ID2D1PixelShader
{
    /// <inheritdoc/>
    public float4 Execute()
    {
        return D2D.GetInput(0);
    }
}
