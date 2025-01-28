using Microsoft.UI.Xaml.Controls;
using WinUIGallery.Pages;

namespace WinUIGallery.ControlPages;

public sealed partial class XamlResourcesPage : Page
{
    public XamlResourcesPage()
    {
        this.InitializeComponent();
    }

    private void Hyperlink_Click(Microsoft.UI.Xaml.Documents.Hyperlink sender, Microsoft.UI.Xaml.Documents.HyperlinkClickEventArgs args)
    {
        NavigationRootPage.GetForElement(this).Navigate(typeof(ItemPage), "Color");
    }
}
