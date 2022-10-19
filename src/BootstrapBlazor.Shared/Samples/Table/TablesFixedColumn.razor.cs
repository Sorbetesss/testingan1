﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

namespace BootstrapBlazor.Shared.Samples.Table;

/// <summary>
/// Fixed column example
/// </summary>
public partial class TablesFixedColumn
{
    [NotNull]
    private List<Foo>? Items { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<Foo>? LocalizerFoo { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<TablesFixedColumn>? Localizer { get; set; }

    /// <summary>
    /// OnInitialized method
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        Items = Foo.GenerateFoo(LocalizerFoo);
    }
}
