// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation.Metadata;

namespace WinUIGallery.SamplePages;

public sealed partial class SamplePage1 : Page
{
    public SamplePage1()
    {
        this.InitializeComponent();
    }

    public void PrepareConnectedAnimation(ConnectedAnimationConfiguration config)
    {
        var anim = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("ForwardConnectedAnimation", SourceElement);

        if (config != null && ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
        {
            anim.Configuration = config;
        }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        var anim = ConnectedAnimationService.GetForCurrentView().GetAnimation("BackwardConnectedAnimation");
        if (anim != null)
        {
            anim.TryStart(SourceElement);
        }
    }
}
