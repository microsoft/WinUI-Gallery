// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;

namespace WinUIGallery.Models;
public partial class CategoryBase { }

public partial class Category : CategoryBase
{
    public string Name { get; set; }
    public string Tooltip { get; set; }
    public Symbol Glyph { get; set; }
}

public partial class Separator : CategoryBase { }

public partial class Header : CategoryBase
{
    public string Name { get; set; }
}
