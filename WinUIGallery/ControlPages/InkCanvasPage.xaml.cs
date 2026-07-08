//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Input.Inking;

namespace WinUIGallery.ControlPages
{
    // The InkCanvas public surface mirrors the classic
    // Windows.UI.Xaml.Controls.InkCanvas: a default constructor and a single
    // InkPresenter property. The control inks by default, so a bare <InkCanvas/>
    // draws wet ink for mouse, pen, and touch with no app code. Example 2 shows an
    // InkToolBar driving the canvas via TargetInkCanvas. Example 3 configures the
    // InkPresenter from code exactly like the classic InkCanvas.
    public sealed partial class InkCanvasPage : Page
    {
        public InkCanvasPage()
        {
            this.InitializeComponent();

            // Example 3: configure the presenter directly, just like WUXC's InkCanvas.
            inkCanvas3.InkPresenter.InputDeviceTypes =
                CoreInputDeviceTypes.Mouse | CoreInputDeviceTypes.Pen | CoreInputDeviceTypes.Touch;
            ApplyDrawingAttributes(Microsoft.UI.Colors.Red, 6);

            // Example 4: listen to stroke events through the InkPresenter.
            inkCanvas4.InkPresenter.StrokesCollected += (s, e) =>
                statusText4.Text = $"Collected {e.Strokes.Count} stroke(s). Total: {inkCanvas4.InkPresenter.StrokeContainer.GetStrokes().Count}.";
            inkCanvas4.InkPresenter.StrokesErased += (s, e) =>
                statusText4.Text = $"Erased {e.Strokes.Count} stroke(s). Total: {inkCanvas4.InkPresenter.StrokeContainer.GetStrokes().Count}.";
        }

        private void ApplyDrawingAttributes(Windows.UI.Color color, double size)
        {
            var attributes = inkCanvas3.InkPresenter.CopyDefaultDrawingAttributes();
            attributes.Color = color;
            attributes.Size = new Size(size, size);
            inkCanvas3.InkPresenter.UpdateDefaultDrawingAttributes(attributes);
        }

        private void OnRedPenClick(object sender, RoutedEventArgs e)
        {
            ApplyDrawingAttributes(Microsoft.UI.Colors.Red, 6);
        }

        private void OnBluePenClick(object sender, RoutedEventArgs e)
        {
            ApplyDrawingAttributes(Microsoft.UI.Colors.Blue, 2);
        }

        private void OnTouchToggled(object sender, RoutedEventArgs e)
        {
            // Toggled fires while IsOn="True" is applied during InitializeComponent,
            // before inkCanvas3 (declared later in the XAML) is assigned. The
            // constructor configures the presenter, so ignore these early events.
            if (inkCanvas3 == null)
            {
                return;
            }

            var types = CoreInputDeviceTypes.Mouse | CoreInputDeviceTypes.Pen;
            if (touchToggle.IsOn)
            {
                types |= CoreInputDeviceTypes.Touch;
            }

            inkCanvas3.InkPresenter.InputDeviceTypes = types;
        }

        private void OnEnabledToggled(object sender, RoutedEventArgs e)
        {
            if (inkCanvas3 == null)
            {
                return;
            }

            inkCanvas3.InkPresenter.IsInputEnabled = enabledToggle.IsOn;
        }

        private void OnCountStrokesClick(object sender, RoutedEventArgs e)
        {
            var count = inkCanvas4.InkPresenter.StrokeContainer.GetStrokes().Count;
            statusText4.Text = $"The canvas has {count} stroke(s).";
        }

        private void OnClearStrokesClick(object sender, RoutedEventArgs e)
        {
            inkCanvas4.InkPresenter.StrokeContainer.Clear();
            statusText4.Text = "Cleared all strokes.";
        }

        private async void OnSaveReloadClick(object sender, RoutedEventArgs e)
        {
            var container = inkCanvas4.InkPresenter.StrokeContainer;
            var before = container.GetStrokes().Count;
            if (before == 0)
            {
                statusText4.Text = "Draw something first, then save & reload.";
                return;
            }

            // Round-trip the strokes through an in-memory stream: save, clear, reload.
            var stream = new InMemoryRandomAccessStream();
            await container.SaveAsync(stream.GetOutputStreamAt(0));
            container.Clear();
            await container.LoadAsync(stream.GetInputStreamAt(0));

            var after = container.GetStrokes().Count;
            statusText4.Text = $"Saved and reloaded {before} stroke(s); canvas now has {after}.";
        }
    }
}
