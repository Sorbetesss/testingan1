﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BootstrapBlazor.Components
{
    /// <summary>
    /// Captcha 滑块验证码组件
    /// </summary>
    public abstract class CaptchaBase : BootstrapComponentBase
    {
        private JSInterop<CaptchaBase>? Interop { get; set; }

        private static Random ImageRandomer { get; set; } = new Random();

        private int OriginX { get; set; }

        /// <summary>
        /// 获得/设置 Captcha DOM 元素实例
        /// </summary>
        protected ElementReference Captcha { get; set; }

        /// <summary>
        /// 获得 样式集合
        /// </summary>
        /// <returns></returns>
        protected string? ClassName => CssBuilder.Default()
            .AddClassFromAttributes(AdditionalAttributes)
            .Build();

        /// <summary>
        /// 获得 组件宽度
        /// </summary>
        protected string? StyleString => CssBuilder.Default()
            .AddClass($"width: {Width + 42}px;", Width > 0)
            .Build();

        /// <summary>
        /// 获得 加载图片失败样式
        /// </summary>
        protected string? FailedStyle => CssBuilder.Default()
            .AddClass($"width: {Width}px;", Width > 0)
            .AddClass($"height: {Height}px;", Height > 0)
            .Build();

        /// <summary>
        /// 获得/设置 验证码结果回调委托
        /// </summary>
        [Parameter]
        public Action<bool>? OnValid { get; set; }

        /// <summary>
        /// 获得/设置 图床路径 默认值为 images
        /// </summary>
        [Parameter]
        public string ImagesPath { get; set; } = "images";

        /// <summary>
        /// 获得/设置 图床路径 默认值为 Pic.jpg
        /// </summary>
        [Parameter]
        public string ImagesName { get; set; } = "Pic.jpg";

        /// <summary>
        /// 获得/设置 获取背景图方法委托
        /// </summary>
        [Parameter]
        public Func<string>? GetImageName { get; set; }

        /// <summary>
        /// 获得/设置 Header 显示文本
        /// </summary>
        [Parameter]
        public string HeaderText { get; set; } = "请完成安全验证";

        /// <summary>
        /// 获得/设置 Bar 显示文本
        /// </summary>
        [Parameter]
        public string BarText { get; set; } = "向右滑动填充拼图";

        /// <summary>
        /// 获得/设置 Bar 显示文本
        /// </summary>
        [Parameter]
        public string FailedText { get; set; } = "加载失败";

        /// <summary>
        /// 获得/设置 Bar 显示文本
        /// </summary>
        [Parameter]
        public string LoadText { get; set; } = "正在加载 ...";

        /// <summary>
        /// 获得/设置 Bar 显示文本
        /// </summary>
        [Parameter]
        public string TryText { get; set; } = "再试一次";

        /// <summary>
        /// 获得/设置 容错偏差
        /// </summary>
        [Parameter]
        public int Offset { get; set; } = 5;

        /// <summary>
        /// 获得/设置 图片宽度
        /// </summary>
        [Parameter]
        public int Width { get; set; } = 280;

        /// <summary>
        /// 获得/设置 图片高度
        /// </summary>
        [Parameter]
        public int Height { get; set; } = 155;

        /// <summary>
        /// OnAfterRender 方法
        /// </summary>
        /// <param name="firstRender"></param>
        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (firstRender) Reset();
        }

        /// <summary>
        /// 清除 ToastBox 方法
        /// </summary>
        [JSInvokable]
        public Task<bool> Verify(int offset, IEnumerable<int> trails)
        {
            var ret = Math.Abs(offset - OriginX) < Offset && CalcStddev(trails);
            OnValid?.Invoke(ret);
            return Task.FromResult(ret);
        }

        private bool CalcStddev(IEnumerable<int> trails)
        {
            var ret = false;
            if (trails.Any())
            {
                var average = trails.Sum() * 1.0 / trails.Count();
                var dev = trails.Select(t => t - average);
                var stddev = Math.Sqrt(dev.Sum() * 1.0 / dev.Count());
                ret = stddev != 0;
            }
            return ret;
        }

        private CaptchaOption GetCaptchaOption()
        {
            var option = new CaptchaOption()
            {
                Width = Width,
                Height = Height
            };
            option.BarWidth = option.SideLength + option.Diameter * 2 + 6; // 滑块实际边长
            var start = option.BarWidth + 10;
            var end = option.Width - start;
            option.OffsetX = Convert.ToInt32(Math.Ceiling(ImageRandomer.Next(0, 100) / 100.0 * (end - start) + start));
            OriginX = option.OffsetX;

            start = 10 + option.Diameter * 2;
            end = option.Height - option.SideLength - 10;
            option.OffsetY = Convert.ToInt32(Math.Ceiling(ImageRandomer.Next(0, 100) / 100.0 * (end - start) + start));

            if (GetImageName == null)
            {
                var index = Convert.ToInt32(ImageRandomer.Next(0, 8) / 1.0);
                var imageName = Path.GetFileNameWithoutExtension(ImagesName);
                var extendName = Path.GetExtension(ImagesName);
                var fileName = $"{imageName}{index}{extendName}";
                option.ImageUrl = Path.Combine(ImagesPath, fileName);
            }
            else
                option.ImageUrl = GetImageName();

            return option;
        }

        /// <summary>
        /// 点击刷新按钮时回调此方法
        /// </summary>
        protected void OnClickRefresh() => Reset();

        /// <summary>
        /// Dispose 方法
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing) Interop?.Dispose();
        }

        /// <summary>
        /// 重置组件方法
        /// </summary>
        public void Reset()
        {
            var option = GetCaptchaOption();
            if (Interop == null) Interop = new JSInterop<CaptchaBase>(JSRuntime);
            Interop?.Invoke(this, Captcha, "captcha", nameof(Verify), option);
        }
    }
}
