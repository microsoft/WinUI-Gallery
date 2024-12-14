// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUIGallery.Data;
using WinUIGallery.Helper;
using Uri = System.Uri;

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

        public void SetSamplePageSourceLinks(string BaseUri, string PageName)
        {
            // Pagetype is not null!
            // So lets generate the github links and set them!
            var pageName = PageName + ".xaml";
            PageMarkupGitHubLink.NavigateUri = new Uri(BaseUri + pageName);
            PageCodeGitHubLink.NavigateUri = new Uri(BaseUri + pageName + ".cs");
        }

        public void SetControlSourceLink(string BaseUri, string SourceLink)
        {
            if (!string.IsNullOrEmpty(SourceLink))
            {
                ControlSourcePanel.Visibility = Visibility.Visible;
                ControlSourceLink.NavigateUri = new Uri(BaseUri + SourceLink);
            }
            else
            {
                ControlSourcePanel.Visibility = Visibility.Collapsed;
            }

        }

        private void OnCopyLinkButtonClick(object sender, RoutedEventArgs e)
        {
            CopyLinkAction?.Invoke();

            if (ProtocolActivationClipboardHelper.ShowCopyLinkTeachingTip)
            {
                CopyLinkButtonTeachingTip.IsOpen = true;
            }
        }

        public void OnThemeButtonClick(object sender, RoutedEventArgs e)
        {
            ToggleThemeAction?.Invoke();
            UIHelper.AnnounceActionForAccessibility(ThemeButton, "Theme changed.", "ThemeChangedSuccessNotificationId");
        }

        private void OnCopyDontShowAgainButtonClick(TeachingTip sender, object args)
        {
            ProtocolActivationClipboardHelper.ShowCopyLinkTeachingTip = false;
            CopyLinkButtonTeachingTip.IsOpen = false;
        }

        private void OnCopyLink()
        {
            ProtocolActivationClipboardHelper.Copy(Item);
        }

        public async void OnFeedBackButtonClick(object sender, RoutedEventArgs e)
        {
             await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/microsoft/WinUI-Gallery/issues/new/choose"));
        }

        [GeneratedComInterface]
        [Guid("3A3DCD6C-3EAB-43DC-BCDE-45671CE800C8")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public partial interface IDataTransferManagerInterop
        {
            IntPtr GetForWindow(in IntPtr appWindow, in Guid riid);
            void ShowShareUIForWindow(IntPtr appWindow);
        }

        private static Guid _dtm_iid =
            new(0xa5caee9b, 0x8708, 0x49d1, 0x8d, 0x36, 0x67, 0xd2, 0x5a, 0x8d, 0xa0, 0x0c);

        public void ShareButton_Click(object sender, RoutedEventArgs e)
        {
            // example: https://learn.microsoft.com/windows/apps/develop/ui-input/display-ui-objects#for-classes-that-implement-idatatransfermanagerinterop

            return; // CODE DOESN'T WORK -- GetWindowHandle(this) -- 'this' should equal MainWindow.Xaml.cs

            // Retrieve the window handle (HWND) of the current WinUI 3 window.
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

            IDataTransferManagerInterop interop =
            Windows.ApplicationModel.DataTransfer.DataTransferManager.As
                <IDataTransferManagerInterop>();

            IntPtr result = interop.GetForWindow(hWnd, riid: in _dtm_iid);
            var dataTransferManager = WinRT.MarshalInterface
                <Windows.ApplicationModel.DataTransfer.DataTransferManager>.FromAbi(result);

            dataTransferManager.DataRequested += (sender, args) =>
            {
                args.Request.Data.Properties.Title = "In a desktop app...";
                args.Request.Data.SetText("...display WinRT UI objects that depend on CoreWindow.");
                args.Request.Data.RequestedOperation =
                    Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy;
            };

            // Show the Share UI
            interop.ShowShareUIForWindow(hWnd);
        }
    }
}
