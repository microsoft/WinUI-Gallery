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
using System.Linq;
using System.Numerics;
using System.Reflection;
using AppUIBasics.Data;
using AppUIBasics.Helper;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.System;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;

namespace AppUIBasics
{
    /// <summary>
    /// A page that displays details for a single item within a group.
    /// </summary>
    public sealed partial class ItemPage : Page
    {
        public Action CopyLinkAction { get; set; }
        public Action ToggleThemeAction { get; set; }
        private Compositor _compositor;
        private ControlInfoDataItem _item;
        private ElementTheme? _currentElementTheme;
        public ControlInfoDataItem Item
        {
            get { return _item; }
            set { _item = value; }
        }

        public ItemPage()
        {
            this.InitializeComponent();
            Loaded += (s,e) => SetInitialVisuals();
        }

        public void SetInitialVisuals()
        {
         
            var navigationRootPage = NavigationRootPage.GetForElement(this);
            if (navigationRootPage != null)
            {
      
                ToggleThemeAction = OnToggleTheme;
                navigationRootPage.NavigationViewLoaded = OnNavigationViewLoaded;
                CopyLinkAction = OnCopyLink;
                ResetCopyLinkButton();

                    this.Focus(FocusState.Programmatic);
                
            }


          

            if (UIHelper.IsScreenshotMode)
            {
                var controlExamples = (this.contentFrame.Content as UIElement)?.GetDescendantsOfType<ControlExample>();

                if (controlExamples != null)
                {
                    foreach (var controlExample in controlExamples)
                    {
                        VisualStateManager.GoToState(controlExample, "ScreenshotMode", false);
                    }
                }
            }
        }

      

        private void OnNavigationViewLoaded()
        {
            NavigationRootPage.GetForElement(this).EnsureNavigationSelection(this.Item.UniqueId);
        }

        private void OnCopyLink()
        {
            ProtocolActivationClipboardHelper.Copy(this.Item);
        }

        private void OnToggleTheme()
        {
            var currentElementTheme = ((_currentElementTheme ?? ElementTheme.Default) == ElementTheme.Default) ? ThemeHelper.ActualTheme : _currentElementTheme.Value;
            var newTheme = currentElementTheme == ElementTheme.Dark ? ElementTheme.Light : ElementTheme.Dark;
            SetControlExamplesTheme(newTheme);
        }

        private void SetControlExamplesTheme(ElementTheme theme)
        {
            var controlExamples = (this.contentFrame.Content as UIElement)?.GetDescendantsOfType<ControlExample>();

            if (controlExamples != null)
            {
                _currentElementTheme = theme;
                foreach (var controlExample in controlExamples)
                {
                    var exampleContent = controlExample.Example as FrameworkElement;
                    exampleContent.RequestedTheme = theme;
                    controlExample.ExampleContainer.RequestedTheme = theme;
                }
            }
        }

        private void OnRelatedControlClick(object sender, RoutedEventArgs e)
        {
            ButtonBase b = (ButtonBase)sender;

            NavigationRootPage.GetForElement(this).Navigate(typeof(ItemPage), b.DataContext.ToString());
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationRootPageArgs args = (NavigationRootPageArgs)e.Parameter;
            var item = await ControlInfoDataSource.Instance.GetItemAsync((String)args.Parameter);

            if (item != null)
            {
                Item = item;

                // Load control page into frame.
                string pageRoot = "AppUIBasics.ControlPages.";
                string pageString = pageRoot + item.UniqueId + "Page";
                Type pageType = Type.GetType(pageString);

                if (pageType != null)
                {
                    // Pagetype is not null!
                    // So lets generate the github links and set them!
                    var gitHubBaseURI = "https://github.com/microsoft/WinUI-Gallery/tree/main/WinUIGallery/ControlPages/";
                    var pageName = pageType.Name + ".xaml";
                    PageCodeGitHubLink.NavigateUri = new Uri(gitHubBaseURI + pageName + ".cs");
                    PageMarkupGitHubLink.NavigateUri = new Uri(gitHubBaseURI + pageName);

                    System.Diagnostics.Debug.WriteLine(string.Format("[ItemPage] Navigate to {0}", pageType.ToString()));
                    this.contentFrame.Navigate(pageType);
                }
                args.NavigationRootPage.EnsureNavigationSelection(item?.UniqueId);
            }

            base.OnNavigatedTo(e);
        }

        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Escape)
            {
                this.Item = null;
                if (this.contentFrame.CanGoBack)
                {
                    this.contentFrame.GoBack();
                }
                else
                {
                    NavigationRootPage.GetForElement(this).Navigate(typeof(AllControlsPage));
                }
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            SetControlExamplesTheme(ThemeHelper.ActualTheme);

            base.OnNavigatingFrom(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            var navigationRootPage = NavigationRootPage.GetForElement(this);
            if (navigationRootPage != null)
            {
                navigationRootPage.NavigationViewLoaded = null;
                ToggleThemeAction = null;
                CopyLinkAction = null;
            }

            // We use reflection to call the OnNavigatedFrom function the user leaves this page
            // See this PR for more information: https://github.com/microsoft/WinUI-Gallery/pull/145
            Frame contentFrameAsFrame = contentFrame as Frame;
            Page innerPage = contentFrameAsFrame.Content as Page;
            if (innerPage != null)
            {
                MethodInfo dynMethod = innerPage.GetType().GetMethod("OnNavigatedFrom",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                dynMethod.Invoke(innerPage, new object[] { e });
            }

            base.OnNavigatedFrom(e);
        }

        public static ItemPage GetForElement(object obj)
        {
            UIElement element = (UIElement)obj;
            Window window = WindowHelper.GetWindowForElement(element);
            if (window != null)
            {
                return (ItemPage)window.Content;
            }
            return null;
        }


        private void ToggleThemeTeachingTip2_ActionButtonClick(Microsoft.UI.Xaml.Controls.TeachingTip sender, object args)
        {
            // NavigationRootPage.GetForElement(this).PageHeader.ToggleThemeAction?.Invoke();
        }


        private void OnCopyLinkButtonClick(object sender, RoutedEventArgs e)
            {
                this.CopyLinkAction?.Invoke();

                if (ProtocolActivationClipboardHelper.ShowCopyLinkTeachingTip)
                {
                    this.CopyLinkButtonTeachingTip.IsOpen = true;
                }
            this.CopyLinkButtonIcon.Glyph = "\uE8FB";
        }

            public void OnThemeButtonClick(object sender, RoutedEventArgs e)
            {
                ToggleThemeAction?.Invoke();
            }

            public void ResetCopyLinkButton()
            {
                this.CopyLinkButtonTeachingTip.IsOpen = false;
                this.CopyLinkButtonIcon.Glyph = "\uE71B";
        }

            private void OnCopyDontShowAgainButtonClick(TeachingTip sender, object args)
            {
                ProtocolActivationClipboardHelper.ShowCopyLinkTeachingTip = false;
                this.CopyLinkButtonTeachingTip.IsOpen = false;
            }

        }
    }
