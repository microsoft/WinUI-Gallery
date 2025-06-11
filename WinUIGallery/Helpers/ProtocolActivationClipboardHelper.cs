using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Microsoft.Windows.Storage;
using WinUIGallery.Models;

namespace WinUIGallery.Helpers;

/// <summary>
/// Class providing functionality to support generating and copying protocol activation URIs.
/// </summary>
public static class ProtocolActivationClipboardHelper
{
    private static bool _showCopyLinkTeachingTip = true;
    private static ApplicationData appData = ApplicationData.GetDefault();

    public static bool ShowCopyLinkTeachingTip
    {
        get
        {
            if (NativeHelper.IsAppPackaged)
            {
                object valueFromSettings = appData.LocalSettings.Values[SettingsKeys.ShowCopyLinkTeachingTip];
                if (valueFromSettings == null)
                {
                    appData.LocalSettings.Values[SettingsKeys.ShowCopyLinkTeachingTip] = true;
                    valueFromSettings = true;
                }
                return (bool)valueFromSettings;
            }
            else
            {
                return _showCopyLinkTeachingTip;
            }
        }

        set
        {
            if (NativeHelper.IsAppPackaged)
            {
                appData.LocalSettings.Values[SettingsKeys.ShowCopyLinkTeachingTip] = value;

            }
            else
            {
                _showCopyLinkTeachingTip = value;
            }
        }
    }

    public static void Copy(ControlInfoDataItem item)
    {
        var uri = new Uri($"winui3gallery://item/{item.UniqueId}", UriKind.Absolute);
        ProtocolActivationClipboardHelper.Copy(uri, $"{Package.Current.DisplayName} - {item.Title} Sample");
    }

    public static void Copy(ControlInfoDataGroup group)
    {
        var uri = new Uri($"winui3gallery://category/{group.UniqueId}", UriKind.Absolute);
        ProtocolActivationClipboardHelper.Copy(uri, $"{Package.Current.DisplayName} - {group.Title} Samples");
    }

    private static void Copy(Uri uri, string displayName)
    {
        string htmlFormat = HtmlFormatHelper.CreateHtmlFormat($"<a href='{uri}'>{displayName}</a>");

        var dataPackage = new DataPackage();
        dataPackage.SetApplicationLink(uri);
        dataPackage.SetWebLink(uri);
        dataPackage.SetText(uri.ToString());
        dataPackage.SetHtmlFormat(htmlFormat);

        Clipboard.SetContentWithOptions(dataPackage, null);
    }
}
