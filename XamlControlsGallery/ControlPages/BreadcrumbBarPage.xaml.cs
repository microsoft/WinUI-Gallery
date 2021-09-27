using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;


namespace AppUIBasics.ControlPages
{
    public sealed partial class BreadcrumbBarPage : Page
    {
 
        public BreadcrumbBarPage()
        {
            this.InitializeComponent();
            BreadcrumbBar1.ItemsSource = new string[] { "Home", "Documents", "Design", "Northwind", "Images", "Folder1", "Folder2", "Folder3" };

            BreadcrumbBar2.ItemsSource = new List<Foo>
            {
                new Foo { Name = "Home"},
                new Foo { Name = "Folder1" },
                new Foo { Name = "Folder2" }
            };
        }
    }

    public class Foo
    {
        public string Name { get; set; }
    }
}
