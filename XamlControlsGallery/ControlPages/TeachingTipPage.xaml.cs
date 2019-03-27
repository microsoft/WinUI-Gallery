//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace AppUIBasics.ControlPages
{
    public sealed partial class TeachingTipPage : Page
    { 
        public TeachingTipPage()
        {
            this.InitializeComponent();
            ToggleThemeTeachingTip1.Target = PageHeader.TeachingTipTarget;
            ToggleThemeTeachingTip3.Target = PageHeader.TeachingTipTarget;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            ToggleThemeTeachingTip1.IsOpen = false;
            ToggleThemeTeachingTip2.IsOpen = false;
            ToggleThemeTeachingTip3.IsOpen = false;

        }

        private void TestButtonClick1(object sender, RoutedEventArgs e)
        {
            ToggleThemeTeachingTip1.IsOpen = true;
        }

        private void TestButtonClick2(object sender, RoutedEventArgs e)
        {
            ToggleThemeTeachingTip2.IsOpen = true;
        }

        private void TestButtonClick3(object sender, RoutedEventArgs e)
        {
            ToggleThemeTeachingTip3.IsOpen = true;
        }

        private void ToggleThemeTeachingTip2_ActionButtonClick(Microsoft.UI.Xaml.Controls.TeachingTip sender, object args)
        {
            NavigationRootPage.Current.PageHeader.ToggleThemeAction?.Invoke();
        }
    }
}