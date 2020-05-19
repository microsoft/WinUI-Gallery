//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using AppUIBasics.Common;
using AppUIBasics.Data;
using System;
using System.Linq;
using System.Numerics;
using Windows.ApplicationModel.Resources;
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
using System.Reflection;

namespace AppUIBasics
{
    /// <summary>
    /// A page that displays details for a single item within a group.
    /// </summary>
    public partial class ItemPage : Page
    {
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

            LayoutVisualStates.CurrentStateChanged += (s, e) => UpdateSeeAlsoPanelVerticalTranslationAnimation();
            Loaded += (s,e) => SetInitialVisuals();
            this.Unloaded += this.ItemPage_Unloaded;
        }

        private void ItemPage_Unloaded(object sender, RoutedEventArgs e)
        {
            // Notifying the pageheader that this Itempage was unloaded
            NavigationRootPage.Current.PageHeader.Event_ItemPage_Unloaded(sender, e);
        }

        public void SetInitialVisuals()
        {
            NavigationRootPage.Current.PageHeader.TopCommandBar.Visibility = Visibility.Visible;
            NavigationRootPage.Current.PageHeader.ToggleThemeAction = OnToggleTheme;

            if (NavigationRootPage.Current.IsFocusSupported)
            {
                this.Focus(FocusState.Programmatic);
            }

            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

            UpdateSeeAlsoPanelVerticalTranslationAnimation();
        }

        private void UpdateSeeAlsoPanelVerticalTranslationAnimation()
        {
            var isEnabled = LayoutVisualStates.CurrentState == LargeLayout;

            ElementCompositionPreview.SetIsTranslationEnabled(seeAlsoPanel, true);

            var targetPanelVisual = ElementCompositionPreview.GetElementVisual(seeAlsoPanel);
            targetPanelVisual.Properties.InsertVector3("Translation", Vector3.Zero);

            if (isEnabled)
            {
                var scrollProperties = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(svPanel);

                var expression = _compositor.CreateExpressionAnimation("ScrollManipulation.Translation.Y * -1");
                expression.SetReferenceParameter("ScrollManipulation", scrollProperties);
                expression.Target = "Translation.Y";
                targetPanelVisual.StartAnimation(expression.Target, expression);
            }
            else
            {
                targetPanelVisual.StopAnimation("Translation.Y");
            }
        }

        private void OnToggleTheme()
        {
            var currentElementTheme = ((_currentElementTheme ?? ElementTheme.Default) == ElementTheme.Default) ? App.ActualTheme : _currentElementTheme.Value;
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

            this.Frame.Navigate(typeof(ItemPage), b.DataContext.ToString());
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var item = await ControlInfoDataSource.Instance.GetItemAsync((String)e.Parameter);

            if (item != null)
            {
                Item = item;

                // Load control page into frame.
                var loader = ResourceLoader.GetForCurrentView();

                string pageRoot = loader.GetString("PageStringRoot");

                string pageString = pageRoot + item.UniqueId + "Page";
                Type pageType = Type.GetType(pageString);

                if (pageType != null)
                {
                    this.contentFrame.Navigate(pageType);
                }

                NavigationRootPage.Current.NavigationView.Header = item?.Title;
                if (item.IsNew && NavigationRootPage.Current.CheckNewControlSelected())
                {
                    PlayConnectedAnimation();
                    return;
                }

                ControlInfoDataGroup group = await ControlInfoDataSource.Instance.GetGroupFromItemAsync((String)e.Parameter);
                var menuItem = NavigationRootPage.Current.NavigationView.MenuItems.Cast<Microsoft.UI.Xaml.Controls.NavigationViewItemBase>().FirstOrDefault(m => m.Tag?.ToString() == group.UniqueId);
                if (menuItem != null)
                {
                    menuItem.IsSelected = true;
                }

                PlayConnectedAnimation();
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
                    this.Frame.Navigate(typeof(AllControlsPage));
                }
            }
        }

        void PlayConnectedAnimation()
        {
            if (NavigationRootPage.Current.PageHeader != null)
            {
                var connectedAnimation = ConnectedAnimationService.GetForCurrentView().GetAnimation("controlAnimation");

                if (connectedAnimation != null)
                {
                    var target = NavigationRootPage.Current.PageHeader.TitlePanel;

                    // Setup the "basic" configuration if the API is present. 
                    if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
                    {
                        connectedAnimation.Configuration = new BasicConnectedAnimationConfiguration();
                    }

                    connectedAnimation.TryStart(target, new UIElement[] { subTitleText });
                }
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            SetControlExamplesTheme(App.ActualTheme);

            base.OnNavigatingFrom(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            NavigationRootPage.Current.PageHeader.TopCommandBar.Visibility = Visibility.Collapsed;
            NavigationRootPage.Current.PageHeader.ToggleThemeAction = null;

            // Reverse Connected Animation
            if (e.SourcePageType != typeof(ItemPage))
            {
                var target = NavigationRootPage.Current.PageHeader.TitlePanel;
                ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("controlAnimation", target);
            }

            // We use reflection to call the OnNavigatedFrom function the user leaves this page
            // See this PR for more information: https://github.com/microsoft/Xaml-Controls-Gallery/pull/145
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

        private void OnContentRootSizeChanged(object sender, SizeChangedEventArgs e)
        {
            string targetState = "NormalFrameContent";

            if ((contentColumn.ActualWidth) >= 1000)
            {
                targetState = "WideFrameContent";
            }

            VisualStateManager.GoToState(this, targetState, false);
        }
    }
}
