//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using AppUIBasics.Common;
using ColorCode;
using ColorCode.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using System.Reflection;

namespace AppUIBasics
{
    /// <summary>
    /// Describes a textual substitution in sample content.
    /// If enabled (default), then $(Key) is replaced with the stringified value.
    /// If disabled, then $(Key) is replaced with the empty string.
    /// </summary>
    public sealed class ControlExampleSubstitution : DependencyObject
    {
        public event TypedEventHandler<ControlExampleSubstitution, object> ValueChanged;

        public string Key { get; set; }

        private object _value = null;
        public object Value
        {
            get { return _value; }
            set
            {
                _value = value;
                ValueChanged?.Invoke(this, null);
            }
        }

        private bool _enabled = true;
        public bool IsEnabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                ValueChanged?.Invoke(this, null);
            }
        }

        public string ValueAsString()
        {
            if (!IsEnabled)
            {
                return string.Empty;
            }

            object value = Value;

            // For solid color brushes, use the underlying color.
            if (value is SolidColorBrush)
            {
                value = ((SolidColorBrush)value).Color;
            }

            if (value == null)
            {
                return string.Empty;
            }
            
            return value.ToString();
        }
    }

    [ContentProperty(Name = "Example")]
    public sealed partial class ControlExample : UserControl
    {
        public static readonly DependencyProperty HeaderTextProperty = DependencyProperty.Register("HeaderText", typeof(string), typeof(ControlExample), new PropertyMetadata(null));
        public string HeaderText
        {
            get { return (string)GetValue(HeaderTextProperty); }
            set { SetValue(HeaderTextProperty, value); }
        }

        public static readonly DependencyProperty ExampleProperty = DependencyProperty.Register("Example", typeof(object), typeof(ControlExample), new PropertyMetadata(null));
        public object Example
        {
            get { return GetValue(ExampleProperty); }
            set { SetValue(ExampleProperty, value); }
        }

        public static readonly DependencyProperty OptionsProperty = DependencyProperty.Register("Options", typeof(object), typeof(ControlExample), new PropertyMetadata(null));
        public object Options
        {
            get { return GetValue(OptionsProperty); }
            set { SetValue(OptionsProperty, value); }
        }

        public static readonly DependencyProperty XamlProperty = DependencyProperty.Register("Xaml", typeof(string), typeof(ControlExample), new PropertyMetadata(null));
        public string Xaml
        {
            get { return (string)GetValue(XamlProperty); }
            set { SetValue(XamlProperty, value); }
        }

        public static readonly DependencyProperty XamlSourceProperty = DependencyProperty.Register("XamlSource", typeof(string), typeof(ControlExample), new PropertyMetadata(null));
        public string XamlSource
        {
            get { return (string)GetValue(XamlSourceProperty); }
            set { SetValue(XamlSourceProperty, value); }
        }

        public static readonly DependencyProperty CSharpProperty = DependencyProperty.Register("Xaml", typeof(string), typeof(ControlExample), new PropertyMetadata(null));
        public string CSharp
        {
            get { return (string)GetValue(CSharpProperty); }
            set { SetValue(CSharpProperty, value); }
        }

        public static readonly DependencyProperty CSharpSourceProperty = DependencyProperty.Register("CSharpSource", typeof(string), typeof(ControlExample), new PropertyMetadata(null));
        public string CSharpSource
        {
            get { return (string)GetValue(CSharpSourceProperty); }
            set { SetValue(CSharpSourceProperty, value); }
        }

        public static readonly DependencyProperty SubstitutionsProperty = DependencyProperty.Register("Substitutions", typeof(IList<ControlExampleSubstitution>), typeof(ControlExample), new PropertyMetadata(null));
        public IList<ControlExampleSubstitution> Substitutions
        {
            get { return (IList<ControlExampleSubstitution>)GetValue(SubstitutionsProperty); }
            set { SetValue(SubstitutionsProperty, value); }
        }

        private static readonly GridLength defaultExampleHeight = new GridLength(1, GridUnitType.Star);

        public static readonly DependencyProperty ExampleHeightProperty = DependencyProperty.Register("ExampleHeight", typeof(GridLength), typeof(ControlExample), new PropertyMetadata(defaultExampleHeight));
        public GridLength ExampleHeight
        {
            get { return (GridLength)GetValue(ExampleHeightProperty); }
            set { SetValue(ExampleHeightProperty, value); }
        }

        public static readonly DependencyProperty WebViewHeightProperty = DependencyProperty.Register("WebViewHeight", typeof(Int32), typeof(ControlExample), new PropertyMetadata(400));
        public Int32 WebViewHeight
        {
            get { return (Int32)GetValue(WebViewHeightProperty); }
            set { SetValue(WebViewHeightProperty, value); }
        }

        public static readonly DependencyProperty WebViewWidthProperty = DependencyProperty.Register("WebViewWidth", typeof(Int32), typeof(ControlExample), new PropertyMetadata(800));
        public Int32 WebViewWidth
        {
            get { return (Int32)GetValue(WebViewWidthProperty); }
            set { SetValue(WebViewWidthProperty, value); }
        }

        public new static readonly DependencyProperty HorizontalContentAlignmentProperty = DependencyProperty.Register("HorizontalContentAlignment", typeof(HorizontalAlignment), typeof(ControlExample), new PropertyMetadata(HorizontalAlignment.Left));
        public new HorizontalAlignment HorizontalContentAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalContentAlignmentProperty); }
            set { SetValue(HorizontalContentAlignmentProperty, value); }
        }

        public static readonly DependencyProperty MinimumUniversalAPIContractProperty = DependencyProperty.Register("MinimumUniversalAPIContract", typeof(int), typeof(ControlExample), new PropertyMetadata(null));
        public int MinimumUniversalAPIContract
        {
            get { return (int)GetValue(MinimumUniversalAPIContractProperty); }
            set { SetValue(MinimumUniversalAPIContractProperty, value); }
        }

        public ControlExample()
        {
            this.InitializeComponent();
            Substitutions = new List<ControlExampleSubstitution>();
        }

        private void rootGrid_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var substitution in Substitutions)
            {
                substitution.ValueChanged += OnValueChanged;
            }

            if (MinimumUniversalAPIContract != 0 && !(ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", (ushort)MinimumUniversalAPIContract)))
            {
                ErrorTextBlock.Visibility = Visibility.Visible;
            }
        }

        private enum SyntaxHighlightLanguage { Xml, CSharp };

        private void XamlPresenter_Loaded(object sender, RoutedEventArgs e)
        {
            GenerateSyntaxHighlightedContent(sender as ContentPresenter, Xaml as string, XamlSource, Languages.Xml);
        }

        private void CSharpPresenter_Loaded(object sender, RoutedEventArgs e)
        {
            GenerateSyntaxHighlightedContent(sender as ContentPresenter, CSharp, CSharpSource, Languages.CSharp);
        }

        private void GenerateAllSyntaxHighlightedContent()
        {
            GenerateSyntaxHighlightedContent(XamlPresenter, Xaml as string, XamlSource, Languages.Xml);
            GenerateSyntaxHighlightedContent(CSharpPresenter, CSharp, CSharpSource, Languages.CSharp);
        }

        private void GenerateSyntaxHighlightedContent(ContentPresenter presenter, string sampleString, string sampleUri, ILanguage highlightLanguage)
        {
            if (!String.IsNullOrEmpty(sampleString))
            {
                FormatAndRenderSampleFromString(sampleString, presenter, highlightLanguage);
            }
            else
            {
                FormatAndRenderSampleFromFile(sampleUri, presenter, highlightLanguage);
            }
        }

        private async void FormatAndRenderSampleFromFile(string source, ContentPresenter presenter, ILanguage highlightLanguage)
        {
            if (source != null && System.IO.Path.GetExtension(source) == ".txt")
            {
                string sampleSource = await FileLoader.LoadText(Path.Combine("ControlPagesSampleCode", source));
                FormatAndRenderSampleFromString(sampleSource, presenter, highlightLanguage);
            }
            else
            {
                presenter.Visibility = Visibility.Collapsed;
            }
        }

        private static Regex SubstitutionPattern = new Regex(@"\$\(([^\)]+)\)");
        private void FormatAndRenderSampleFromString(String sampleString, ContentPresenter presenter, ILanguage highlightLanguage)
        {
            // Trim out stray blank lines at start and end.
            sampleString = sampleString.TrimStart('\n').TrimEnd();

            // Perform any applicable substitutions.
            sampleString = SubstitutionPattern.Replace(sampleString, match =>
            {
                foreach (var substitution in Substitutions)
                {
                    if (substitution.Key == match.Groups[1].Value)
                    {
                        return substitution.ValueAsString();
                    }
                }
                throw new KeyNotFoundException(match.Groups[1].Value);
            });

            var sampleCodeRTB = new RichTextBlock();
            sampleCodeRTB.FontFamily = new FontFamily("Consolas");

            //var formatter = GenerateRichTextFormatter();
            //formatter.FormatRichTextBlock(sampleString, highlightLanguage, sampleCodeRTB);
            presenter.Content = new TextBlock() { Text = sampleString, FontFamily = new FontFamily("Consolas"), IsTextSelectionEnabled = true }; // sampleCodeRTB;
        }

        // TODO: RichTextBlockFormatter is coming from a nuget package that is built against Windows.UI.Xaml
        // Hence it cannot be used in a Microsoft.UI.Xaml project. The package is ColorCode.UWP

        //private RichTextBlockFormatter GenerateRichTextFormatter()
        //{
        //    var formatter = new RichTextBlockFormatter(App.ActualTheme);

        //    if (App.ActualTheme == ElementTheme.Dark)
        //    {
        //        UpdateFormatterDarkThemeColors(formatter);
        //    }

        //    return formatter;
        //}

        //private void UpdateFormatterDarkThemeColors(RichTextBlockFormatter formatter)
        //{
        //    // Replace the default dark theme resources with ones that more closely align to VS Code dark theme.
        //    formatter.Styles.Remove(formatter.Styles[ScopeName.XmlAttribute]);
        //    formatter.Styles.Remove(formatter.Styles[ScopeName.XmlAttributeQuotes]);
        //    formatter.Styles.Remove(formatter.Styles[ScopeName.XmlAttributeValue]);
        //    formatter.Styles.Remove(formatter.Styles[ScopeName.HtmlComment]);
        //    formatter.Styles.Remove(formatter.Styles[ScopeName.XmlDelimiter]);
        //    formatter.Styles.Remove(formatter.Styles[ScopeName.XmlName]);

        //    formatter.Styles.Add(new ColorCode.Styling.Style(ScopeName.XmlAttribute)
        //    {
        //        Foreground = "#FF87CEFA",
        //        ReferenceName = "xmlAttribute"
        //    });
        //    formatter.Styles.Add(new ColorCode.Styling.Style(ScopeName.XmlAttributeQuotes)
        //    {
        //        Foreground = "#FFFFA07A",
        //        ReferenceName = "xmlAttributeQuotes"
        //    });
        //    formatter.Styles.Add(new ColorCode.Styling.Style(ScopeName.XmlAttributeValue)
        //    {
        //        Foreground = "#FFFFA07A",
        //        ReferenceName = "xmlAttributeValue"
        //    });
        //    formatter.Styles.Add(new ColorCode.Styling.Style(ScopeName.HtmlComment)
        //    {
        //        Foreground = "#FF6B8E23",
        //        ReferenceName = "htmlComment"
        //    });
        //    formatter.Styles.Add(new ColorCode.Styling.Style(ScopeName.XmlDelimiter)
        //    {
        //        Foreground = "#FF808080",
        //        ReferenceName = "xmlDelimiter"
        //    });
        //    formatter.Styles.Add(new ColorCode.Styling.Style(ScopeName.XmlName)
        //    {
        //        Foreground = "#FF5F82E8",
        //        ReferenceName = "xmlName"
        //    });
        //}

        private void SampleCode_ActualThemeChanged(FrameworkElement sender, object args)
        {
            // If the theme has changed after the user has already opened the app (ie. via settings), then the new locally set theme will overwrite the colors that are set during Loaded.
            // Therefore we need to re-format the REB to use the correct colors. 

            GenerateAllSyntaxHighlightedContent();
        }

        private void OnValueChanged(ControlExampleSubstitution sender, object e)
        {
            GenerateAllSyntaxHighlightedContent();
        }
    }
}