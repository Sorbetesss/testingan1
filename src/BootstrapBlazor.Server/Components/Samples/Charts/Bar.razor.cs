﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

namespace BootstrapBlazor.Server.Components.Samples.Charts;

/// <summary>
/// Bar 图表示例
/// </summary>
public partial class Bar
{
    private Random Randomer { get; } = new Random();

    private int _barDatasetCount = 2;
    private int _barDataCount = 7;

    private int BarDatasetCount { get; set; } = 2;

    private int BarDataCount { get; set; } = 7;

    [NotNull]
    private Chart? BarChart { get; set; }

    [NotNull]
    private ConsoleLogger? Logger { get; set; }

    /// <summary>
    /// OnAfterRender
    /// </summary>
    /// <param name = "firstRender"></param>
    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);

        if (firstRender)
        {
            Logger.Log("Bar loading data ...");
        }
    }

    private Task OnAfterInit()
    {
        Logger.Log("Bar initialization is complete");
        return Task.CompletedTask;
    }

    private Task OnAfterUpdate(ChartAction action) => InvokeAsync(() => Logger.Log($"Bar Figure update data operation completed -- {action}"));

    private Task<ChartDataSource> OnInit(bool stacked, bool setTitle = true)
    {
        var ds = new ChartDataSource();
        if (setTitle)
        {
            ds.Options.Title = "Bar Histogram";
        }

        ds.Options.X.Title = "days";
        ds.Options.Y.Title = "Numerical value";
        ds.Options.X.Stacked = stacked;
        ds.Options.Y.Stacked = stacked;
        ds.Labels = Enumerable.Range(1, _barDataCount).Select(i => i.ToString());
        for (var index = 0; index < _barDatasetCount; index++)
        {
            ds.Data.Add(new ChartDataset() { Label = $"Set {index}", Data = Enumerable.Range(1, _barDataCount).Select(i => Randomer.Next(20, 37)).Cast<object>() });
        }

        return Task.FromResult(ds);
    }

    private CancellationTokenSource _chartCancellationTokenSource = new();

    private Task OnPlayChart()
    {
        _chartCancellationTokenSource = new CancellationTokenSource();
        return Task.Run(async () =>
        {
            while (!_chartCancellationTokenSource.IsCancellationRequested)
            {
                await Task.Delay(800, _chartCancellationTokenSource.Token);
                if (!_chartCancellationTokenSource.IsCancellationRequested)
                {
                    await Utility.RandomData(BarChart);
                }
            }
        });
    }

    private void OnStopChart() => _chartCancellationTokenSource.Cancel();

    /// <summary>
    /// 强刷控件,重新初始化控件外观
    /// </summary>
    private Task OnReloadChart()
    {
        BarDataCount = Randomer.Next(5, 15);
        BarChart.Reload();
        return Task.CompletedTask;
    }

    private Task<ChartDataSource> OnInitTwoYAxes(bool stacked, bool setTitle = true)
    {
        var ds = new ChartDataSource();
        if (setTitle)
        {
            ds.Options.Title = "Bar Histogram";
        }
        ds.Options.X.Title = "days";
        ds.Options.Y.Title = "Numerical value";
        ds.Options.X.Stacked = stacked;
        ds.Options.Y.Stacked = stacked;
        ds.Options.Y2.Title = "Y2 value";
        ds.Options.Y2.PositionLeft = false;

        ds.Labels = Enumerable.Range(1, BarDataCount).Select(i => i.ToString());
        var index = 0;
        ds.Data.Add(new ChartDataset()
        {
            Label = $"Y2 Set {index}",
            IsAxisY2 = index == 0,
            Data = Enumerable.Range(1, BarDataCount).Select(i => Randomer.Next(20, 7000)).Cast<object>()
        });

        for (index = 1; index < BarDatasetCount; index++)
        {
            ds.Data.Add(new ChartDataset()
            {
                Label = $"Y Set {index}",
                IsAxisY2 = index == 0,
                Data = Enumerable.Range(1, BarDataCount).Select(i => Randomer.Next(20, 37)).Cast<object>()
            });
        }
        return Task.FromResult(ds);
    }

    private Task<ChartDataSource> OnInitStack(bool stacked, bool setTitle = true)
    {
        var ds = new ChartDataSource();
        if (setTitle)
        {
            ds.Options.Title = "Bar Histogram";
        }
        ds.Options.X.Title = "days";
        ds.Options.Y.Title = "Numerical value";
        ds.Options.X.Stacked = stacked;
        ds.Options.Y.Stacked = stacked;
        ds.Labels = Enumerable.Range(1, BarDataCount).Select(i => i.ToString());
        for (var index = 0; index < BarDatasetCount; index++)
        {
            ds.Data.Add(new ChartDataset()
            {
                Label = $"Set {index}",
                Data = Enumerable.Range(1, BarDataCount).Select(i => Randomer.Next(20, 37)).Cast<object>()
            });
        }
        return Task.FromResult(ds);
    }

    private Task<ChartDataSource> OnInitAspectRatio(bool stacked, bool setTitle = true)
    {
        var ds = new ChartDataSource();
        if (setTitle)
        {
            ds.Options.Title = "Bar Histogram";
        }
        ds.Options.X.Title = "days";
        ds.Options.Y.Title = "Numerical value";
        ds.Options.X.Stacked = stacked;
        ds.Options.Y.Stacked = stacked;
        ds.Labels = Enumerable.Range(1, BarDataCount).Select(i => i.ToString());
        for (var index = 0; index < BarDatasetCount; index++)
        {
            ds.Data.Add(new ChartDataset()
            {
                Label = $"Set {index}",
                Data = Enumerable.Range(1, BarDataCount).Select(i => Randomer.Next(20, 37)).Cast<object>()
            });
        }
        return Task.FromResult(ds);
    }
}
