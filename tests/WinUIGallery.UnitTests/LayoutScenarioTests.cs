// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#nullable enable

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using System.Linq;
using WinUIGallery.ControlPages;

namespace WinUIGallery.UnitTests;

[TestClass]
public class LayoutScenarioTests
{
    [UITestMethod]
    public void AdaptiveListDetailsPreservesSelectionAtTheWidthBoundary()
    {
        ListDetailsPage page = new() { Width = 1100 };
        UnitTestApp.UnitTestAppWindow.AddToVisualTree(page);
        try
        {
            page.UpdateLayout();
            Slider width = (Slider)page.FindName("PreviewWidth");
            ListView list = (ListView)page.FindName("AdaptiveList");
            Grid example = (Grid)page.FindName("AdaptiveExample");
            VisualStateGroup states = VisualStateManager.GetVisualStateGroups((Grid)page.Content)
                .Single(group => group.Name == "AdaptiveStates");
            list.SelectedItem = page.Items[2];

            width.Value = 641;
            page.UpdateLayout();
            Assert.AreEqual(641, example.ActualWidth, 0.01);
            Assert.AreEqual("AdaptiveWide", states.CurrentState.Name);

            width.Value = 640;
            page.UpdateLayout();
            Assert.AreEqual(640, example.ActualWidth, 0.01);
            Assert.AreEqual("AdaptiveDetailOnly", states.CurrentState.Name);
            Assert.AreSame(page.Items[2], list.SelectedItem);

            width.Value = 641;
            page.UpdateLayout();
            Assert.AreEqual("AdaptiveWide", states.CurrentState.Name);
            Assert.AreSame(page.Items[2], list.SelectedItem);
            Assert.AreEqual("Item 3", ((TextBlock)page.FindName("AdaptiveDetailTitle")).Text);
        }
        finally
        {
            UnitTestApp.UnitTestAppWindow.CleanupVisualTree();
        }
    }

    [UITestMethod]
    public void VisualStateToggleChangesAndRestoresTheState()
    {
        StateTriggersPage page = new();
        UnitTestApp.UnitTestAppWindow.AddToVisualTree(page);
        try
        {
            page.UpdateLayout();
            ToggleSwitch toggle = (ToggleSwitch)page.FindName("EmphasisSwitch");
            VisualStateGroup states = VisualStateManager.GetVisualStateGroups((Grid)page.Content)
                .Single(group => group.Name == "BasicStates");

            toggle.IsOn = true;
            Assert.AreEqual("Emphasized", states.CurrentState.Name);

            toggle.IsOn = false;
            Assert.AreEqual("Normal", states.CurrentState.Name);
        }
        finally
        {
            UnitTestApp.UnitTestAppWindow.CleanupVisualTree();
        }
    }
}