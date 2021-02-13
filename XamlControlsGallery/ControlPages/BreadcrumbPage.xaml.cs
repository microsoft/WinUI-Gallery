using Windows.UI.Xaml.Controls;


namespace AppUIBasics.ControlPages
{
    public sealed partial class BreadcrumbPage : Page
    {
 
    public BreadcrumbPage()
        {
            this.InitializeComponent();
            Breadcrumb1.ItemsSource = new string[] { "Home", "Documents", "Design", "Northwind", "Images" };
        }
    }
}
