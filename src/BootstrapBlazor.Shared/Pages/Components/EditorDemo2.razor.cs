﻿using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace BootstrapBlazor.Shared.Pages.Components
{
    /// <summary>
    /// 
    /// </summary>
    public partial class EditorDemo2
    {
#nullable disable
        private Logger Trace2 { get; set; }
#nullable restore

        private Dummy DummyModel = new Dummy();

        private IEnumerable<SelectedItem>? Educations { get; set; }

        private IEnumerable<SelectedItem> Hobbys = new List<SelectedItem>()
        {
            new SelectedItem("游泳", "游泳"),
            new SelectedItem("登山", "登山"),
            new SelectedItem("打球", "打球"),
            new SelectedItem("下棋", "下棋")
        };

        /// <summary>
        /// OnInitialized 方法
        /// </summary>
        protected override void OnInitialized()
        {
            base.OnInitialized();

            // 初始化参数
            Educations = typeof(EnumEducation).ToSelectList(new SelectedItem("", "请选择 ..."));
        }
        private Task OnValidSubmit(EditContext context)
        {
            Trace2.Log("OnValidSubmit 回调委托");
            return Task.CompletedTask;
        }

        private Task OnInvalidSubmit(EditContext context)
        {
            Trace2.Log("OnInvalidSubmit 回调委托");
            return Task.CompletedTask;
        }
    }
}
