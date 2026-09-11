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

            BreadcrumbBar2.ItemsSource = new List<Folder>
            {
                new Folder { Name = "Home"},
                new Folder { Name = "Folder1" },
                new Folder { Name = "Folder2" }
            };
        }
    }

    public class Folder
    {
        public string Name { get; set; }
    }
}
