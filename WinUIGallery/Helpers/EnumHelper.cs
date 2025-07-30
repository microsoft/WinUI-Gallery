// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Reflection;

namespace WinUIGallery.Helpers;
internal static partial class EnumHelper
{
    /// <summary>
    /// Converts a string into an enum.
    /// </summary>
    /// <typeparam name="TEnum">The output enum type.</typeparam>
    /// <param name="text">The input text.</param>
    /// <returns>The parsed enum.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the TEnum type is not a enum.</exception>
    public static TEnum GetEnum<TEnum>(string text) where TEnum : struct
    {
        if (!typeof(TEnum).GetTypeInfo().IsEnum)
        {
            throw new InvalidOperationException("Generic parameter 'TEnum' must be an enum.");
        }
        return (TEnum)Enum.Parse(typeof(TEnum), text);
    }
}
