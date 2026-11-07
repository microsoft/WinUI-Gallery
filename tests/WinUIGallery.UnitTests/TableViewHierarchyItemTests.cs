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
public class TableViewHierarchyItemTests
{
    [TestMethod]
    public void ItemsHaveUniqueIdsAndKnownParents()
    {
        ObservableCollection<TableViewHierarchyItem> items = TableViewHierarchyItem.CreateItems();
        HashSet<int> ids = items.Select(item => item.Id).ToHashSet();

        Assert.AreEqual(items.Count, ids.Count);
        foreach (TableViewHierarchyItem item in items)
        {
            if (item.ParentId is int parentId)
            {
                Assert.IsTrue(ids.Contains(parentId), $"{item.Name} has an unknown parent {parentId}.");
                Assert.AreEqual("Folder", items.Single(parent => parent.Id == parentId).Kind);
            }
        }
    }

    [TestMethod]
    public void ItemsFormAnAcyclicTreeWithNestedLevels()
    {
        ObservableCollection<TableViewHierarchyItem> items = TableViewHierarchyItem.CreateItems();
        Dictionary<int, int?> parentOf = items.ToDictionary(item => item.Id, item => item.ParentId);
        int maxDepth = 0;

        foreach (TableViewHierarchyItem item in items)
        {
            int depth = 0;
            for (int? current = item.ParentId; current is int id; current = parentOf[id])
            {
                depth++;
                Assert.IsTrue(depth < items.Count, $"{item.Name} is part of a parent cycle.");
            }

            maxDepth = System.Math.Max(maxDepth, depth);
        }

        Assert.IsTrue(items.Count(item => item.ParentId is null) > 1, "The sample needs more than one top-level row.");
        Assert.AreEqual(2, maxDepth, "The sample shows three levels of rows.");
    }

    [TestMethod]
    public void CollectionsAreIndependent()
    {
        ObservableCollection<TableViewHierarchyItem> first = TableViewHierarchyItem.CreateItems();
        ObservableCollection<TableViewHierarchyItem> second = TableViewHierarchyItem.CreateItems();

        first.Clear();

        Assert.AreNotSame(first, second);
        Assert.AreEqual(13, second.Count);
    }

    [TestMethod]
    public void AutomationNameDescribesRowAndOmitsEmptySize()
    {
        ObservableCollection<TableViewHierarchyItem> items = TableViewHierarchyItem.CreateItems();

        Assert.AreEqual("Documents, Folder", items.Single(item => item.Name == "Documents").AutomationName);
        Assert.AreEqual("Beach.jpg, Image, 2.4 MB", items.Single(item => item.Name == "Beach.jpg").AutomationName);
    }
}
