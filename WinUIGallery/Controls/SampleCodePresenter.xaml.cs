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
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AppUIBasics.Helper;
using ColorCode;
using ColorCode.Common;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace AppUIBasics.Controls
{
    public sealed partial class SampleCodePresenter : UserControl
    {
        public static readonly DependencyProperty CodeProperty = DependencyProperty.Register("Code", typeof(string), typeof(SampleCodePresenter), new PropertyMetadata("", OnDependencyPropertyChanged));
        public string Code
        {
            get { return (string)GetValue(CodeProperty); }
            set { SetValue(CodeProperty, value); }
        }

        public static readonly DependencyProperty CodeSourceFileProperty = DependencyProperty.Register("CodeSourceFile", typeof(object), typeof(SampleCodePresenter), new PropertyMetadata(null, OnDependencyPropertyChanged));
        public Uri CodeSourceFile
        {
            get { return (Uri)GetValue(CodeSourceFileProperty); }
            set { SetValue(CodeSourceFileProperty, value); }
        }

        public static readonly DependencyProperty IsCSharpSampleProperty = DependencyProperty.Register("IsCSharpSample", typeof(bool), typeof(SampleCodePresenter), new PropertyMetadata(false));
        public bool IsCSharpSample
        {
            get { return (bool)GetValue(IsCSharpSampleProperty); }
            set { SetValue(IsCSharpSampleProperty, value); }
        }

        public static readonly DependencyProperty SubstitutionsProperty = DependencyProperty.Register("Substitutions", typeof(IList<ControlExampleSubstitution>), typeof(ControlExample), new PropertyMetadata(null));
        public IList<ControlExampleSubstitution> Substitutions
        {
            get { return (IList<ControlExampleSubstitution>)GetValue(SubstitutionsProperty); }
            set { SetValue(SubstitutionsProperty, value); }
        }

        public bool IsEmpty => Code.Length == 0 && CodeSourceFile == null;

        private string actualCode = "";
        private static Regex SubstitutionPattern = new Regex(@"\$\(([^\)]+)\)");

        public SampleCodePresenter()
        {
            this.InitializeComponent();
        }

        private static void OnDependencyPropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            if (target is SampleCodePresenter presenter)
            {
                presenter.ReevaluateVisibility();
            }
        }

        private void ReevaluateVisibility()
        {
            if (Code.Length == 0 && CodeSourceFile == null)
            {
                Visibility = Visibility.Collapsed;
            }
            else
            {
                Visibility = Visibility.Visible;
            }
        }

        private void SampleCodePresenter_Loaded(object sender, RoutedEventArgs e)
        {
            ReevaluateVisibility();
            VisualStateManager.GoToState(this, IsCSharpSample ? "CSharpSample" : "XAMLSample", false);
            foreach (var substitution in Substitutions)
            {
                substitution.ValueChanged += OnValueChanged;
            }
        }

        private void CodePresenter_Loaded(object sender, RoutedEventArgs e)
        {
            GenerateSyntaxHighlightedContent();
        }

        private void SampleCodePresenter_ActualThemeChanged(FrameworkElement sender, object args)
        {
            // If the theme has changed after the user has already opened the app (ie. via settings), then the new locally set theme will overwrite the colors that are set during Loaded.
            // Therefore we need to re-format the REB to use the correct colors.
            GenerateSyntaxHighlightedContent();
        }

        private void OnValueChanged(ControlExampleSubstitution sender, object e)
        {
            GenerateSyntaxHighlightedContent();
        }

        private Uri GetDerivedSource(Uri rawSource)
        {
            // Get the full path of the source string
            string concatString = "";
            for (int i = 2; i < rawSource.Segments.Length; i++)
            {
                concatString += rawSource.Segments[i];
            }
            Uri derivedSource = new Uri(new Uri("ms-appx:///ControlPagesSampleCode/"), concatString);

            return derivedSource;
        }

        private void GenerateSyntaxHighlightedContent()
        {
            if (!string.IsNullOrEmpty(Code))
            {
                FormatAndRenderSampleFromString(Code, CodePresenter, IsCSharpSample ? Languages.CSharp : Languages.Xml);
            }
            else
            {
                FormatAndRenderSampleFromFile(CodeSourceFile, CodePresenter, IsCSharpSample ? Languages.CSharp : Languages.Xml);
            }
        }

        private async void FormatAndRenderSampleFromFile(Uri source, ContentPresenter presenter, ILanguage highlightLanguage)
        {
            if (source != null && source.AbsolutePath.EndsWith("txt"))
            {
                Uri derivedSource = GetDerivedSource(source);
                var file = await StorageFile.GetFileFromApplicationUriAsync(derivedSource);
                string sampleString = await FileIO.ReadTextAsync(file);

                FormatAndRenderSampleFromString(sampleString, presenter, highlightLanguage);
            }
            else
            {
                presenter.Visibility = Visibility.Collapsed;
            }
        }

        private void FormatAndRenderSampleFromString(string sampleString, ContentPresenter presenter, ILanguage highlightLanguage)
        {
            // Trim out stray blank lines at start and end.
            sampleString = sampleString.TrimStart('\n').TrimEnd();

            // Also trim out spaces at the end of each line
            sampleString = string.Join('\n', sampleString.Split('\n').Select(s => s.TrimEnd()));

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

            actualCode = sampleString;

            var sampleCodeRTB = new RichTextBlock { FontFamily = new FontFamily("Consolas") };

            //var formatter = GenerateRichTextFormatter();
            //formatter.FormatRichTextBlock(sampleString, highlightLanguage, sampleCodeRTB);
            presenter.Content = new TextBlock() { Text = sampleString, FontFamily = new FontFamily("Consolas"), IsTextSelectionEnabled = true }; // sampleCodeRTB;
        }

        //private RichTextBlockFormatter GenerateRichTextFormatter()
        //{
        //    var formatter = new RichTextBlockFormatter(ThemeHelper.ActualTheme);

        //    if (ThemeHelper.ActualTheme == ElementTheme.Dark)
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

        private void CopyCodeButton_Click(object sender, RoutedEventArgs e)
        {
            DataPackage package = new DataPackage();
            package.SetText(actualCode);
            Clipboard.SetContent(package);

            VisualStateManager.GoToState(this, "ConfirmationDialogVisible", false);
            Microsoft.UI.Dispatching.DispatcherQueue dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();

            // Automatically close teachingtip after 1 seconds
            if (dispatcherQueue != null)
            {
                dispatcherQueue.TryEnqueue(async () =>
                {
                    await Task.Delay(1000);
                    VisualStateManager.GoToState(this, "ConfirmationDialogHidden", false);
                });
            }
        }
    }
}
