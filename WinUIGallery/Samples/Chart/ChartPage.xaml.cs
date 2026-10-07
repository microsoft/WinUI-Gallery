// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Charts;
using Microsoft.UI.Xaml.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ChartSamples = Microsoft.UI.Xaml.Controls.Charts.Samples;

namespace WinUIGallery.ControlPages;

public sealed partial class ChartPage : Page, INotifyPropertyChanged
{
    private readonly double[] _incomingResponseTimes = [176, 169, 181, 165, 172];
    private int _nextResponseTimeIndex;
    private double[] _responseTimes = [184, 179, 173, 171, 168];
    private string _liveSeriesSummary = string.Empty;
    private DateTimeAxis? _timeSeriesAxis;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string[] Months { get; } =
    [
        "January",
        "February",
        "March",
        "April",
        "May",
        "June",
    ];

    public double[] Profits { get; } = [18, 26, 33, 39, 47, 52];

    public double[] Actuals { get; } = [12, 23, 37, 31, 46, 52];

    public double[] Forecasts { get; } = [16, 25, 35, 40, 48, 55];

    public double[] Targets { get; } = [20, 25, 40, 40, 50, 50];

    public string[] Regions { get; } = ["North", "South", "East", "West"];

    public double[] UnitsSold { get; } = [42, 38, 51, 47];

    public string[] Quarters { get; } = ["Q1", "Q2", "Q3", "Q4"];

    public double[] QuarterlyVisitors { get; } = [35, 62, 48, 81];

    public string[] Days { get; } = ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"];

    public double[] DailyVisits { get; } = [120, 135, 128, 172, 150, 98, 110];

    public List<DateTimeOffset> ReadingDates { get; } =
    [
        new DateTimeOffset(2026, 8, 3, 0, 0, 0, TimeSpan.Zero),
        new DateTimeOffset(2026, 8, 10, 0, 0, 0, TimeSpan.Zero),
        new DateTimeOffset(2026, 8, 17, 0, 0, 0, TimeSpan.Zero),
        new DateTimeOffset(2026, 8, 24, 0, 0, 0, TimeSpan.Zero),
        new DateTimeOffset(2026, 8, 31, 0, 0, 0, TimeSpan.Zero),
    ];

    public double[] TemperatureReadings { get; } = [72, 76, 74, 79, 77];

    public double[] ResponseTimes
    {
        get => _responseTimes;
        private set
        {
            _responseTimes = value;
            OnPropertyChanged();
        }
    }

    public string LiveSeriesSummary
    {
        get => _liveSeriesSummary;
        private set
        {
            if (_liveSeriesSummary != value)
            {
                _liveSeriesSummary = value;
                OnPropertyChanged();
            }
        }
    }

    public ChartPage()
    {
        InitializeComponent();

        HighlightPeakVisits();
        CreateTimeSeriesChart();
        UpdateLiveSeriesSummary();
    }

    private static T? GetSelectedTag<T>(ComboBox comboBox)
        where T : struct, Enum
    {
        return comboBox.SelectedItem is ComboBoxItem { Tag: string tag } && Enum.TryParse(tag, out T value)
            ? value
            : null;
    }

    // The charts declare their initial values in markup, so option handlers only apply changes
    // the user makes after the page loads.
    private void LegendToggle_Toggled(object sender, RoutedEventArgs e)
    {
        if (IsLoaded)
        {
            BasicLineChart.ShowLegend = LegendToggle.IsOn;
        }
    }

    private void BarOrientationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (IsLoaded && GetSelectedTag<BarOrientation>(BarOrientationComboBox) is BarOrientation orientation)
        {
            UnitsSeries.Orientation = orientation;
        }
    }

    private void CategorySortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!IsLoaded)
        {
            return;
        }

        if (GetSelectedTag<CategorySortKey>(SortKeyComboBox) is CategorySortKey sortKey)
        {
            RegionX.SortKey = sortKey;
        }

        if (GetSelectedTag<SortOrder>(SortOrderComboBox) is SortOrder sortOrder)
        {
            RegionX.SortOrder = sortOrder;
        }
    }

    private void GridLinesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (IsLoaded && GetSelectedTag<GridLines>(GridLinesComboBox) is GridLines gridLines)
        {
            VisitorsY.GridLines = gridLines;
        }
    }

    private void AxisOptionToggle_Toggled(object sender, RoutedEventArgs e)
    {
        if (!IsLoaded)
        {
            return;
        }

        foreach (CartesianAxis axis in new CartesianAxis[] { QuarterX, VisitorsY })
        {
            axis.IsVisible = AxesVisibleToggle.IsOn;
            axis.ShowTickLabels = TickLabelsToggle.IsOn;
            axis.ShowTickMarks = TickMarksToggle.IsOn;
        }
    }

    private void SeriesVisibilityCheckBox_Click(object sender, RoutedEventArgs e)
    {
        ForecastSeries.IsVisible = ForecastCheckBox.IsChecked == true;
        ActualSeries.IsVisible = ActualCheckBox.IsChecked == true;
        TargetSeries.IsVisible = TargetCheckBox.IsChecked == true;
    }

    private void DashStyleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (IsLoaded && GetSelectedTag<StrokeDashStyle>(DashStyleComboBox) is StrokeDashStyle dashStyle)
        {
            VisitsSeries.StrokeDashStyle = dashStyle;
        }
    }

    private void StrokeThicknessSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        if (IsLoaded)
        {
            VisitsSeries.StrokeThickness = e.NewValue;
        }
    }

    private void MarkerShapeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (IsLoaded && GetSelectedTag<MarkerShape>(MarkerShapeComboBox) is MarkerShape markerShape)
        {
            VisitsSeries.MarkerShape = markerShape;
        }
    }

    private void DataPointToggle_Toggled(object sender, RoutedEventArgs e)
    {
        if (IsLoaded)
        {
            VisitsSeries.ShowDataMarkers = DataMarkersToggle.IsOn;
            VisitsSeries.ShowDataLabels = DataLabelsToggle.IsOn;
        }
    }

    private void HighlightPeakVisits()
    {
        uint peakIndex = (uint)Array.IndexOf(DailyVisits, DailyVisits.Max());

        // The series leaves DataLabelBrush unset, so passing it keeps the theme-aware default.
        VisitsSeries.DataLabelOverrides[peakIndex] = new DataLabelOverride("Peak", VisitsSeries.DataLabelBrush);
        VisitsSeries.DataMarkerOverrides[peakIndex] = new DataMarkerOverride(MarkerShape.Diamond, VisitsSeries.DataMarkerBrush);
    }

    private void CreateTimeSeriesChart()
    {
        ChartSamples day = new() { ItemsSource = ReadingDates };
        ChartSamples reading = new() { ItemsSource = TemperatureReadings };

        _timeSeriesAxis = new DateTimeAxis
        {
            Label = "Date",
            Minimum = new DateTimeOffset(2026, 7, 27, 0, 0, 0, TimeSpan.Zero),
            Maximum = new DateTimeOffset(2026, 9, 7, 0, 0, 0, TimeSpan.Zero),
            IntervalType = DateTimeIntervalType.Week,
            LabelFormat = "month day",
        };

        LinearAxis temperatureY = new()
        {
            Label = "Temperature (°F)",
            GridLines = GridLines.Major,
        };

        TimeSeriesChart.Data.Add(day);
        TimeSeriesChart.Data.Add(reading);
        TimeSeriesChart.Axes.Add(_timeSeriesAxis);
        TimeSeriesChart.Axes.Add(temperatureY);
        TimeSeriesChart.Series.Add(new AreaSeries
        {
            Title = "Temperature",
            XAxis = _timeSeriesAxis,
            YAxis = temperatureY,
            XValues = day,
            YValues = reading,
        });
    }

    private void DateTimeAxisOption_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!IsLoaded || _timeSeriesAxis is null)
        {
            return;
        }

        if (GetSelectedTag<DateTimeIntervalType>(IntervalTypeComboBox) is DateTimeIntervalType intervalType)
        {
            _timeSeriesAxis.IntervalType = intervalType;
        }

        if (LabelFormatComboBox?.SelectedItem is ComboBoxItem { Tag: string labelFormat })
        {
            _timeSeriesAxis.LabelFormat = labelFormat;
        }
    }

    private void AddResponseTimeSampleButton_Click(object sender, RoutedEventArgs e)
    {
        double next = _incomingResponseTimes[_nextResponseTimeIndex];
        _nextResponseTimeIndex = (_nextResponseTimeIndex + 1) % _incomingResponseTimes.Length;

        // Assign a new array so the chart redraws. Keep the eight most recent values.
        ResponseTimes = [.. ResponseTimes.TakeLast(7), next];

        UpdateLiveSeriesSummary();
    }

    private void UpdateLiveSeriesSummary()
    {
        LiveSeriesSummary = $"Latest: {ResponseTimes[^1]:F0} ms ({ResponseTimes.Length} samples)";
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}