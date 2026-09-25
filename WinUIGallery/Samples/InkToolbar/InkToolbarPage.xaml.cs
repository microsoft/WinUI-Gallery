//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System;
using System.Text;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;
using Windows.UI.Input.Inking;

namespace WinUIGallery.ControlPages;

public sealed partial class InkToolbarPage : Page
{
    public InkToolbarPage()
    {
        InitializeComponent();
    }

    // =====================================================================
    // Example 1 — ActiveTool + GetToolButton
    // =====================================================================

    private string CurrentToolName1()
    {
        if (rbPencil1?.IsChecked == true) return "Pencil";
        if (rbHighlighter1?.IsChecked == true) return "Highlighter";
        if (rbEraser1?.IsChecked == true) return "Eraser";
        return "Pen";
    }

    private void ToolRadio1_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            string tool = CurrentToolName1();
            if (tool == "Eraser")
            {
                // The subset InkPresenter proxy does not expose an erase mode;
                // erasing is driven by the InkToolbar's eraser tool.
                Status1.Text = "Selected: Eraser (erasing is driven by the InkToolbar tool).";
                return;
            }

            var attrs = new InkDrawingAttributes();
            switch (tool)
            {
                case "Pencil":
                    attrs.Color = Colors.Gray;
                    attrs.Size = new Size(1.5, 1.5);
                    break;
                case "Highlighter":
                    attrs.Color = Colors.Yellow;
                    attrs.Size = new Size(8, 8);
                    attrs.DrawAsHighlighter = true;
                    break;
                default:
                    attrs.Color = Colors.Black;
                    attrs.Size = new Size(2, 2);
                    break;
            }
            inkCanvas1.InkPresenter.UpdateDefaultDrawingAttributes(attrs);
            Status1.Text = "Selected: " + tool + " (InkPresenter drawing attributes updated).";
        }
        catch (Exception ex) { Status1.Text = Fmt(ex); }
    }

    private void Clear1_Click(object sender, RoutedEventArgs e) => ClearStrokes(inkCanvas1, Status1);

    private void ApplyViaToolBar1_Click(object sender, RoutedEventArgs e)
    {
        string tool = CurrentToolName1();
        try
        {
            InkToolbarTool kind = ToolKind(tool);
            var btn = InkToolbar1.GetToolButton(kind);
            if (btn == null)
            {
                Status1.Text = "GetToolButton(" + kind + ") returned null.";
                return;
            }
            InkToolbar1.ActiveTool = btn;
            Status1.Text = "InkToolbar1.ActiveTool set via GetToolButton(" + kind + ").\n"
                + "Read back: " + (InkToolbar1.ActiveTool?.ToolKind.ToString() ?? "null");
        }
        catch (Exception ex) { Status1.Text = Fmt(ex); }
    }

    private void ReadActiveTool1_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var active = InkToolbar1.ActiveTool;
            Status1.Text = active == null
                ? "InkToolbar1.ActiveTool = null"
                : "InkToolbar1.ActiveTool.ToolKind = " + active.ToolKind;
        }
        catch (Exception ex) { Status1.Text = Fmt(ex); }
    }

    // =====================================================================
    // Example 2 — TargetInkCanvas rebinding
    // =====================================================================

    private void WireToA_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            InkToolbar2.TargetInkCanvas = inkCanvas2A;
            Status2.Text = "InkToolbar2.TargetInkCanvas = inkCanvas2A\n(only Canvas A is now driven by the toolbar)";
        }
        catch (Exception ex) { Status2.Text = Fmt(ex); }
    }

    private void WireToB_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            InkToolbar2.TargetInkCanvas = inkCanvas2B;
            Status2.Text = "InkToolbar2.TargetInkCanvas = inkCanvas2B\n(only Canvas B is now driven by the toolbar)";
        }
        catch (Exception ex) { Status2.Text = Fmt(ex); }
    }

    private void ReadTarget2_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var t = InkToolbar2.TargetInkCanvas;
            if (t == null) { Status2.Text = "InkToolbar2.TargetInkCanvas = null"; return; }
            string which = ReferenceEquals(t, inkCanvas2A) ? "inkCanvas2A"
                : ReferenceEquals(t, inkCanvas2B) ? "inkCanvas2B" : "(other)";
            Status2.Text = "InkToolbar2.TargetInkCanvas = " + which;
        }
        catch (Exception ex) { Status2.Text = Fmt(ex); }
    }

    // =====================================================================
    // Example 3 — TargetInkPresenter alternative wiring
    // =====================================================================

    private void WireViaPresenter_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            InkToolbar3.TargetInkCanvas = null;
            InkToolbar3.TargetInkPresenter = inkCanvas3.InkPresenter;
            Status3.Text = "InkToolbar3.TargetInkPresenter = inkCanvas3.InkPresenter\n"
                + "TargetInkCanvas: " + (InkToolbar3.TargetInkCanvas == null ? "null" : "set");
        }
        catch (Exception ex) { Status3.Text = Fmt(ex); }
    }

    private void WireViaCanvas_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            InkToolbar3.TargetInkPresenter = null;
            InkToolbar3.TargetInkCanvas = inkCanvas3;
            Status3.Text = "InkToolbar3.TargetInkCanvas = inkCanvas3\n"
                + "TargetInkPresenter: " + (InkToolbar3.TargetInkPresenter == null ? "null" : "set");
        }
        catch (Exception ex) { Status3.Text = Fmt(ex); }
    }

    private void ClearWiring_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            InkToolbar3.TargetInkCanvas = null;
            InkToolbar3.TargetInkPresenter = null;
            Status3.Text = "Both TargetInkCanvas and TargetInkPresenter cleared.";
        }
        catch (Exception ex) { Status3.Text = Fmt(ex); }
    }

    // =====================================================================
    // Example 4 — InitialControls
    // =====================================================================

    private void ApplyInitialControls_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            string sel = (InitialControlsCombo.SelectedItem as ComboBoxItem)?.Content as string ?? "All";
            var value = sel switch
            {
                "None" => InkToolbarInitialControls.None,
                "PensOnly" => InkToolbarInitialControls.PensOnly,
                "AllExceptPens" => InkToolbarInitialControls.AllExceptPens,
                _ => InkToolbarInitialControls.All,
            };
            InkToolbar4.InitialControls = value;
            var sb = new StringBuilder();
            sb.Append("InitialControls = ").Append(InkToolbar4.InitialControls).Append('\n');
            foreach (InkToolbarTool t in Enum.GetValues(typeof(InkToolbarTool)))
            {
                object? btn = null;
                try { btn = InkToolbar4.GetToolButton(t); } catch { }
                sb.Append("  GetToolButton(").Append(t).Append(") = ").Append(btn == null ? "null" : "found").Append('\n');
            }
            Status4.Text = sb.ToString();
        }
        catch (Exception ex) { Status4.Text = Fmt(ex); }
    }

    // =====================================================================
    // Example 5 — Orientation
    // =====================================================================

    private void ApplyOrientation_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            string sel = (OrientationCombo.SelectedItem as ComboBoxItem)?.Content as string ?? "Horizontal";
            var value = sel == "Vertical" ? Orientation.Vertical : Orientation.Horizontal;
            InkToolbar5.Orientation = value;
            Status5.Text = "InkToolbar5.Orientation = " + InkToolbar5.Orientation;
        }
        catch (Exception ex) { Status5.Text = Fmt(ex); }
    }

    // =====================================================================
    // Example 6 — ButtonFlyoutPlacement
    // =====================================================================

    private void ApplyFlyoutPlacement_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            string sel = (FlyoutPlacementCombo.SelectedItem as ComboBoxItem)?.Content as string ?? "Auto";
            var value = sel switch
            {
                "Top" => InkToolbarButtonFlyoutPlacement.Top,
                "Bottom" => InkToolbarButtonFlyoutPlacement.Bottom,
                "Left" => InkToolbarButtonFlyoutPlacement.Left,
                "Right" => InkToolbarButtonFlyoutPlacement.Right,
                _ => InkToolbarButtonFlyoutPlacement.Auto,
            };
            InkToolbar6.ButtonFlyoutPlacement = value;
            Status6.Text = "InkToolbar6.ButtonFlyoutPlacement = " + InkToolbar6.ButtonFlyoutPlacement;
        }
        catch (Exception ex) { Status6.Text = Fmt(ex); }
    }

    // =====================================================================
    // Example 7 — Enumerate Get*Button APIs
    // =====================================================================

    private void EnumerateApi_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var sb = new StringBuilder();
            sb.Append("GetToolButton:\n");
            foreach (InkToolbarTool t in Enum.GetValues(typeof(InkToolbarTool)))
            {
                object? btn = null;
                try { btn = InkToolbar7.GetToolButton(t); } catch { }
                sb.Append("  ").Append(t).Append(" -> ").Append(btn == null ? "null" : "found").Append('\n');
            }
            sb.Append("GetToggleButton:\n");
            foreach (InkToolbarToggle t in Enum.GetValues(typeof(InkToolbarToggle)))
            {
                object? btn = null;
                try { btn = InkToolbar7.GetToggleButton(t); } catch { }
                sb.Append("  ").Append(t).Append(" -> ").Append(btn == null ? "null" : "found").Append('\n');
            }
            sb.Append("GetMenuButton:\n");
            foreach (InkToolbarMenuKind k in Enum.GetValues(typeof(InkToolbarMenuKind)))
            {
                object? btn = null;
                try { btn = InkToolbar7.GetMenuButton(k); } catch { }
                sb.Append("  ").Append(k).Append(" -> ").Append(btn == null ? "null" : "found").Append('\n');
            }
            Status7.Text = sb.ToString();
        }
        catch (Exception ex) { Status7.Text = Fmt(ex); }
    }

    // =====================================================================
    // Example 8 — IsRulerButtonChecked / IsStencilButtonChecked
    // =====================================================================

    private void ApplyStencilState_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            bool wantRuler = cbRuler8.IsChecked == true;
            bool wantStencil = cbStencil8.IsChecked == true;
            InkToolbar8.IsRulerButtonChecked = wantRuler;
            InkToolbar8.IsStencilButtonChecked = wantStencil;
            Status8.Text =
                "Wrote IsRulerButtonChecked=" + wantRuler +
                ", IsStencilButtonChecked=" + wantStencil + "\n" +
                "Read back: Ruler=" + InkToolbar8.IsRulerButtonChecked +
                ", Stencil=" + InkToolbar8.IsStencilButtonChecked;
        }
        catch (Exception ex) { Status8.Text = Fmt(ex); }
    }

    // =====================================================================
    // Example 9 — InkDrawingAttributes (read)
    // =====================================================================

    private void ReadDrawingAttrs9_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var attrs = InkToolbar9.InkDrawingAttributes;
            Status9.Text = attrs == null
                ? "InkToolbar9.InkDrawingAttributes = null (no active tool / not yet templated)"
                : "InkToolbar9.InkDrawingAttributes:\n"
                    + "  Color=" + attrs.Color
                    + "\n  Size=" + attrs.Size.Width + " x " + attrs.Size.Height
                    + "\n  PenTip=" + attrs.PenTip
                    + "\n  DrawAsHighlighter=" + attrs.DrawAsHighlighter
                    + "\n  IgnorePressure=" + attrs.IgnorePressure;
        }
        catch (Exception ex) { Status9.Text = Fmt(ex); }
    }

    private void ReadInkCanvasAttrs9_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var attrs = inkCanvas9.InkPresenter.CopyDefaultDrawingAttributes();
            Status9.Text = attrs == null
                ? "inkCanvas9.InkPresenter.CopyDefaultDrawingAttributes() = null"
                : "inkCanvas9.InkPresenter.CopyDefaultDrawingAttributes():\n"
                    + "  Color=" + attrs.Color
                    + "\n  Size=" + attrs.Size.Width + " x " + attrs.Size.Height
                    + "\n  PenTip=" + attrs.PenTip
                    + "\n  DrawAsHighlighter=" + attrs.DrawAsHighlighter;
        }
        catch (Exception ex) { Status9.Text = Fmt(ex); }
    }

    // =====================================================================
    // Example 10 — Children + static DependencyProperties
    // =====================================================================

    private void ReadChildren10_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var children = InkToolbar10.Children;
            int count = children?.Count ?? -1;
            var sb = new StringBuilder();
            sb.Append("InkToolbar10.Children:\n");
            sb.Append("  type=").Append(children?.GetType().Name ?? "null").Append('\n');
            sb.Append("  count=").Append(count).Append('\n');
            if (children != null)
            {
                for (int i = 0; i < children.Count; i++)
                {
                    sb.Append("  [").Append(i).Append("] ").Append(children[i]?.GetType().Name ?? "null").Append('\n');
                }
            }
            Status10.Text = sb.ToString();
        }
        catch (Exception ex) { Status10.Text = Fmt(ex); }
    }

    private void VerifyDPs10_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var sb = new StringBuilder();
            sb.Append("Static DependencyProperty surface:\n");
            Append10(sb, "InitialControlsProperty", InkToolbar.InitialControlsProperty);
            Append10(sb, "ChildrenProperty", InkToolbar.ChildrenProperty);
            Append10(sb, "ActiveToolProperty", InkToolbar.ActiveToolProperty);
            Append10(sb, "InkDrawingAttributesProperty", InkToolbar.InkDrawingAttributesProperty);
            Append10(sb, "IsRulerButtonCheckedProperty", InkToolbar.IsRulerButtonCheckedProperty);
            Append10(sb, "TargetInkCanvasProperty", InkToolbar.TargetInkCanvasProperty);
            Append10(sb, "IsStencilButtonCheckedProperty", InkToolbar.IsStencilButtonCheckedProperty);
            Append10(sb, "ButtonFlyoutPlacementProperty", InkToolbar.ButtonFlyoutPlacementProperty);
            Append10(sb, "OrientationProperty", InkToolbar.OrientationProperty);
            Status10.Text = sb.ToString();
        }
        catch (Exception ex) { Status10.Text = Fmt(ex); }
    }

    private static void Append10(StringBuilder sb, string name, DependencyProperty dp)
    {
        sb.Append("  ").Append(name).Append(" -> ").Append(dp == null ? "null" : "OK").Append('\n');
    }

    // =====================================================================
    // Helpers
    // =====================================================================

    private static InkToolbarTool ToolKind(string tool) => tool switch
    {
        "Pencil" => InkToolbarTool.Pencil,
        "Highlighter" => InkToolbarTool.Highlighter,
        "Eraser" => InkToolbarTool.Eraser,
        _ => InkToolbarTool.BallpointPen,
    };

    private static string Fmt(Exception ex) =>
        string.Format("Error 0x{0:X8}: {1}", ex.HResult, ex.Message);

    private static void ClearStrokes(InkCanvas canvas, TextBlock status)
    {
        try
        {
            canvas.InkPresenter.StrokeContainer.Clear();
            if (status != null)
            {
                status.Text = "Cleared all strokes via InkPresenter.StrokeContainer.Clear().";
            }
        }
        catch (Exception ex)
        {
            if (status != null)
            {
                status.Text = Fmt(ex);
            }
        }
    }
}