//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Input.Inking;

namespace WinUIGallery.ControlPages
{
    public sealed partial class InkCanvasPage : Page
    {
        private bool _initialized = false;
        private int _example1StrokeCount = 0;

        public InkCanvasPage()
        {
            this.InitializeComponent();
            _initialized = true;
            SetupExample1StrokeEvents();
            SetupStrokeEvents();
        }

        // ====================================================================
        // Example 1: Mode switching and input types
        // ====================================================================

        private void ModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_initialized || inkCanvas1 == null) return;

            try
            {
                var selected = (ComboBoxItem)ModeComboBox.SelectedItem;
                var tag = selected?.Tag?.ToString();
                switch (tag)
                {
                    case "Draw":
                        inkCanvas1.Mode = InkCanvasMode.Draw;
                        break;
                    case "Erase":
                        inkCanvas1.Mode = InkCanvasMode.Erase;
                        break;
                }
                StatusText.Text = $"Mode: {tag}";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Mode error: {ex.Message}";
            }
        }

        private void InputType_Changed(object sender, RoutedEventArgs e)
        {
            if (!_initialized || inkCanvas1 == null) return;

            try
            {
                var types = InkInputType.None;
                if (PenCheck.IsChecked == true) types |= InkInputType.Pen;
                if (MouseCheck.IsChecked == true) types |= InkInputType.Mouse;
                if (TouchCheck.IsChecked == true) types |= InkInputType.Touch;

                // Don't allow None - at least one must be selected
                if (types == InkInputType.None)
                {
                    types = InkInputType.Pen;
                    PenCheck.IsChecked = true;
                }

                inkCanvas1.AllowedInputTypes = types;
                StatusText.Text = $"Inputs: {types}";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Input error: {ex.Message}";
            }
        }

        private void SetupExample1StrokeEvents()
        {
            inkCanvas1.StrokeCollected += (s, e) =>
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    _example1StrokeCount++;
                    StrokeCountText.Text = $"Strokes: {_example1StrokeCount}";
                });
            };
            inkCanvas1.StrokesErased += (s, e) =>
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    _example1StrokeCount -= e.Strokes?.Count ?? 0;
                    if (_example1StrokeCount < 0) _example1StrokeCount = 0;
                    StrokeCountText.Text = $"Strokes: {_example1StrokeCount}";
                });
            };
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                inkCanvas1.ClearStrokes();
                _example1StrokeCount = 0;
                StrokeCountText.Text = "Strokes: 0";
                StatusText.Text = "Cleared.";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Clear error: {ex.Message}";
            }
        }

        // ====================================================================
        // Example 2: Pen color and size
        // ====================================================================

        private void ColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_initialized || inkCanvas2 == null) return;

            try
            {
                var selected = (ComboBoxItem)ColorComboBox.SelectedItem;
                var tag = selected?.Tag?.ToString();

                var attrs = new InkDrawingAttributes();
                double size = SizeSlider?.Value ?? 2;
                attrs.Size = new Size(size, size);

                switch (tag)
                {
                    case "Red":
                        attrs.Color = Colors.Red;
                        break;
                    case "Blue":
                        attrs.Color = Colors.Blue;
                        break;
                    case "Green":
                        attrs.Color = Colors.Green;
                        break;
                    default:
                        attrs.Color = Colors.Black;
                        break;
                }

                inkCanvas2.DefaultDrawingAttributes = attrs;
                StatusText2.Text = $"Color: {tag}, Size: {size}";
            }
            catch (Exception ex)
            {
                StatusText2.Text = $"Color error: {ex.Message}";
            }
        }

        private void SizeSlider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if (!_initialized || inkCanvas2 == null || SizeSlider == null) return;

            try
            {
                double size = SizeSlider.Value;
                SizeText.Text = $"Size: {size:F0}";

                var attrs = new InkDrawingAttributes();
                attrs.Size = new Size(size, size);

                // Preserve current color
                var selected = (ComboBoxItem)ColorComboBox?.SelectedItem;
                var tag = selected?.Tag?.ToString();
                switch (tag)
                {
                    case "Red": attrs.Color = Colors.Red; break;
                    case "Blue": attrs.Color = Colors.Blue; break;
                    case "Green": attrs.Color = Colors.Green; break;
                    default: attrs.Color = Colors.Black; break;
                }

                inkCanvas2.DefaultDrawingAttributes = attrs;
            }
            catch (Exception ex)
            {
                StatusText2.Text = $"Size error: {ex.Message}";
            }
        }

        private void ClearButton2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                inkCanvas2.ClearStrokes();
                StatusText2.Text = "Cleared.";
            }
            catch (Exception ex)
            {
                StatusText2.Text = $"Clear error: {ex.Message}";
            }
        }

        // ====================================================================
        // Example 3: Save and Load
        // ====================================================================

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var picker = new FileSavePicker();
                // Get the window handle for the picker
                var window = App.StartupWindow;
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
                WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

                picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                picker.SuggestedFileName = "InkDrawing";
                picker.FileTypeChoices.Add("Ink Serialized Format", new string[] { ".isf" });

                var file = await picker.PickSaveFileAsync();
                if (file != null)
                {
                    SaveLoadStatus.Text = "Saving...";
                    using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        await inkCanvas3.SaveAsync(stream.GetOutputStreamAt(0));
                    }
                    SaveLoadStatus.Text = $"Saved to {file.Name}";
                }
                else
                {
                    SaveLoadStatus.Text = "Save cancelled.";
                }
            }
            catch (Exception ex)
            {
                SaveLoadStatus.Text = $"Save error: {ex.Message}";
            }
        }

        private async void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var picker = new FileOpenPicker();
                var window = App.StartupWindow;
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
                WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

                picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                picker.FileTypeFilter.Add(".isf");

                var file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    SaveLoadStatus.Text = "Loading...";
                    using (var stream = await file.OpenAsync(FileAccessMode.Read))
                    {
                        await inkCanvas3.LoadAsync(stream.GetInputStreamAt(0));
                    }
                    SaveLoadStatus.Text = $"Loaded from {file.Name}";
                }
                else
                {
                    SaveLoadStatus.Text = "Load cancelled.";
                }
            }
            catch (Exception ex)
            {
                SaveLoadStatus.Text = $"Load error: {ex.Message}";
            }
        }

        private void ClearButton3_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                inkCanvas3.ClearStrokes();
                SaveLoadStatus.Text = "Cleared.";
            }
            catch (Exception ex)
            {
                SaveLoadStatus.Text = $"Clear error: {ex.Message}";
            }
        }

        // ====================================================================
        // Example 4: IsEnabled toggle
        // ====================================================================

        private void EnableToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (!_initialized || inkCanvas4 == null) return;

            try
            {
                inkCanvas4.IsEnabled = EnableToggle.IsOn;
                StatusText4.Text = EnableToggle.IsOn ? "Inking enabled" : "Inking disabled";
            }
            catch (Exception ex)
            {
                StatusText4.Text = $"Toggle error: {ex.Message}";
            }
        }

        private void ClearButton4_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                inkCanvas4.ClearStrokes();
                StatusText4.Text = "Cleared.";
            }
            catch (Exception ex)
            {
                StatusText4.Text = $"Clear error: {ex.Message}";
            }
        }

        // ====================================================================
        // Example 5: Stroke Events
        // ====================================================================

        private int _eventLogLineCount = 0;

        private void SetupStrokeEvents()
        {
            try
            {
                inkCanvas5.StrokeCollected += InkCanvas5_StrokeCollected;
                inkCanvas5.StrokesErased += InkCanvas5_StrokesErased;
            }
            catch (Exception ex)
            {
                EventLogText.Text = $"Event setup error: {ex.Message}";
            }
        }

        private void InkCanvas5_StrokeCollected(InkCanvas sender, InkCanvasStrokeCollectedEventArgs args)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                _eventLogLineCount++;
                var msg = $"[{_eventLogLineCount}] StrokeCollected\n";
                EventLogText.Text = msg + EventLogText.Text;
            });
        }

        private void InkCanvas5_StrokesErased(InkCanvas sender, InkCanvasStrokesErasedEventArgs args)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                _eventLogLineCount++;
                var count = args.Strokes?.Count ?? 0;
                var msg = $"[{_eventLogLineCount}] StrokesErased (count: {count})\n";
                EventLogText.Text = msg + EventLogText.Text;
            });
        }

        private void ClearButton5_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                inkCanvas5.ClearStrokes();
            }
            catch (Exception ex)
            {
                EventLogText.Text = $"Clear error: {ex.Message}";
            }
        }

        private void ClearLog_Click(object sender, RoutedEventArgs e)
        {
            _eventLogLineCount = 0;
            EventLogText.Text = "Draw strokes to see events...";
        }

        // ====================================================================
        // Example 6: Pen Tip Shape and Highlighter
        // ====================================================================

        private void TipComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_initialized || inkCanvas6 == null) return;
            ApplyTipAttributes();
        }

        private void HighlighterCheck_Changed(object sender, RoutedEventArgs e)
        {
            if (!_initialized || inkCanvas6 == null) return;
            ApplyTipAttributes();
        }

        private void TipSize_Changed(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if (!_initialized || inkCanvas6 == null) return;
            ApplyTipAttributes();
        }

        private void ApplyTipAttributes()
        {
            try
            {
                var attrs = new InkDrawingAttributes();

                var selected = (ComboBoxItem)TipComboBox?.SelectedItem;
                var tag = selected?.Tag?.ToString();
                attrs.PenTip = (tag == "Rectangle") ? PenTipShape.Rectangle : PenTipShape.Circle;

                double w = TipWidthSlider?.Value ?? 4;
                double h = TipHeightSlider?.Value ?? 4;
                attrs.Size = new Size(w, h);

                attrs.DrawAsHighlighter = HighlighterCheck?.IsChecked == true;
                if (attrs.DrawAsHighlighter)
                {
                    attrs.Color = Color.FromArgb(255, 255, 255, 0); // yellow
                }

                inkCanvas6.DefaultDrawingAttributes = attrs;
                StatusText6.Text = $"Tip: {tag}, Size: {w:F0}x{h:F0}, Highlighter: {attrs.DrawAsHighlighter}";
            }
            catch (Exception ex)
            {
                StatusText6.Text = $"Tip error: {ex.Message}";
            }
        }

        private void ClearButton6_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                inkCanvas6.ClearStrokes();
                StatusText6.Text = "Cleared.";
            }
            catch (Exception ex)
            {
                StatusText6.Text = $"Clear error: {ex.Message}";
            }
        }

        // ====================================================================
        // Example 7: QueueInkPresenterWorkItem
        // ====================================================================

        private async void GetStrokeCount_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int count = 0;
                await inkCanvas7.QueueInkPresenterWorkItem((presenter) =>
                {
                    var strokes = presenter.StrokeContainer.GetStrokes();
                    count = strokes.Count;
                });
                StatusText7.Text = $"Stroke count (from ink thread): {count}";
            }
            catch (Exception ex)
            {
                StatusText7.Text = $"Error: {ex.Message}";
            }
        }

        private async void SetPencilMode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await inkCanvas7.QueueInkPresenterWorkItem((presenter) =>
                {
                    var pencilAttrs = InkDrawingAttributes.CreateForPencil();
                    pencilAttrs.PencilProperties.Opacity = 0.6;
                    presenter.UpdateDefaultDrawingAttributes(pencilAttrs);
                });
                StatusText7.Text = "Switched to Pencil mode on ink thread.";
            }
            catch (Exception ex)
            {
                StatusText7.Text = $"Error: {ex.Message}";
            }
        }

        private void ClearButton7_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                inkCanvas7.ClearStrokes();
                StatusText7.Text = "Cleared.";
            }
            catch (Exception ex)
            {
                StatusText7.Text = $"Clear error: {ex.Message}";
            }
        }

        // ====================================================================
        // Example 8: Custom Drying (Phase 2)
        // ====================================================================

        private InkCanvas _customDryInkCanvas;
        private InkSynchronizer _inkSynchronizer;
        private DispatcherTimer _customDryTimer;

        private async void ActivateCustomDrying_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_inkSynchronizer != null)
                {
                    CustomDryStatus.Text = "Already active. Draw above — watch harvest counter update live.";
                    return;
                }

                CustomDryStatus.Text = "Creating InkCanvas and activating custom drying...";
                _customDryInkCanvas = new InkCanvas();

                InkSynchronizer syncResult = null;
                try
                {
                    syncResult = await _customDryInkCanvas.ActivateCustomDryingAsync();
                }
                catch (Exception activateEx)
                {
                    CustomDryStatus.Text = $"ActivateCustomDryingAsync FAILED: 0x{activateEx.HResult:X8}\n{activateEx.Message}";
                    CustomDryingHost.Children.Clear();
                    CustomDryingHost.Children.Add(_customDryInkCanvas);
                    return;
                }

                _inkSynchronizer = syncResult;
                CustomDryingHost.Children.Clear();
                CustomDryingHost.Children.Add(_customDryInkCanvas);

                bool isActive = false;
                try { isActive = _customDryInkCanvas.IsCustomDryingActive; } catch { }

                // Start polling the log file for harvest count
                _customDryTimer = new DispatcherTimer();
                _customDryTimer.Interval = TimeSpan.FromMilliseconds(500);
                _customDryTimer.Tick += (s, te) => UpdateCustomDryCount();
                _customDryTimer.Start();

                CustomDryStatus.Text = $"SUCCESS! IsCustomDryingActive={isActive}\n" +
                    $"InkSynchronizer={_inkSynchronizer}\n\n" +
                    "Draw above — each pen lift fires StrokesCollected on the ink thread.\n" +
                    "The harvest counter updates live below.";
            }
            catch (Exception ex)
            {
                CustomDryStatus.Text = $"Error: 0x{ex.HResult:X8} {ex.Message}";
            }
        }

        private void UpdateCustomDryCount()
        {
            try
            {
                int harvestCount = 0;
                int totalStrokes = 0;
                var logPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "InkCanvasDebug.log");
                if (System.IO.File.Exists(logPath))
                {
                    using var fs = new System.IO.FileStream(logPath, System.IO.FileMode.Open,
                        System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
                    using var reader = new System.IO.StreamReader(fs);
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        // Match "CustomDry: harvested N strokes, total=T"
                        if (line.Contains("CustomDry: harvested"))
                        {
                            harvestCount++;
                            var totalIdx = line.IndexOf("total=");
                            if (totalIdx >= 0)
                            {
                                var numStr = line.Substring(totalIdx + 6).Trim();
                                int.TryParse(numStr, out totalStrokes);
                            }
                        }
                    }
                }
                CustomDryStrokeCount.Text = $"StrokesCollected fired: {harvestCount} times, total strokes: {totalStrokes}";
            }
            catch { }
        }

        private void BeginDry_Click(object sender, RoutedEventArgs e)
        {
            UpdateCustomDryCount();
            if (_customDryInkCanvas == null || !_customDryInkCanvas.IsCustomDryingActive)
            {
                CustomDryStatus.Text = "Activate custom drying first.";
                return;
            }
            CustomDryStatus.Text = $"IsCustomDryingActive: {_customDryInkCanvas.IsCustomDryingActive}\n" +
                $"InkSynchronizer: {_inkSynchronizer}\n\n" +
                "Custom drying is active. StrokesCollected fires on ink thread\n" +
                "each time pen lifts. Stroke data (args.Strokes()) is captured.\n" +
                "A D2D rendering pipeline would use this data for custom rendering.";
        }

        // ====================================================================
        // Example 9: CoreInkIndependentInputSource (Phase 2)
        // ====================================================================

        private Windows.UI.Input.Inking.Core.CoreInkIndependentInputSource _independentInput;
        private DispatcherTimer _inputSourceTimer;

        private async void CreateInputSource_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_independentInput != null)
                {
                    InputSourceStatus.Text = "Already created. Draw on inkCanvas9 above — counters update live.";
                    return;
                }

                InputSourceStatus.Text = "Creating CoreInkIndependentInputSource on ink thread...";

                Windows.UI.Input.Inking.Core.CoreInkIndependentInputSource result = null;
                try
                {
                    result = await inkCanvas9.CreateCoreIndependentInputSourceAsync();
                }
                catch (Exception createEx)
                {
                    InputSourceStatus.Text = $"FAILED: 0x{createEx.HResult:X8}\n{createEx.Message}";
                    return;
                }

                if (result == null)
                {
                    InputSourceStatus.Text = "FAILED: CreateCoreIndependentInputSourceAsync returned null.";
                    return;
                }

                _independentInput = result;

                // Start polling the log file for independent input event counts
                _inputSourceTimer = new DispatcherTimer();
                _inputSourceTimer.Interval = TimeSpan.FromMilliseconds(300);
                _inputSourceTimer.Tick += (s, te) => UpdateIndependentInputStats();
                _inputSourceTimer.Start();

                InputSourceStatus.Text = $"SUCCESS!\n" +
                    $"CoreInkIndependentInputSource created on ink thread.\n\n" +
                    "Draw above — Press/Move/Release counters update live.\n" +
                    "Events fire on the ink background thread (independent of UI).";
                InputEventLog.Text = "Pointer tracking active on ink thread.";
            }
            catch (Exception ex)
            {
                InputSourceStatus.Text = $"Error: 0x{ex.HResult:X8} {ex.Message}";
            }
        }

        private void UpdateIndependentInputStats()
        {
            try
            {
                int press = 0, move = 0, release = 0;
                var logPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "InkCanvasDebug.log");
                if (System.IO.File.Exists(logPath))
                {
                    using var fs = new System.IO.FileStream(logPath, System.IO.FileMode.Open,
                        System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
                    using var reader = new System.IO.StreamReader(fs);
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Contains("IndependentInput: PointerPressing"))
                            press++;
                        else if (line.Contains("IndependentInput: PointerMoving"))
                            move++;
                        else if (line.Contains("IndependentInput: PointerReleasing"))
                            release++;
                    }
                }
                InputEventStats.Text = $"Press: {press}  Move: {move}+  Release: {release}  (ink thread events)";
            }
            catch { }
        }

        private void ClearInputLog_Click(object sender, RoutedEventArgs e)
        {
            InputEventStats.Text = "Press: 0  Move: 0  Release: 0";
            InputEventLog.Text = "";
        }

        // ====================================================================
        // Example 10: InkToolBar (Phase 3)
        // ====================================================================

        private string m_currentTool10 = "Pen";

        private void GetActiveTool_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var activeTool = inkToolBar10.ActiveTool;
                string toolInfo = activeTool != null
                    ? $"InkToolBar.ActiveTool: {activeTool.ToolKind}"
                    : "InkToolBar.ActiveTool: (null — toolbar not yet initialized)";
                ToolBarStatus.Text = $"{toolInfo}\nRadio selection: {m_currentTool10}\ninkCanvas10.Mode: {inkCanvas10.Mode}";
            }
            catch (Exception ex)
            {
                ToolBarStatus.Text = $"Error reading ActiveTool: 0x{ex.HResult:X8}\n{ex.Message}";
            }
        }

        private void ToolRadioButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is RadioButton rb)
                {
                    var name = rb.Name;

                    if (name.Contains("Eraser"))
                    {
                        m_currentTool10 = "Eraser";
                        inkCanvas10.Mode = InkCanvasMode.Erase;
                    }
                    else
                    {
                        inkCanvas10.Mode = InkCanvasMode.Draw;

                        var attrs = new InkDrawingAttributes();
                        if (name.Contains("Pencil"))
                        {
                            m_currentTool10 = "Pencil";
                            attrs.Color = Colors.Gray;
                            attrs.Size = new Size(1.5, 1.5);
                        }
                        else if (name.Contains("Highlighter"))
                        {
                            m_currentTool10 = "Highlighter";
                            attrs.Color = Colors.Yellow;
                            attrs.Size = new Size(8, 8);
                            attrs.DrawAsHighlighter = true;
                        }
                        else
                        {
                            m_currentTool10 = "Pen";
                            attrs.Color = Colors.Black;
                            attrs.Size = new Size(2, 2);
                        }

                        inkCanvas10.DefaultDrawingAttributes = attrs;
                    }

                    ToolBarStatus.Text = $"Switched to: {m_currentTool10}";
                }
            }
            catch (Exception ex)
            {
                ToolBarStatus.Text = $"Error: 0x{ex.HResult:X8} {ex.Message}";
            }
        }
    }
}
