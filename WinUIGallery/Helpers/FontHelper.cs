// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Media;
using System.Collections.Generic;

namespace WinUIGallery.Helpers;
public partial class FontHelper
{
    public static List<FontItem> Fonts { get; } = new()
    {
        new FontItem("Arial", new FontFamily("Arial")),
        new FontItem("Comic Sans MS", new FontFamily("Comic Sans MS")),
        new FontItem("Courier New", new FontFamily("Courier New")),
        new FontItem("Segoe UI", new FontFamily("Segoe UI")),
        new FontItem("Times New Roman", new FontFamily("Times New Roman"))
    };
}
public partial class FontItem
{
    public string Name { get; set; }
    public FontFamily Font { get; set; }

    public FontItem(string name, FontFamily font)
    {
        Name = name;
        Font = font;
    }
}