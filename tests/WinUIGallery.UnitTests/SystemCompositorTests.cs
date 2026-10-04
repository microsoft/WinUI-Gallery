// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Content;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using System.Reflection;
using WinUIGallery.ControlPages;
using WinUIGallery.Helpers;
using WinUIGallery.SamplePages;

namespace WinUIGallery.UnitTests;

[TestClass]
public class SystemCompositorTests
{
    public TestContext TestContext { get; set; }

    [UITestMethod]
    public void XamlInitializationProvidesSystemDispatcherQueueAndMucCompositor()
    {
        DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        global::Windows.System.DispatcherQueue systemDispatcherQueue = global::Windows.System.DispatcherQueue.GetForCurrentThread();
        Compositor compositor = CompositionTarget.GetCompositorForCurrentThread();

        Assert.IsNotNull(dispatcherQueue);
        Assert.IsNotNull(systemDispatcherQueue);
        Assert.AreEqual(UnitTestApp.UnitTestAppWindow.DispatcherQueue, dispatcherQueue);
        Assert.IsNotNull(compositor);
        Assert.AreEqual(dispatcherQueue, compositor.DispatcherQueue);
        Assert.IsNotNull(CompositionEngine.GetForSystemEngine(compositor));

        TestContext.WriteLine($"Package identity: {NativeMethods.IsAppPackaged}");
        TestContext.WriteLine($"DispatcherQueue thread access: {dispatcherQueue.HasThreadAccess}");
    }

    [UITestMethod]
    public void XamlCompInteropUsesTheXamlMucCompositor()
    {
        XamlCompInteropPage page = new();

        try
        {
            UnitTestApp.UnitTestAppWindow.AddToVisualTree(page);
            page.UpdateLayout();

            Compositor xamlCompositor = CompositionTarget.GetCompositorForCurrentThread();

            Assert.AreEqual(xamlCompositor, page.Compositor);

            using SpringVector3NaturalMotionAnimation animation = page.Compositor.CreateSpringVector3Animation();
            Assert.AreEqual(page.Compositor, animation.Compositor);
        }
        finally
        {
            UnitTestApp.UnitTestAppWindow.CleanupVisualTree();
        }
    }

    [UITestMethod]
    public void ContentIslandUsesTheXamlMucCompositor()
    {
        ContentIslandPage page = new();

        try
        {
            UnitTestApp.UnitTestAppWindow.AddToVisualTree(page);
            page.UpdateLayout();

            Assert.IsNotNull(page.XamlRoot);

            ContentIsland parentIsland = page.XamlRoot.ContentIsland;
            Assert.IsNotNull(parentIsland);

            WrapPanel rectanglePanel = page.FindName("_rectanglePanel") as WrapPanel;
            Assert.IsNotNull(rectanglePanel);

            Rectangle rectangle = rectanglePanel.Children[0] as Rectangle;
            Assert.IsNotNull(rectangle);

            ContainerVisual placementVisual = ElementCompositionPreview.GetElementVisual(rectangle) as ContainerVisual;
            Assert.IsNotNull(placementVisual);
            Assert.AreEqual(CompositionTarget.GetCompositorForCurrentThread(), placementVisual.Compositor);
            Assert.IsNotNull(CompositionEngine.GetForSystemEngine(placementVisual));

            using ChildSiteLink childSiteLink = ChildSiteLink.Create(parentIsland, placementVisual);
            Assert.IsFalse(childSiteLink.IsClosed);
        }
        finally
        {
            UnitTestApp.UnitTestAppWindow.CleanupVisualTree();
        }
    }

    [UITestMethod]
    public void SystemBackdropSupportsMultipleWindowsWithoutQueueBootstrap()
    {
        bool micaSupported = MicaController.IsSupported();
        bool acrylicSupported = DesktopAcrylicController.IsSupported();
        if (!micaSupported && !acrylicSupported)
        {
            Assert.Inconclusive("Neither Mica nor Desktop Acrylic is supported in this environment.");
        }

        DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        SampleSystemBackdropsWindow firstWindow = new();
        SampleSystemBackdropsWindow secondWindow = new();
        SampleSystemBackdropsWindow.BackdropType supportedBackdrop = micaSupported
            ? SampleSystemBackdropsWindow.BackdropType.Mica
            : SampleSystemBackdropsWindow.BackdropType.Acrylic;

        try
        {
            firstWindow.Activate();
            secondWindow.Activate();

            Assert.AreEqual(dispatcherQueue, firstWindow.DispatcherQueue);
            Assert.AreEqual(dispatcherQueue, secondWindow.DispatcherQueue);
            Assert.IsNotNull((firstWindow.Content as FrameworkElement)?.XamlRoot);
            Assert.IsNotNull((secondWindow.Content as FrameworkElement)?.XamlRoot);

            firstWindow.SetBackdrop(supportedBackdrop);
            secondWindow.SetBackdrop(supportedBackdrop);

            Assert.IsTrue(HasActiveBackdropController(firstWindow));
            Assert.IsTrue(HasActiveBackdropController(secondWindow));
            Assert.IsNotNull((firstWindow.Content as FrameworkElement)?.XamlRoot);
            Assert.IsNotNull((secondWindow.Content as FrameworkElement)?.XamlRoot);
        }
        finally
        {
            firstWindow.Close();
            secondWindow.Close();
        }
    }

    private static bool HasActiveBackdropController(SampleSystemBackdropsWindow window)
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
        FieldInfo micaControllerField = typeof(SampleSystemBackdropsWindow).GetField("micaController", flags);
        FieldInfo acrylicControllerField = typeof(SampleSystemBackdropsWindow).GetField("acrylicController", flags);

        Assert.IsNotNull(micaControllerField);
        Assert.IsNotNull(acrylicControllerField);

        return micaControllerField.GetValue(window) != null || acrylicControllerField.GetValue(window) != null;
    }
}