// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Linq;

namespace WinUIGallery.Samples.ControlPages.Fundamentals.Controls;

public sealed class ValidatedPasswordBox : Control
{
    public static readonly DependencyProperty PasswordProperty =
        DependencyProperty.Register(nameof(Password), typeof(string), typeof(ValidatedPasswordBox), new PropertyMetadata(string.Empty, OnPasswordChanged));

    public static readonly DependencyProperty IsValidProperty =
        DependencyProperty.Register(nameof(IsValid), typeof(bool), typeof(ValidatedPasswordBox), new PropertyMetadata(false));

    public static readonly DependencyProperty MinLengthProperty =
        DependencyProperty.Register(nameof(MinLength), typeof(int), typeof(ValidatedPasswordBox), new PropertyMetadata(8, OnPasswordChanged));

    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(nameof(Header), typeof(string), typeof(ValidatedPasswordBox), new PropertyMetadata(string.Empty, OnPasswordChanged));

    public static readonly DependencyProperty PlaceholderTextProperty =
        DependencyProperty.Register(nameof(PlaceholderText), typeof(string), typeof(ValidatedPasswordBox), new PropertyMetadata(string.Empty, OnPasswordChanged));

    public ValidatedPasswordBox()
    {
        this.DefaultStyleKey = typeof(ValidatedPasswordBox);
    }

    public string Password
    {
        get => (string)GetValue(PasswordProperty);
        set => SetValue(PasswordProperty, value);
    }

    public bool IsValid
    {
        get => (bool)GetValue(IsValidProperty);
        set => SetValue(IsValidProperty, value);
    }

    public int MinLength
    {
        get => (int)GetValue(MinLengthProperty);
        set => SetValue(MinLengthProperty, value);
    }

    public string Header
    {
        get => (string)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public string PlaceholderText
    {
        get => (string)GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    private PasswordBox PasswordInput { get; set; }
    private StackPanel MissingUppercaseText { get; set; }
    private StackPanel MissingNumberText { get; set; }
    private StackPanel TooShortText { get; set; }
    private StackPanel ValidPasswordText { get; set; }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        PasswordInput = GetTemplateChild(nameof(PasswordInput)) as PasswordBox;
        if (PasswordInput != null)
        {
            PasswordInput.Header = Header;
            PasswordInput.PlaceholderText = PlaceholderText;
        }

        MissingUppercaseText = GetTemplateChild(nameof(MissingUppercaseText)) as StackPanel;
        MissingNumberText = GetTemplateChild(nameof(MissingNumberText)) as StackPanel;
        TooShortText = GetTemplateChild(nameof(TooShortText)) as StackPanel;
        ValidPasswordText = GetTemplateChild(nameof(ValidPasswordText)) as StackPanel;

        if (PasswordInput is not null)
        {
            PasswordInput.PasswordChanged += (sender, e) =>
            {
                Password = PasswordInput.Password;
                UpdateValidationMessages();
            };
        }

        UpdateValidationMessages();
    }

    private static void OnPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (ValidatedPasswordBox)d;
        control.UpdateValidationMessages();
    }

    private void UpdateValidationMessages()
    {
        bool hasMinLength = Password.Length >= MinLength;
        bool hasUppercase = Password.Any(char.IsUpper);
        bool hasNumber = Password.Any(char.IsDigit);

        IsValid = hasMinLength && hasUppercase && hasNumber;

        if (MissingUppercaseText is not null)
        {
            MissingUppercaseText.Visibility = (hasUppercase || string.IsNullOrEmpty(Password)) ? Visibility.Collapsed : Visibility.Visible;
        }

        if (MissingNumberText is not null)
        {
            MissingNumberText.Visibility = (hasNumber || string.IsNullOrEmpty(Password)) ? Visibility.Collapsed : Visibility.Visible;
        }

        if (TooShortText is not null)
        {
            TooShortText.Visibility = (hasMinLength || string.IsNullOrEmpty(Password)) ? Visibility.Collapsed : Visibility.Visible;
        }

        if (ValidPasswordText is not null)
        {
            ValidPasswordText.Visibility = IsValid ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}

