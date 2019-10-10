using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace AppUIBasics.ControlPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ItemsRepeaterPage : ItemsPageBase
    {
        private Random random = new Random();
        private int MaxLength = 425;
        private bool isHorizontal = false;

        public ObservableCollection<Bar> BarItems;
        public ItemsRepeaterPage()
        {
            this.InitializeComponent();
            InitializeData();
            repeater2.ItemsSource = Enumerable.Range(0, 500);
        }

        private void InitializeData()
        {
            if (BarItems == null)
            {
                BarItems = new ObservableCollection<Bar>();
            }
            BarItems.Add(new Bar(300, this.MaxLength));
            BarItems.Add(new Bar(25, this.MaxLength));
            BarItems.Add(new Bar(175, this.MaxLength));

            ObservableCollection<object> basicData = new ObservableCollection<object>();
            basicData.Add(64);
            basicData.Add("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.");
            basicData.Add(128);
            basicData.Add("Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.");
            basicData.Add(256);
            basicData.Add("Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.");
            basicData.Add(512);
            basicData.Add("Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.");
            basicData.Add(1024);
            repeater0.ItemsSource = basicData;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            BarItems.Add(new Bar(random.Next(this.MaxLength), this.MaxLength));
            DeleteBtn.IsEnabled = true;
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (BarItems.Count > 0)
            {
                BarItems.RemoveAt(0);
                if (BarItems.Count == 0)
                {
                    DeleteBtn.IsEnabled = false;
                }
            }
        }

        private void OrientationBtn_Click(object sender, RoutedEventArgs e)
        {
            string layoutKey = String.Empty, itemTemplateKey = String.Empty;

            if (isHorizontal)
            {
                layoutKey = "VerticalStackLayout";
                itemTemplateKey = "HorizontalBarTemplate";
            }
            else
            {
                layoutKey = "HorizontalStackLayout";
                itemTemplateKey = "VerticalBarTemplate";
            }

            repeater.Layout = Resources[layoutKey] as Microsoft.UI.Xaml.Controls.VirtualizingLayout;
            repeater.ItemTemplate = Resources[itemTemplateKey] as DataTemplate;
            repeater.ItemsSource = BarItems;

            layout.Value = layoutKey;
            elementGenerator.Value = itemTemplateKey;

            isHorizontal = !isHorizontal;
        }

        private void LayoutBtn_Click(object sender, RoutedEventArgs e)
        {
            string layoutKey = ((FrameworkElement)sender).Tag as string;

            repeater2.Layout = Resources[layoutKey] as Microsoft.UI.Xaml.Controls.VirtualizingLayout;

            layout2.Value = layoutKey;
        }

        private void RadioBtn_Click(object sender, RoutedEventArgs e)
        {
            string itemTemplateKey = String.Empty;
            var layoutKey = ((FrameworkElement)sender).Tag as string;

            if (layoutKey.Equals(nameof(this.VerticalStackLayout))) // we used x:Name in the resources which both acts as the x:Key value and creates a member field by the same name
            {
                layout.Value = layoutKey;
                itemTemplateKey = "HorizontalBarTemplate";

                repeater.MaxWidth = MaxLength + 12;
            }
            else if (layoutKey.Equals(nameof(this.HorizontalStackLayout)))
            {
                layout.Value = layoutKey;
                itemTemplateKey = "VerticalBarTemplate";

                repeater.MaxWidth = 6000;
            }
            else if (layoutKey.Equals(nameof(this.UniformGridLayout)))
            {
                layout.Value = layoutKey;
                itemTemplateKey = "CircularTemplate";

                repeater.MaxWidth = 540;
            }

            repeater.Layout = Resources[layoutKey] as Microsoft.UI.Xaml.Controls.VirtualizingLayout;
            repeater.ItemTemplate = Resources[itemTemplateKey] as DataTemplate;
            repeater.ItemsSource = BarItems;

            elementGenerator.Value = itemTemplateKey;
        }
    }

    public class MyDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Normal { get; set; }
        public DataTemplate Accent { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if ((int)item % 2 == 0)
            {
                return Normal;
            }
            else
            {
                return Accent;
            }
        }
    }

    public class StringOrIntTemplateSelector : DataTemplateSelector
    {
        public DataTemplate StringTemplate { get; set; }

        public DataTemplate IntTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item.GetType() == typeof(String))
            {
                return StringTemplate;
            }

            else if (item.GetType() == typeof(int))
            {
                return IntTemplate;
            }

            else
            {
                return null;
            }
        }
    }

    public class Bar
    {
        public Bar(double length, int max)
        {
            Length = length;
            MaxLength = max;

            Height = length / 4;
            MaxHeight = max / 4;

            Diameter = length / 6;
            MaxDiameter = max / 6;
        }
        public double Length { get; set; }
        public int MaxLength { get; set; }

        public double Height { get; set; }
        public double MaxHeight { get; set; }

        public double Diameter { get; set; }
        public double MaxDiameter { get; set; }
    }
}
