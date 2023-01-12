//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using AppUIBasics.Helper;
using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using System;

namespace AppUIBasics
{
    public sealed partial class PageHeader : UserControl
    {
        public Action CopyLinkAction { get; set; }
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


        public string ApiNamespace
        {
            get { return (string)GetValue(ApiNamespaceProperty); }
            set { SetValue(ApiNamespaceProperty, value); }
        }

        public static readonly DependencyProperty ApiNamespaceProperty =
            DependencyProperty.Register("ApiNamespace", typeof(string), typeof(PageHeader), new PropertyMetadata(null));

        public object Subtitle
        {
            get { return GetValue(SubtitleProperty); }
            set { SetValue(SubtitleProperty, value); }
        }

        public static readonly DependencyProperty SubtitleProperty =
            DependencyProperty.Register("Subtitle", typeof(object), typeof(PageHeader), new PropertyMetadata(null));



        public Thickness HeaderPadding
        {
            get { return (Thickness)GetValue(HeaderPaddingProperty); }
            set { SetValue(HeaderPaddingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackgroundColorOpacity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderPaddingProperty =
            DependencyProperty.Register("HeaderPadding", typeof(Thickness), typeof(PageHeader), new PropertyMetadata((Thickness)App.Current.Resources["PageHeaderDefaultPadding"]));


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
            // this.InitializeDropShadow(ShadowHost, TitleTextBlock.GetAlphaMask());
            this.ResetCopyLinkButton();
        }

        private async void OnOpenFigmaLinkButtonClick(object sender, RoutedEventArgs e)
        {
            //this.CopyLinkAction?.Invoke();

            //if (ProtocolActivationClipboardHelper.ShowCopyLinkTeachingTip)
            //{
            //    this.CopyLinkButtonTeachingTip.IsOpen = true;
            //}

            //this.OnOpenFigmaLinkButton.Label = "Copied to Clipboard";
            //this.OnOpenFigmaLinkButtonIcon.Symbol = Symbol.Accept;
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://www.figma.com/file/i19Jgv1A05ayFUXTuP7U7A/Windows-UI-3-(Community)?node-id=72491%3A280391&t=GTUagQuxXRLGWcsv-1"));

        }

        public void OnThemeButtonClick(object sender, RoutedEventArgs e)
        {
            ToggleThemeAction?.Invoke();
        }

        public void ResetCopyLinkButton()
        {
            //this.CopyLinkButtonTeachingTip.IsOpen = false;
            this.OpenFigmaLinkButton.Label = "Open in Figma";
            this.OpenFigmaLinkButtonIcon.Symbol = Symbol.Link;
        }

        //private void OnCopyDontShowAgainButtonClick(TeachingTip sender, object args)
        //{
        //    ProtocolActivationClipboardHelper.ShowCopyLinkTeachingTip = false;
        //    this.CopyLinkButtonTeachingTip.IsOpen = false;
        //}

        private void ToggleThemeTeachingTip2_ActionButtonClick(Microsoft.UI.Xaml.Controls.TeachingTip sender, object args)
        {
            NavigationRootPage.GetForElement(this).PageHeader.ToggleThemeAction?.Invoke();
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

    }
}
