﻿using System.ComponentModel;

namespace BootstrapBlazor.Components
{
    /// <summary>
    /// Size 枚举类型
    /// </summary>
    public enum Size
    {
        /// <summary>
        /// 无设置
        /// </summary>
        None,

        /// <summary>
        /// xs 超小设置 576px
        /// </summary>
        [Description("xs")]
        ExtraSmall,

        /// <summary>
        /// sm 小设置 576px
        /// </summary>
        [Description("sm")]
        Small,

        /// <summary>
        /// md 中等设置 768px
        /// </summary>
        [Description("md")]
        Medium,

        /// <summary>
        /// lg 大设置 992px
        /// </summary>
        [Description("lg")]
        Large,

        /// <summary>
        /// xl 超大设置 1200px
        /// </summary>
        [Description("xl")]
        ExtraLarge
    }
}
