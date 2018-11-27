//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace AppUIBasics.ControlPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RichTextBlockPage : Page
    {
        public RichTextBlockPage()
        {
            this.InitializeComponent();
        }

        private void RichTextBlock_IsTextTrimmedChanged(RichTextBlock sender, IsTextTrimmedChangedEventArgs args)
        {
            //this event fires as expected (which is to say always because this is the first column of a multi-column example
            if (sender.IsTextTrimmed)
            {
                //do something
            }
        }

        private void firstOverflowContainer_IsTextTrimmedChanged(RichTextBlockOverflow sender, IsTextTrimmedChangedEventArgs args)
        {
            //this event never fires for me
            if (sender.IsTextTrimmed)
            {
                ColumnGrid.Height += 100;
            }
        }
    }
}
