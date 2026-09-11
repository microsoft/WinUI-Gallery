// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using System;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Composition;

namespace WinUIGallery.ControlPages;

public sealed partial class WebView2Page : Page
{
    private const string TransparentTestHtml = """
        <!doctype html>
        <html style="height:100%;background:transparent">
        <head>
          <meta name="color-scheme" content="light dark">
          <style>
            html, body {
              height: 100%;
              margin: 0;
              overflow: hidden;
              background: transparent !important;
              color: white;
              font-family: "Segoe UI", sans-serif;
            }
            body {
              display: grid;
              place-items: start center;
            }
            .label {
              margin-top: 24px;
              padding: 10px 16px;
              border: 1px solid rgba(255,255,255,.7);
              border-radius: 8px;
              background: rgba(0,0,0,.55);
            }
          </style>
        </head>
        <body>
          <div class="label">Transparent HTML; the XAML gradient should remain visible.</div>
        </body>
        </html>
        """;

    private bool _transparencyTestInitialized;

    public WebView2Page()
    {
        InitializeComponent();
        Loaded += WebView2Page_Loaded;
    }

    private async void WebView2Page_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (!_transparencyTestInitialized)
        {
            await TransparentWebView2.EnsureCoreWebView2Async();
            TransparentWebView2.NavigateToString(TransparentTestHtml);
            _transparencyTestInitialized = true;
        }

        await RefreshTransparencyDiagnosticAsync();
    }

    private async void UseTransparentBackgroundToggle_Toggled(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        TransparentWebView2.DefaultBackgroundColor =
            UseTransparentBackgroundToggle.IsOn ? Colors.Transparent : Colors.White;
        await RefreshTransparencyDiagnosticAsync();
    }

    private async void RefreshTransparencyDiagnostic_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        await RefreshTransparencyDiagnosticAsync();
    }

    private async Task RefreshTransparencyDiagnosticAsync()
    {
        if (!_transparencyTestInitialized)
        {
            return;
        }

        await Task.Delay(500);

        Color requestedColor = TransparentWebView2.DefaultBackgroundColor;
        Visual? childVisual = ElementCompositionPreview.GetElementChildVisual(TransparentWebView2);

        if (childVisual is SpriteVisual spriteVisual &&
            spriteVisual.Brush is CompositionColorBrush colorBrush)
        {
            Color hostColor = colorBrush.Color;
            bool reproducesTransparencyBug = requestedColor.A == 0 && hostColor.A != 0;

            TransparencyVerdictText.Text = reproducesTransparencyBug
                ? "REPRODUCED: WebView2 requested transparency, but its host visual is opaque."
                : requestedColor.A == 0
                    ? "NOT REPRODUCED: WebView2 and its host visual are transparent."
                    : "Opaque baseline enabled.";
            TransparencyDetailsText.Text =
                $"Requested background: {FormatColor(requestedColor)}; host visual brush: {FormatColor(hostColor)}.";
            return;
        }

        TransparencyVerdictText.Text =
            "INCONCLUSIVE: the WebView2 host visual or its color brush was not available.";
        TransparencyDetailsText.Text =
            $"Requested background: {FormatColor(requestedColor)}.";
    }

    private static string FormatColor(Color color)
    {
        return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
    }
}
