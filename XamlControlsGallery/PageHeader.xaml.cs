﻿//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using AppUIBasics.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Microsoft.UI.Xaml.Controls;

namespace AppUIBasics
{
    public sealed partial class PageHeader : UserControl
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(object), typeof(PageHeader), new PropertyMetadata(null));

        public Action ToggleThemeAction { get; set; }

        public static TeachingTip TeachingTip1;
        public static TeachingTip TeachingTip2;
        public static TeachingTip TeachingTip3;

        public object Title
        {
            get { return GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public double BackgroundColorOpacity
        {
            get { return (double)GetValue(BackgroundColorOpacityProperty); }
            set { SetValue(BackgroundColorOpacityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackgroundColorOpacity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackgroundColorOpacityProperty =
            DependencyProperty.Register("BackgroundColorOpacity", typeof(double), typeof(PageHeader), new PropertyMetadata(0.0));


        public double AcrylicOpacity
        {
            get { return (double)GetValue(AcrylicOpacityProperty); }
            set { SetValue(AcrylicOpacityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackgroundColorOpacity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AcrylicOpacityProperty =
            DependencyProperty.Register("AcrylicOpacity", typeof(double), typeof(PageHeader), new PropertyMetadata(0.3));


        public CommandBar TopCommandBar
        {
            get { return topCommandBar; }
        }

        public UIElement TitlePanel
        {
            get { return pageTitle; }
        }

        public PageHeader()
        {
            this.InitializeComponent();
            TeachingTip1 = ToggleThemeTeachingTip1;
            TeachingTip2 = ToggleThemeTeachingTip2;
            TeachingTip3 = ToggleThemeTeachingTip3;
        }


        public void UpdateBackground(bool isFilteredPage)
        {
            VisualStateManager.GoToState(this, isFilteredPage ? "FilteredPage" : "NonFilteredPage", false);
        }

        public void OnThemeButtonClick(object sender, RoutedEventArgs e)
        {
            ToggleThemeAction?.Invoke();
        }

        private void ToggleThemeTeachingTip2_ActionButtonClick(Microsoft.UI.Xaml.Controls.TeachingTip sender, object args)
        {
            NavigationRootPage.Current.PageHeader.ToggleThemeAction?.Invoke();
        }
    }
}
