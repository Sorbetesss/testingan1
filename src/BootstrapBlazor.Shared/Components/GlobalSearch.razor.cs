﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using BootstrapBlazor.Shared.Shared;
using Microsoft.Extensions.Options;

namespace BootstrapBlazor.Shared.Components;

/// <summary>
/// Pre 组件
/// </summary>
public partial class GlobalSearch
{
    [Inject]
    [NotNull]
    private IStringLocalizer<BaseLayout>? Localizer { get; set; }

    [Inject]
    [NotNull]
    private IOptionsMonitor<WebsiteOptions>? WebsiteOption { get; set; }

    [Inject]
    [NotNull]
    private NavigationManager? NavigationManager { get; set; }

    [NotNull]
    private List<string>? ComponentItems { get; set; }

    private IEnumerable<MenuItem> AvalidMenus => WebsiteOption.CurrentValue.SiteMenus.SelectMany(i => i.Items).Where(i => !string.IsNullOrEmpty(i.Url));

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override void OnInitialized()
    {
        ComponentItems = AvalidMenus.Select(i => i.Text!).ToList();
    }

    private Task OnSearch(string searchText)
    {
        var item = AvalidMenus.FirstOrDefault(i => i.Text!.Contains(searchText, StringComparison.OrdinalIgnoreCase));
        if (item != null && !string.IsNullOrEmpty(item.Url))
        {
            NavigationManager.NavigateTo(item.Url);
        }
        return Task.CompletedTask;
    }
}
