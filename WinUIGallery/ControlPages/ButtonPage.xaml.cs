//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using System;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace AppUIBasics.ControlPages
{
    public sealed class ButtonResource
    {
        public string Name;
        public SolidColorBrush LightBrush;
        public SolidColorBrush DarkBrush;
        public SolidColorBrush HighContrastBrush;
    }

    public sealed partial class ButtonPage : Page
    {
        public ButtonPage()
        {
            this.InitializeComponent();

            var rawButtonResources = Application.Current.Resources.MergedDictionaries
                .SelectMany((d) =>
                {
                    return d.ThemeDictionaries.SelectMany((t) =>
                    {
                        // TODO: this isn't the best approach. It works, since lightweight styles always have a key that starts with "Button",
                        // but it would be nicer to inspect the default template somehow & get a complete list of styles.
                        //
                        // Similarly, it'd be nice to automatically know to fetch things like "AccentButton".
                        return (t.Value as ResourceDictionary)
                            .Where((r) => r.Key.ToString().StartsWith("Button") && r.Value is SolidColorBrush)
                            .Select((r) =>
                            {
                                return new Tuple<string, string, SolidColorBrush>(t.Key.ToString(), r.Key.ToString(), r.Value as SolidColorBrush);
                            });
                    });
                });

            var buttonResources = rawButtonResources
                .GroupBy((t) => t.Item2)
                .Select((grouped) => {
                    return new ButtonResource
                    {
                        Name = grouped.Key,
                        LightBrush = grouped.Where((t) => t.Item1 == "Light").Select((t) => t.Item3).FirstOrDefault(),
                        DarkBrush = grouped.Where((t) => t.Item1 == "Default").Select((t) => t.Item3).FirstOrDefault(),
                        HighContrastBrush = grouped.Where((t) => t.Item1 == "HighContrast").Select((t) => t.Item3).FirstOrDefault()
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
