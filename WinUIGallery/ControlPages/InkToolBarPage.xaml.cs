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

namespace WinUIGallery.ControlPages
{
    public sealed partial class InkToolBarPage : Page
    {
        public InkToolBarPage()
        {
            this.InitializeComponent();
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
                    // erasing is driven by the InkToolBar's eraser tool.
                    Status1.Text = "Selected: Eraser (erasing is driven by the InkToolBar tool).";
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
                InkToolBarTool kind = ToolKind(tool);
                var btn = inkToolBar1.GetToolButton(kind);
                if (btn == null)
                {
                    Status1.Text = "GetToolButton(" + kind + ") returned null.";
                    return;
                }
                inkToolBar1.ActiveTool = btn;
                Status1.Text = "inkToolBar1.ActiveTool set via GetToolButton(" + kind + ").\n"
                    + "Read back: " + (inkToolBar1.ActiveTool?.ToolKind.ToString() ?? "null");
            }
            catch (Exception ex) { Status1.Text = Fmt(ex); }
        }

        private void ReadActiveTool1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var active = inkToolBar1.ActiveTool;
                Status1.Text = active == null
                    ? "inkToolBar1.ActiveTool = null"
                    : "inkToolBar1.ActiveTool.ToolKind = " + active.ToolKind;
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
                inkToolBar2.TargetInkCanvas = inkCanvas2A;
                Status2.Text = "inkToolBar2.TargetInkCanvas = inkCanvas2A\n(only Canvas A is now driven by the toolbar)";
            }
            catch (Exception ex) { Status2.Text = Fmt(ex); }
        }

        private void WireToB_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                inkToolBar2.TargetInkCanvas = inkCanvas2B;
                Status2.Text = "inkToolBar2.TargetInkCanvas = inkCanvas2B\n(only Canvas B is now driven by the toolbar)";
            }
            catch (Exception ex) { Status2.Text = Fmt(ex); }
        }

        private void ReadTarget2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var t = inkToolBar2.TargetInkCanvas;
                if (t == null) { Status2.Text = "inkToolBar2.TargetInkCanvas = null"; return; }
                string which = ReferenceEquals(t, inkCanvas2A) ? "inkCanvas2A"
                    : ReferenceEquals(t, inkCanvas2B) ? "inkCanvas2B" : "(other)";
                Status2.Text = "inkToolBar2.TargetInkCanvas = " + which;
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
                inkToolBar3.TargetInkCanvas = null;
                inkToolBar3.TargetInkPresenter = inkCanvas3.InkPresenter;
                Status3.Text = "inkToolBar3.TargetInkPresenter = inkCanvas3.InkPresenter\n"
                    + "TargetInkCanvas: " + (inkToolBar3.TargetInkCanvas == null ? "null" : "set");
            }
            catch (Exception ex) { Status3.Text = Fmt(ex); }
        }

        private void WireViaCanvas_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                inkToolBar3.TargetInkPresenter = null;
                inkToolBar3.TargetInkCanvas = inkCanvas3;
                Status3.Text = "inkToolBar3.TargetInkCanvas = inkCanvas3\n"
                    + "TargetInkPresenter: " + (inkToolBar3.TargetInkPresenter == null ? "null" : "set");
            }
            catch (Exception ex) { Status3.Text = Fmt(ex); }
        }

        private void ClearWiring_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                inkToolBar3.TargetInkCanvas = null;
                inkToolBar3.TargetInkPresenter = null;
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
                    "None" => InkToolBarInitialControls.None,
                    "PensOnly" => InkToolBarInitialControls.PensOnly,
                    "AllExceptPens" => InkToolBarInitialControls.AllExceptPens,
                    _ => InkToolBarInitialControls.All,
                };
                inkToolBar4.InitialControls = value;
                var sb = new StringBuilder();
                sb.Append("InitialControls = ").Append(inkToolBar4.InitialControls).Append('\n');
                foreach (InkToolBarTool t in Enum.GetValues(typeof(InkToolBarTool)))
                {
                    object btn = null;
                    try { btn = inkToolBar4.GetToolButton(t); } catch { }
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
                inkToolBar5.Orientation = value;
                Status5.Text = "inkToolBar5.Orientation = " + inkToolBar5.Orientation;
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
                    "Top" => InkToolBarButtonFlyoutPlacement.Top,
                    "Bottom" => InkToolBarButtonFlyoutPlacement.Bottom,
                    "Left" => InkToolBarButtonFlyoutPlacement.Left,
                    "Right" => InkToolBarButtonFlyoutPlacement.Right,
                    _ => InkToolBarButtonFlyoutPlacement.Auto,
                };
                inkToolBar6.ButtonFlyoutPlacement = value;
                Status6.Text = "inkToolBar6.ButtonFlyoutPlacement = " + inkToolBar6.ButtonFlyoutPlacement;
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
                foreach (InkToolBarTool t in Enum.GetValues(typeof(InkToolBarTool)))
                {
                    object btn = null;
                    try { btn = inkToolBar7.GetToolButton(t); } catch { }
                    sb.Append("  ").Append(t).Append(" -> ").Append(btn == null ? "null" : "found").Append('\n');
                }
                sb.Append("GetToggleButton:\n");
                foreach (InkToolBarToggle t in Enum.GetValues(typeof(InkToolBarToggle)))
                {
                    object btn = null;
                    try { btn = inkToolBar7.GetToggleButton(t); } catch { }
                    sb.Append("  ").Append(t).Append(" -> ").Append(btn == null ? "null" : "found").Append('\n');
                }
                sb.Append("GetMenuButton:\n");
                foreach (InkToolBarMenuKind k in Enum.GetValues(typeof(InkToolBarMenuKind)))
                {
                    object btn = null;
                    try { btn = inkToolBar7.GetMenuButton(k); } catch { }
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
                bool wantRuler   = cbRuler8.IsChecked == true;
                bool wantStencil = cbStencil8.IsChecked == true;
                inkToolBar8.IsRulerButtonChecked   = wantRuler;
                inkToolBar8.IsStencilButtonChecked = wantStencil;
                Status8.Text =
                    "Wrote IsRulerButtonChecked=" + wantRuler +
                    ", IsStencilButtonChecked=" + wantStencil + "\n" +
                    "Read back: Ruler="   + inkToolBar8.IsRulerButtonChecked +
                    ", Stencil=" + inkToolBar8.IsStencilButtonChecked;
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
                var attrs = inkToolBar9.InkDrawingAttributes;
                Status9.Text = attrs == null
                    ? "inkToolBar9.InkDrawingAttributes = null (no active tool / not yet templated)"
                    : "inkToolBar9.InkDrawingAttributes:\n"
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
                var children = inkToolBar10.Children;
                int count = children?.Count ?? -1;
                var sb = new StringBuilder();
                sb.Append("inkToolBar10.Children:\n");
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
                Append10(sb, "InitialControlsProperty",      InkToolBar.InitialControlsProperty);
                Append10(sb, "ChildrenProperty",             InkToolBar.ChildrenProperty);
                Append10(sb, "ActiveToolProperty",           InkToolBar.ActiveToolProperty);
                Append10(sb, "InkDrawingAttributesProperty", InkToolBar.InkDrawingAttributesProperty);
                Append10(sb, "IsRulerButtonCheckedProperty", InkToolBar.IsRulerButtonCheckedProperty);
                Append10(sb, "TargetInkCanvasProperty",      InkToolBar.TargetInkCanvasProperty);
                Append10(sb, "IsStencilButtonCheckedProperty", InkToolBar.IsStencilButtonCheckedProperty);
                Append10(sb, "ButtonFlyoutPlacementProperty", InkToolBar.ButtonFlyoutPlacementProperty);
                Append10(sb, "OrientationProperty",          InkToolBar.OrientationProperty);
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

        private static InkToolBarTool ToolKind(string tool) => tool switch
        {
            "Pencil" => InkToolBarTool.Pencil,
            "Highlighter" => InkToolBarTool.Highlighter,
            "Eraser" => InkToolBarTool.Eraser,
            _ => InkToolBarTool.BallpointPen,
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
}
