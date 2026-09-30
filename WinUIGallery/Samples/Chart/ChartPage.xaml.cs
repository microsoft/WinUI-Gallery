// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Charts;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ChartSamples = Microsoft.UI.Xaml.Controls.Charts.Samples;

namespace WinUIGallery.ControlPages;

public sealed partial class ChartPage : Page, INotifyPropertyChanged
{
    private readonly double[] _incomingResponseTimes = [176, 169, 181, 165, 172];
    private int _nextResponseTimeIndex;
    private string _liveSeriesSummary = string.Empty;

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

    public List<DateTimeOffset> ReadingDates { get; } =
    [
        new DateTimeOffset(2026, 8, 3, 0, 0, 0, TimeSpan.Zero),
        new DateTimeOffset(2026, 8, 10, 0, 0, 0, TimeSpan.Zero),
        new DateTimeOffset(2026, 8, 17, 0, 0, 0, TimeSpan.Zero),
        new DateTimeOffset(2026, 8, 24, 0, 0, 0, TimeSpan.Zero),
        new DateTimeOffset(2026, 8, 31, 0, 0, 0, TimeSpan.Zero),
    ];

    public double[] TemperatureReadings { get; } = [72, 76, 74, 79, 77];

    public ObservableCollection<double> ResponseTimes { get; } =
    [
        184,
        179,
        173,
        171,
        168,
    ];

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

        SolidColorBrush labelHighlight = new(Colors.Black);
        SolidColorBrush markerHighlight = new(Colors.Magenta);
        TargetSeries.DataLabelOverrides[2] = new DataLabelOverride("Peak", labelHighlight);
        TargetSeries.DataMarkerOverrides[2] = new DataMarkerOverride(MarkerShape.Asterisk, markerHighlight);

        ChartSamples day = new() { ItemsSource = ReadingDates };
        ChartSamples reading = new() { ItemsSource = TemperatureReadings };
        DateTimeAxis dayX = new()
        {
            Label = "Date",
            IntervalType = DateTimeIntervalType.Week,
            LabelFormat = "month day",
        };

        TimeSeriesChart.Data.Add(day);
        TimeSeriesChart.Data.Add(reading);
        TimeSeriesChart.Axes.Add(dayX);
        TimeSeriesChart.Series.Add(new AreaSeries
        {
            Title = "Temperature",
            XAxis = dayX,
            XValues = day,
            YValues = reading,
        });

        UpdateLiveSeriesSummary();
    }

    private void AddResponseTimeSampleButton_Click(object sender, RoutedEventArgs e)
    {
        ResponseTimes.Add(_incomingResponseTimes[_nextResponseTimeIndex]);
        _nextResponseTimeIndex = (_nextResponseTimeIndex + 1) % _incomingResponseTimes.Length;

        if (ResponseTimes.Count > 8)
        {
            ResponseTimes.RemoveAt(0);
        }
        UpdateLiveSeriesSummary();
    }

    private void UpdateLiveSeriesSummary()
    {
        LiveSeriesSummary = $"Current response times in milliseconds: {string.Join(", ", ResponseTimes.Select(value => value.ToString("F0")))}. Latest: {ResponseTimes[^1]:F0} ms.";
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
