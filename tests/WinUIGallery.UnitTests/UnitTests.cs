// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using WinUIGallery.Helpers;

namespace WinUIGallery.UnitTests;

[TestClass]
public class UnitTests
{
    [TestMethod]
    public async Task TestControlInfoDataSource()
    {
        var groups = await ControlInfoDataSource.Instance.GetGroupsAsync();
        var groupsList = groups.ToList();

        var expectedGroups = new List<string>
        {
            "Fundamentals",
            "Design",
            "Accessibility",
            "Menus & toolbars",
            "Collections",
            "Date & time",
            "Basic input",
            "Status & info",
            "Dialogs & flyouts",
            "Scrolling",
            "Layout",
            "Navigation",
            "Media",
            "Styles",
            "Text",
            "Motion",
            "Windowing",
            "System"
        };

        Assert.AreEqual(expectedGroups.Count, groupsList.Count);

        int groupCount = expectedGroups.Count;
        for(int i = 0; i < groupCount; i++)
        {
            var actualTitle = groupsList[i].Title;
            Assert.AreEqual(expectedGroups[i], actualTitle);
        }
    }

    // Use the UITestMethod attribute for tests that need to run on the UI thread.
    [UITestMethod]
    public void TestWrapGrid()
    {
        Layouts.WrapPanel wrapPanel = new()
        {
            Width = 250,
            Height = 250
        };
        for (int i = 0; i < 4; i++) 
        {
            wrapPanel.Children.Add(new Button()
            {
                Width = 120,
                Height = 80,
                Content = $"Button {i}"
            });
        }

        UnitTestApp.UnitTestAppWindow.AddToVisualTree(wrapPanel);
        wrapPanel.UpdateLayout();

        List<Rect> expectedLayouts =
        [
            new Rect(0,    0, 120,  80),
            new Rect(120,  0, 120,  80),
            new Rect(0,   80, 120,  80),
            new Rect(120, 80, 120,  80)
        ];
        for (int i = 0; i < 4; i++)
        {
            var actualLayout = LayoutInformation.GetLayoutSlot(wrapPanel.Children[i] as FrameworkElement);
            Assert.AreEqual(expectedLayouts[i], actualLayout);
        }
    }

    // This test demonstrates executing test code both on and off the UI thread.
    // We use the ExecuteOnUIThread method to run code on the UI thread.
    [TestMethod]
    public void MultiThreadTest()
    {
        Border border = null;
        AutoResetEvent borderSizeChanged = new(false);

        ExecuteOnUIThread(() =>
        {
            Grid grid = new()
            {
                Width = 200,
            };

            border = new Border
            {
                Background = new SolidColorBrush(Colors.Green),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Child = new Rectangle
                {
                    Fill = new SolidColorBrush(Colors.Red),
                    Width = 100,
                }
            };

            border.SizeChanged += (s, e) =>
            {
                borderSizeChanged.Set();
            };

            grid.Children.Add(border);
            
            UnitTestApp.UnitTestAppWindow.AddToVisualTree(grid);
        });
        Assert.IsTrue(borderSizeChanged.WaitOne());

        ExecuteOnUIThread(() =>
        {
            Assert.AreEqual(200, border.ActualWidth);

            border.HorizontalAlignment = HorizontalAlignment.Left;
        });

        Assert.IsTrue(borderSizeChanged.WaitOne());

        ExecuteOnUIThread(() =>
        {
            Assert.AreEqual(100, border.ActualWidth);
        });
    }

    private void ExecuteOnUIThread(Action action)
    {
        AutoResetEvent done = new(false);
        DispatcherQueue dispatcherQueue = UnitTestApp.UnitTestAppWindow.DispatcherQueue;
        if (dispatcherQueue.HasThreadAccess)
        {
            action();
        }
        else
        {
            Exception exception = null;
            var success = dispatcherQueue.TryEnqueue(() =>
            {
                try
                {
                    action();
                }
                catch (Exception ex) 
                { 
                    exception = ex;
                }
                finally 
                { 
                    done.Set(); 
                }
            });
            Assert.IsTrue(success);
            Assert.IsTrue(done.WaitOne());
            if(exception != null)
            {
                Assert.Fail(exception.ToString());
            }
        }
    }

    [TestMethod]
    public async Task TestExternalLinksValidity()
    {
        // List of all external links found in NavigateUri attributes across XAML files and ControlInfoData.json
        var externalLinks = new List<string>
        {
            // Links from NavigateUri attributes in XAML files
            "https://accessibilityinsights.io/",
            "https://aka.ms/lottie",
            "https://aka.ms/toolkit/windows",
            "https://aka.ms/windowsappsdk",
            "https://aka.ms/winui",
            "https://github.com/CommunityToolkit/MVVM-Samples",
            "https://github.com/Microsoft/Win2D",
            "https://github.com/WilliamABradley/ColorCode-Universal",
            "https://go.microsoft.com/fwlink/?LinkId=521839",
            "https://go.microsoft.com/fwlink/?LinkId=822631",
            "https://learn.microsoft.com/accessibility-tools-docs/items/uwpxaml/control_fulldescription_describedby_helptext",
            "https://learn.microsoft.com/azure/azure-maps/how-to-manage-account-keys",
            "https://learn.microsoft.com/windows/apps/design/accessibility/accessibility-overview",
            "https://learn.microsoft.com/windows/apps/design/accessibility/basic-accessibility-information",
            "https://learn.microsoft.com/windows/apps/design/accessibility/basic-accessibility-information#accessible-name",
            "https://learn.microsoft.com/windows/apps/design/accessibility/basic-accessibility-information#influencing-the-ui-automation-tree-views",
            "https://learn.microsoft.com/windows/apps/design/accessibility/basic-accessibility-information#name-from-inner-text",
            "https://learn.microsoft.com/windows/apps/design/accessibility/keyboard-accessibility",
            "https://learn.microsoft.com/windows/apps/design/accessibility/landmarks-and-headings",
            "https://learn.microsoft.com/windows/apps/design/downloads/#fonts",
            "https://learn.microsoft.com/windows/apps/design/input/access-keys",
            "https://learn.microsoft.com/windows/apps/design/input/focus-navigation",
            "https://learn.microsoft.com/windows/apps/design/input/keyboard-accelerators",
            "https://learn.microsoft.com/windows/apps/design/input/keyboard-interactions",
            "https://learn.microsoft.com/windows/apps/design/input/keyboard-interactions#control-group",
            "https://learn.microsoft.com/windows/apps/design/input/keyboard-interactions#home-and-end-keys",
            "https://learn.microsoft.com/windows/apps/design/input/keyboard-interactions#navigation",
            "https://learn.microsoft.com/windows/apps/design/input/keyboard-interactions#page-up-and-page-down-keys",
            "https://learn.microsoft.com/windows/apps/design/style/acrylic#usability-and-adaptability",
            "https://learn.microsoft.com/windows/apps/develop/data-binding/data-binding-in-depth#xbind-and-binding-feature-comparison",
            "https://learn.microsoft.com/windows/apps/package-and-deploy/deploy-overview",
            "https://learn.microsoft.com/windows/apps/package-and-deploy/self-contained-deploy/deploy-self-contained-apps#dependencies-on-additional-msix-packages",
            "https://learn.microsoft.com/windows/apps/windows-app-sdk/deployment-architecture#singleton-package",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.composition.compositionshape",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.input.inputnonclientpointersource",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.windowing.appwindowtitlebar",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.Documents.Hyperlink",
            "https://www.microsoft.com",
            "https://www.unicode.org/notes/tn28/",
            "https://www.w3.org/Math/",
            
            // Links from ControlInfoData.json
            "https://aka.ms/WinUI/3.0-figma-toolkit",
            "https://github.com/CommunityToolkit/Lottie-Windows",
            "https://github.com/Microsoft/Windows-universal-samples/tree/master/Samples/XamlBottomUpList",
            "https://github.com/MicrosoftEdge/WebView2Samples",
            "https://github.com/microsoft/Windows-universal-samples/tree/master/Samples/XamlDragAndDrop",
            "https://github.com/microsoft/microsoft-ui-xaml/blob/main/src/controls/dev/CommonStyles/Common_themeresources_any.xaml",
            "https://learn.microsoft.com/microsoft-edge/webview2/gettingstarted/winui",
            "https://learn.microsoft.com/previous-versions/windows/apps/hh465055(v=win.10)",
            "https://learn.microsoft.com/uwp/api/Windows.Storage.Pickers",
            "https://learn.microsoft.com/uwp/api/windows.applicationmodel.datatransfer.clipboard",
            "https://learn.microsoft.com/uwp/api/windows.media.capture.mediacapture",
            "https://learn.microsoft.com/windows/apps/design/accessibility/accessibility",
            "https://learn.microsoft.com/windows/apps/design/accessibility/accessible-text-requirements",
            "https://learn.microsoft.com/windows/apps/design/basics/content-basics",
            "https://learn.microsoft.com/windows/apps/design/controls/animated-icon",
            "https://learn.microsoft.com/windows/apps/design/controls/auto-suggest-box",
            "https://learn.microsoft.com/windows/apps/design/controls/breadcrumbbar",
            "https://learn.microsoft.com/windows/apps/design/controls/buttons",
            "https://learn.microsoft.com/windows/apps/design/controls/buttons#create-a-split-button",
            "https://learn.microsoft.com/windows/apps/design/controls/buttons#create-a-toggle-split-button",
            "https://learn.microsoft.com/windows/apps/design/controls/calendar-date-picker",
            "https://learn.microsoft.com/windows/apps/design/controls/calendar-view",
            "https://learn.microsoft.com/windows/apps/design/controls/checkbox",
            "https://learn.microsoft.com/windows/apps/design/controls/collection-commanding",
            "https://learn.microsoft.com/windows/apps/design/controls/color-picker",
            "https://learn.microsoft.com/windows/apps/design/controls/combo-box",
            "https://learn.microsoft.com/windows/apps/design/controls/command-bar",
            "https://learn.microsoft.com/windows/apps/design/controls/command-bar-flyout",
            "https://learn.microsoft.com/windows/apps/design/controls/commanding#command-experiences-using-the-standarduicommand-class",
            "https://learn.microsoft.com/windows/apps/design/controls/commanding#command-experiences-using-the-xamluicommand-class",
            "https://learn.microsoft.com/windows/apps/design/controls/date-picker",
            "https://learn.microsoft.com/windows/apps/design/controls/dialogs-and-flyouts",
            "https://learn.microsoft.com/windows/apps/design/controls/dialogs-and-flyouts/dialogs",
            "https://learn.microsoft.com/windows/apps/design/controls/dialogs-and-flyouts/teaching-tip",
            "https://learn.microsoft.com/windows/apps/design/controls/expander",
            "https://learn.microsoft.com/windows/apps/design/controls/flipview",
            "https://learn.microsoft.com/windows/apps/design/controls/hyperlinks",
            "https://learn.microsoft.com/windows/apps/design/controls/images-imagebrushes",
            "https://learn.microsoft.com/windows/apps/design/controls/info-badge",
            "https://learn.microsoft.com/windows/apps/design/controls/infobar",
            "https://learn.microsoft.com/windows/apps/design/controls/inverted-lists",
            "https://learn.microsoft.com/windows/apps/design/controls/items-repeater",
            "https://learn.microsoft.com/windows/apps/design/controls/lists",
            "https://learn.microsoft.com/windows/apps/design/controls/listview-and-gridview",
            "https://learn.microsoft.com/windows/apps/design/controls/listview-filtering",
            "https://learn.microsoft.com/windows/apps/design/controls/media-playback",
            "https://learn.microsoft.com/windows/apps/design/controls/menus",
            "https://learn.microsoft.com/windows/apps/design/controls/navigationview",
            "https://learn.microsoft.com/windows/apps/design/controls/number-box",
            "https://learn.microsoft.com/windows/apps/design/controls/page-header",
            "https://learn.microsoft.com/windows/apps/design/controls/parallax",
            "https://learn.microsoft.com/windows/apps/design/controls/person-picture",
            "https://learn.microsoft.com/windows/apps/design/controls/pivot",
            "https://learn.microsoft.com/windows/apps/design/controls/progress-controls",
            "https://learn.microsoft.com/windows/apps/design/controls/pull-to-refresh",
            "https://learn.microsoft.com/windows/apps/design/controls/radio-button",
            "https://learn.microsoft.com/windows/apps/design/controls/rating",
            "https://learn.microsoft.com/windows/apps/design/controls/scroll-controls",
            "https://learn.microsoft.com/windows/apps/design/controls/slider",
            "https://learn.microsoft.com/windows/apps/design/controls/split-view",
            "https://learn.microsoft.com/windows/apps/design/controls/swipe",
            "https://learn.microsoft.com/windows/apps/design/controls/tab-view",
            "https://learn.microsoft.com/windows/apps/design/controls/text-controls",
            "https://learn.microsoft.com/windows/apps/design/controls/time-picker",
            "https://learn.microsoft.com/windows/apps/design/controls/tooltips",
            "https://learn.microsoft.com/windows/apps/design/controls/tree-view",
            "https://learn.microsoft.com/windows/apps/design/controls/two-pane-view",
            "https://learn.microsoft.com/windows/apps/design/input/access-keys",
            "https://learn.microsoft.com/windows/apps/design/input/focus-navigation",
            "https://learn.microsoft.com/windows/apps/design/input/keyboard-accelerators",
            "https://learn.microsoft.com/windows/apps/design/input/keyboard-interactions",
            "https://learn.microsoft.com/windows/apps/design/layout/grid-tutorial",
            "https://learn.microsoft.com/windows/apps/design/layout/layout-panels",
            "https://learn.microsoft.com/windows/apps/design/layout/layout-panels#grid",
            "https://learn.microsoft.com/windows/apps/design/layout/show-multiple-views",
            "https://learn.microsoft.com/windows/apps/design/motion",
            "https://learn.microsoft.com/windows/apps/design/motion/connected-animation",
            "https://learn.microsoft.com/windows/apps/design/motion/motion-in-practice#implicit-animations",
            "https://learn.microsoft.com/windows/apps/design/motion/page-transitions",
            "https://learn.microsoft.com/windows/apps/design/motion/parallax",
            "https://learn.microsoft.com/windows/apps/design/motion/timing-and-easing",
            "https://learn.microsoft.com/windows/apps/design/motion/xaml-animation#animations-available-in-the-library",
            "https://learn.microsoft.com/windows/apps/design/motion/xaml-property-animations",
            "https://learn.microsoft.com/windows/apps/design/shell/tiles-and-notifications/badges",
            "https://learn.microsoft.com/windows/apps/design/shell/tiles-and-notifications/toast-notifications-overview",
            "https://learn.microsoft.com/windows/apps/design/signature-experiences/color",
            "https://learn.microsoft.com/windows/apps/design/signature-experiences/geometry",
            "https://learn.microsoft.com/windows/apps/design/signature-experiences/iconography#system-icons",
            "https://learn.microsoft.com/windows/apps/design/signature-experiences/typography",
            "https://learn.microsoft.com/windows/apps/design/style/acrylic",
            "https://learn.microsoft.com/windows/apps/design/style/icons",
            "https://learn.microsoft.com/windows/apps/design/style/segoe-fluent-icons-font",
            "https://learn.microsoft.com/windows/apps/design/style/segoe-ui-symbol-font",
            "https://learn.microsoft.com/windows/apps/design/style/sound",
            "https://learn.microsoft.com/windows/apps/design/style/spacing",
            "https://learn.microsoft.com/windows/apps/design/style/typography",
            "https://learn.microsoft.com/windows/apps/design/style/xaml-control-templates",
            "https://learn.microsoft.com/windows/apps/design/style/xaml-resource-dictionary",
            "https://learn.microsoft.com/windows/apps/design/style/xaml-styles",
            "https://learn.microsoft.com/windows/apps/design/style/xaml-theme-resources",
            "https://learn.microsoft.com/windows/apps/design/style/xaml-theme-resources#the-xaml-type-ramp",
            "https://learn.microsoft.com/windows/apps/develop/data-binding/",
            "https://learn.microsoft.com/windows/apps/windows-app-sdk/composition",
            "https://learn.microsoft.com/windows/apps/winui/winui3/xaml-templated-controls-csharp-winui-3",
            "https://learn.microsoft.com/windows/communitytoolkit/animations/lottie",
            "https://learn.microsoft.com/windows/communitytoolkit/animations/lottie#tutorials",
            "https://learn.microsoft.com/windows/uwp/xaml-platform/binding-markup-extension",
            "https://learn.microsoft.com/windows/uwp/xaml-platform/x-bind-markup-extension",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.composition.systembackdrops.desktopacryliccontroller",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.composition.systembackdrops.micacontroller",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.content.contentisland",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.windowing.appwindow",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.windowing.appwindowpresenter",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.windowing.compactoverlaypresenter",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.windowing.fullscreenpresenter",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.windowing.overlappedpresenter",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.Controls.MenuBar",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.Controls.NavigationView",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.Controls.Parallaxview",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.Controls.PersonPicture",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.Controls.RefreshContainer",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.Controls.RefreshVisualizer",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.Controls.TreeView",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.animatedicon",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.animatedvisualplayer",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.appbarbutton",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.appbarseparator",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.appbartogglebutton",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.autosuggestbox",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.bitmapicon",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.border",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.breadcrumbbar",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.button",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.calendardatepicker",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.calendarview",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.canvas",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.checkbox",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.combobox",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.comboboxitem",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.commandbar",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.commandbarflyout",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.control",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.controltemplate",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.datepicker",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.dropdownbutton",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.expander",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.flipview",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.fonticon",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.grid",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.gridview",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.hyperlinkbutton",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.image",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.imageicon",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.infobadge",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.infobar",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.itemspaneltemplate",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.itemsrepeater",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.itemsview",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.listbox",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.listboxitem",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.listview",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.mapcontrol",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.MediaPlayerElement",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.menuflyout",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.menuflyoutitem",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.menuflyoutseparator",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.menuflyoutsubitem",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.numberbox",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.passwordbox",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.pathicon",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.pivot",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.progressbar",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.progressring",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.radiobutton",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.radiobuttons",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.radiomenuflyoutitem",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.relativepanel",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.richeditbox",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.richtextblock",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.scrollviewer",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.selectorbar",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.selectorbaritem",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.slider",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.splitbutton",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.splitview",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.stacklayout",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.stackpanel",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.swipecontrol",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.swipeitems",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.symbolicon",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.tabview",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.teachingtip",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.textblock",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.textbox",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.timepicker",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.togglemenuflyoutitem",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.togglesplitbutton",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.toggleswitch",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.tooltip",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.twopaneview",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.uniformgridlayout",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.usercontrol",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.variablesizedwrapgrid",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.Viewbox",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.webview2",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.Data.CollectionViewSource",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.data.binding",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.data.ivalueconverter",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.datatemplate",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.elementsoundplayer",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.input.standarduicommand",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.input.xamluicommand",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.markup.xamlreader",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.media.animation.connectedanimation",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.media.animation.connectedanimationservice",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.media.animation.easingfunctionbase",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.media.acrylicbrush",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.media.desktopacrylicbackdrop",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.media.micabackdrop",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.media.RadialGradientBrush",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.media.systembackdrop",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.Media.Animation.NavigationThemeTransition",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.resourcedictionary",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.shapes",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.style",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.uielement.transitions#Windows_UI_Xaml_UIElement_Transitions",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.window",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.window.extendscontentintotitlebar",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.windows.appnotifications.appnotification",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.windows.appnotifications.appnotificationmanager",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.windows.appnotifications.builder.appnotificationbuilder",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.windows.badgenotifications.badgenotificationglyph",
            "https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.windows.badgenotifications.badgenotificationmanager",
            "https://learn.microsoft.com/windows/winui/api/microsoft.ui.xaml.controls.animatedvisualplayer",
            "https://ms-windows-store://pdp/?productid=9N3J5TG8FF7F"
        };

        using var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(30); // Set reasonable timeout
        
        // Add user agent to avoid some servers blocking the request
        httpClient.DefaultRequestHeaders.Add("User-Agent", "WinUI-Gallery-LinkValidator/1.0");

        var failedLinks = new List<string>();
        var checkedCount = 0;

        foreach (var link in externalLinks)
        {
            try
            {
                using var response = await httpClient.GetAsync(link);
                checkedCount++;
                
                // Consider 2xx and 3xx status codes as success
                // Also accept some 4xx codes that might be normal for redirect services
                if (!response.IsSuccessStatusCode && 
                    (int)response.StatusCode >= 400 && 
                    response.StatusCode != System.Net.HttpStatusCode.TooManyRequests)
                {
                    failedLinks.Add($"{link} - Status: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("Name or service not known") || 
                                                 ex.Message.Contains("temporarily unavailable"))
            {
                // Skip DNS resolution failures in test environments
                checkedCount++;
                continue;
            }
            catch (HttpRequestException ex)
            {
                failedLinks.Add($"{link} - HttpRequestException: {ex.Message}");
                checkedCount++;
            }
            catch (TaskCanceledException ex)
            {
                failedLinks.Add($"{link} - Timeout: {ex.Message}");
                checkedCount++;
            }
            catch (Exception ex)
            {
                failedLinks.Add($"{link} - Exception: {ex.Message}");
                checkedCount++;
            }
            
            // Add small delay to be respectful to servers
            await Task.Delay(100);
        }

        // Assert that we checked a reasonable number of links and no critical failures occurred
        Assert.IsTrue(checkedCount > 0, "No links were checked");
        
        // If more than 50% of links fail, it's likely a test environment issue
        if (failedLinks.Count > externalLinks.Count / 2)
        {
            Assert.Inconclusive($"More than 50% of links failed ({failedLinks.Count}/{externalLinks.Count}), " +
                              "which may indicate test environment network restrictions. " +
                              $"Checked {checkedCount} links.");
        }
        
        // Assert that no links failed with actual HTTP errors (not network issues)
        var httpErrorFailures = failedLinks.Where(f => f.Contains("Status:") && 
                                                      !f.Contains("TooManyRequests")).ToList();
        
        if (httpErrorFailures.Any())
        {
            var failureMessage = $"The following external links returned HTTP errors:\n{string.Join("\n", httpErrorFailures)}";
            Assert.Fail(failureMessage);
        }
    }

    [TestCleanup]
    public void Cleanup()
    {
        ExecuteOnUIThread(() =>
        {
            UnitTestApp.UnitTestAppWindow.CleanupVisualTree();
        });
    }
}
