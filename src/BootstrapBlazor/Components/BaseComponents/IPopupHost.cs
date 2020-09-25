﻿using Microsoft.AspNetCore.Components;

namespace BootstrapBlazor.Components
{
    /// <summary>
    /// IPopupOption 接口定义
    /// </summary>
    public interface IPopupHost
    {
        /// <summary>
        /// 获得/设置 弹窗主体实例 默认为空
        /// </summary>
        public ComponentBase? Host { get; set; }
    }
}
