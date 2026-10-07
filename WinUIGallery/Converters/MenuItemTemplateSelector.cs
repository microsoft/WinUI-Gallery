// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;
using WinUIGallery.Models;

namespace WinUIGallery.Converters;


[ContentProperty(Name = "ItemTemplate")]
partial class MenuItemTemplateSelector : DataTemplateSelector
{
    public DataTemplate? ItemTemplate { get; set; }

    protected override DataTemplate? SelectTemplateCore(object item)
    {
        return item is Separator ? SeparatorTemplate : item is Header ? HeaderTemplate : ItemTemplate;
    }

    protected override DataTemplate? SelectTemplateCore(object item, DependencyObject container)
    {
        return item is Separator ? SeparatorTemplate : item is Header ? HeaderTemplate : ItemTemplate;
    }

    internal DataTemplate HeaderTemplate = new DataTemplate(() =>
    {
        NavigationViewItemHeader header = new NavigationViewItemHeader();
        header.SetBinding(ContentControl.ContentProperty, new Binding { Path = new PropertyPath(nameof(Header.Name)) });
        return header;
    });

    internal DataTemplate SeparatorTemplate = new DataTemplate(() => new NavigationViewItemSeparator());
}
