﻿using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace BootstrapBlazor.Components
{
    /// <summary>
    /// PopoverConfirm 弹出窗组件
    /// </summary>
    public abstract class PopoverConfirmBase : BootstrapComponentBase
    {
        /// <summary>
        /// 获得/设置 PopoverConfirm 服务实例
        /// </summary>
        [Inject] PopoverService? PopoverService { get; set; }

        /// <summary>
        /// OnInitialized 方法
        /// </summary>
        protected override void OnInitialized()
        {
            base.OnInitialized();

            // 注册 Toast 弹窗事件
            PopoverService?.Register(() => StateHasChanged());
        }

        /// <summary>
        /// OnAfterRender 方法
        /// </summary>
        /// <param name="firstRender"></param>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            // 生成代码后，调用 javascript 进行弹窗操作
            if (PopoverService != null) await PopoverService.InvokeRun();
        }
    }
}
