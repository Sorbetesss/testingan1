﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BootstrapBlazor.Components
{
    /// <summary>
    /// AutoComplete 组件基类
    /// </summary>
    public abstract class AutoCompleteBase : BootstrapInputBase<string>
    {
        private bool _isLoading;
        private bool _isShown;

        /// <summary>
        /// 获得 组件样式
        /// </summary>
        protected virtual string? ClassString => CssBuilder.Default("auto-complete")
            .AddClass("is-loading", _isLoading)
            .AddClass("is-complete", _isShown)
            .Build();

        /// <summary>
        /// 获得 最终候选数据源
        /// </summary>
        protected List<string> FilterItems { get; private set; } = new List<string>();

        /// <summary>
        /// 获得/设置 通过输入字符串获得匹配数据集合
        /// </summary>
        [Parameter]
        public IEnumerable<string> Items { get; set; } = new string[0];

        /// <summary>
        /// 获得/设置 无匹配数据时显示提示信息 默认提示"无匹配数据"
        /// </summary>
        [Parameter]
        public string NoDataTip { get; set; } = "无匹配数据";

        private string? _placeholder;
        /// <summary>
        /// 获得 PlaceHolder 属性
        /// </summary>
        [Parameter]
        public string? PlaceHolder
        {
            get
            {
                if (string.IsNullOrEmpty(_placeholder))
                {
                    _placeholder = "请输入";
                    if (AdditionalAttributes != null && AdditionalAttributes.TryGetValue("placeholder", out var ph) && !string.IsNullOrEmpty(Convert.ToString(ph)))
                    {
                        _placeholder = ph.ToString();
                    }
                }
                return _placeholder;
            }
            set
            {
                _placeholder = value;
            }
        }

        /// <summary>
        /// 是否开启模糊查询，默认为 false
        /// </summary>
        [Parameter]
        public bool IsLikeMatch { get; set; } = false;

        /// <summary>
        /// 匹配时是否忽略大小写，默认为 true
        /// </summary>
        [Parameter]
        public bool IgnoreCase { get; set; } = true;

        /// <summary>
        /// 自定义集合过滤规则
        /// </summary>
        [Parameter]
        public Func<Task<IEnumerable<string>>>? CustomFilter { get; set; }

        private string _selectedItem = "";
        /// <summary>
        /// 获得 候选项样式
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected string? ItemClassString(string item) => CssBuilder.Default("dropdown-item")
            .AddClass("active", item == _selectedItem)
            .Build();

        /// <summary>
        /// OnBlur 方法
        /// </summary>
        protected void OnBlur()
        {
            _selectedItem = "";
            _isShown = false;
        }

        /// <summary>
        /// 鼠标点击候选项时回调此方法
        /// </summary>
        protected Task OnItemClick(string val)
        {
            CurrentValue = val;
            return Task.CompletedTask;
        }

        /// <summary>
        /// OnKeyUp 方法
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected virtual async Task OnKeyUp(KeyboardEventArgs args)
        {
            if (!_isLoading)
            {
                _isLoading = true;
                if (CustomFilter != null)
                {
                    var items = await CustomFilter();
                    FilterItems = items.ToList();
                }
                else
                {
                    var comparison = IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
                    var items = IsLikeMatch ?
                        Items.Where(s => s.Contains(CurrentValueAsString, comparison)) :
                        Items.Where(s => s.StartsWith(CurrentValueAsString, comparison));
                    FilterItems = items.ToList();
                }
                _isLoading = false;
                _isShown = true;
            }

            var source = FilterItems;
            if (source.Any())
            {
                // 键盘向上选择
                if (args.Key == "ArrowUp")
                {
                    var index = Math.Max(0, Math.Min(source.Count - 1, source.IndexOf(_selectedItem) - 1));
                    _selectedItem = source[index];
                }
                else if (args.Key == "ArrowDown")
                {
                    var index = Math.Max(0, Math.Min(source.Count - 1, source.IndexOf(_selectedItem) + 1));
                    _selectedItem = source[index];
                }
                else if (args.Key == "Escape")
                {
                    OnBlur();
                }
                else if (args.Key == "Enter")
                {
                    if (!string.IsNullOrEmpty(_selectedItem))
                    {
                        CurrentValueAsString = _selectedItem;
                        OnBlur();
                    }
                }
            }
        }
    }
}
