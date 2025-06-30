// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;

namespace WinUIGallery.Models;
public class CategoryBase { }

public class Category : CategoryBase
{
    public string Name { get; set; }
    public string Tooltip { get; set; }
    public Symbol Glyph { get; set; }
}

public class Separator : CategoryBase { }

public class Header : CategoryBase
{
    public string Name { get; set; }
}
