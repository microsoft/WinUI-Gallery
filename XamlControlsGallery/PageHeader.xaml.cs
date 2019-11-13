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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.Composition;
using Windows.UI.Xaml.Hosting;
using Windows.UI;

namespace AppUIBasics
{
    public sealed partial class PageHeader : UserControl
    {
        
        public Action ToggleThemeAction { get; set; }

        public TeachingTip TeachingTip1 => ToggleThemeTeachingTip1;
        public TeachingTip TeachingTip2 => ToggleThemeTeachingTip2;
        public TeachingTip TeachingTip3 => ToggleThemeTeachingTip3;


        public object Title
        {
            get { return GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty = 
            DependencyProperty.Register("Title", typeof(object), typeof(PageHeader), new PropertyMetadata(null));


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
        
        public double ShadowOpacity
        {
            get { return (double)GetValue(ShadowOpacityProperty); }
            set { SetValue(ShadowOpacityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackgroundColorOpacity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShadowOpacityProperty =
            DependencyProperty.Register("ShadowOpacity", typeof(double), typeof(PageHeader), new PropertyMetadata(0.0));

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
            this.InitializeDropShadow(ShadowHost, TitleTextBlock.GetAlphaMask());
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

        /// <summary>
        /// This method will be called when a <see cref="ItemPage"/> gets unloaded. 
        /// Put any code in here that should be done when a <see cref="ItemPage"/> gets unloaded.
        /// </summary>
        /// <param name="sender">The sender (the ItemPage)</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> of the ItemPage that was unloaded.</param>
        public void Event_ItemPage_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void InitializeDropShadow(UIElement shadowHost, CompositionBrush shadowTargetBrush)
        {
            Visual hostVisual = ElementCompositionPreview.GetElementVisual(shadowHost);
            Compositor compositor = hostVisual.Compositor;

            // Create a drop shadow
            var dropShadow = compositor.CreateDropShadow();
            dropShadow.Color = Color.FromArgb(102, 0, 0, 0);
            dropShadow.BlurRadius = 4.0f;
            // Associate the shape of the shadow with the shape of the target element
            dropShadow.Mask = shadowTargetBrush;

            // Create a Visual to hold the shadow
            var shadowVisual = compositor.CreateSpriteVisual();
            shadowVisual.Shadow = dropShadow;

            // Add the shadow as a child of the host in the visual tree
            ElementCompositionPreview.SetElementChildVisual(shadowHost, shadowVisual);

            // Make sure size of shadow host and shadow visual always stay in sync
            var bindSizeAnimation = compositor.CreateExpressionAnimation("hostVisual.Size");
            bindSizeAnimation.SetReferenceParameter("hostVisual", hostVisual);

            shadowVisual.StartAnimation("Size", bindSizeAnimation);
        }
    }
}
