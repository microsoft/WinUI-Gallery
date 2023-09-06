//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;

namespace AppUIBasics.ControlPages
{
    public sealed partial class AnnotatedScrollBarPage : Page
    {
        private const int AzureCount = 32;
        private const int CrimsonCount = 50;
        private const int CyanCount = 8;
        private const int FuchsiaCount = 70;
        private const int GoldCount = 90;
        private const int ItemWidth = 120;
        private const int ItemHeight = 90;

        public ObservableCollection<SolidColorBrush> ColorCollection = new ObservableCollection<SolidColorBrush>();

        public AnnotatedScrollBarPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
            this.Loaded += AnnotatedScrollBarPage_Loaded;

            PopulateColorCollection();
        }

        private void AnnotatedScrollBarPage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            scrollView.ScrollPresenter.VerticalScrollController = annotatedScrollBar.ScrollController;
        }

        private void AnnotatedScrollBar_DetailLabelRequested(object sender, AnnotatedScrollBarDetailLabelRequestedEventArgs e)
        {
            e.Content = GetOffsetLabel(e.ScrollOffset);
        }

        private void ItemsRepeater_SizeChanged(object sender, Microsoft.UI.Xaml.SizeChangedEventArgs e)
        {
            PopulateLabelCollection();
        }

        private void AnnotatedScrollBarMaxHeightSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (annotatedScrollBar != null)
            {
                annotatedScrollBar.MaxHeight = (sender as Slider).Value;
            }
        }

        private void PopulateColorCollection()
        {
            SolidColorBrush solidColorBrush = new SolidColorBrush(Microsoft.UI.Colors.Azure);

            for (int colorInstance = 0; colorInstance < AzureCount; colorInstance++)
            {
                this.ColorCollection.Add(solidColorBrush);
            }

            solidColorBrush = new SolidColorBrush(Microsoft.UI.Colors.Crimson);

            for (int colorInstance = 0; colorInstance < CrimsonCount; colorInstance++)
            {
                this.ColorCollection.Add(solidColorBrush);
            }

            solidColorBrush = new SolidColorBrush(Microsoft.UI.Colors.Cyan);

            for (int colorInstance = 0; colorInstance < CyanCount; colorInstance++)
            {
                this.ColorCollection.Add(solidColorBrush);
            }

            solidColorBrush = new SolidColorBrush(Microsoft.UI.Colors.Fuchsia);

            for (int colorInstance = 0; colorInstance < FuchsiaCount; colorInstance++)
            {
                this.ColorCollection.Add(solidColorBrush);
            }

            solidColorBrush = new SolidColorBrush(Microsoft.UI.Colors.Gold);

            for (int colorInstance = 0; colorInstance < GoldCount; colorInstance++)
            {
                this.ColorCollection.Add(solidColorBrush);
            }
        }

        private void PopulateLabelCollection()
        {
            if (annotatedScrollBar != null)
            {
                annotatedScrollBar.Labels.Clear();

                annotatedScrollBar.Labels.Add(new AnnotatedScrollBarLabel("Azure", GetOffsetOfItem(0)));
                annotatedScrollBar.Labels.Add(new AnnotatedScrollBarLabel("Crimson", GetOffsetOfItem(AzureCount)));
                annotatedScrollBar.Labels.Add(new AnnotatedScrollBarLabel("Cyan", GetOffsetOfItem(AzureCount + CrimsonCount)));
                annotatedScrollBar.Labels.Add(new AnnotatedScrollBarLabel("Fuchsia", GetOffsetOfItem(AzureCount + CrimsonCount + CyanCount)));
                annotatedScrollBar.Labels.Add(new AnnotatedScrollBarLabel("Gold", GetOffsetOfItem(AzureCount + CrimsonCount + CyanCount + FuchsiaCount)));
            }
        }

        private string GetOffsetLabel(double offset)
        {
            if (offset <= GetOffsetOfItem(AzureCount - 1))
            {
                return GetItemColor(AzureCount - 1);
            }
            else if (offset <= GetOffsetOfItem(AzureCount + CrimsonCount - 1))
            {
                return GetItemColor(AzureCount + CrimsonCount - 1);
            }
            else if (offset <= GetOffsetOfItem(AzureCount + CrimsonCount + CyanCount - 1))
            {
                return GetItemColor(AzureCount + CrimsonCount + CyanCount - 1);
            }
            else if (offset <= GetOffsetOfItem(AzureCount + CrimsonCount + CyanCount + FuchsiaCount - 1))
            {
                return GetItemColor(AzureCount + CrimsonCount + CyanCount + FuchsiaCount - 1);
            }
            else
            {
                return GetItemColor(AzureCount + CrimsonCount + CyanCount + FuchsiaCount);
            }
        }

        private int GetOffsetOfItem(int itemIndex)
        {
            return ItemHeight * (itemIndex / GetItemsPerRow());
        }

        private string GetItemColor(int itemIndex)
        {
            if (itemIndex < AzureCount)
            {
                return "Azure";
            }
            else if (itemIndex < AzureCount + CrimsonCount)
            {
                return "Crimson";
            }
            else if (itemIndex < AzureCount + CrimsonCount + CyanCount)
            {
                return "Cyan";
            }
            else if (itemIndex < AzureCount + CrimsonCount + CyanCount + FuchsiaCount)
            {
                return "Fuchsia";
            }
            else
            {
                return "Gold";
            }
        }

        private int GetItemsPerRow()
        {
            return (itemsRepeater == null || itemsRepeater.ActualWidth == 0) ? 1 : (int) (itemsRepeater.ActualWidth / ItemWidth);
        }
    }
}
