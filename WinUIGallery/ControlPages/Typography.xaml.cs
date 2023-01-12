using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using AppUIBasics.SamplePages;

namespace AppUIBasics.ControlPages
{
    public sealed partial class TypographyPage : Page
    {
        public TypographyPage()
        {
            this.InitializeComponent();

        }

        //private void Example1_Loaded(object sender, RoutedEventArgs e)
        //{
        //    ContentFrame.Navigate(typeof(SampleStandardSizingPage), null, new SuppressNavigationTransitionInfo());
        //}

        //private void Standard_Checked(object sender, RoutedEventArgs e)
        //{
        //    var oldPage = ContentFrame.Content as SampleTypographyPage;

        //    ContentFrame.Navigate(typeof(SampleStandardSizingPage), null, new SuppressNavigationTransitionInfo());

        //    if (oldPage != null)
        //    {
        //        var page = ContentFrame.Content as SampleStandardSizingPage;
        //        page.CopyState(oldPage);
        //    }
        //}

        //private void Compact_Checked(object sender, RoutedEventArgs e)
        //{
        //    var oldPage = ContentFrame.Content as SampleStandardSizingPage;

        //    ContentFrame.Navigate(typeof(SampleTypographyPage), null, new SuppressNavigationTransitionInfo());

        //    if (oldPage != null)
        //    {
        //        var page = ContentFrame.Content as SampleTypographyPage;
        //        page.CopyState(oldPage);
        //    }
        //}
    }
}
