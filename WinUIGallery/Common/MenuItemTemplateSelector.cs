using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using System;

namespace WinUIGallery.Common
{

    [ContentProperty(Name = "ItemTemplate")]
    partial class MenuItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ItemTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            return item is Separator ? SeparatorTemplate : item is Header ? HeaderTemplate : ItemTemplate;
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return item is Separator ? SeparatorTemplate : item is Header ? HeaderTemplate : ItemTemplate;
        }

        internal DataTemplate HeaderTemplate = (DataTemplate)XamlReader.Load(
            @"<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
                   <NavigationViewItemHeader Content='{Binding Name}' />
                  </DataTemplate>");

        internal DataTemplate SeparatorTemplate = (DataTemplate)XamlReader.Load(
            @"<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
                    <NavigationViewItemSeparator />
                  </DataTemplate>");
    }
}
