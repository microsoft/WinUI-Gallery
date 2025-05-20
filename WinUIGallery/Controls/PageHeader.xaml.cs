// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System;
using WinUIGallery.Models;
using WinUIGallery.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Uri = System.Uri;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.Windows.AppNotifications.Builder;
using Microsoft.Windows.AppNotifications;

namespace WinUIGallery.Controls;

public sealed partial class PageHeader : UserControl
{
    public Visibility ThemeButtonVisibility
    {
        get { return (Visibility)GetValue(ThemeButtonVisibilityProperty); }
        set { SetValue(ThemeButtonVisibilityProperty, value); }
    }
    public static readonly DependencyProperty ThemeButtonVisibilityProperty =
        DependencyProperty.Register("ThemeButtonVisibility", typeof(Visibility), typeof(PageHeader), new PropertyMetadata(Visibility.Visible));

    public string PageName { get; set; }
    public Action CopyLinkAction { get; set; }
    public Action ToggleThemeAction { get; set; }

    public ControlInfoDataItem Item
    {
        get { return _item; }
        set { _item = value; }
    }

    private ControlInfoDataItem _item;

    private const int _maxRecentlyVisitedPages = 7;

    public PageHeader()
    {
        this.InitializeComponent();
        CopyLinkAction = OnCopyLink;
    }

    public void SetSamplePageSourceLinks(string BaseUri, string PageName)
    {
        // Pagetype is not null!
        // So lets generate the github links and set them!
        var pageName = PageName + ".xaml";
        PageCodeGitHubLink.NavigateUri = new Uri(BaseUri + pageName + ".cs");
        PageMarkupGitHubLink.NavigateUri = new Uri(BaseUri + pageName);
    }

    public void SetControlSourceLink(string BaseUri, string SourceLink)
    {
        if (!string.IsNullOrEmpty(SourceLink))
        {
            ControlSourcePanel.Visibility = Visibility.Visible;
            ControlSourceLink.NavigateUri = new Uri(BaseUri + SourceLink);
        }
        else
        {
            ControlSourcePanel.Visibility = Visibility.Collapsed;
        }

    }

    private void OnCopyLinkButtonClick(object sender, RoutedEventArgs e)
    {
        this.CopyLinkAction?.Invoke();

        if (ProtocolActivationClipboardHelper.ShowCopyLinkTeachingTip)
        {
            this.CopyLinkButtonTeachingTip.IsOpen = true;
        }
    }

    public void OnThemeButtonClick(object sender, RoutedEventArgs e)
    {
        ToggleThemeAction?.Invoke();
        UIHelper.AnnounceActionForAccessibility(ThemeButton, "Theme changed.", "ThemeChangedSuccessNotificationId");
    }

    private void OnCopyDontShowAgainButtonClick(TeachingTip sender, object args)
    {
        ProtocolActivationClipboardHelper.ShowCopyLinkTeachingTip = false;
        this.CopyLinkButtonTeachingTip.IsOpen = false;
    }

    private void OnCopyLink()
    {
        ProtocolActivationClipboardHelper.Copy(this.Item);
    }
    public async void OnFeedBackButtonClick(object sender, RoutedEventArgs e)
    {
         await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/microsoft/WinUI-Gallery/issues/new/choose"));
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (Item == null || (string.IsNullOrEmpty(Item.ApiNamespace) && Item.BaseClasses == null))
        {
            APIDetailsBtn.Visibility = Visibility.Collapsed;
        }
        if (Item != null)
        {
            FavoriteButton.IsChecked = StringListSettingsHelper.Contains(SettingsKeys.Favorites, Item.UniqueId);
            StringListSettingsHelper.AddItem(SettingsKeys.RecentlyVisited, Item.UniqueId, InsertPosition.First, _maxRecentlyVisitedPages);
        }
    }

    private string GetFavoriteGlyph(bool? isFavorite)
    {
        return (bool)isFavorite ? "\uE735" : "\uE734";
    }

    private string GetFavoriteToolTip(bool? isFavorite)
    {
        return (bool)isFavorite ? "Remove from favorites" : "Add to favorites";
    }

    private void FavoriteButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleButton toggleButton && Item != null)
        {
            if (toggleButton.IsChecked == true)
            {
                if (!StringListSettingsHelper.AddItem(SettingsKeys.Favorites, Item.UniqueId))
                {
                    AppNotificationManager.Default.Show(new AppNotificationBuilder()
                        .AddText("Favorites limit reached")
                        .AddText("You cannot add more favorites. Please remove some first.")
                        .BuildNotification());

                    toggleButton.IsChecked = false; // revert toggle state since add failed
                }
            }
            else
            {
                StringListSettingsHelper.RemoveItem(SettingsKeys.Favorites, Item.UniqueId);
            }
        }
    }
}
