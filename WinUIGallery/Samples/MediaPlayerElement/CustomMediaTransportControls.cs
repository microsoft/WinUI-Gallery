// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace WinUIGallery.ControlPages;

/// <summary>
/// MediaTransportControls with a custom template (CustomMediaTransportControls.xaml) that adds a "Like" command.
/// </summary>
public sealed partial class CustomMediaTransportControls : MediaTransportControls
{
    private Button? likeButton;

    public CustomMediaTransportControls()
    {
        DefaultStyleKey = typeof(CustomMediaTransportControls);
    }

    public event EventHandler? Liked;

    protected override void OnApplyTemplate()
    {
        if (likeButton is not null)
        {
            likeButton.Click -= LikeButton_Click;
        }

        base.OnApplyTemplate();

        likeButton = GetTemplateChild("LikeButton") as Button;
        if (likeButton is not null)
        {
            likeButton.Click += LikeButton_Click;
        }
    }

    private void LikeButton_Click(object sender, RoutedEventArgs e)
    {
        Liked?.Invoke(this, EventArgs.Empty);
    }
}