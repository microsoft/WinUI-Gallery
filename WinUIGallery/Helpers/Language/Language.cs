// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace WinUIGallery.Helpers;
public partial class Language
{
    public string Name { get; set; }
    public string Code { get; set; }

    public Language(string name, string code)
    {
        Name = name;
        Code = code;
    }
}