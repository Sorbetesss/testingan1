﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

namespace BootstrapBlazor.Server.Components.Components;

/// <summary>
/// FooSortableListRightItem 组件
/// </summary>
public partial class FooSortableListRightItem
{
    /// <summary>
    /// 获得/设置 Foo 实例
    /// </summary>
    [Parameter, NotNull]
    public Foo? Value { get; set; }
}
