﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

namespace BootstrapBlazor.Server.Components.Samples;

/// <summary>
/// WinBox 示例组件
/// </summary>
public partial class WinBoxes
{
    [Inject, NotNull]
    private WinBoxService? WinBoxService { get; set; }

    private async Task OpenWinBox()
    {
        await WinBoxService.Show(new WinBoxOption());
    }
}
