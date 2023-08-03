// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System;
using AppUIBasics.Data;
using AppUIBasics.Helper;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Uri = System.Uri;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUIGallery.DesktopWap.Controls
{
    public sealed partial class PageHeader : UserControl
    {
        public Visibility ThemeButtonVisibility
        {
            get { return (Visibility)GetValue(ThemeButtonVisibilityProperty); }
            set { SetValue(ThemeButtonVisibilityProperty, value); }
        }
        public static readonly DependencyProperty ThemeButtonVisibilityProperty =
            DependencyProperty.Register("ThemeButtonVisibility", typeof(Visibility), typeof(PageHeader), new PropertyMetadata(Visibility.Visible));

        public string PageName { get; set; }
        public Action CopyLinkAction { get; set; }
        public Action ToggleThemeAction { get; set; }

        public ControlInfoDataItem Item
        {
            get { return _item; }
            set { _item = value; }
        }

        private ControlInfoDataItem _item;

        public PageHeader()
        {
            this.InitializeComponent();
            CopyLinkAction = OnCopyLink;
            
        }

        public void SetSourceLinks(string BaseUri, string PageName)
        {
            // Pagetype is not null!
            // So lets generate the github links and set them!
            var pageName = PageName + ".xaml";
            PageCodeGitHubLink.NavigateUri = new Uri(BaseUri + pageName + ".cs");
            PageMarkupGitHubLink.NavigateUri = new Uri(BaseUri + pageName);
        }

        private void OnCopyLinkButtonClick(object sender, RoutedEventArgs e)
        {
            this.CopyLinkAction?.Invoke();

            if (ProtocolActivationClipboardHelper.ShowCopyLinkTeachingTip)
            {
                this.CopyLinkButtonTeachingTip.IsOpen = true;
            }
        }

        public void OnThemeButtonClick(object sender, RoutedEventArgs e)
        {
            ToggleThemeAction?.Invoke();
        }

        private void OnCopyDontShowAgainButtonClick(TeachingTip sender, object args)
        {
            ProtocolActivationClipboardHelper.ShowCopyLinkTeachingTip = false;
            this.CopyLinkButtonTeachingTip.IsOpen = false;
        }

        private void OnCopyLink()
        {
            ProtocolActivationClipboardHelper.Copy(this.Item);
        }
        public async void OnFeedBackButtonClick(object sender, RoutedEventArgs e)
        {
             await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/microsoft/WinUI-Gallery/issues/new/choose"));
        }
    }
}
