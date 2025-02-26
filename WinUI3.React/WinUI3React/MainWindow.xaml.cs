// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI3React
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private string BaseSource => (Application.Current as App).BaseURL;
        public MainViewModel ViewModel { get; } = new MainViewModel() {
            WebMessage = "(message from web)"
        };

        public MainWindow()
        {
            Debug.WriteLine("loading...");
            InitializeComponent();
        }
        private void webView_CoreWebView2Initialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            Debug.WriteLine("wv2 initialized");
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            await Attach();
        }

        private async Task Attach()
        {
            Debug.WriteLine("awaiting core webview2");
            await webView.EnsureCoreWebView2Async();
            Debug.WriteLine("core webview2 ensured");
            webView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
            Debug.WriteLine("webview attached.");
        }

        private void CoreWebView2_WebMessageReceived(Microsoft.Web.WebView2.Core.CoreWebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs args)
        {
            var message = args.TryGetWebMessageAsString();
            Debug.WriteLine($"web->native: {message}");
            ViewModel.WebMessage = message;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            webView.CoreWebView2.PostWebMessageAsString("test");
        }

        public class MainViewModel : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            string _webMessage;
            public string WebMessage
            {
                get => _webMessage;
                set => SetProperty(ref _webMessage, value);
            }

            /// <summary>
            /// Checks if a property already matches a desired value.  Sets the property and
            /// notifies listeners only when necessary.
            /// </summary>
            /// <typeparam name="T">Type of the property.</typeparam>
            /// <param name="storage">Reference to a property with both getter and setter.</param>
            /// <param name="value">Desired value for the property.</param>
            /// <param name="propertyName">Name of the property used to notify listeners.  This
            /// value is optional and can be provided automatically when invoked from compilers that
            /// support CallerMemberName.</param>
            /// <returns>True if the value was changed, false if the existing value matched the
            /// desired value.</returns>
            private bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
            {
                if (object.Equals(storage, value)) return false;

                storage = value;
                OnPropertyChanged(propertyName);
                return true;
            }

            /// <summary>
            /// Notifies listeners that a property value has changed.
            /// </summary>
            /// <param name="propertyName">Name of the property used to notify listeners.  This
            /// value is optional and can be provided automatically when invoked from compilers
            /// that support <see cref="CallerMemberNameAttribute"/>.</param>
            private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
