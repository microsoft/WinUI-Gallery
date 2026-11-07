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
    public void ItemsHaveUniqueIdsNamesAndValuesInRange()
    {
        ObservableCollection<TableViewLiveItem> items = TableViewLiveItem.CreateItems();

        Assert.AreEqual(9, items.Count);
        Assert.AreEqual(items.Count, items.Select(item => item.Id).Distinct().Count());
        Assert.AreEqual(items.Count, items.Select(item => item.Name).Distinct().Count());
        Assert.IsTrue(items.All(item => item.Value is >= 0 and <= 100));
        Assert.IsTrue(items.All(item => item.Change == 0 && item.Trend == "Steady"));
        Assert.IsTrue(items.Any(item => item.Value >= 50) && items.Any(item => item.Value < 50),
            "The filter option needs values on both sides of 50.");
    }

    [TestMethod]
    public void ItemsFormATwoLevelTree()
    {
        ObservableCollection<TableViewLiveItem> items = TableViewLiveItem.CreateItems();
        Dictionary<int, TableViewLiveItem> byId = items.ToDictionary(item => item.Id);
        List<TableViewLiveItem> roots = items.Where(item => item.ParentId is null).ToList();

        Assert.AreEqual(3, roots.Count);
        foreach (TableViewLiveItem child in items.Where(item => item.ParentId is not null))
        {
            Assert.IsTrue(byId.TryGetValue(child.ParentId!.Value, out TableViewLiveItem? parent), $"{child.Name} has an unknown parent.");
            Assert.IsNull(parent!.ParentId, $"{child.Name} should be nested one level deep.");
        }

        Assert.IsTrue(roots.All(root => items.Count(item => item.ParentId == root.Id) >= 2),
            "Each top-level row needs siblings to reorder under it.");
    }

    [TestMethod]
    public void UpdateChangesValueTrendAndRaisesNotifications()
    {
        TableViewLiveItem item = new(1, null, "Alpha", 40);
        List<string?> changed = [];
        item.PropertyChanged += (_, args) => changed.Add(args.PropertyName);

        item.Update(7);

        Assert.AreEqual(47, item.Value);
        Assert.AreEqual(7, item.Change);
        Assert.AreEqual("▲ 7", item.ChangeText);
        Assert.AreEqual("Rising", item.Trend);
        Assert.AreEqual("Alpha, value 47, change +7", item.AutomationName);
        CollectionAssert.AreEquivalent(
            new[] { nameof(item.Value), nameof(item.Change), nameof(item.ChangeText), nameof(item.Trend), nameof(item.AutomationName) },
            changed);

        item.Update(-3);
        Assert.AreEqual("Falling", item.Trend);
    }

    [TestMethod]
    public void UpdateClampsToRangeAndReportsActualChange()
    {
        TableViewLiveItem high = new(1, null, "High", 95);
        high.Update(15);
        Assert.AreEqual(100, high.Value);
        Assert.AreEqual(5, high.Change);

        TableViewLiveItem low = new(2, null, "Low", 4);
        low.Update(-15);
        Assert.AreEqual(0, low.Value);
        Assert.AreEqual(-4, low.Change);
        Assert.AreEqual("▼ 4", low.ChangeText);

        low.Update(-3);
        Assert.AreEqual(0, low.Change);
        Assert.AreEqual("–", low.ChangeText);
        Assert.AreEqual("Steady", low.Trend);
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
