// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WinUIGallery.ControlPages;

namespace WinUIGallery.UnitTests;

[TestClass]
public class TableViewLiveItemTests
{
    [TestMethod]
    public void ItemsHaveUniqueNamesAndValuesInRange()
    {
        ObservableCollection<TableViewLiveItem> items = TableViewLiveItem.CreateItems();

        Assert.AreEqual(8, items.Count);
        Assert.AreEqual(items.Count, items.Select(item => item.Name).Distinct().Count());
        Assert.IsTrue(items.All(item => item.Value is >= 0 and <= 100));
        Assert.IsTrue(items.All(item => item.Change == 0));
        Assert.IsTrue(items.Any(item => item.Value >= 50) && items.Any(item => item.Value < 50),
            "The filter option needs values on both sides of 50.");
    }

    [TestMethod]
    public void UpdateChangesValueAndRaisesNotifications()
    {
        TableViewLiveItem item = new("Alpha", 40);
        List<string?> changed = [];
        item.PropertyChanged += (_, args) => changed.Add(args.PropertyName);

        item.Update(7);

        Assert.AreEqual(47, item.Value);
        Assert.AreEqual(7, item.Change);
        Assert.AreEqual("▲ 7", item.ChangeText);
        Assert.AreEqual("Alpha, value 47, change +7", item.AutomationName);
        CollectionAssert.AreEquivalent(
            new[] { nameof(item.Value), nameof(item.Change), nameof(item.ChangeText), nameof(item.AutomationName) },
            changed);
    }

    [TestMethod]
    public void UpdateClampsToRangeAndReportsActualChange()
    {
        TableViewLiveItem high = new("High", 95);
        high.Update(15);
        Assert.AreEqual(100, high.Value);
        Assert.AreEqual(5, high.Change);

        TableViewLiveItem low = new("Low", 4);
        low.Update(-15);
        Assert.AreEqual(0, low.Value);
        Assert.AreEqual(-4, low.Change);
        Assert.AreEqual("▼ 4", low.ChangeText);

        low.Update(-3);
        Assert.AreEqual(0, low.Change);
        Assert.AreEqual("–", low.ChangeText);
    }

    [TestMethod]
    public void CollectionsAreIndependent()
    {
        ObservableCollection<TableViewLiveItem> first = TableViewLiveItem.CreateItems();
        ObservableCollection<TableViewLiveItem> second = TableViewLiveItem.CreateItems();

        first[0].Update(10);

        Assert.AreNotSame(first[0], second[0]);
        Assert.AreEqual(72, second[0].Value);
    }
}
