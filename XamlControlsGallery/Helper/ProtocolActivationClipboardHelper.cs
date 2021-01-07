using System;
using AppUIBasics.Data;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;

namespace AppUIBasics.Helper
{
    /// <summary>
    /// Class providing functionality to support generating and copying protocol activation URIs.
    /// </summary>
    public static class ProtocolActivationClipboardHelper
    {
        public static void Copy(ControlInfoDataItem item)
        {
            var uri = new Uri($"xamlcontrolsgallery://item/{item.UniqueId}", UriKind.Absolute);
            ProtocolActivationClipboardHelper.Copy(uri, $"{Package.Current.DisplayName} - {item.Title} Sample");
        }

        public static void Copy(ControlInfoDataGroup group)
        {
            var uri = new Uri($"xamlcontrolsgallery://category/{group.UniqueId}", UriKind.Absolute);
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
}
