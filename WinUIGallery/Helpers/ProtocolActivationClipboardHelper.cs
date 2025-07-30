// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Microsoft.Windows.Storage;
using WinUIGallery.Models;
using Uri = System.Uri;

namespace WinUIGallery.Helpers;

/// <summary>
/// Class providing functionality to support generating and copying protocol activation URIs.
/// </summary>
public static partial class ProtocolActivationClipboardHelper
{
    private static bool _showCopyLinkTeachingTip = true;
    private static ApplicationData appData = ApplicationData.GetDefault();

    public static bool ShowCopyLinkTeachingTip
    {
        get
        {
            if (NativeMethods.IsAppPackaged)
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
            if (NativeMethods.IsAppPackaged)
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
        var uri = new Uri($"{GetAppName()}://item/{item.UniqueId}", UriKind.Absolute);
        Copy(uri, $"{Package.Current.DisplayName} - {item.Title} Sample");
    }

    public static void Copy(ControlInfoDataGroup group)
    {
        var uri = new Uri($"{GetAppName()}://category/{group.UniqueId}", UriKind.Absolute);
        Copy(uri, $"{Package.Current.DisplayName} - {group.Title} Samples");
    }

    private static string GetAppName()
    {
#if DEBUG
        return "winui3gallerydev";
#else
        return "winui3gallery";
#endif
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
