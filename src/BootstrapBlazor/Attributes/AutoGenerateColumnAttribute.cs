﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using Microsoft.AspNetCore.Components;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace BootstrapBlazor.Components
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoGenerateColumnAttribute : Attribute, ITableColumn
    {
        /// <summary>
        /// 获得/设置 显示顺序 ，规则如下：
        /// <para></para>
        /// &gt;0时排前面，1,2,3...
        /// <para></para>
        /// =0时排中间(默认)
        /// <para></para>
        /// &lt;0时排后面，...-3,-2,-1
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 获得/设置 是否忽略 默认为 false 不忽略
        /// </summary>
        public bool Ignore { get; set; }

        /// <summary>
        /// 获得/设置 当前列是否可编辑 默认为 true 当设置为 false 时自动生成编辑 UI 不生成此列
        /// </summary>
        public bool Editable { get; set; } = true;

        /// <summary>
        /// 获得/设置 当前列编辑时是否只读 默认为 false
        /// </summary>
        public bool Readonly { get; set; }

        /// <summary>
        /// 获得/设置 是否允许排序 默认为 false
        /// </summary>
        public bool Sortable { get; set; }

        /// <summary>
        /// 获得/设置 是否为默认排序列 默认为 false
        /// </summary>
        public bool DefaultSort { get; set; }

        /// <summary>
        /// 获得/设置 是否为默认排序规则 默认为 SortOrder.Unset
        /// </summary>
        public SortOrder DefaultSortOrder { get; set; }

        /// <summary>
        /// 获得/设置 是否允许过滤数据 默认为 false
        /// </summary>
        public bool Filterable { get; set; }

        /// <summary>
        /// 获得/设置 是否参与搜索 默认为 false
        /// </summary>
        public bool Searchable { get; set; }

        /// <summary>
        /// 获得/设置 列宽
        /// </summary>
        public int Width { get; set; }

        int? ITableColumn.Width
        {
            get => Width <= 0 ? null : Width;
            set => Width = value == null ? 0 : Width;
        }

        /// <summary>
        /// 获得/设置 是否固定本列 默认 false 不固定
        /// </summary>
        public bool Fixed { get; set; }

        /// <summary>
        /// 获得/设置 列是否显示 默认为 true 可见的
        /// </summary>
        public bool Visible { get; set; } = true;

        /// <summary>
        /// 获得/设置 本列是否允许换行 默认为 false
        /// </summary>
        public bool AllowTextWrap { get; set; }

        /// <summary>
        /// 获得/设置 本列文本超出省略 默认为 false
        /// </summary>
        public bool TextEllipsis { get; set; }

        /// <summary>
        /// 获得/设置 列 td 自定义样式 默认为 null 未设置
        /// </summary>
        public string? CssClass { get; set; }

        /// <summary>
        /// 获得/设置 显示节点阈值 默认值 BreakPoint.None 未设置
        /// </summary>
        public BreakPoint ShownWithBreakPoint { get; set; }

        /// <summary>
        /// 获得/设置 格式化字符串 如时间类型设置 yyyy-MM-dd
        /// </summary>
        public string? FormatString { get; set; }

        /// <summary>
        /// 获得/设置 文字对齐方式 默认为 Alignment.None
        /// </summary>
        public Alignment Align { get; set; }

        /// <summary>
        /// 获得/设置 字段鼠标悬停提示
        /// </summary>
        public bool ShowTips { get; set; }

        /// <summary>
        /// 获得/设置 列格式化回调委托
        /// </summary>
        public Func<object?, Task<string>>? Formatter { get; set; }

        /// <summary>
        /// 获得/设置 编辑模板
        /// </summary>
        public RenderFragment<object>? EditTemplate { get; set; }

        /// <summary>
        /// 获得/设置 显示模板
        /// </summary>
        public RenderFragment<object>? Template { get; set; }

        /// <summary>
        /// 获得/设置 搜索模板
        /// </summary>
        public RenderFragment<object>? SearchTemplate { get; set; }

        /// <summary>
        /// 获得/设置 过滤模板
        /// </summary>
        public RenderFragment? FilterTemplate { get; set; }

        /// <summary>
        /// 获得/设置 步长 默认为 1
        /// </summary>
        public object? Step { get; set; }

        /// <summary>
        /// 获得/设置 列过滤器
        /// </summary>
        public IFilter? Filter { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [NotNull]
        public Type? PropertyType { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Text { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [NotNull]
        internal string? FieldName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetDisplayName() => Text ?? "";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetFieldName() => FieldName;
    }
}
