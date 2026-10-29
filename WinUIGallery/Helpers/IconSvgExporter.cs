// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using System;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using Windows.Foundation;
using WinUIGallery.Models;

namespace WinUIGallery.Helpers;

internal static partial class IconSvgExporter
{
    public static string CreateSvg(IconData icon)
    {
        string[] fonts = CanvasTextFormat.GetSystemFontFamilies();
        string fontFamily = fonts.Contains("Segoe Fluent Icons", StringComparer.OrdinalIgnoreCase)
            ? "Segoe Fluent Icons"
            : !icon.IsSegoeFluentOnly && fonts.Contains("Segoe MDL2 Assets", StringComparer.OrdinalIgnoreCase)
                ? "Segoe MDL2 Assets"
                : throw new InvalidOperationException("Install Segoe Fluent Icons to export this icon.");

        using CanvasTextFormat format = new()
        {
            FontFamily = fontFamily,
            FontSize = 32,
            WordWrapping = CanvasWordWrapping.NoWrap
        };
        using CanvasTextLayout layout = new(CanvasDevice.GetSharedDevice(), icon.Character, format, 32, 32);
        using CanvasGeometry geometry = CanvasGeometry.CreateText(layout);
        Rect bounds = geometry.ComputeBounds();
        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            throw new InvalidOperationException("This icon has no outline to export.");
        }

        SvgPathReceiver receiver = new();
        geometry.SendPathTo(receiver);
        string viewBox = $"{Number(bounds.X)} {Number(bounds.Y)} {Number(bounds.Width)} {Number(bounds.Height)}";
        return $"<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"32\" height=\"32\" viewBox=\"{viewBox}\">\n" +
            $"  <path fill=\"currentColor\" fill-rule=\"{receiver.FillRule}\" d=\"{receiver.Path}\" />\n" +
            "</svg>";
    }

    private static string Number(double value) => value.ToString("0.#####", CultureInfo.InvariantCulture);

    private sealed partial class SvgPathReceiver : ICanvasPathReceiver
    {
        private readonly StringBuilder path = new();

        public string Path => path.ToString().TrimEnd();
        public string FillRule { get; private set; } = "evenodd";

        public void BeginFigure(Vector2 startPoint, CanvasFigureFill figureFill) =>
            path.Append($"M {Point(startPoint)} ");

        public void AddLine(Vector2 endPoint) =>
            path.Append($"L {Point(endPoint)} ");

        public void AddCubicBezier(Vector2 controlPoint1, Vector2 controlPoint2, Vector2 endPoint) =>
            path.Append($"C {Point(controlPoint1)} {Point(controlPoint2)} {Point(endPoint)} ");

        public void AddQuadraticBezier(Vector2 controlPoint, Vector2 endPoint) =>
            path.Append($"Q {Point(controlPoint)} {Point(endPoint)} ");

        public void AddArc(Vector2 endPoint, float radiusX, float radiusY, float rotationAngle,
            CanvasSweepDirection sweepDirection, CanvasArcSize arcSize)
        {
            int largeArc = arcSize == CanvasArcSize.Large ? 1 : 0;
            int sweep = sweepDirection == CanvasSweepDirection.Clockwise ? 1 : 0;
            path.Append($"A {Number(radiusX)} {Number(radiusY)} {Number(rotationAngle * 180 / Math.PI)} {largeArc} {sweep} {Point(endPoint)} ");
        }

        public void EndFigure(CanvasFigureLoop figureLoop)
        {
            if (figureLoop == CanvasFigureLoop.Closed)
            {
                path.Append("Z ");
            }
        }

        public void SetFilledRegionDetermination(CanvasFilledRegionDetermination filledRegionDetermination) =>
            FillRule = filledRegionDetermination == CanvasFilledRegionDetermination.Winding ? "nonzero" : "evenodd";

        // Segment options affect strokes, not the filled glyph outline.
        public void SetSegmentOptions(CanvasFigureSegmentOptions figureSegmentOptions) { }

        private static string Point(Vector2 point) => $"{Number(point.X)} {Number(point.Y)}";
    }
}