//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace AppUIBasics.ControlPages
{
    public sealed partial class ButtonPage : Page
    {
        public ButtonPage()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;

            if (b != null)
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
