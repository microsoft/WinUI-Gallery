using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Documents;
using System.ComponentModel;

namespace WinUIGallery.ControlPages
{
    public sealed partial class BindingPage : Page
    {
        public ExampleViewModel ViewModel { get; set; }

        public BindingPage()
        {
            this.InitializeComponent();

            ViewModel = new ExampleViewModel
            {
                Title = "Welcome to WinUI 3",
                Description = "This is an example of binding to a view model.",
                NullString = null
            };
            DataContext = ViewModel;

            mvvmToolkitSampleAppHyperlinkButton.NavigateUri = new Uri("https://github.com/CommunityToolkit/MVVM-Samples");
        }

        private void BindingModeGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is RadioButtons radioButtons)
            {
                switch (radioButtons.SelectedIndex)
                {
                    case 0:
                        UpdateBindingAndDescription(BindingMode.OneWay);
                        break;
                    case 1:
                        UpdateBindingAndDescription(BindingMode.TwoWay);
                        break;
                }
            }
        }

        private void UpdateBindingAndDescription(BindingMode bindingMode)
        {
            var binding = new Binding
            {
                Source = SourceTextBox,
                Path = new PropertyPath("Text"),
                Mode = bindingMode,
            };
            TargetTextBox.SetBinding(TextBox.TextProperty, binding);

            BindingModeDescription.Blocks.Clear();
            var paragraph = new Paragraph();
            if (bindingMode == BindingMode.OneWay)
            {
                paragraph.Inlines.Add(new Run
                {
                    Text = "In ",
                });
                paragraph.Inlines.Add(new Bold { Inlines = { new Run { Text = "OneWay" } } });
                paragraph.Inlines.Add(new Run
                {
                    Text = " binding mode, changes in the source (`SourceTextBox`) are reflected in the target, but not vice versa."
                });
            }
            else if (bindingMode == BindingMode.TwoWay)
            {
                paragraph.Inlines.Add(new Run
                {
                    Text = "In ",
                });
                paragraph.Inlines.Add(new Bold { Inlines = { new Run { Text = "TwoWay" } } });
                paragraph.Inlines.Add(new Run
                {
                    Text = " binding mode, changes in either box update the other."
                });
            }

            BindingModeDescription.Blocks.Add(paragraph);
        }

        public string FormatDate(DateTimeOffset? date)
        {
            if (date.HasValue)
            {
                return "Selected date is: " + date.Value.ToString("dddd, MMMM d, yyyy");
            }
            else
            {
                return "No date selected";
            }
        }
    }

    public class ExampleViewModel : INotifyPropertyChanged
    {
        private string _title;
        private string _description;
        private string _nullString;

        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        public string NullString
        {
            get => _nullString;
            set
            {
                if (_nullString != value)
                {
                    _nullString = value;
                    OnPropertyChanged(nameof(_nullString));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (Visibility)value == Visibility.Visible;
        }
    }
}
