// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;


namespace WinUIGallery.Controls;

public partial class SamplePage : Page
{
    public SamplePage()
    {
        InitializeComponent();
        Loaded += SamplePage_Loaded;
    }

    private void SamplePage_Loaded(object sender, RoutedEventArgs e)
    {
        if (Content is Panel panel)
        {
            if (panel.ChildrenTransitions == null)
            {
                panel.ChildrenTransitions = new TransitionCollection();
            }

            // Add RepositionThemeTransition only once
            bool hasReposition = false;
            foreach (var t in panel.ChildrenTransitions)
            {
                if (t is RepositionThemeTransition)
                {
                    hasReposition = true;
                    break;
                }
            }

            if (!hasReposition)
            {
                panel.ChildrenTransitions.Add(new RepositionThemeTransition());
            }
        }
    }
}
