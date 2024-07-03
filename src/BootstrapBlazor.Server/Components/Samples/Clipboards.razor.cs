﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

namespace BootstrapBlazor.Server.Components.Samples;

/// <summary>
/// 剪切板示例
/// </summary>
public partial class Clipboards
{
    [Inject]
    [NotNull]
    private ClipboardService? ClipboardService { get; set; }

    [Inject]
    [NotNull]
    private ToastService? ToastService { get; set; }

    private string content { get; set; } = "Hello BootstrapBlazor";

    private async Task Copy()
    {
        await ClipboardService.Copy(content);
        await ToastService.Success("Clipboard", Localizer["ClipboardMessage", content]);
    }

    private async Task Get()
    {
        var text = await ClipboardService.GetText();
        if (!string.IsNullOrEmpty(text))
        {
            content = text;
            await ToastService.Success("Clipboard", Localizer["ClipboardGetTextMessage", text]);
        }
    }

    private MethodItem[] GetMethods() =>
    [
        new()
        {
            Name = "Copy",
            Description = Localizer["ClipboardCopyMethod"],
            Parameters = " — ",
            ReturnValue = "Task"
        },
        new()
        {
            Name = "GetText",
            Description = Localizer["ClipboardGetTextMethod"],
            Parameters = " — ",
            ReturnValue = "Task<string?>"
        }
    ];
}
