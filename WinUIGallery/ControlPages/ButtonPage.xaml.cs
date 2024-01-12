//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace AppUIBasics.ControlPages
{
    public sealed class ButtonResource
    {
        public string Name;
        public SolidColorBrush Brush;
    }

    public sealed partial class ButtonPage : Page
    {
        public ButtonPage()
        {
            this.InitializeComponent();

            var buttonResources = Application.Current.Resources.MergedDictionaries
                .SelectMany((d) =>
                {
                    return d.ThemeDictionaries.SelectMany((t) =>
                    {
                        return (t.Value as ResourceDictionary).Where((r) => r.Key.ToString().StartsWith("Button"));
                    });
                })
                .Where(r => r.Value is SolidColorBrush)
                .Select((r) =>
                {
                    return new ButtonResource()
                    {
                        Name = r.Key.ToString(),
                        Brush = r.Value as SolidColorBrush
                    };
                });
            LightweightStylesBox.ItemsSource = buttonResources;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b)
            {
                string name = b.Name;

                switch (name)
                {
                    case "Button1":
                        Control1Output.Text = "You clicked: " + name;
                        break;
                    case "Button2":
                        Control2Output.Text = "You clicked: " + name;
                        break;
                    
                }
            }
        }
    }
}
