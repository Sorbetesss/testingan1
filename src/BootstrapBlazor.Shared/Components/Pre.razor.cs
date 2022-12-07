﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using BootstrapBlazor.Shared.Services;
using Microsoft.JSInterop;

namespace BootstrapBlazor.Shared.Components;

/// <summary>
/// Pre 组件
/// </summary>
public partial class Pre
{
    private bool Loaded { get; set; }

    private bool CanCopy { get; set; }

    /// <summary>
    /// 获得 样式集合
    /// </summary>
    /// <returns></returns>
    private string? ClassString => CssBuilder.Default("pre-code")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    [Inject]
    [NotNull]
    private CodeSnippetService? Example { get; set; }

    /// <summary>
    /// 获得/设置 子组件 CodeFile 为空时生效
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// 获得/设置 示例文档名称
    /// </summary>
    [Parameter]
    public string? CodeFile { get; set; }

    /// <summary>
    /// 获得/设置 代码段的标题
    /// </summary>
    [Parameter]
    public string? BlockTitle { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<Pre>? Localizer { get; set; }

    private string? LoadingText { get; set; }

    private string? TooltipTitle { get; set; }

    private string? CopiedText { get; set; }

    /// <summary>
    /// OnInitializedAsync 方法
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (ChildContent == null)
        {
            await GetCodeAsync();
        }
        else
        {
            Loaded = true;
            CanCopy = true;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        LoadingText ??= Localizer[nameof(LoadingText)];
        TooltipTitle ??= Localizer[nameof(TooltipTitle)];
        CopiedText ??= Localizer[nameof(CopiedText)];
    }

    /// <summary>
    /// OnAfterRender 方法
    /// </summary>
    /// <param name="firstRender"></param>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (Loaded)
        {
            await Hightlight();
        }
    }

    private async Task GetCodeAsync()
    {
        if (!string.IsNullOrEmpty(CodeFile))
        {
            var code = await Example.GetCodeAsync(CodeFile, BlockTitle);
            if (!string.IsNullOrEmpty(code))
            {
                ChildContent = builder =>
                {
                    builder.AddContent(0, code);
                };
            }
            CanCopy = !string.IsNullOrEmpty(code) && !code.StartsWith("Error: ");
        }
        Loaded = true;
    }

    private Task Hightlight() => InvokeExecuteAsync(Id, "highlight");
}
