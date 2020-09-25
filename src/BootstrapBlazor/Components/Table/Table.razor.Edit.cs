﻿using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BootstrapBlazor.Components
{
    public partial class Table<TItem>
    {
        /// <summary>
        /// 获得 Checkbox 样式表集合
        /// </summary>
        /// <returns></returns>
        protected string? ButtonColumnClass => CssBuilder.Default("table-th-button")
            .Build();

        /// <summary>
        /// 获得/设置 删除按钮提示弹框实例
        /// </summary>
        protected PopoverConfirm? DeleteConfirm { get; set; }

        /// <summary>
        /// 获得/设置 删除按钮提示弹框实例
        /// </summary>
        protected PopoverConfirm? ButtonDeleteConfirm { get; set; }

        /// <summary>
        /// 获得/设置 编辑弹窗 Title 文字
        /// </summary>
        protected string? EditModalTitleString { get; set; }

        /// <summary>
        /// 获得/设置 被选中数据集合
        /// </summary>
        /// <value></value>
        protected List<TItem> SelectedItems { get; set; } = new List<TItem>();

        /// <summary>
        /// 获得 渲染模式
        /// </summary>
        protected TableRenderModel ActiveRenderModel => RenderModel switch
        {
            TableRenderModel.Auto => ScreenSize < RenderModelResponsiveWidth ? TableRenderModel.CardView : TableRenderModel.Table,
            _ => RenderModel
        };

        /// <summary>
        /// 获得/设置 客户端屏幕宽度
        /// </summary>
        protected decimal ScreenSize { get; set; }

        /// <summary>
        /// 获得/设置 组件渲染模式是否使用组件宽度来判断 默认为 false
        /// </summary>
        [Parameter]
        public bool UseComponentWidth { get; set; }

        /// <summary>
        /// 获得/设置 组件布局方式 默认为 Auto
        /// </summary>
        [Parameter]
        public TableRenderModel RenderModel { get; set; }

        /// <summary>
        /// 获得/设置 组件布局自适应切换阈值 默认为 768
        /// </summary>
        [Parameter]
        public decimal RenderModelResponsiveWidth { get; set; } = 768;

        /// <summary>
        /// 获得/设置 编辑弹框是否 Body 出现滚动条 默认 false
        /// </summary>
        [Parameter]
        public bool ScrollingDialogContent { get; set; }

        /// <summary>
        /// 获得/设置 被选中的数据集合
        /// </summary>
        [Parameter]
        public IEnumerable<TItem> SelectedRows
        {
            get
            {
                return SelectedItems;
            }
            set
            {
                if (SelectedItems != value) SelectedItems = value.ToList();
            }
        }

        /// <summary>
        /// 获得/设置 被选中的数据集合回调委托
        /// </summary>
        [Parameter]
        public EventCallback<IEnumerable<TItem>> SelectedRowsChanged { get; set; }

        /// <summary>
        /// 获得/设置 行样式格式回调委托
        /// </summary>
        [Parameter]
        public Func<TItem, string?>? SetRowClassFormatter { get; set; }

        /// <summary>
        /// 获得/设置 编辑数据弹窗 Title
        /// </summary>
        [Parameter]
        public string EditModalTitle { get; set; } = "编辑数据窗口";

        /// <summary>
        /// 获得/设置 新建数据弹窗 Title
        /// </summary>
        [Parameter]
        public string AddModalTitle { get; set; } = "新建数据窗口";

#nullable disable
        /// <summary>
        /// 获得/设置 EditModel 实例
        /// </summary>
        [Parameter]
        public TItem EditModel { get; set; }

        /// <summary>
        /// 获得/设置 EditTemplate 实例
        /// </summary>
        [Parameter]
        public RenderFragment<TItem> EditTemplate { get; set; }

        /// <summary>
        /// 获得/设置 RowButtonTemplate 实例
        /// </summary>
        [Parameter]
        public RenderFragment<TItem> RowButtonTemplate { get; set; }
#nullable restore

        /// <summary>
        /// 获得/设置 行内功能按钮列头文本 默认为 操作
        /// </summary>
        [Parameter]
        public string ColumnButtonTemplateHeaderText { get; set; } = "操作";

        /// <summary>
        /// 获得/设置 点击行即选中本行 默认为 false
        /// </summary>
        [Parameter]
        public bool ClickToSelect { get; set; }

        /// <summary>
        /// 获得/设置 单选模式下双击即编辑本行 默认为 false
        /// </summary>
        [Parameter]
        public bool DoubleClickToEdit { get; set; }

        /// <summary>
        /// 单选模式下选择行时调用此方法
        /// </summary>
        /// <param name="val"></param>
        protected virtual async Task ClickRow(TItem val)
        {
            if (ClickToSelect)
            {
                // 反转选择
                if (!IsMultipleSelect)
                {
                    SelectedItems.Clear();
                }
                else if (SelectedItems.Contains(val))
                {
                    SelectedItems.Remove(val);
                }

                SelectedItems.Add(val);

                if (SelectedRowsChanged.HasDelegate) await SelectedRowsChanged.InvokeAsync(SelectedRows);

                StateHasChanged();
            }

            if (OnClickRowCallback != null) await OnClickRowCallback(val);
        }

        /// <summary>
        /// 检查当前行是否被选中方法
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        protected virtual bool CheckActive(TItem val) => SelectedItems.Contains(val);

        /// <summary>
        /// 查询按钮调用此方法
        /// </summary>
        /// <returns></returns>
        public async Task QueryAsync()
        {
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
                    SearchText = SearchText,
                    SortOrder = SortOrder,
                    SortName = SortName,
                    Filters = Filters.Values,
                    SearchModel = SearchModel
                });
            }
            if (queryData != null)
            {
                Items = queryData.Items;
                TotalCount = queryData.TotalCount;
                IsFiltered = queryData.IsFiltered;
                IsSorted = queryData.IsSorted;
                IsSearch = queryData.IsSearch;

                // 外部未过滤，内部自行过滤
                if (!IsFiltered && Filters.Any())
                {
                    Items = Items.Where(Filters.Values.GetFilterFunc<TItem>());
                    TotalCount = Items.Count();
                }

                // 外部未处理排序，内部自行排序
                if (!IsSorted && SortOrder != SortOrder.Unset && !string.IsNullOrEmpty(SortName))
                {
                    var invoker = SortLambdaCache.GetOrAdd(typeof(TItem), key => Items.GetSortLambda().Compile());
                    Items = invoker(Items, SortName, SortOrder);
                }
            }
        }

        private static readonly ConcurrentDictionary<Type, Func<IEnumerable<TItem>, string, SortOrder, IEnumerable<TItem>>> SortLambdaCache = new ConcurrentDictionary<Type, Func<IEnumerable<TItem>, string, SortOrder, IEnumerable<TItem>>>();

        /// <summary>
        /// 行尾列编辑按钮点击回调此方法
        /// </summary>
        /// <param name="item"></param>
        protected async Task ClickEditButton(TItem item)
        {
            SelectedItems.Clear();
            SelectedItems.Add(item);

            // 更新行选中状态
            await EditAsync();
        }

        /// <summary>
        /// 双击行回调此方法
        /// </summary>
        /// <param name="item"></param>
        protected async Task DoubleClickRow(TItem item)
        {
            if (DoubleClickToEdit)
            {
                await ClickEditButton(item);
            }

            if (OnDoubleClickRowCallback != null) await OnDoubleClickRowCallback(item);
        }

        /// <summary>
        /// 行尾列按钮点击回调此方法
        /// </summary>
        /// <param name="item"></param>
        protected Task<bool> ClickDeleteButton(TItem item)
        {
            SelectedItems.Clear();
            SelectedItems.Add(item);

            return Task.FromResult(true);
        }
    }
}
