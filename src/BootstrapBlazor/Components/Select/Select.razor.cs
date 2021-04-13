﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace BootstrapBlazor.Components
{
    /// <summary>
    /// Select 组件实现类
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public sealed partial class Select<TValue>
    {
        private ElementReference SelectElement { get; set; }

        private JSInterop<Select<TValue>>? Interop { get; set; }

        /// <summary>
        /// 获得 样式集合
        /// </summary>
        private string? ClassName => CssBuilder.Default("form-select dropdown")
            .AddClass("is-disabled", IsDisabled)
            .AddClassFromAttributes(AdditionalAttributes)
            .Build();

        /// <summary>
        /// 获得 样式集合
        /// </summary>
        private string? InputClassName => CssBuilder.Default("form-control form-select-input")
            .AddClass($"border-{Color.ToDescriptionString()}", Color != Color.None && !IsDisabled)
            .AddClass(CssClass).AddClass(ValidCss)
            .Build();

        /// <summary>
        /// 获得 样式集合
        /// </summary>
        private string? AppendClassName => CssBuilder.Default("form-select-append")
            .AddClass($"text-{Color.ToDescriptionString()}", Color != Color.None && !IsDisabled)
            .Build();

        /// <summary>
        /// 设置当前项是否 Active 方法
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private string? ActiveItem(SelectedItem item) => CssBuilder.Default("dropdown-item")
            .AddClass("active", () => item.Value == CurrentValueAsString)
            .AddClass("is-disabled", item.IsDisabled)
            .Build();

        /// <summary>
        /// 获得/设置 搜索文本发生变化时回调此方法
        /// </summary>
        [Parameter]
        public Func<string, IEnumerable<SelectedItem>>? OnSearchTextChanged { get; set; }

        [Inject]
        [NotNull]
        private IStringLocalizer<Select<TValue>>? Localizer { get; set; }

        /// <summary>
        /// OnInitialized 方法
        /// </summary>
        protected override void OnInitialized()
        {
            base.OnInitialized();

            PlaceHolder ??= Localizer[nameof(PlaceHolder)];

            if (OnSearchTextChanged == null)
            {
                OnSearchTextChanged = text => Items.Where(i => i.Text.Contains(text, StringComparison.OrdinalIgnoreCase));
            }

            // 内置对枚举类型的支持
            var t = typeof(TValue);
            if (!Items.Any() && t.IsEnum())
            {
                var item = "";
                // 如果可为空枚举增加 请选择 ...
                if (NullableUnderlyingType != null)
                {
                    // 优先查找 placeholder 字样 如果未设置使用资源文件中
                    if (AdditionalAttributes != null && AdditionalAttributes.TryGetValue("placeholder", out var pl))
                    {
                        item = pl.ToString();
                    }
                    else
                    {
                        item = Localizer["PlaceHolder"].Value;
                    }
                }
                Items = typeof(TValue).ToSelectList(string.IsNullOrEmpty(item) ? null : new SelectedItem("", item));
            }
        }

        /// <summary>
        /// OnAfterRenderAsync 方法
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                if (Interop == null)
                {
                    Interop = new JSInterop<Select<TValue>>(JSRuntime);
                }
                await Interop.InvokeVoidAsync(this, SelectElement, "bb_select", nameof(ConfirmSelectedItem));

                if (SelectedItem != null && OnSelectedItemChanged != null)
                {
                    await OnSelectedItemChanged.Invoke(SelectedItem);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        [JSInvokable]
        public async Task ConfirmSelectedItem(int index)
        {
            var item = GetShownItems().ElementAt(index);
            await OnItemClick(item);
            StateHasChanged();
        }

        /// <summary>
        /// 获取显示的候选项集合
        /// </summary>
        /// <returns></returns>
        private IEnumerable<SelectedItem> GetShownItems()
        {
            var ret = GetItems();

            // handler SearchText
            if (!string.IsNullOrEmpty(SearchText))
            {
                ret = OnSearchTextChanged!.Invoke(SearchText);
            }
            return ret;
        }
    }
}
