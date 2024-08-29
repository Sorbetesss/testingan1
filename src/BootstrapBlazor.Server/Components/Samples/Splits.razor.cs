﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

namespace BootstrapBlazor.Server.Components.Samples;

/// <summary>
/// Splits
/// </summary>
public sealed partial class Splits
{
    private bool _showBarHandle = true;

    private string? _barHandleText;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        _barHandleText = _showBarHandle ? Localizer["SplitsBarHandleShow"] : Localizer["SplitsBarHandleHide"];
    }

    private Task OnShowBarHandle(bool v)
    {
        _showBarHandle = v;
        _barHandleText = _showBarHandle ? Localizer["SplitsBarHandleShow"] : Localizer["SplitsBarHandleHide"];
        StateHasChanged();
        return Task.CompletedTask;
    }

    /// <summary>
    /// 获得属性方法
    /// </summary>
    /// <returns></returns>
    private AttributeItem[] GetAttributes() =>
    [
        new()
        {
            Name = "IsVertical",
            Description = Localizer["SplitsIsVertical"],
            Type = "bool",
            ValueList = "true|false",
            DefaultValue = "false"
        },
        new()
        {
            Name = "ShowBarHandle",
            Description = Localizer["SplitsShowBarHandle"],
            Type = "bool",
            ValueList = "true|false",
            DefaultValue = "true"
        },
        new()
        {
            Name = "Basis",
            Description = Localizer["SplitsBasis"],
            Type = "string",
            ValueList = " — ",
            DefaultValue = "50%"
        },
        new()
        {
            Name = "FirstPanelTemplate",
            Description = Localizer["SplitsFirstPanelTemplate"],
            Type = "RenderFragment",
            ValueList = " — ",
            DefaultValue = " — "
        },
        new()
        {
            Name = "SecondPanelTemplate",
            Description = Localizer["SplitsSecondPanelTemplate"],
            Type = "RenderFragment",
            ValueList = " — ",
            DefaultValue = " — "
        },
        new()
        {
            Name = "IsCollapsible",
            Description = Localizer["SplitsIsCollapsible"],
            Type = "bool",
            ValueList = "true|false",
            DefaultValue = "false"
        },
        new()
        {
            Name = "IsKeepOriginalSize",
            Description = Localizer["SplitsIsKeepOriginalSize"],
            Type = "bool",
            ValueList = "true|false",
            DefaultValue = "true"
        }
    ];
}
