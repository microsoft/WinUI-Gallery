using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;

namespace WinUIGallery.ControlPages;

public sealed partial class CustomUserControlsPage : Page
{
    public CustomUserControlsPage()
    {
        this.InitializeComponent();

        // Example synonyms dictionary
        var synonymsDictionary = new Dictionary<string, List<string>>
            {
                { "happy", new List<string> { "joyful", "cheerful", "content" } },
                { "fast", new List<string> { "quick", "speedy", "rapid" } },
                { "smart", new List<string> { "intelligent", "clever", "bright" } }
            };

        // Set data source
        SynonymFinder.SynonymsSource = synonymsDictionary;
    }
}
