// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using System.Numerics;

namespace WinUIGallery.ControlPages;

public sealed partial class ThemeShadowPage : Page
{
    public ThemeShadowPage()
    {
        this.InitializeComponent();
    }

    private void TranslationSliderInApp_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        ShadowRect.Translation = new Vector3(0,0, (float)e.NewValue);
    }

    private void ShadowRect_Loaded(object sender, RoutedEventArgs e)
    {
        shadow.Receivers.Add(ShadowCastGrid);
    }
}
