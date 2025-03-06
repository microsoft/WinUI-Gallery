using System.Linq;
using Microsoft.UI.Xaml.Navigation;
using WinUIGallery.Helpers;
using WinUIGallery.Pages;

namespace WinUIGallery.ControlPages;

public sealed partial class ParallaxViewPage : ItemsPageBase
{
    public ParallaxViewPage()
    {
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        Items = ControlInfoDataSource.Instance.Groups.SelectMany(g => g.Items).OrderBy(i => i.Title).ToList();
    }
}
