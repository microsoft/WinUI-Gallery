using Microsoft.Graphics.Canvas;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using System;
using System.Numerics;
using Windows.Foundation;
using WinUIGallery.Shaders;

namespace WinUIGallery.ControlPages;

public sealed partial class EffectsPage : Page
{
    // The bitmap that holds the screen capture of the dialog so we can run shaders on it.
    private CanvasRenderTarget m_canvasRenderTarget;
    private bool m_isCapturePending = false;
    private CanvasRenderTarget m_fullBitmap;

    public EffectsPage()
    {
        this.InitializeComponent();
    }

    private async void ShowDialog_Click(object sender, RoutedEventArgs e)
    {
        ContentDialog dialog = new ContentDialog();

        // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
        dialog.XamlRoot = this.XamlRoot;
        dialog.Title = "Close the dialog to see the effect";
        dialog.CloseButtonText = "Close";
        dialog.Closing += Dialog_Closing;

        var result = await dialog.ShowAsync();
    }

    private async void Dialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
    {
        // Get a deferral until the shader starts rendering.
        // This keeps the dialog open until the capture is complete.
        var deferral = args.GetDeferral();

        // A dialog is actually a full window sized element because it darkens the window
        // underneath it. Get the "real" dialog box inside of it.
        var dialogBox = sender.GetDialogBox();

        // Capture the dialog to our bitmap using the Composition VisualCapture API.
        m_canvasRenderTarget = await CaptureHelper.CaptureElementAsync(dialogBox);

        // Create our shader panel which will run "TwirlDismiss" on the dialog capture.
        // ShaderPanel is a thin wrapper around Win2D's CanvasControl
        var dialogShaderPanel = new ShaderPanel();
        dialogShaderPanel.InitializeForShader<TwirlDismiss>();
        dialogShaderPanel.Width = m_canvasRenderTarget.Size.Width;
        dialogShaderPanel.Height = m_canvasRenderTarget.Size.Height;

        dialogShaderPanel.SetShaderInput(m_canvasRenderTarget);

        // Calculate the offset of the dialog box from our overlays.
        var dialogToWindowTransform = dialogBox.TransformToVisual(sender);
        var windowToOverlayTransform = XamlRoot.Content.TransformToVisual(overlayPanel);
        var offsetFromWindow = dialogToWindowTransform.TransformPoint(new Point(0, 0));
        var offsetFromOverlay = windowToOverlayTransform.TransformPoint(offsetFromWindow);

        // Display the shader panel by adding it as an overlay in the same position as the
        // dialog box.
        overlayPanel.AddOverlay(dialogShaderPanel, offsetFromOverlay);

        // Close the dialog once the shader starts running, and remove the shader panel when
        // it's done.
        dialogShaderPanel.FirstRender += (s, e) => deferral.Complete();
        dialogShaderPanel.ShaderCompleted += (s, e) => overlayPanel.ClearOverlay(dialogShaderPanel);
    }

    private void PlayEffect_Click(object sender, RoutedEventArgs e)
    {
        RenderEffect();
    }

    private async void RenderEffect()
    {
        overlayPanel.ClearOverlays();

        var overlayVisual = ElementCompositionPreview.GetElementVisual(overlayPanel);
        var compositor = overlayVisual.Compositor;
        var dpiScale = CaptureHelper.GetDpi(rootFrame) / 96.0f;

        // As part of capture, we scale the UIElement to account for DPI.
        // To cover those couple frames, we will use a non-scaled capture using VisualSurface
        var size = new Vector2((float)(rootFrame.RenderSize.Width), (float)(rootFrame.RenderSize.Height));
        CompositionVisualSurface visualSurface = compositor.CreateVisualSurface();
        visualSurface.SourceVisual = ElementCompositionPreview.GetElementVisual(rootFrame);
        visualSurface.SourceSize = size;
        var spriteVisual = compositor.CreateSpriteVisual();
        var surfaceBrush = compositor.CreateSurfaceBrush();
        surfaceBrush.Surface = visualSurface;
        surfaceBrush.Stretch = CompositionStretch.None;
        spriteVisual.Brush = surfaceBrush;
        spriteVisual.Size = size;

        m_isCapturePending = true;
        // Cover the UI with the VisualSurface before capturing, because we will scale the UI to capture at high DPI
        ElementCompositionPreview.SetElementChildVisual(rootFrameInFront, spriteVisual);
        m_fullBitmap = await CaptureHelper.CaptureElementAsync(rootFrameParent, rootFrame);
        ElementCompositionPreview.SetElementChildVisual(rootFrameInFront, null);
        m_isCapturePending = false;

        // Commented out - uncomment to save the capture we got to a file.
        //await m_fullBitmap.SaveAsync("C:\\temp\\myBitmap.bmp", Microsoft.Graphics.Canvas.CanvasBitmapFileFormat.Bmp);

        // Set up the UIElement to hold the shader
        UIElement frame = rootFrame;
        var shaderPanel = new ShaderPanel();
        shaderPanel.Width = frame.RenderSize.Width;
        shaderPanel.Height = frame.RenderSize.Height;
        overlayVisual.Opacity = 1.0f;

        if (EffectComboBox.SelectedIndex == 0)
        {
            shaderPanel.InitializeForShader<RippleFade>();
            var opacityAnimation = compositor.CreateScalarKeyFrameAnimation();
            opacityAnimation.InsertKeyFrame(0.0f, 1.0f);
            opacityAnimation.InsertKeyFrame(0.5f, 1.0f);
            opacityAnimation.InsertKeyFrame(1.0f, 0.0f);
            opacityAnimation.Duration = shaderPanel.Duration;
            overlayVisual.StartAnimation("Opacity", opacityAnimation);
        }
        else
        {
            shaderPanel.InitializeForShader<Wipe>();
            float radians = (float)new Random().NextDouble() * 3.14f * 2;
            shaderPanel.WipeDirection = new Vector2(MathF.Cos(radians), MathF.Sin(radians));
        }

        var transform = frame.TransformToVisual(null);
        Point offset = transform.TransformPoint(new Point(0, 0));
        Rect clip = new Rect(offset.X, offset.Y, shaderPanel.Width, shaderPanel.Height);

        shaderPanel.SetShaderInput(m_fullBitmap);

        overlayPanel.AddOverlay(shaderPanel, offset);
        shaderPanel.ShaderCompleted += (s, e) => overlayPanel.ClearOverlay(shaderPanel);
    }
}
