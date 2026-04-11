// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using Windows.Foundation.Metadata;
using WinUIGallery.ControlPages;

namespace WinUIGallery.SamplePages;

public sealed partial class CardPage : Page
{
    CustomDataObject? _storedItem;

    public CardPage()
    {
        this.InitializeComponent();
        collection.ItemsSource = CustomDataObject.GetDataObjects(includeAllItems: true);
    }

    private async void BackButton_Click(object sender, RoutedEventArgs e)
    {
        ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("backwardsAnimation", destinationElement);
        SmokeGrid.Children.Remove(destinationElement);

        // Collapse the smoke when the animation completes.
        animation.Completed += Animation_Completed;

        // If the connected item appears outside the viewport, scroll it into view.
        collection.ScrollIntoView(_storedItem, ScrollIntoViewAlignment.Default);
        collection.UpdateLayout();

        // Use the Direct configuration to go back (if the API is available).
        if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
        {
            animation.Configuration = new DirectConnectedAnimationConfiguration();
        }

        // Play the second connected animation.
        await collection.TryStartConnectedAnimationAsync(animation, _storedItem, "connectedElement");
    }

    private void Animation_Completed(ConnectedAnimation sender, object args)
    {
        SmokeGrid.Visibility = Visibility.Collapsed;
        SmokeGrid.Children.Add(destinationElement);
    }

    private void TipsGrid_ItemClick(object sender, ItemClickEventArgs e)
    {
        ConnectedAnimation? animation = null;

        if (collection.ContainerFromItem(e.ClickedItem) is GridViewItem container)
        {
            _storedItem = container.Content as CustomDataObject;

            animation = collection.PrepareConnectedAnimation("forwardAnimation", _storedItem, "connectedElement");
        }

        // Update the detail view with the clicked item's data.
        if (_storedItem != null)
        {
            detailImage.Source = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(new System.Uri("ms-appx://" + _storedItem.ImageLocation));
            detailTitle.Text = _storedItem.Title;
            detailDescription.Text = _storedItem.Description;
        }

        SmokeGrid.Visibility = Visibility.Visible;

        animation?.TryStart(destinationElement);
    }
}
