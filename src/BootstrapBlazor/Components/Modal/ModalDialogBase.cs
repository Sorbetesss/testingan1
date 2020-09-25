﻿using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace BootstrapBlazor.Components
{
    /// <summary>
    /// ModalBody 组件内嵌与 Modal 组件内
    /// </summary>
    public class ModalDialogBase : BootstrapComponentBase
    {
        /// <summary>
        /// 获得 弹窗组件样式
        /// </summary>
        protected virtual string? ClassName => CssBuilder.Default("modal-dialog")
            .AddClass("modal-dialog-centered", IsCentered)
            .AddClass($"modal-{Size.ToDescriptionString()}", Size != Size.None)
            .AddClass("modal-dialog-scrollable", IsScrolling)
            .AddClass("is-draggable", IsDraggable)
            .AddClass("d-none", !IsShown)
            .Build();

        /// <summary>
        /// 获得/设置 是否显示对话框
        /// </summary>
        internal bool IsShown { get; set; }

        /// <summary>
        /// 获得/设置 弹窗标题
        /// </summary>
        [Parameter]
        public string Title { get; set; } = "未设置";

        /// <summary>
        /// 获得/设置 弹窗大小
        /// </summary>
        [Parameter]
        public Size Size { get; set; } = Size.Large;

        /// <summary>
        /// 获得/设置 是否垂直居中 默认为 true
        /// </summary>
        [Parameter]
        public bool IsCentered { get; set; }

        /// <summary>
        /// 获得/设置 是否弹窗正文超长时滚动
        /// </summary>
        [Parameter]
        public bool IsScrolling { get; set; }

        /// <summary>
        /// 获得/设置 是否可以拖拽弹窗
        /// </summary>
        [Parameter]
        public bool IsDraggable { get; set; }

        /// <summary>
        /// 获得/设置 是否显示关闭按钮
        /// </summary>
        [Parameter]
        public bool ShowCloseButton { get; set; } = true;

        /// <summary>
        /// 获得/设置 是否显示 Footer 默认为 true
        /// </summary>
        [Parameter]
        public bool ShowFooter { get; set; } = true;

        /// <summary>
        /// 获得/设置 弹窗内容相关数据 多用于传值
        /// </summary>
        [Parameter]
        public object? BodyContext { get; set; }

        /// <summary>
        /// 获得/设置 ModalBody 组件
        /// </summary>
        [Parameter]
        public RenderFragment? BodyTemplate { get; set; }

        /// <summary>
        /// 获得/设置 ModalFooter 组件
        /// </summary>
        [Parameter]
        public RenderFragment? FooterTemplate { get; set; }

        /// <summary>
        /// 获得/设置 关闭弹窗是回调委托
        /// </summary>
        [Parameter]
        public Func<Task>? OnClose { get; set; }

        /// <summary>
        /// 获得/设置 弹窗容器实例
        /// </summary>
        [CascadingParameter]
        public ModalBase? Modal { get; set; }

        /// <summary>
        /// OnInitialized 方法
        /// </summary>
        protected override void OnInitialized()
        {
            base.OnInitialized();

            Modal?.AddDialog(this);
        }

        /// <summary>
        /// 显示弹窗方法
        /// </summary>
        public void Show() => Modal?.ShowDialog(this);
    }
}
