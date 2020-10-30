using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WindowsGallaryApp.Scripts;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AppUIBasics.ControlPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PagerControlPage : Page
    {
        private List<ImageGenerator> imageList = new List<ImageGenerator>();
        private const int imagesPerPage = 5;
        public PagerControlPage()
        {
            this.InitializeComponent();
        }
        private async void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            imageList = await ImageGenerator.GenerateImages();
            pager.NumberOfPages = await ImageGenerator.GetNumberOfPages(imagesPerPage, imageList);
            dataGrid.ItemsSource = await ImageGenerator.GetImagesInPage(0, imagesPerPage, imageList);
        }
        private async void OnPageChanged(PagerControl sender, PagerControlSelectedIndexChangedEventArgs args)
        {
            dataGrid.ItemsSource = await ImageGenerator.GetImagesInPage(args.NewPageIndex, imagesPerPage, imageList);
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb)
            {
                string DisplayModeName = rb.Content.ToString();
                switch (DisplayModeName)
                {
                    case "ComboBox":
                        pager.DisplayMode = PagerControlDisplayMode.ComboBox;
                        break;
                    case "NumberBox":
                        pager.DisplayMode = PagerControlDisplayMode.NumberBox;
                        break;
                    case "ButtonPanel":
                        pager.DisplayMode = PagerControlDisplayMode.ButtonPanel;
                        break;
                }
            }
        }
    }
}
