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

        // ------------------------------------------------------------------
        // Example 5: advanced ink-thread features.
        // ------------------------------------------------------------------

        private InkCanvas _customDryInkCanvas;
        private int _customDryHarvestCount;

        private void OnActivateCustomDryingClick(object sender, RoutedEventArgs e)
        {
            if (_customDryInkCanvas != null)
            {
                customDryStatus.Text = "Already active. Draw on the canvas below — the app harvests each stroke as it dries.";
                return;
            }

            try
            {
                // NOTE ON DESKTOP CUSTOM DRYING:
                // Classic InkSynchronizer custom drying (ActivateCustomDrying + BeginDry/EndDry,
                // where the app owns ALL dry-ink rendering and the presenter draws nothing) is
                // NOT available in the desktop-hosted InkCanvas, and this is a hard architectural
                // limit — proven, not assumed:
                //   * The desktop presenter (IInkPresenterDesktop) receives pointer input ONLY
                //     through the HWND association established by SetRootVisual.
                //   * A CoreInkIndependentInputSource created WITHOUT SetRootVisual receives zero
                //     input even when created after load with the HWND present (verified: 0
                //     PointerPressing events), because it rides on that same SetRootVisual
                //     association.
                //   * With SetRootVisual present, BeginDry throws E_UNEXPECTED (the presenter is
                //     driving its own drying), so custom drying and input are mutually exclusive.
                //
                // The supported equivalent: the presenter renders the ink, and the app receives
                // every stroke as it is dried through InkPresenter.StrokesCollected, so it can
                // store, analyze, or re-render the dry strokes on its own surface.
                _customDryInkCanvas = new InkCanvas();
                _customDryInkCanvas.InkPresenter.StrokesCollected += (s, ev) =>
                {
                    _customDryHarvestCount += ev.Strokes.Count;
                    customDryStatus.Text =
                        $"App harvested {_customDryHarvestCount} dry stroke(s) (latest batch: {ev.Strokes.Count}). " +
                        "Each stroke is delivered to the app as the presenter dries it.";
                };

                customDryingHost.Child = _customDryInkCanvas;

                customDryStatus.Text =
                    "Ready. Draw on the canvas below — the presenter renders the ink and the app is " +
                    "handed each stroke as it dries via InkPresenter.StrokesCollected.";
            }
            catch (Exception ex)
            {
                _customDryInkCanvas = null;
                customDryStatus.Text = $"Failed: 0x{ex.HResult:X8} {ex.Message}";
            }
        }

        private Windows.UI.Input.Inking.Core.CoreInkIndependentInputSource _independentInput;
        private int _pressCount;
        private int _moveCount;
        private int _releaseCount;

        private async void OnCreateInputSourceClick(object sender, RoutedEventArgs e)
        {
            if (_independentInput != null)
            {
                inputSourceStatus.Text = "Independent input source already created. Draw on the canvas below.";
                return;
            }

            try
            {
                inputSourceStatus.Text = "Creating CoreInkIndependentInputSource on the ink thread...";
                _independentInput = await inkCanvas5.CreateCoreIndependentInputSourceAsync();

                // Raw pointer events are captured on the ink thread and marshaled to the UI
                // thread via the InkPointer* events.
                inkCanvas5.InkPointerPressing += OnInkPointerPressing;
                inkCanvas5.InkPointerMoving += OnInkPointerMoving;
                inkCanvas5.InkPointerReleasing += OnInkPointerReleasing;

                inputSourceStatus.Text = _independentInput != null
                    ? "Independent input source created. Draw on the canvas below — counters update live."
                    : "CreateCoreIndependentInputSourceAsync returned null.";
            }
            catch (Exception ex)
            {
                inputSourceStatus.Text = $"CreateCoreIndependentInputSourceAsync failed: 0x{ex.HResult:X8} {ex.Message}";
            }
        }

        private void OnInkPointerPressing(InkCanvas sender, InkCanvasPointerEventArgs args)
        {
            _pressCount++;
            UpdateInputStats(args, "Pressing");
        }

        private void OnInkPointerMoving(InkCanvas sender, InkCanvasPointerEventArgs args)
        {
            _moveCount++;
            UpdateInputStats(args, "Moving");
        }

        private void OnInkPointerReleasing(InkCanvas sender, InkCanvasPointerEventArgs args)
        {
            _releaseCount++;
            UpdateInputStats(args, "Releasing");
        }

        private void UpdateInputStats(InkCanvasPointerEventArgs args, string phase)
        {
            inputEventStats.Text = $"Press: {_pressCount}  Move: {_moveCount}  Release: {_releaseCount}  (ink-thread events)";
            inputEventLast.Text =
                $"{phase}: id={args.PointerId} pos=({args.Position.X:F0},{args.Position.Y:F0}) " +
                $"pressure={args.Pressure:F2} inContact={args.IsInContact}";
        }
    }
}
