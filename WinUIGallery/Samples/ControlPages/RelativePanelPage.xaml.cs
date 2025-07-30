// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;

namespace WinUIGallery.ControlPages;

public sealed partial class RelativePanelPage : Page, INotifyPropertyChanged
{
    public RelativePanelPage()
    {
        this.InitializeComponent();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private string _positionProperty = "";
    public string PositionProperty 
    { 
        get => _positionProperty;
        private set
        {
            _positionProperty = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PositionProperty)));
        }
    }

    private string _positionValue = "";
    public string PositionValue 
    { 
        get => _positionValue;
        private set
        {
            _positionValue = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PositionValue)));
        }
    }

    private string _alignmentProperty = "";
    public string AlignmentProperty 
    { 
        get => _alignmentProperty;
        private set
        {
            _alignmentProperty = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AlignmentProperty)));
        }
    }

    private string _alignmentValue = "";
    public string AlignmentValue 
    { 
        get => _alignmentValue;
        private set
        {
            _alignmentValue = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AlignmentValue)));
        }
    }

    private void PositionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (PositionComboBox.SelectedItem is ComboBoxItem item)
        {
            // Clear previous position properties
            MovableElement.ClearValue(RelativePanel.AboveProperty);
            MovableElement.ClearValue(RelativePanel.BelowProperty);
            MovableElement.ClearValue(RelativePanel.LeftOfProperty);
            MovableElement.ClearValue(RelativePanel.RightOfProperty);

            var tag = item.Tag?.ToString();
            switch (tag)
            {
                case "Above":
                    RelativePanel.SetAbove(MovableElement, FixedElement);
                    PositionProperty = "RelativePanel.Above";
                    break;
                case "Below":
                    RelativePanel.SetBelow(MovableElement, FixedElement);
                    PositionProperty = "RelativePanel.Below";
                    break;
                case "LeftOf":
                    RelativePanel.SetLeftOf(MovableElement, FixedElement);
                    PositionProperty = "RelativePanel.LeftOf";
                    break;
                case "RightOf":
                    RelativePanel.SetRightOf(MovableElement, FixedElement);
                    PositionProperty = "RelativePanel.RightOf";
                    break;
            }
            PositionValue = "FixedElement";
        }
    }

    private void AlignmentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (AlignmentComboBox.SelectedItem is ComboBoxItem item)
        {
            // Clear previous alignment properties
            MovableElement.ClearValue(RelativePanel.AlignTopWithPanelProperty);
            MovableElement.ClearValue(RelativePanel.AlignBottomWithPanelProperty);
            MovableElement.ClearValue(RelativePanel.AlignLeftWithPanelProperty);
            MovableElement.ClearValue(RelativePanel.AlignRightWithPanelProperty);
            MovableElement.ClearValue(RelativePanel.AlignHorizontalCenterWithPanelProperty);
            MovableElement.ClearValue(RelativePanel.AlignVerticalCenterWithPanelProperty);

            var tag = item.Tag?.ToString();
            switch (tag)
            {
                case "None":
                    AlignmentProperty = "";
                    AlignmentValue = "";
                    break;
                case "Top":
                    RelativePanel.SetAlignTopWithPanel(MovableElement, true);
                    AlignmentProperty = "RelativePanel.AlignTopWithPanel";
                    AlignmentValue = "True";
                    break;
                case "Bottom":
                    RelativePanel.SetAlignBottomWithPanel(MovableElement, true);
                    AlignmentProperty = "RelativePanel.AlignBottomWithPanel";
                    AlignmentValue = "True";
                    break;
                case "Left":
                    RelativePanel.SetAlignLeftWithPanel(MovableElement, true);
                    AlignmentProperty = "RelativePanel.AlignLeftWithPanel";
                    AlignmentValue = "True";
                    break;
                case "Right":
                    RelativePanel.SetAlignRightWithPanel(MovableElement, true);
                    AlignmentProperty = "RelativePanel.AlignRightWithPanel";
                    AlignmentValue = "True";
                    break;
                case "CenterH":
                    RelativePanel.SetAlignHorizontalCenterWithPanel(MovableElement, true);
                    AlignmentProperty = "RelativePanel.AlignHorizontalCenterWithPanel";
                    AlignmentValue = "True";
                    break;
                case "CenterV":
                    RelativePanel.SetAlignVerticalCenterWithPanel(MovableElement, true);
                    AlignmentProperty = "RelativePanel.AlignVerticalCenterWithPanel";
                    AlignmentValue = "True";
                    break;
            }
        }
    }
}
