using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

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

        public TestObservableCollection<Bar> BarItems;
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
                BarItems = new TestObservableCollection<Bar>();
            }
            BarItems.Add(new Bar(300, this.MaxLength));
            BarItems.Add(new Bar(25, this.MaxLength));
            BarItems.Add(new Bar(175, this.MaxLength));
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
