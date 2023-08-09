using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;


namespace AppUIBasics.ControlPages
{
    public sealed partial class BreadcrumbBarPage : Page
    {
        readonly List<Folder> folders = new() {
                new Folder { Name = "Home"},
                new Folder { Name = "Folder1" },
                new Folder { Name = "Folder2" },
                new Folder { Name = "Folder3" },
        };

        private DispatcherTimer resetTimer = new DispatcherTimer();

        public BreadcrumbBarPage()
        {
            this.InitializeComponent();
            BreadcrumbBar1.ItemsSource = new string[] { "Home", "Documents", "Design", "Northwind", "Images", "Folder1", "Folder2", "Folder3" };

            BreadcrumbBar2.ItemsSource = new ObservableCollection<Folder>(folders);
            BreadcrumbBar2.ItemClicked += BreadcrumbBar2_ItemClicked;

            resetTimer.Interval = TimeSpan.FromSeconds(5);
            resetTimer.Tick += ResetTimer_Tick;
        }

        private void BreadcrumbBar2_ItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
        {
            var items = BreadcrumbBar2.ItemsSource as ObservableCollection<Folder>;
            for (int i = items.Count - 1; i >= args.Index + 1; i--)
            {
                items.RemoveAt(i);
            }
        }

        private void ResetSampleButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var items = BreadcrumbBar2.ItemsSource as ObservableCollection<Folder>;
            for (int i = items.Count; i < folders.Count; i++)
            {
                items.Add(folders[i]);
            }

            // Toggle success message for accessibility.
            ResetSampleSucessTextBlock.Text = "Reset successful";
            ResetSampleSucessTextBlock.Opacity = 1;

            var peer = FrameworkElementAutomationPeer.FromElement(ResetSampleSucessTextBlock);

            if (peer != null)
            {
                peer.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
            }

            // Start the timer to hide the success message after 5 seconds.
            resetTimer.Start();
        }

        private void ResetTimer_Tick(object sender, object e)
        {
            // Reset the message and stop the timer
            ResetSampleSucessTextBlock.Text = string.Empty;
            ResetSampleSucessTextBlock.Opacity = 0;
            resetTimer.Stop();
        }
    }

    public class Folder
    {
        public string Name { get; set; }
    }
}
