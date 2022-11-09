//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;

namespace AppUIBasics.ControlPages
{
    public sealed partial class ScrollViewerPage : Page
    {
        public ScrollViewerPage()
        {
            this.InitializeComponent();
            ScrollViewerControl.ZoomToFactor((float)2.0);
        }

        private void ZoomModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ScrollViewerControl != null && ZoomSlider != null)
            {
                if (sender is ComboBox cb)
                {
                    switch (cb.SelectedIndex)
                    {
                        case 0: // Enabled
                            ScrollViewerControl.ZoomMode = ZoomMode.Enabled;
                            ZoomSlider.IsEnabled = true;
                            break;
                        case 1: // Disabled
                            ScrollViewerControl.ZoomMode = ZoomMode.Disabled;
                            ScrollViewerControl.ChangeView(null, null, (float)1.0);
                            ZoomSlider.Value = 1;
                            ZoomSlider.IsEnabled = false;
                            break;
                        default: // Enabled
                            ScrollViewerControl.ZoomMode = ZoomMode.Enabled;
                            ZoomSlider.IsEnabled = true;
                            break;
                    }
                }
            }
        }

        private void ZoomSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (ScrollViewerControl != null)
            {
                ScrollViewerControl.ChangeView(null, null, (float)e.NewValue);
            }
        }

        private void hsmCombo_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (ScrollViewerControl != null)
            {
                if (sender is ComboBox cb)
                {
                    switch (cb.SelectedIndex)
                    {
                        case 0: // Auto
                            ScrollViewerControl.HorizontalScrollMode = ScrollMode.Auto;
                            break;
                        case 1: //Enabled
                            ScrollViewerControl.HorizontalScrollMode = ScrollMode.Enabled;
                            break;
                        case 2: // Disabled
                            ScrollViewerControl.HorizontalScrollMode = ScrollMode.Disabled;
                            break;
                        default:
                            ScrollViewerControl.HorizontalScrollMode = ScrollMode.Enabled;
                            break;
                    }
                }
            }
        }

        private void hsbvCombo_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (ScrollViewerControl != null)
            {
                if (sender is ComboBox cb)
                {
                    switch (cb.SelectedIndex)
                    {
                        case 0: // Auto
                            ScrollViewerControl.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                            break;
                        case 1: //Visible
                            ScrollViewerControl.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                            break;
                        case 2: // Hidden
                            ScrollViewerControl.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                            break;
                        case 3: // Disabled
                            ScrollViewerControl.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                            break;
                        default:
                            ScrollViewerControl.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                            break;
                    }
                }
            }
        }

        private void vsmCombo_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (ScrollViewerControl != null)
            {
                if (sender is ComboBox cb)
                {
                    switch (cb.SelectedIndex)
                    {
                        case 0: // Auto
                            ScrollViewerControl.VerticalScrollMode = ScrollMode.Auto;
                            break;
                        case 1: //Enabled
                            ScrollViewerControl.VerticalScrollMode = ScrollMode.Enabled;
                            break;
                        case 2: // Disabled
                            ScrollViewerControl.VerticalScrollMode = ScrollMode.Disabled;
                            break;
                        default:
                            ScrollViewerControl.VerticalScrollMode = ScrollMode.Enabled;
                            break;
                    }
                }
            }
        }

        private void vsbvCombo_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (ScrollViewerControl != null)
            {
                if (sender is ComboBox cb)
                {
                    switch (cb.SelectedIndex)
                    {
                        case 0: // Auto
                            ScrollViewerControl.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                            break;
                        case 1: //Visible
                            ScrollViewerControl.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                            break;
                        case 2: // Hidden
                            ScrollViewerControl.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                            break;
                        case 3: // Disabled
                            ScrollViewerControl.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                            break;
                        default:
                            ScrollViewerControl.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                            break;
                    }
                }
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Grid.GetColumnSpan(ScrollViewerControl) == 1)
            {
                ScrollViewerControl.Width = (e.NewSize.Width / 2) - 50;
            }
            else
            {
                ScrollViewerControl.Width = e.NewSize.Width - 50;
            }

        }

        private void ScrollViewerControl_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            ZoomSlider.Value = ScrollViewerControl.ZoomFactor;
        }

        private void ScrollViewerControl_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            ZoomSlider.Value = ScrollViewerControl.ZoomFactor;
        }
    }
}
