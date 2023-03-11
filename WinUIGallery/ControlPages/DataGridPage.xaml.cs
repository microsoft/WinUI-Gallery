using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.System;

namespace AppUIBasics.ControlPages
{ 
    public sealed partial class DataGridPage : Page
    {
        public DataGridPage()
        {
            this.InitializeComponent();
        }

        private async void LaunchToolkitButton_Click(object sender, RoutedEventArgs e)
        {
            // Either open the app if already instealled or the Microsoft Store

            var isToolkitAvailable = await Windows.System.Launcher.QueryUriSupportAsync(new Uri("uwpct://controls?sample=datagrid"), Windows.System.LaunchQuerySupportType.Uri);

            if (isToolkitAvailable == LaunchQuerySupportStatus.Available)
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("uwpct://controls?sample=datagrid"));
            }
            else
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri(@"ms-windows-store://pdp/?ProductId=9NBLGGH4TLCQ"));
            }
        }
    }
}
