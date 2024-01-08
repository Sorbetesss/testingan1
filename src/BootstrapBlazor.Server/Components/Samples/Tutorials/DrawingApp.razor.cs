﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

namespace BootstrapBlazor.Server.Components.Samples.Tutorials;

/// <summary>
/// DrawingApp Tutorial
/// </summary>
public partial class DrawingApp
{
    /// <summary>
    /// LineThickness
    /// </summary>
    private int LineThickness { get; set; } = 2;

    /// <summary>
    /// DrawingColor
    /// </summary>
    private string DrawingColor { get; set; } = "#6610f2";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    protected override Task InvokeInitAsync() => InvokeVoidAsync("init", Id, LineThickness, DrawingColor);

    /// <summary>
    /// ChangeSize
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    private async Task ChangeSize(int val) => await InvokeVoidAsync("changeSize", val);

    /// <summary>
    /// ChangeColor
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    private async Task ChangeColor(string val) => await InvokeVoidAsync("changeColor", val);

    /// <summary>
    /// ClearRect
    /// </summary>
    /// <returns></returns>
    private async Task ClearCanvas()
    {
        await InvokeVoidAsync("clearRect", Id);
        await MessageService.Show(new MessageOption()
        {
            Content = "已清空画板",
            ForceDelay = true,
            Delay = 500
        });
    }

    /// <summary>
    /// DownloadImage
    /// </summary>
    /// <returns></returns>
    private async Task DownloadImage()
    {
        var base64String = await InvokeAsync<string>("exportImage", Id);
        if (!string.IsNullOrEmpty(base64String))
        {
            byte[] byteArray = Convert.FromBase64String(base64String.Replace("data:image/jpeg;base64,", ""));
            await DownloadService.DownloadFromByteArrayAsync("drawing-app.jpeg", byteArray);
            await MessageService.Show(new MessageOption()
            {
                Content = "已下载图片"
            });
        }
    }
}
