// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Composition;
using Microsoft.Graphics.DirectX;
using Microsoft.UI.Composition;
using Microsoft.UI.Composition.Scenes;
using Microsoft.UI.Content;
using System;
using System.Numerics;
using System.Threading.Tasks;

partial class HelmetScenario
{
    public static async Task<ContentIsland> CreateIsland(Compositor compositor)
    {
        var visual = await LoadScene_DamagedHelmet(compositor);

        var island = ContentIsland.Create(visual);
        return island;
    }

    private static async Task<Visual> LoadScene_DamagedHelmet(Compositor compositor)
    {
        // Initialize Win2D, used for loading bitmaps.

        var canvasDevice = new CanvasDevice();
        var graphicsDevice = CanvasComposition.CreateCompositionGraphicsDevice(
            compositor, canvasDevice);

        // Create the Visuals and SceneNode structure, along with default rotation animations.

        var sceneVisual = SceneVisual.Create(compositor);
        sceneVisual.RelativeOffsetAdjustment = new Vector3(0.5f, 0.5f, 0.0f);

        var worldNode = SceneNode.Create(compositor);
        sceneVisual.Root = worldNode;

        var rotateAngleAnimation = compositor.CreateScalarKeyFrameAnimation();
        rotateAngleAnimation.InsertKeyFrame(0.0f, 0.0f);
        rotateAngleAnimation.InsertKeyFrame(0.5f, 360.0f);
        rotateAngleAnimation.InsertKeyFrame(1.0f, 0.0f);
        rotateAngleAnimation.Duration = TimeSpan.FromSeconds(15);
        rotateAngleAnimation.IterationBehavior = AnimationIterationBehavior.Forever;
        worldNode.Transform.RotationAxis = new Vector3(0, 1, 0);
        worldNode.Transform.StartAnimation("RotationAngleInDegrees", rotateAngleAnimation);

        var sceneNode0 = SceneNode.Create(compositor);
        sceneNode0.Transform.Scale = new Vector3(170);
        sceneNode0.Transform.Orientation = new Quaternion(0.70710683f, 0.0f, 0.0f, 0.70710683f);
        worldNode.Children.Add(sceneNode0);

        var sceneNodeForTheGLTFMesh0 = SceneNode.Create(compositor);
        sceneNode0.Children.Add(sceneNodeForTheGLTFMesh0);

        // Load all file data in parallel:
        // - Although Scene Graph objects prefer a UI thread, Win2D can load and create the bitmaps
        //   on parallel background threads.

        var vertexData = SceneNodeCommon.LoadMemoryBufferFromUriAsync(
            "Assets/SceneNode/DamagedHelmet6.bin");

        var normalData = SceneNodeCommon.LoadMemoryBufferFromUriAsync(
            "Assets/SceneNode/DamagedHelmet7.bin");

        var texCoordData = SceneNodeCommon.LoadMemoryBufferFromUriAsync(
            "Assets/SceneNode/DamagedHelmet8.bin");

        var indexData = SceneNodeCommon.LoadMemoryBufferFromUriAsync(
            "Assets/SceneNode/DamagedHelmet9.bin");

        var canvasBitmap0 = SceneNodeCommon.LoadIntoCanvasBitmap(
            canvasDevice, "Assets/SceneNode/DamagedHelmet1.bmp");

        var canvasBitmap1 = SceneNodeCommon.LoadIntoCanvasBitmap(
            canvasDevice, "Assets/SceneNode/DamagedHelmet2.bmp");

        var canvasBitmap2 = SceneNodeCommon.LoadIntoCanvasBitmap(
            canvasDevice, "Assets/SceneNode/DamagedHelmet3.bmp");

        var canvasBitmap3 = SceneNodeCommon.LoadIntoCanvasBitmap(
            canvasDevice, "Assets/SceneNode/DamagedHelmet4.bmp");

        var canvasBitmap4 = SceneNodeCommon.LoadIntoCanvasBitmap(
            canvasDevice, "Assets/SceneNode/DamagedHelmet5.bmp");

        await vertexData;
        await normalData;
        await texCoordData;
        await indexData;
        await canvasBitmap0;
        await canvasBitmap1;
        await canvasBitmap2;
        await canvasBitmap3;
        await canvasBitmap4;


        // Generate mipmaps from the bitmaps, which are needed for 3D rendering.

        var materialInput0 = SceneNodeCommon.LoadMipmapFromBitmap(graphicsDevice, canvasBitmap0.Result);
        var materialInput1 = SceneNodeCommon.LoadMipmapFromBitmap(graphicsDevice, canvasBitmap1.Result);
        var materialInput2 = SceneNodeCommon.LoadMipmapFromBitmap(graphicsDevice, canvasBitmap2.Result);
        var materialInput3 = SceneNodeCommon.LoadMipmapFromBitmap(graphicsDevice, canvasBitmap3.Result);
        var materialInput4 = SceneNodeCommon.LoadMipmapFromBitmap(graphicsDevice, canvasBitmap4.Result);


        // Copy loaded binary data into mesh: verticies, normals, ...

        var mesh0 = SceneMesh.Create(compositor);
        mesh0.PrimitiveTopology = DirectXPrimitiveTopology.TriangleList;
        mesh0.FillMeshAttribute(SceneAttributeSemantic.Vertex, DirectXPixelFormat.R32G32B32Float, vertexData.Result);
        mesh0.FillMeshAttribute(SceneAttributeSemantic.Normal, DirectXPixelFormat.R32G32B32Float, normalData.Result);
        mesh0.FillMeshAttribute(SceneAttributeSemantic.TexCoord0, DirectXPixelFormat.R32G32Float, texCoordData.Result);
        mesh0.FillMeshAttribute(SceneAttributeSemantic.Index, DirectXPixelFormat.R16UInt, indexData.Result);


        // Initialize the material with different texture inputs (color, roughness, normals, ...)

        var sceneMaterial0 = SceneMetallicRoughnessMaterial.Create(compositor);

        var renderComponent0 = SceneMeshRendererComponent.Create(compositor);
        renderComponent0.Mesh = mesh0;
        renderComponent0.Material = sceneMaterial0;
        sceneNodeForTheGLTFMesh0.Components.Add(renderComponent0);

        sceneMaterial0.BaseColorFactor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        sceneMaterial0.BaseColorInput = SceneNodeCommon.CreateMaterial(
            compositor, materialInput0, renderComponent0, "BaseColorInput"); ;

        sceneMaterial0.RoughnessFactor = 1.0f;
        sceneMaterial0.MetallicFactor = 1.0f;
        sceneMaterial0.MetallicRoughnessInput = SceneNodeCommon.CreateMaterial(
            compositor, materialInput1, renderComponent0, "MetallicRoughnessInput");

        sceneMaterial0.NormalScale = 1.0f;
        sceneMaterial0.NormalInput = SceneNodeCommon.CreateMaterial(
            compositor, materialInput2, renderComponent0, "NormalInput");

        sceneMaterial0.OcclusionStrength = 1.0f;
        sceneMaterial0.OcclusionInput = SceneNodeCommon.CreateMaterial(
            compositor, materialInput3, renderComponent0, "OcclusionInput");

        sceneMaterial0.AlphaMode = SceneAlphaMode.Opaque;
        sceneMaterial0.IsDoubleSided = false;
        sceneMaterial0.EmissiveFactor = new Vector3(1.0f, 1.0f, 1.0f);
        sceneMaterial0.EmissiveInput = SceneNodeCommon.CreateMaterial(
            compositor, materialInput4, renderComponent0, "EmissiveInput");

        return sceneVisual;
    }
}
