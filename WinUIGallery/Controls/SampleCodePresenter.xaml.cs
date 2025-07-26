// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using ColorCode;
using ColorCode.Common;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using WinUIGallery.Helpers;

namespace WinUIGallery.Controls;

public enum SampleCodePresenterType
{
    XAML,
    CSharp,
    Inline
}

public sealed partial class SampleCodePresenter : UserControl
{
    public static readonly DependencyProperty CodeProperty = DependencyProperty.Register("Code", typeof(string), typeof(SampleCodePresenter), new PropertyMetadata("", OnDependencyPropertyChanged));
    public string Code
    {
        get { return (string)GetValue(CodeProperty); }
        set { SetValue(CodeProperty, value); }
    }

    public static readonly DependencyProperty CodeSourceFileProperty = DependencyProperty.Register("CodeSourceFile", typeof(object), typeof(SampleCodePresenter), new PropertyMetadata(null, OnDependencyPropertyChanged));
    public string CodeSourceFile
    {
        get { return (string)GetValue(CodeSourceFileProperty); }
        set { SetValue(CodeSourceFileProperty, value); }
    }

    public static readonly DependencyProperty SampleTypeProperty = DependencyProperty.Register("SampleType", typeof(SampleCodePresenterType), typeof(SampleCodePresenter), new PropertyMetadata(SampleCodePresenterType.XAML));
    public SampleCodePresenterType SampleType
    {
        get { return (SampleCodePresenterType)GetValue(SampleTypeProperty); }
        set { SetValue(SampleTypeProperty, value); }
    }

    public static readonly DependencyProperty SubstitutionsProperty = DependencyProperty.Register("Substitutions", typeof(IList<ControlExampleSubstitution>), typeof(ControlExample), new PropertyMetadata(null));
    public IList<ControlExampleSubstitution> Substitutions
    {
        get { return (IList<ControlExampleSubstitution>)GetValue(SubstitutionsProperty); }
        set { SetValue(SubstitutionsProperty, value); }
    }

    public bool IsEmpty => string.IsNullOrEmpty(Code) && string.IsNullOrEmpty(CodeSourceFile);

    private string actualCode = "";
    private static Regex SubstitutionPattern = new Regex(@"\$\(([^\)]+)\)");
    private RichTextBlock sampleCodeRTB;

    public SampleCodePresenter()
    {
        this.InitializeComponent();
    }

    private static void OnDependencyPropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
    {
        if (target is SampleCodePresenter presenter)
        {
            presenter.ReevaluateVisibility();
            presenter.GenerateSyntaxHighlightedContent();
        }
    }

    private void ReevaluateVisibility()
    {
        if (IsEmpty)
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
        VisualStateManager.GoToState(this, GetSampleLanguageVisualState(), false);
        if (Substitutions != null)
        {
            foreach (var substitution in Substitutions)
            {
                substitution.ValueChanged += OnValueChanged;
            }
        }
    }

    private string GetSampleLanguageVisualState()
    {
        switch (SampleType)
        {
            case SampleCodePresenterType.XAML:
                return "XAMLSample";
            case SampleCodePresenterType.CSharp:
                return "CSharpSample";
            default:
                return "InlineSample";
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

    private Uri GetDerivedSource(string sourceRelativePath)
    {
        Uri derivedSource = new Uri(new Uri("ms-appx:///Samples/SampleCode/"), sourceRelativePath);

        return derivedSource;
    }

    private string GetDerivedSourceUnpackaged(string sourceRelativePath)
    {
        string derviedSourceString = "Samples\\SampleCode\\" + sourceRelativePath;
        return derviedSourceString;
    }

    private void GenerateSyntaxHighlightedContent()
    {
        var language = SampleType switch
        {
            SampleCodePresenterType.XAML => Languages.Xml,
            SampleCodePresenterType.CSharp => Languages.CSharp,
            _ => Languages.Markdown
        };
        if (!string.IsNullOrEmpty(Code))
        {
            FormatAndRenderSampleFromString(Code, CodePresenter, language);
        }
        else
        {
            FormatAndRenderSampleFromFile(CodeSourceFile, CodePresenter, language);
        }
    }

    private async void FormatAndRenderSampleFromFile(string sourceRelativePath, ContentPresenter presenter, ILanguage highlightLanguage)
    {
        if (sourceRelativePath != null && sourceRelativePath.EndsWith("txt"))
        {
            string sampleString = null;
            StorageFile file = null;
            if (!NativeMethods.IsAppPackaged)
            {
                var relativePath = GetDerivedSourceUnpackaged(sourceRelativePath);

                var sourcePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, relativePath));

                file = await StorageFile.GetFileFromPathAsync(sourcePath);
            }
            else
            {
                Uri derivedSource = GetDerivedSource(sourceRelativePath);
                file = await StorageFile.GetFileFromApplicationUriAsync(derivedSource);
            }

            sampleString = await FileIO.ReadTextAsync(file);

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

        if (Substitutions != null)
        {
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
        }

        actualCode = sampleString;

        var name = GetSampleLanguageVisualState() == "InlineSample" ? actualCode : SampleType.ToString();
        var automationName = "Copy " + name + " Code";
        AutomationProperties.SetName(CopyCodeButton, automationName);

        var formatter = GenerateRichTextFormatter();
        if (SampleType == SampleCodePresenterType.Inline)
        {
            CodeScrollViewer.Content = new TextBlock() { FontFamily = new FontFamily("Consolas, Cascadia Code"), Text = actualCode, IsTextSelectionEnabled = true, TextTrimming = TextTrimming.CharacterEllipsis };
            CodeScrollViewer.UpdateLayout();
        }
        else
        {
            sampleCodeRTB = new RichTextBlock { FontFamily = new FontFamily("Consolas, Cascadia Code") };
            formatter.FormatRichTextBlock(sampleString, highlightLanguage, sampleCodeRTB);
            CodePresenter.Content = sampleCodeRTB;
            CodeScrollViewer.Content = CodePresenter;
            sampleCodeRTB.SelectionChanged += SampleCodeRTB_SelectionChanged;
        }
    }

    private void SampleCodeRTB_SelectionChanged(object sender, RoutedEventArgs e)
    {
        if (sender is RichTextBlock sampleCode)
        {
            if (!string.IsNullOrEmpty(sampleCode.SelectedText))
            {
                CopyButtonBorder.Visibility = Visibility.Collapsed;
            }
            else
            {
                CopyButtonBorder.Visibility = Visibility.Visible;
            }
        }
    }

    private RichTextBlockFormatter GenerateRichTextFormatter()
    {
        var formatter = new RichTextBlockFormatter(ActualTheme);

        if (ActualTheme == ElementTheme.Dark)
        {
            UpdateFormatterDarkThemeColors(formatter);
        }

        return formatter;
    }

    private void UpdateFormatterDarkThemeColors(RichTextBlockFormatter formatter)
    {
        // Replace the default dark theme resources with ones that more closely align to VS Code dark theme.
        formatter.Styles.Remove(formatter.Styles[ScopeName.XmlAttribute]);
        formatter.Styles.Remove(formatter.Styles[ScopeName.XmlAttributeQuotes]);
        formatter.Styles.Remove(formatter.Styles[ScopeName.XmlAttributeValue]);
        formatter.Styles.Remove(formatter.Styles[ScopeName.HtmlComment]);
        formatter.Styles.Remove(formatter.Styles[ScopeName.XmlDelimiter]);
        formatter.Styles.Remove(formatter.Styles[ScopeName.XmlName]);

        formatter.Styles.Add(new ColorCode.Styling.Style(ScopeName.XmlAttribute)
        {
            Foreground = "#FF87CEFA",
            ReferenceName = "xmlAttribute"
        });
        formatter.Styles.Add(new ColorCode.Styling.Style(ScopeName.XmlAttributeQuotes)
        {
            Foreground = "#FFFFA07A",
            ReferenceName = "xmlAttributeQuotes"
        });
        formatter.Styles.Add(new ColorCode.Styling.Style(ScopeName.XmlAttributeValue)
        {
            Foreground = "#FFFFA07A",
            ReferenceName = "xmlAttributeValue"
        });
        formatter.Styles.Add(new ColorCode.Styling.Style(ScopeName.HtmlComment)
        {
            Foreground = "#FF6B8E23",
            ReferenceName = "htmlComment"
        });
        formatter.Styles.Add(new ColorCode.Styling.Style(ScopeName.XmlDelimiter)
        {
            Foreground = "#FF808080",
            ReferenceName = "xmlDelimiter"
        });
        formatter.Styles.Add(new ColorCode.Styling.Style(ScopeName.XmlName)
        {
            Foreground = "#FF5F82E8",
            ReferenceName = "xmlName"
        });
    }

    private void CopyCodeButton_Click(object sender, RoutedEventArgs e)
    {
        DataPackage package = new DataPackage();
        package.SetText(actualCode);
        Clipboard.SetContent(package);
    }

    private void CodeScrollViewer_Loaded(object sender, RoutedEventArgs e)
    {
        ScrollBar horizontalScrollBar = FindHorizontalScrollBar(CodeScrollViewer);
        if (horizontalScrollBar != null)
        {
            // Create a timer and store it in the ScrollBar's Tag property.
            DispatcherTimer scrollTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            scrollTimer.Tick += (s, args) =>
            {
                scrollTimer.Stop();
                if (sampleCodeRTB != null && string.IsNullOrEmpty(sampleCodeRTB.SelectedText))
                {
                    CopyButtonBorder.Visibility = Visibility.Visible;
                }
            };
            horizontalScrollBar.Tag = scrollTimer;

            horizontalScrollBar.Scroll += HorizontalScrollBar_Scroll;
        }
    }

    private void HorizontalScrollBar_Scroll(object sender, ScrollEventArgs e)
    {
        if (sender is ScrollBar scrollBar)
        {
            // Immediately collapse the button when scrolling starts.
            CopyButtonBorder.Visibility = Visibility.Collapsed;

            // Retrieve the timer from the ScrollBar's Tag.
            if (scrollBar.Tag is DispatcherTimer scrollTimer)
            {
                // Restart the timer so that it ticks only after scrolling stops.
                if (scrollTimer.IsEnabled)
                {
                    scrollTimer.Stop();
                }
                scrollTimer.Start();
            }
        }
    }

    private ScrollBar FindHorizontalScrollBar(DependencyObject element)
    {
        if (element is ScrollBar sb && sb.Orientation == Orientation.Horizontal)
        {
            return sb;
        }

        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
        {
            var child = VisualTreeHelper.GetChild(element, i);
            var result = FindHorizontalScrollBar(child);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }
}
