// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;

namespace WinUIGallery.ControlPages;

public sealed partial class InputTypeTrigger : StateTriggerBase
{
    public static readonly DependencyProperty TargetElementProperty =
        DependencyProperty.Register(nameof(TargetElement), typeof(FrameworkElement),
            typeof(InputTypeTrigger), new PropertyMetadata(null, OnTargetElementChanged));

    public static readonly DependencyProperty PointerTypeProperty =
        DependencyProperty.Register(nameof(PointerType), typeof(PointerDeviceType),
            typeof(InputTypeTrigger), new PropertyMetadata(PointerDeviceType.Mouse, OnPointerTypeChanged));

    private readonly PointerEventHandler _pointerHandler;
    private PointerDeviceType? _lastPointerType;
    private bool _isListening;

    public InputTypeTrigger()
    {
        _pointerHandler = OnPointerPressed;
    }

    public FrameworkElement? TargetElement
    {
        get => (FrameworkElement?)GetValue(TargetElementProperty);
        set => SetValue(TargetElementProperty, value);
    }

    public PointerDeviceType PointerType
    {
        get => (PointerDeviceType)GetValue(PointerTypeProperty);
        set => SetValue(PointerTypeProperty, value);
    }

    private static void OnTargetElementChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        InputTypeTrigger trigger = (InputTypeTrigger)sender;
        if (args.OldValue is FrameworkElement oldTarget)
        {
            oldTarget.Loaded -= trigger.OnTargetLoaded;
            oldTarget.Unloaded -= trigger.OnTargetUnloaded;
            trigger.StopListening(oldTarget);
        }

        trigger._lastPointerType = null;
        trigger.SetActive(false);

        if (args.NewValue is FrameworkElement newTarget)
        {
            newTarget.Loaded += trigger.OnTargetLoaded;
            newTarget.Unloaded += trigger.OnTargetUnloaded;
            if (newTarget.IsLoaded)
            {
                trigger.StartListening(newTarget);
            }
        }
    }

    private static void OnPointerTypeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        InputTypeTrigger trigger = (InputTypeTrigger)sender;
        trigger.SetActive(trigger._lastPointerType == trigger.PointerType);
    }

    private void OnTargetLoaded(object sender, RoutedEventArgs args)
    {
        StartListening((FrameworkElement)sender);
    }

    private void OnTargetUnloaded(object sender, RoutedEventArgs args)
    {
        StopListening((FrameworkElement)sender);
        _lastPointerType = null;
        SetActive(false);
    }

    private void StartListening(FrameworkElement target)
    {
        if (!_isListening)
        {
            // Buttons can handle PointerPressed before it bubbles to the target.
            target.AddHandler(UIElement.PointerPressedEvent, _pointerHandler, true);
            _isListening = true;
        }
    }

    private void StopListening(FrameworkElement target)
    {
        if (_isListening)
        {
            target.RemoveHandler(UIElement.PointerPressedEvent, _pointerHandler);
            _isListening = false;
        }
    }

    private void OnPointerPressed(object sender, PointerRoutedEventArgs args)
    {
        _lastPointerType = args.Pointer.PointerDeviceType;
        SetActive(_lastPointerType == PointerType);
    }
}