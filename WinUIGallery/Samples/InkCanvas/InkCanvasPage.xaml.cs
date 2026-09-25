// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Navigation;
using System;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Input.Inking;

namespace WinUIGallery.ControlPages;

public sealed partial class InkCanvasPage : Page
{
    private bool _updatingToolbarOptions;
    private InMemoryRandomAccessStream? _savedInk;
    private int _savedStrokeCount;

    public InkCanvasPage()
    {
        InitializeComponent();

        penInput.Checked += OnInputDevicesChanged;
        penInput.Unchecked += OnInputDevicesChanged;
        mouseInput.Checked += OnInputDevicesChanged;
        mouseInput.Unchecked += OnInputDevicesChanged;
        touchInput.Checked += OnInputDevicesChanged;
        touchInput.Unchecked += OnInputDevicesChanged;
        inkColor.SelectionChanged += OnInkColorChanged;
        strokeWidth.ValueChanged += OnStrokeWidthChanged;
        inkingEnabled.Toggled += OnInkingEnabledChanged;

        showPencil.Checked += OnToolbarToolsChanged;
        showPencil.Unchecked += OnToolbarToolsChanged;
        showHighlighter.Checked += OnToolbarToolsChanged;
        showHighlighter.Unchecked += OnToolbarToolsChanged;
        showEraser.Checked += OnToolbarToolsChanged;
        showEraser.Unchecked += OnToolbarToolsChanged;
        activeTool.SelectionChanged += OnActiveToolSelectionChanged;
        buttonFlyoutPlacement.SelectionChanged += OnButtonFlyoutPlacementChanged;
        sampleToolbar.ActiveToolChanged += OnToolbarActiveToolChanged;
        sampleToolbar.Loaded += OnToolbarLoaded;

        strokesCanvas.InkPresenter.StrokesCollected += (sender, args) => UpdateSavedStrokeCount();
        strokesCanvas.InkPresenter.StrokesErased += (sender, args) => UpdateSavedStrokeCount();

        ApplyInputDevices();
        ApplyDrawingAttributes();
        drawingCanvas.InkPresenter.IsInputEnabled = inkingEnabled.IsOn;
        UpdateSavedStrokeCount();
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        _savedInk?.Dispose();
        _savedInk = null;
        base.OnNavigatedFrom(e);
    }

    private void OnInputDevicesChanged(object sender, RoutedEventArgs e)
    {
        ApplyInputDevices();
    }

    private void ApplyInputDevices()
    {
        CoreInputDeviceTypes types = default;
        if (penInput.IsChecked == true)
        {
            types |= CoreInputDeviceTypes.Pen;
        }
        if (mouseInput.IsChecked == true)
        {
            types |= CoreInputDeviceTypes.Mouse;
        }
        if (touchInput.IsChecked == true)
        {
            types |= CoreInputDeviceTypes.Touch;
        }

        drawingCanvas.InkPresenter.InputDeviceTypes = types;
    }

    private void OnInkColorChanged(object sender, SelectionChangedEventArgs e)
    {
        ApplyDrawingAttributes();
    }

    private void OnStrokeWidthChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        ApplyDrawingAttributes();
    }

    private void ApplyDrawingAttributes()
    {
        Windows.UI.Color color = inkColor.SelectedIndex switch
        {
            0 => Colors.Black,
            1 => Colors.Red,
            2 => Colors.Blue,
            _ => throw new InvalidOperationException("Unexpected ink color selection."),
        };

        InkDrawingAttributes attributes = drawingCanvas.InkPresenter.CopyDefaultDrawingAttributes();
        attributes.Color = color;
        attributes.Size = new Size(strokeWidth.Value, strokeWidth.Value);
        drawingCanvas.InkPresenter.UpdateDefaultDrawingAttributes(attributes);
    }

    private void OnInkingEnabledChanged(object sender, RoutedEventArgs e)
    {
        drawingCanvas.InkPresenter.IsInputEnabled = inkingEnabled.IsOn;
    }

    private void OnClearDrawingClick(object sender, RoutedEventArgs e)
    {
        drawingCanvas.InkPresenter.StrokeContainer.Clear();
    }

    private void OnToolbarLoaded(object sender, RoutedEventArgs e)
    {
        if (sampleToolbar.ActiveTool is null)
        {
            sampleToolbar.ActiveTool = ballpointButton;
        }

        SyncActiveToolSelection();
    }

    private void OnToolbarActiveToolChanged(InkToolbar sender, object args)
    {
        if (!_updatingToolbarOptions)
        {
            SyncActiveToolSelection();
        }
    }

    private void OnToolbarToolsChanged(object sender, RoutedEventArgs e)
    {
        if (_updatingToolbarOptions)
        {
            return;
        }

        InkToolbarTool previouslyActive = sampleToolbar.ActiveTool?.ToolKind ?? InkToolbarTool.BallpointPen;
        _updatingToolbarOptions = true;
        try
        {
            UpdateToolbarButtons(previouslyActive);
        }
        finally
        {
            _updatingToolbarOptions = false;
        }

        SyncActiveToolSelection();
    }

    private void OnActiveToolSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_updatingToolbarOptions || activeTool.SelectedIndex < 0)
        {
            return;
        }

        InkToolbarTool requestedTool = activeTool.SelectedIndex switch
        {
            0 => InkToolbarTool.BallpointPen,
            1 => InkToolbarTool.Pencil,
            2 => InkToolbarTool.Highlighter,
            3 => InkToolbarTool.Eraser,
            _ => throw new InvalidOperationException("Unexpected toolbar tool selection."),
        };

        _updatingToolbarOptions = true;
        try
        {
            if (requestedTool == InkToolbarTool.Pencil)
            {
                showPencil.IsChecked = true;
            }
            else if (requestedTool == InkToolbarTool.Highlighter)
            {
                showHighlighter.IsChecked = true;
            }
            else if (requestedTool == InkToolbarTool.Eraser)
            {
                showEraser.IsChecked = true;
            }

            UpdateToolbarButtons(requestedTool);
        }
        finally
        {
            _updatingToolbarOptions = false;
        }

        SyncActiveToolSelection();
    }

    private void OnButtonFlyoutPlacementChanged(object sender, SelectionChangedEventArgs e)
    {
        sampleToolbar.ButtonFlyoutPlacement = buttonFlyoutPlacement.SelectedIndex switch
        {
            0 => InkToolbarButtonFlyoutPlacement.Auto,
            1 => InkToolbarButtonFlyoutPlacement.Top,
            2 => InkToolbarButtonFlyoutPlacement.Bottom,
            3 => InkToolbarButtonFlyoutPlacement.Left,
            4 => InkToolbarButtonFlyoutPlacement.Right,
            _ => throw new InvalidOperationException("Unexpected button flyout placement selection."),
        };
    }

    private void UpdateToolbarButtons(InkToolbarTool requestedTool)
    {
        InkToolbarToolButton selectedButton = requestedTool switch
        {
            InkToolbarTool.Pencil when showPencil.IsChecked == true => pencilButton,
            InkToolbarTool.Highlighter when showHighlighter.IsChecked == true => highlighterButton,
            InkToolbarTool.Eraser when showEraser.IsChecked == true => eraserButton,
            _ => ballpointButton,
        };

        if (showPencil.IsChecked == true)
        {
            pencilButton.Visibility = Visibility.Visible;
        }
        if (showHighlighter.IsChecked == true)
        {
            highlighterButton.Visibility = Visibility.Visible;
        }
        if (showEraser.IsChecked == true)
        {
            eraserButton.Visibility = Visibility.Visible;
        }

        if (!ReferenceEquals(sampleToolbar.ActiveTool, selectedButton))
        {
            sampleToolbar.ActiveTool = selectedButton;
        }

        if (showPencil.IsChecked != true)
        {
            pencilButton.Visibility = Visibility.Collapsed;
        }
        if (showHighlighter.IsChecked != true)
        {
            highlighterButton.Visibility = Visibility.Collapsed;
        }
        if (showEraser.IsChecked != true)
        {
            eraserButton.Visibility = Visibility.Collapsed;
        }
    }

    private void SyncActiveToolSelection()
    {
        int index = sampleToolbar.ActiveTool?.ToolKind switch
        {
            InkToolbarTool.BallpointPen => 0,
            InkToolbarTool.Pencil => 1,
            InkToolbarTool.Highlighter => 2,
            InkToolbarTool.Eraser => 3,
            _ => -1,
        };

        if (index >= 0 && activeTool.SelectedIndex != index)
        {
            _updatingToolbarOptions = true;
            try
            {
                activeTool.SelectedIndex = index;
            }
            finally
            {
                _updatingToolbarOptions = false;
            }
        }

    }

    private void OnClearToolbarClick(object sender, RoutedEventArgs e)
    {
        toolbarCanvas.InkPresenter.StrokeContainer.Clear();
    }

    private void UpdateSavedStrokeCount()
    {
        int count = strokesCanvas.InkPresenter.StrokeContainer.GetStrokes().Count;
        strokeCount.Text = $"{count} stroke(s) on canvas";
        restoreButton.IsEnabled = _savedInk is not null && count == 0;
    }

    private async void OnSaveDrawingClick(object sender, RoutedEventArgs e)
    {
        Microsoft.UI.Xaml.Controls.InkStrokeContainer container = strokesCanvas.InkPresenter.StrokeContainer;
        int count = container.GetStrokes().Count;
        if (count == 0)
        {
            strokesStatus.Text = "Draw something before saving.";
            return;
        }

        InMemoryRandomAccessStream? stream = new();
        try
        {
            using IOutputStream output = stream.GetOutputStreamAt(0);
            await container.SaveAsync(output);
            _savedInk?.Dispose();
            _savedInk = stream;
            stream = null;
            _savedStrokeCount = count;
            UpdateSavedStrokeCount();
            strokesStatus.Text = $"Saved {count} stroke(s). Clear the canvas to restore them.";
        }
        finally
        {
            stream?.Dispose();
        }
    }

    private void OnClearStrokesClick(object sender, RoutedEventArgs e)
    {
        strokesCanvas.InkPresenter.StrokeContainer.Clear();
        UpdateSavedStrokeCount();
        strokesStatus.Text = _savedInk is null
            ? "Canvas cleared. Draw and save something to restore it."
            : $"Canvas cleared. {_savedStrokeCount} saved stroke(s) are ready to restore.";
    }

    private async void OnRestoreDrawingClick(object sender, RoutedEventArgs e)
    {
        if (_savedInk is not InMemoryRandomAccessStream saved)
        {
            strokesStatus.Text = "Save a drawing before restoring it.";
            return;
        }

        Microsoft.UI.Xaml.Controls.InkStrokeContainer container = strokesCanvas.InkPresenter.StrokeContainer;
        if (container.GetStrokes().Count != 0)
        {
            strokesStatus.Text = "Clear the canvas before restoring saved ink.";
            return;
        }

        using IInputStream input = saved.GetInputStreamAt(0);
        await container.LoadAsync(input);
        UpdateSavedStrokeCount();
        strokesStatus.Text = $"Restored {container.GetStrokes().Count} stroke(s).";
    }
}
