using AppUIBasics.SamplePages;
using AppUIBasics.Common;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using System.Linq;
using Windows.System;
using Microsoft.UI.Xaml.Automation;

namespace AppUIBasics.ControlPages
{
    public sealed partial class NavigationViewPage : Page
    {
        public static bool CameFromToggle = false;

        public static bool CameFromGridChange = false;

        public VirtualKey ArrowKey;

        public ObservableCollection<CategoryBase> Categories { get; set; }

        public NavigationViewPage()
        {
            this.InitializeComponent();

            nvSample2.SelectedItem = nvSample2.MenuItems.OfType<Microsoft.UI.Xaml.Controls.NavigationViewItem>().First();
            nvSample5.SelectedItem = nvSample5.MenuItems.OfType<Microsoft.UI.Xaml.Controls.NavigationViewItem>().First();
            nvSample6.SelectedItem = nvSample6.MenuItems.OfType<Microsoft.UI.Xaml.Controls.NavigationViewItem>().First();
            nvSample7.SelectedItem = nvSample7.MenuItems.OfType<Microsoft.UI.Xaml.Controls.NavigationViewItem>().First();
            nvSample8.SelectedItem = nvSample8.MenuItems.OfType<Microsoft.UI.Xaml.Controls.NavigationViewItem>().First();
            nvSample9.SelectedItem = nvSample9.MenuItems.OfType<Microsoft.UI.Xaml.Controls.NavigationViewItem>().First();

            Categories = new ObservableCollection<CategoryBase>();
            Category firstCategory = new Category { Name = "Category 1", Glyph = Symbol.Home, Tooltip = "This is category 1" };
            Categories.Add(firstCategory);
            Categories.Add(new Category { Name = "Category 2", Glyph = Symbol.Keyboard, Tooltip = "This is category 2" });
            Categories.Add(new Category { Name = "Category 3", Glyph = Symbol.Library, Tooltip = "This is category 3" });
            Categories.Add(new Category { Name = "Category 4", Glyph = Symbol.Mail, Tooltip = "This is category 4" });
            nvSample4.SelectedItem = firstCategory;

            setASBSubstitutionString();

            // Fixes #218
            nvSample2.UpdateLayout();
        }

        public Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode ChoosePanePosition(bool toggleOn)
        {
            if (toggleOn)
            {
                return Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Left;
            } else
            {
                return Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Top;
            }
        }

        private void NavigationView_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                contentFrame.Navigate(typeof(SampleSettingsPage));
            }
            else
            {
                var selectedItem = (Microsoft.UI.Xaml.Controls.NavigationViewItem)args.SelectedItem;
                if (selectedItem != null)
                {
                    string selectedItemTag = ((string)selectedItem.Tag);
                    sender.Header = "Sample Page " + selectedItemTag.Substring(selectedItemTag.Length - 1);
                    string pageName = "AppUIBasics.SamplePages." + selectedItemTag;
                    Type pageType = Type.GetType(pageName);
                    contentFrame.Navigate(pageType);
                }
            }
        }

        private void NavigationView_SelectionChanged2(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (!CameFromGridChange)
            {
                if (args.IsSettingsSelected)
                {
                    contentFrame2.Navigate(typeof(SampleSettingsPage));
                }
                else
                {
                    var selectedItem = (Microsoft.UI.Xaml.Controls.NavigationViewItem)args.SelectedItem;
                    string selectedItemTag = ((string)selectedItem.Tag);
                    string pageName = "AppUIBasics.SamplePages." + selectedItemTag;
                    Type pageType = Type.GetType(pageName);
                    contentFrame2.Navigate(pageType);
                }
            }

            CameFromGridChange = false;
        }

        private void NavigationView_SelectionChanged4(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                contentFrame4.Navigate(typeof(SampleSettingsPage));
            }
            else
            {
                Debug.WriteLine("Before hitting sample page 1");

                var selectedItem = (Category)args.SelectedItem;
                string selectedItemTag = selectedItem.Name;
                sender.Header = "Sample Page " + selectedItemTag.Substring(selectedItemTag.Length - 1);
                string pageName = "AppUIBasics.SamplePages." + "SamplePage1";
                Type pageType = Type.GetType(pageName);
                contentFrame4.Navigate(pageType);
            }
        }


        private void NavigationView_SelectionChanged5(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                contentFrame5.Navigate(typeof(SampleSettingsPage));
            }
            else
            {
                var selectedItem = (Microsoft.UI.Xaml.Controls.NavigationViewItem)args.SelectedItem;
                string selectedItemTag = ((string)selectedItem.Tag);
                sender.Header = "Sample Page " + selectedItemTag.Substring(selectedItemTag.Length - 1);
                string pageName = "AppUIBasics.SamplePages." + selectedItemTag;
                Type pageType = Type.GetType(pageName);
                contentFrame5.Navigate(pageType);
            }
        }
        private void NavigationView_SelectionChanged6(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                contentFrame6.Navigate(typeof(SampleSettingsPage));
            }
            else
            {
                var selectedItem = (Microsoft.UI.Xaml.Controls.NavigationViewItem)args.SelectedItem;
                string pageName = "AppUIBasics.SamplePages." + ((string)selectedItem.Tag);
                Type pageType = Type.GetType(pageName);
                contentFrame6.Navigate(pageType);
            }
        }

        private void NavigationView_SelectionChanged7(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                contentFrame7.Navigate(typeof(SampleSettingsPage));
            }
            else
            {
                var selectedItem = (Microsoft.UI.Xaml.Controls.NavigationViewItem)args.SelectedItem;
                string pageName = "AppUIBasics.SamplePages." + ((string)selectedItem.Tag);
                Type pageType = Type.GetType(pageName);

                contentFrame7.Navigate(pageType, null, args.RecommendedNavigationTransitionInfo);
            }
        }

        private void NavigationView_SelectionChanged8(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            /* NOTE: for this function to work, every NavigationView must follow the same naming convention: nvSample# (i.e. nvSample3),
            and every corresponding content frame must follow the same naming convention: contentFrame# (i.e. contentFrame3) */

            // Get the sample number
            string sampleNum = (sender.Name).Substring(8);
            Debug.Print("num: " + sampleNum + "\n");

            if (args.IsSettingsSelected)
            {
                contentFrame8.Navigate(typeof(SampleSettingsPage));
            }
            else
            {
                var selectedItem = (Microsoft.UI.Xaml.Controls.NavigationViewItem)args.SelectedItem;
                string selectedItemTag = ((string)selectedItem.Tag);
                sender.Header = "Sample Page " + selectedItemTag.Substring(selectedItemTag.Length - 1);
                string pageName = "AppUIBasics.SamplePages." + selectedItemTag;
                Type pageType = Type.GetType(pageName);
                contentFrame8.Navigate(pageType);
            }
        }

        private void NavigationView_SelectionChanged9(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            var selectedItem = (Microsoft.UI.Xaml.Controls.NavigationViewItem)args.SelectedItem;
            string pageName = "AppUIBasics.SamplePages." + ((string)selectedItem.Tag);
            Type pageType = Type.GetType(pageName);

            contentFrame9.Navigate(pageType, null, args.RecommendedNavigationTransitionInfo);
        }

        private void databindHeader_Checked(object sender, RoutedEventArgs e)
        {
            Categories = new ObservableCollection<CategoryBase>()
            {
                new Header { Name = "Header1 "},
                new Category { Name = "Category 1", Glyph = Symbol.Home, Tooltip = "This is category 1" },
                new Category { Name = "Category 2", Glyph = Symbol.Keyboard, Tooltip = "This is category 2" },
                new Separator(),
                new Header { Name = "Header2 "},
                new Category {Name = "Category 3", Glyph = Symbol.Library, Tooltip = "This is category 3" },
                new Category {Name = "Category 4", Glyph = Symbol.Mail, Tooltip = "This is category 3" }
            };
        }

        private void databindHeader_Checked_Unchecked(object sender, RoutedEventArgs e)
        {
            Categories = new ObservableCollection<CategoryBase>()
            {
                new Category { Name = "Category 1", Glyph = Symbol.Home, Tooltip = "This is category 1" },
                new Category { Name = "Category 2", Glyph = Symbol.Keyboard, Tooltip = "This is category 2" },
                new Category {Name = "Category 3", Glyph = Symbol.Library, Tooltip = "This is category 3" },
                new Category {Name = "Category 4", Glyph = Symbol.Mail, Tooltip = "This is category 3" }
            };
        }

        private void Grid_ManipulationDelta1(object sender, Microsoft.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
            var grid = sender as Grid;
            grid.Width = grid.ActualWidth + e.Delta.Translation.X;
        }

        private void headerCheck_Click(object sender, RoutedEventArgs e)
        {
            nvSample.AlwaysShowHeader = (sender as CheckBox).IsChecked == true ? true : false;
        }

        private void settingsCheck_Click(object sender, RoutedEventArgs e)
        {
            nvSample.IsSettingsVisible = (sender as CheckBox).IsChecked == true ? true : false;
        }

        private void visibleCheck_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked == true)
            {
                nvSample.IsBackButtonVisible = Microsoft.UI.Xaml.Controls.NavigationViewBackButtonVisible.Visible;
            }
            else
            {
                nvSample.IsBackButtonVisible = Microsoft.UI.Xaml.Controls.NavigationViewBackButtonVisible.Collapsed;
            }
        }

        private void enableCheck_Click(object sender, RoutedEventArgs e)
        {
            nvSample.IsBackEnabled = (sender as CheckBox).IsChecked == true ? true : false;
        }

        private void autoSuggestCheck_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked == true)
            {
                AutoSuggestBox asb = new AutoSuggestBox() { QueryIcon = new SymbolIcon(Symbol.Find) };
                asb.SetValue(AutomationProperties.NameProperty, "search");
                nvSample.AutoSuggestBox = asb;

                setASBSubstitutionString();
            }
            else
            {
                nvSample.AutoSuggestBox = null;
                navViewASB.Value = null;
            }
        }

        private void setASBSubstitutionString()
        {
            navViewASB.Value = "\r\n    <NavigationView.AutoSuggestBox> \r\n        <AutoSuggestBox QueryIcon=\"Find\" AutomationProperties.Name=\"Search\" /> \r\n    <" + "/" + "NavigationView.AutoSuggestBox> \r\n";
        }

        private void panemc_Check_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked == true)
            {
                PaneHyperlink.Visibility = Visibility.Visible;
            }
            else
            {
                PaneHyperlink.Visibility = Visibility.Collapsed;
            }
        }

        private void paneFooterCheck_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked == true)
            {
                FooterStackPanel.Visibility = Visibility.Visible;
            }
            else
            {
                FooterStackPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void panePositionLeft_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as RadioButton).IsChecked == true)
            {
                if ((sender as RadioButton).Name == "nvSampleLeft" && nvSample != null)
                {
                    nvSample.PaneDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Left;
                    nvSample.IsPaneOpen = true;
                    FooterStackPanel.Orientation = Orientation.Vertical;
                }
                else if ((sender as RadioButton).Name == "nvSample8Left" && nvSample8 != null)
                {
                    nvSample8.PaneDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Left;
                    nvSample8.IsPaneOpen = true;
                }
                else if ((sender as RadioButton).Name == "nvSample9Left" && nvSample9 != null)
                {
                    nvSample9.PaneDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Left;
                    nvSample9.IsPaneOpen = true;
                }
            }
        }


        private void panePositionTop_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as RadioButton).IsChecked == true)
            {
                if ((sender as RadioButton).Name == "nvSampleTop" && nvSample != null)
                {
                    nvSample.PaneDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Top;
                    nvSample.IsPaneOpen = false;
                    FooterStackPanel.Orientation = Orientation.Horizontal;
                }
                else if ((sender as RadioButton).Name == "nvSample8Top" && nvSample8 != null)
                {
                    nvSample8.PaneDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Top;
                    nvSample8.IsPaneOpen = false;
                }
                else if ((sender as RadioButton).Name == "nvSample9Top" && nvSample9 != null)
                {
                    nvSample9.PaneDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Top;
                    nvSample9.IsPaneOpen = false;
                }
            }
        }

        private void panePositionLeftCompact_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as RadioButton).IsChecked == true)
            {
                if ((sender as RadioButton).Name == "nvSample8LeftCompact" && nvSample8 != null)
                {
                    nvSample8.PaneDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.LeftCompact;
                    nvSample8.IsPaneOpen = false;
                }
            }
        }

        private void sffCheck_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked == true)
            {
                nvSample.SelectionFollowsFocus = Microsoft.UI.Xaml.Controls.NavigationViewSelectionFollowsFocus.Enabled;
            }
            else
            {
                nvSample.SelectionFollowsFocus = Microsoft.UI.Xaml.Controls.NavigationViewSelectionFollowsFocus.Disabled;
            }
        }

        private void suppressselectionCheck_Checked_Click(object sender, RoutedEventArgs e)
        {
            SamplePage2Item.SelectsOnInvoked = (sender as CheckBox).IsChecked == true ? false : true;
        }
    }
}
