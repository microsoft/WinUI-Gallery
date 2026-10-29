// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Svg;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using WinUIGallery.Helpers;
using WinUIGallery.Models;

namespace WinUIGallery.UnitTests;

[TestClass]
public class IconSvgExporterTests
{
    [DataTestMethod]
    [DataRow("E700")]
    [DataRow("E756")]
    [DataRow("E713")]
    [DataRow("EA3A")]
    public void SvgContainsFontIndependentOutline(string code)
    {
        string svg = IconSvgExporter.CreateSvg(new IconData { Code = code });
        XElement root = XElement.Parse(svg);
        XNamespace ns = "http://www.w3.org/2000/svg";

        Assert.AreEqual(ns + "svg", root.Name);
        Assert.AreEqual("32", root.Attribute("width")?.Value);
        Assert.AreEqual("32", root.Attribute("height")?.Value);
        Assert.IsFalse(root.Descendants(ns + "text").Any());
        XElement path = root.Element(ns + "path")!;
        Assert.IsNotNull(path);
        Assert.IsTrue(path.Attribute("d")!.Value.StartsWith("M ", StringComparison.Ordinal));
        Assert.IsTrue(path.Attribute("d")!.Value.Contains('Z'));
        Assert.IsTrue(path.Attribute("fill-rule")!.Value is "nonzero" or "evenodd");

        double[] bounds = root.Attribute("viewBox")!.Value.Split(' ')
            .Select(value => double.Parse(value, CultureInfo.InvariantCulture)).ToArray();
        Assert.AreEqual(4, bounds.Length);
        Assert.IsTrue(bounds[2] > 0 && bounds[3] > 0);

        using CanvasSvgDocument document = CanvasSvgDocument.LoadFromXml(CanvasDevice.GetSharedDevice(), svg);
        Assert.IsNotNull(document.Root);
        using CanvasRenderTarget image = new(CanvasDevice.GetSharedDevice(), 32, 32, 96);
        using (CanvasDrawingSession drawingSession = image.CreateDrawingSession())
        {
            drawingSession.Clear(Microsoft.UI.Colors.Transparent);
            drawingSession.DrawSvg(document, new Windows.Foundation.Size(32, 32));
        }
        Assert.IsTrue(image.GetPixelBytes().Where((_, index) => index % 4 == 3).Any(alpha => alpha > 0),
            "The SVG should render a visible icon without relying on a font.");
    }

    [TestMethod]
    public void SvgUsesInvariantNumbers()
    {
        IconData icon = new() { Code = "E756" };
        string expected = IconSvgExporter.CreateSvg(icon);
        CultureInfo previousCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("nl-NL");
            Assert.AreEqual(expected, IconSvgExporter.CreateSvg(icon));
        }
        finally
        {
            CultureInfo.CurrentCulture = previousCulture;
        }
    }
}