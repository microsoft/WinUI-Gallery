// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using WinUIGallery.Models;
using WinUIGallery.Pages;

namespace WinUIGallery.Telemetry;

internal static class PageViewResolver
{
    public static string? ResolveSampleId(
        Type sourcePageType,
        object? parameter,
        IEnumerable<ControlInfoDataGroup> groups)
    {
        if (sourcePageType != typeof(ItemPage) ||
            parameter is not string pageId ||
            string.IsNullOrEmpty(pageId))
        {
            return null;
        }

        int matches = groups
            .SelectMany(group => group.Items)
            .Count(item => string.Equals(item.UniqueId, pageId, StringComparison.Ordinal));

        return matches == 1 ? pageId : null;
    }
}