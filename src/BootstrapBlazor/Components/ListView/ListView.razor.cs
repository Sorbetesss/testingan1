﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

namespace BootstrapBlazor.Components;

/// <summary>
/// ListView 组件基类
/// </summary>
public partial class ListView<TItem> : BootstrapComponentBase where TItem : class, new()
{
    /// <summary>
    ///  Card组件样式
    /// </summary>
    protected virtual string? ClassString => CssBuilder.Default("listview")
        .AddClass("is-vertical", IsVertical)
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    /// <summary>
    /// 获得/设置 CardHeard
    /// </summary>
    [Parameter]
    public RenderFragment? HeaderTemplate { get; set; }

    /// <summary>
    /// 获得/设置 BodyTemplate
    /// </summary>
    [Parameter]
    public RenderFragment<TItem>? BodyTemplate { get; set; }

    /// <summary>
    /// 获得/设置 FooterTemplate
    /// </summary>
    [Parameter]
    public RenderFragment? FooterTemplate { get; set; }

    /// <summary>
    /// 获得/设置 数据源
    /// </summary>
    [Parameter]
    public IEnumerable<TItem>? Items { get; set; }

    /// <summary>
    /// 获得/设置 是否分页 默认为 false 不分页
    /// </summary>
    [Parameter]
    public bool Pageable { get; set; }

    /// <summary>
    /// 获得/设置 分组名称
    /// </summary>
    [Parameter]
    public Func<TItem, object?>? GroupName { get; set; }

    /// <summary>
    /// 异步查询回调方法
    /// </summary>
    [Parameter]
    public Func<QueryPageOptions, Task<QueryData<TItem>>>? OnQueryAsync { get; set; }

    /// <summary>
    /// 获得/设置 ListView组件元素点击时回调委托
    /// </summary>
    [Parameter]
    public Func<TItem, Task>? OnListViewItemClick { get; set; }

    /// <summary>
    /// 获得/设置 是否为竖向排列 默认为 false
    /// </summary>
    [Parameter]
    public bool IsVertical { get; set; }

    /// <summary>
    /// 获得/设置 每页数据数量 默认 20
    /// </summary>
    [Parameter]
    public int PageItems { get; set; } = 20;

    /// <summary>
    /// 获得/设置 当前页码
    /// </summary>
    private int PageIndex { get; set; }

    /// <summary>
    /// 获得/设置 数据总条目
    /// </summary>
    protected int TotalCount { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="firstRender"></param>
    /// <returns></returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && Items == null)
        {
            await QueryAsync();
        }
    }

    /// <summary>
    /// 点击页码调用此方法
    /// </summary>
    /// <param name="pageIndex"></param>
    protected Task OnPageLinkClick(int pageIndex) => QueryAsync(pageIndex);

    /// <summary>
    /// 查询按钮调用此方法
    /// </summary>
    /// <returns></returns>
    public async Task QueryAsync(int pageIndex = 1)
    {
        PageIndex = pageIndex;
        await QueryData();
        StateHasChanged();
    }

    /// <summary>
    /// 调用 OnQuery 回调方法获得数据源
    /// </summary>
    protected async Task QueryData()
    {
        QueryData<TItem>? queryData = null;
        if (OnQueryAsync != null)
        {
            queryData = await OnQueryAsync(new QueryPageOptions()
            {
                PageIndex = PageIndex,
                PageItems = PageItems,
            });
        }
        if (queryData != null)
        {
            Items = queryData.Items;
            TotalCount = queryData.TotalCount;
        }
    }

    private int PageCount => (int)Math.Ceiling(TotalCount * 1.0 / PageItems);

    /// <summary>
    /// 点击元素事件
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    protected async Task OnClick(TItem item)
    {
        if (OnListViewItemClick != null)
        {
            await OnListViewItemClick(item);
        }
    }
}
