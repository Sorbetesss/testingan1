﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using Microsoft.Extensions.Localization;

namespace BootstrapBlazor.Components;

/// <summary>
/// Select 组件实现类
/// </summary>
/// <typeparam name="TValue"></typeparam>
[JSModuleAutoLoader("select-tree")]
public partial class SelectTree<TValue> : IModelEqualityComparer<TValue>
{
    /// <summary>
    /// 获得 样式集合
    /// </summary>
    private string? ClassName => CssBuilder.Default("select dropdown select-tree")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    /// <summary>
    /// 获得 样式集合
    /// </summary>
    private string? InputClassName => CssBuilder.Default("form-select form-control")
        .AddClass($"border-{Color.ToDescriptionString()}", Color != Color.None && !IsDisabled && !IsValid.HasValue)
        .AddClass($"border-success", IsValid.HasValue && IsValid.Value)
        .AddClass($"border-danger", IsValid.HasValue && !IsValid.Value)
        .AddClass(CssClass).AddClass(ValidCss)
        .Build();

    /// <summary>
    /// 获得 样式集合
    /// </summary>
    private string? AppendClassName => CssBuilder.Default("form-select-append")
        .AddClass($"text-{Color.ToDescriptionString()}", Color != Color.None && !IsDisabled && !IsValid.HasValue)
        .AddClass($"text-success", IsValid.HasValue && IsValid.Value)
        .AddClass($"text-danger", IsValid.HasValue && !IsValid.Value)
        .Build();

    /// <summary>
    /// 获得/设置 颜色 默认 Color.None 无设置
    /// </summary>
    [Parameter]
    public Color Color { get; set; }

    /// <summary>
    /// 获得 PlaceHolder 属性
    /// </summary>
    [Parameter]
    public string? PlaceHolder { get; set; }

    /// <summary>
    /// 获得/设置 字符串比较规则 默认 StringComparison.OrdinalIgnoreCase 大小写不敏感 
    /// </summary>
    [Parameter]
    public StringComparison StringComparison { get; set; } = StringComparison.OrdinalIgnoreCase;

    /// <summary>
    /// 获得/设置 带层次数据集合
    /// </summary>
    [Parameter]
    [NotNull]
#if NET6_0_OR_GREATER
    [EditorRequired]
#endif
    public List<TreeViewItem<TValue>>? Items { get; set; }

    /// <summary>
    /// SelectedItemChanged 回调方法
    /// </summary>
    [Parameter]
    public Func<TValue, Task>? OnSelectedItemChanged { get; set; }

    /// <summary>
    /// 获得/设置 点击节点获取子数据集合回调方法
    /// </summary>
    [Parameter]
    [NotNull]
    public Func<TreeViewItem<TValue>, Task<IEnumerable<TreeViewItem<TValue>>>>? OnExpandNodeAsync { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    [Parameter]
    public Type CustomKeyAttribute { get; set; } = typeof(KeyAttribute);

    /// <summary>
    /// 获得/设置 比较数据是否相同回调方法 默认为 null
    /// </summary>
    /// <remarks>提供此回调方法时忽略 <see cref="CustomKeyAttribute"/> 属性</remarks>
    [Parameter]
    [NotNull]
    public Func<TValue, TValue, bool>? ModelEqualityComparer { get; set; }

    /// <summary>
    /// 获得/设置 是否显示 Icon 图标 默认 false 不显示
    /// </summary>
    [Parameter]
    public bool ShowIcon { get; set; }

    /// <summary>
    /// 获得/设置 下拉箭头 Icon 图标 默认 "fa-solid fa-angle-up"
    /// </summary>
    [Parameter]
    public string? DropdownIcon { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<SelectTree<TValue>>? Localizer { get; set; }

    /// <summary>
    /// 获得 input 组件 Id 方法
    /// </summary>
    /// <returns></returns>
    protected override string? RetrieveId() => InputId;

    /// <summary>
    /// 获得/设置 Select 内部 Input 组件 Id
    /// </summary>
    private string? InputId => $"{Id}_input";

    /// <summary>
    /// 获得/设置 上次选择值
    /// </summary>
    private TValue? SelectedValue { get; set; }

    /// <summary>
    /// 获得/设置 上次选项
    /// </summary>
    private TreeViewItem<TValue>? SelectedItem { get; set; }

    private List<TreeViewItem<TValue>>? ItemCache { get; set; }

    [NotNull]
    private List<TreeViewItem<TValue>>? ExpandedItemsCache { get; set; }

    /// <summary>
    /// OnParametersSet 方法
    /// </summary>
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        DropdownIcon ??= "fa-solid fa-angle-up";
        PlaceHolder ??= Localizer[nameof(PlaceHolder)];

        Items ??= new List<TreeViewItem<TValue>>();

        if (Value == null)
        {
            // 组件未赋值 Value 通过 IsActive 设置默认值
            await TriggerItemChanged(s => s.IsActive);
        }
        else if (!Equals(Value, SelectedValue))
        {
            // 组件外部赋值 导致 Value 与 SelectedValue 不一致重新获取选项
            await TriggerItemChanged(s => Equals(s.Value, Value));
        }
    }

    private async Task TriggerItemChanged(Func<TreeViewItem<TValue>, bool> predicate)
    {
        var currentItem = GetExpandedItems().FirstOrDefault(predicate);
        if (currentItem != null)
        {
            await ItemChanged(currentItem);
        }
    }

    private IEnumerable<TreeViewItem<TValue>> GetExpandedItems()
    {
        if (ItemCache != Items)
        {
            ItemCache = Items;
            ExpandedItemsCache = TreeItemExtensions.GetAllItems(ItemCache).ToList();
        }
        return ExpandedItemsCache;
    }

    /// <summary>
    /// 下拉框选项点击时调用此方法
    /// </summary>
    private async Task OnItemClick(TreeViewItem<TValue> item)
    {
        if (!Equals(item.Value, SelectedValue))
        {
            await ItemChanged(item);
            StateHasChanged();
        }
    }

    /// <summary>
    /// 选中项更改处理方法
    /// </summary>
    /// <returns></returns>
    private async Task ItemChanged(TreeViewItem<TValue> item)
    {
        SelectedItem = item;
        SelectedValue = item.Value;
        CurrentValue = item.Value;

        // 触发 SelectedItemChanged 事件
        if (OnSelectedItemChanged != null)
        {
            await OnSelectedItemChanged.Invoke(CurrentValue);
        }
    }

    /// <summary>
    /// 比较数据是否相同
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool Equals(TValue? x, TValue? y) => this.Equals<TValue>(x, y);
}
