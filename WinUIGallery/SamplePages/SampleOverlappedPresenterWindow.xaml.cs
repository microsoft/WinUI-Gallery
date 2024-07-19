using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUIGallery.SamplePages
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SampleOverlappedPresenterWindow : Window, INotifyPropertyChanged
    {
        OverlappedPresenter presenter;
        public event PropertyChangedEventHandler PropertyChanged;
        public SampleOverlappedPresenterWindow()
        {
            presenter = AppWindow.Presenter as OverlappedPresenter;
            this.InitializeComponent();
        }

        public bool HasBorder
        {
            get => presenter.HasBorder;
            set
            {
                try
                {
                    presenter.SetBorderAndTitleBar(value, HasTitleBar);
                }
                catch (Exception e)
                {
                    ShowExceptionDialog(e.Message);
                }
                NotifyPropertyChanged("HasBorder");
                NotifyPropertyChanged("HasTitleBar");
            }
        }

        public bool HasTitleBar
        {
            get => presenter.HasTitleBar;
            set
            {
                try
                {
                    presenter.SetBorderAndTitleBar(HasBorder, value);
                }
                catch (Exception e)
                {
                    ShowExceptionDialog(e.Message);
                }
                NotifyPropertyChanged("HasTitleBar");
                NotifyPropertyChanged("HasBorder");
            }
        }

        public bool IsAlwaysOnTop
        {
            get => presenter.IsAlwaysOnTop;
            set => presenter.IsAlwaysOnTop = value;
        }

        public bool IsMaximizable
        {
            get => presenter.IsMaximizable;
            set => presenter.IsMaximizable = value;
        }

        public bool IsMinimizable
        {
            get => presenter.IsMinimizable;
            set => presenter.IsMinimizable = value;
        }

        public bool IsModal
        {
            get => presenter.IsModal;
            set => presenter.IsModal = value;
        }

        public bool IsResizable
        {
            get => presenter.IsResizable;
            set => presenter.IsResizable = value;
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ExtendsContentIntoTitleBar = !ExtendsContentIntoTitleBar;
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        private void ShowExceptionDialog(string message)
        {
            new ContentDialog()
            {
                XamlRoot = this.Content.XamlRoot,
                Title = "Exception thrown",
                Content = message,
                PrimaryButtonText = "OK",
            }.ShowAsync();
        }
    }
}
