// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WinUIGallery.ControlPages;

namespace WinUIGallery.UnitTests;

[TestClass]
public class TableViewSampleItemTests
{
    [TestMethod]
    public void ExampleCollectionsHaveIndependentItems()
    {
        ObservableCollection<TableViewSampleItem> first = TableViewSampleItem.CreateItems();
        ObservableCollection<TableViewSampleItem> second = TableViewSampleItem.CreateItems();

        Assert.AreNotSame(first, second);
        for (int index = 0; index < first.Count; index++)
        {
            Assert.AreNotSame(first[index], second[index]);
            Assert.AreEqual(first[index].Id, second[index].Id);
        }

        first[0].Name = "Changed";
        first[0].Category = "Changed";
        first.Clear();

        Assert.AreEqual(5, second.Count);
        Assert.AreEqual("Item 1", second[0].Name);
        Assert.AreEqual("Alpha", second[0].Category);
    }

    [TestMethod]
    public void LargeCollectionHasUniqueIdsAndNumericValues()
    {
        ObservableCollection<TableViewSampleItem> items = TableViewSampleItem.CreateItems(1000);

        Assert.AreEqual(1000, items.Count);
        Assert.AreEqual(1000, items.Select(item => item.Id).Distinct().Count());
        Assert.AreEqual("Item 1000", items[^1].Name);
        CollectionAssert.AreEqual(
            new[] { 10, 25, 50, 80, 100 },
            items.Select(item => item.Value).Distinct().OrderBy(value => value).ToArray());
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("\t")]
    [DataRow(null)]
    public void EmptyNameRaisesValidationFeedback(string? name)
    {
        TableViewSampleItem item = new(1, "Item 1", "Alpha", 25);
        string? errorProperty = null;
        List<string?> changedProperties = [];
        item.ErrorsChanged += (_, args) => errorProperty = args.PropertyName;
        item.PropertyChanged += (_, args) => changedProperties.Add(args.PropertyName);

        item.Name = name!;

        Assert.IsTrue(item.HasErrors);
        Assert.AreEqual(nameof(TableViewSampleItem.Name), errorProperty);
        CollectionAssert.AreEqual(new[] { "Name is required." }, item.GetErrors(nameof(item.Name)).Cast<string>().ToArray());
        CollectionAssert.AreEqual(new[] { "Name is required." }, item.GetErrors(null).Cast<string>().ToArray());
        Assert.AreEqual(0, item.GetErrors(nameof(item.Category)).Cast<string>().Count());
        CollectionAssert.Contains(changedProperties, nameof(item.Name));
        CollectionAssert.Contains(changedProperties, nameof(item.HasErrors));
    }

    [TestMethod]
    public void CorrectingOrRollingBackNameClearsValidation()
    {
        TableViewSampleItem item = new(1, "Item 1", "Alpha", 25);
        int errorChanges = 0;
        item.ErrorsChanged += (_, _) => errorChanges++;

        item.Name = string.Empty;
        item.Name = "Item 1";

        Assert.IsFalse(item.HasErrors);
        Assert.AreEqual(0, item.GetErrors(null).Cast<string>().Count());
        Assert.AreEqual(2, errorChanges);
    }

    [TestMethod]
    public void UnchangedValuesDoNotRaiseNotifications()
    {
        TableViewSampleItem item = new(1, "Item 1", "Alpha", 25);
        int notifications = 0;
        item.PropertyChanged += (_, _) => notifications++;
        item.ErrorsChanged += (_, _) => notifications++;

        item.Name = "Item 1";
        item.Category = "Alpha";

        Assert.AreEqual(0, notifications);
    }

    [TestMethod]
    public void CollectionSizeIsValidated()
    {
        Assert.AreEqual(0, TableViewSampleItem.CreateItems(0).Count);
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => TableViewSampleItem.CreateItems(-1));
    }
}