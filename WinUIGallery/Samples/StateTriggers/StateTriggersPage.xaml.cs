// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;

namespace WinUIGallery.ControlPages;

public sealed partial class StateTriggersPage : Page
{
    private readonly PointerEventHandler _pointerHandler;
    private bool _isListening;

    public StateTriggersPage()
    {
        _pointerHandler = OnPointerPressed;
        InitializeComponent();
    }

    private void OnPageLoaded(object sender, RoutedEventArgs args)
    {
        UpdateEmphasisState();
    }

    private void OnEmphasisToggled(object sender, RoutedEventArgs args)
    {
        if (IsLoaded)
        {
            UpdateEmphasisState();
        }
    }

    private void UpdateEmphasisState()
    {
        string state = EmphasisSwitch.IsOn ? "Emphasized" : "Normal";
        if (!VisualStateManager.GoToState(this, state, false))
        {
            throw new InvalidOperationException($"Visual state '{state}' was not found.");
        }
    }

    private void OnInputTargetLoaded(object sender, RoutedEventArgs args)
    {
        if (!_isListening)
        {
            InputTarget.AddHandler(UIElement.PointerPressedEvent, _pointerHandler, true);
            _isListening = true;
        }
    }

    private void OnInputTargetUnloaded(object sender, RoutedEventArgs args)
    {
        if (_isListening)
        {
            InputTarget.RemoveHandler(UIElement.PointerPressedEvent, _pointerHandler);
            _isListening = false;
        }

        LastPointerStatus.Text = "No pointer press yet.";
    }

    private void OnPointerPressed(object sender, PointerRoutedEventArgs args)
    {
        LastPointerStatus.Text = $"Last pointer: {args.Pointer.PointerDeviceType}";
    }

    private void OnChoiceClicked(object sender, RoutedEventArgs args)
    {
        ChoiceStatus.Text = $"Activated: {((Button)sender).Content}";
    }
}