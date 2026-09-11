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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AppUIBasics.ControlPages
{
    public sealed partial class WebViewPage : Page
    {
        private WebView2 webView;

        public WebViewPage()
        {
            this.InitializeComponent();
            Loaded += WebViewPage_Loaded;
            Unloaded += WebViewPage_Unloaded;
        }

        private void WebViewPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (webView == null)
            {
                webView = new WebView2
                {
                    Source = new Uri("https://learn.microsoft.com/en-us/microsoft-edge/webview2/get-started/winui"),
                    MinHeight = 400,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch
                };
                WebViewHost.Children.Add(webView);
            }
        }

        private void WebViewPage_Unloaded(object sender, RoutedEventArgs e)
        {
            webView?.Close();
            WebViewHost.Children.Clear();
            webView = null;
        }
    }
}
