// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using WinUIGallery.Helper;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;

namespace WinUIGallery.DesktopWap.Controls
{
    public sealed partial class CopyButton : Button
    {
        public CopyButton()
        {
            this.DefaultStyleKey = typeof(CopyButton);
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            if (GetTemplateChild("CopyToClipboardSuccessAnimation") is Storyboard _storyBoard)
            {
                _storyBoard.Begin();
                UIHelper.AnnounceActionForAccessibility(this, "Copied to clipboard", "CopiedToClipboardActivityId");
            }
        }

        protected override void OnApplyTemplate()
        {
            Click -= CopyButton_Click;
            base.OnApplyTemplate();
            Click += CopyButton_Click;
        }
    }
}
