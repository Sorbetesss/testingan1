﻿using Microsoft.AspNetCore.Components;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BootstrapBlazor.Components
{
    /// <summary>
    /// MenuLink 组件内部封装 NavLink 组件
    /// </summary>
    public sealed partial class MenuLink
    {
        private string? ClassString => CssBuilder.Default()
            .AddClass("active", Item.IsActive && !Item.IsDisabled)
            .AddClass("disabled", Item.IsDisabled)
            .AddClassFromAttributes(AdditionalAttributes)
            .Build();

        private string? GetHrefString => (DisableNavigation || Item.IsDisabled) ? null : (Item.Items.Any() ? "#" : Item.Url?.TrimStart('/'));

        /// <summary>
        /// 获得/设置 是否禁止导航 默认为 false 允许导航
        /// </summary>
        [Parameter]
        public bool DisableNavigation { get; set; }

#nullable disable
        /// <summary>
        /// 获得/设置 MenuItem 实例 不可为空
        /// </summary>
        [Parameter]
        public MenuItem Item { get; set; }
#nullable restore

        /// <summary>
        /// 获得/设置 点击菜单回调委托方法
        /// </summary>
        [Parameter]
        public Func<MenuItem, Task>? OnClick { get; set; }

        private async Task OnClickLink()
        {
            if (OnClick != null) await OnClick(Item);
        }
    }
}
