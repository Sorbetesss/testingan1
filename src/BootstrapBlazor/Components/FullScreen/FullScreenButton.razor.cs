﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

namespace BootstrapBlazor.Components;

/// <summary>
/// 
/// </summary>
public partial class FullScreenButton
{
    /// <summary>
    /// 获得/设置 全屏图标 默认 fa-solid fa-maximize
    /// </summary>
    [Parameter]
    public string? FullScreenIcon { get; set; }

    [Inject]
    [NotNull]
    private FullScreenService? FullScrenService { get; set; }

    private string? ClassString => CssBuilder.Default("bb-fs btn btn-none")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? ButtonIconString => CssBuilder.Default()
        .AddClass(Icon)
        .AddClass("fs-off", !string.IsNullOrEmpty(FullScreenIcon))
        .Build();

    private string? FullScreenIconString => CssBuilder.Default("fs-on")
        .AddClass(FullScreenIcon)
        .Build();

    /// <summary>
    /// OnInitialized 方法
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        Icon ??= "fa-solid fa-maximize";
    }

    private Task ToggleFullScreen() => FullScrenService.Toggle();
}
