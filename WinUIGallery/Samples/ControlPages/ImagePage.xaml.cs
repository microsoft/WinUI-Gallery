// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;

namespace WinUIGallery.ControlPages;

public sealed partial class ImagePage : Page
{
    public ImagePage()
    {
        this.InitializeComponent();
    }

    private void ImageStretch_Checked(object sender, RoutedEventArgs e)
    {
        if (StretchImage != null)
        {
            var strStretch = (sender as RadioButton).Content.ToString();
            var stretch = (Stretch)Enum.Parse(typeof(Stretch), strStretch);
            StretchImage.Stretch = stretch;
        }
    }
}
