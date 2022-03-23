﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace AppUIBasics.TabViewPages
{
    public sealed partial class MyTabContentControl : UserControl
    {
        public MyTabContentControl()
        {
            this.InitializeComponent();

#if !UNIVERSAL
            DesktopTextBlock.Visibility = Visibility.Visible;
            UwpTextBlock1.Visibility = Visibility.Collapsed;
            UwpTextBlock2.Visibility = Visibility.Collapsed;
#endif
        }
    }
}
