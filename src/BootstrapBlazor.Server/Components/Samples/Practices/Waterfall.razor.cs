﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using Microsoft.JSInterop;

namespace BootstrapBlazor.Server.Components.Samples.Practices;

/// <summary>
/// 瀑布流图片
/// </summary>
public partial class Waterfall
{
    [Inject]
    [NotNull]
    private IJSRuntime? JSRuntime { get; set; }

    [NotNull]
    private JSModule? Module { get; set; }

    private readonly string id = "b_waterfall";

    private bool _onload { get; set; } = false;

    private bool _showload { get; set; } = false;

    private readonly Random random = new();

    private readonly List<string> ImageList = [];

    private readonly List<string> _imageList =
    [
        "https://images.unsplash.com/photo-1489743342057-3448cc7c3bb9?ixlib=rb-0.3.5&ixid=eyJhcHBfaWQiOjEyMDd9&s=6d284a2efbca5f89528546307f7e7b87&auto=format&fit=crop&w=500&q=60",
        "https://images.unsplash.com/photo-1519996521430-02b798c1d881?ixlib=rb-0.3.5&ixid=eyJhcHBfaWQiOjEyMDd9&s=79f770fc1a5d8ff9b0eb033d0f09e15d&auto=format&fit=crop&w=500&q=60",
        "https://images.unsplash.com/photo-1505210512658-3ae58ae08744?ixlib=rb-0.3.5&ixid=eyJhcHBfaWQiOjEyMDd9&s=2ef2e23adda7b89a804cf232f57e3ca3&auto=format&fit=crop&w=333&q=80",
        "https://images.unsplash.com/photo-1488353816557-87cd574cea04?ixlib=rb-0.3.5&s=06371203b2e3ad3e241c45f4e149a1b3&auto=format&fit=crop&w=334&q=80",
        "https://images.unsplash.com/photo-1518450757707-d21c8c53c8df?ixlib=rb-0.3.5&ixid=eyJhcHBfaWQiOjEyMDd9&s=c88b5f311958f841525fdb01ab3b5deb&auto=format&fit=crop&w=500&q=60",
        "https://images.unsplash.com/photo-1483190656465-2c49e54d29f3?ixlib=rb-0.3.5&ixid=eyJhcHBfaWQiOjEyMDd9&s=7c4d831daffc28f6ce144ae9e44072e2&auto=format&fit=crop&w=500&q=60",
        "https://images.unsplash.com/photo-1501813531019-338a4182efb0?ixlib=rb-0.3.5&ixid=eyJhcHBfaWQiOjEyMDd9&s=ad934c7483b928cae6b0b9cde5ae3445&auto=format&fit=crop&w=500&q=60",
        "https://images.unsplash.com/photo-1513028179155-324cfec2566c?ixlib=rb-0.3.5&ixid=eyJhcHBfaWQiOjEyMDd9&s=32ce1df4016dadc177d6fce1b2df2429&auto=format&fit=crop&w=350&q=80",
        "https://images.unsplash.com/photo-1516601255109-506725109807?ixlib=rb-0.3.5&ixid=eyJhcHBfaWQiOjEyMDd9&s=ce4f3db9818f60686e8e9b62d4920ce5&auto=format&fit=crop&w=500&q=60",
        "https://images.unsplash.com/photo-1509233631037-deb7efd36207?ixlib=rb-0.3.5&ixid=eyJhcHBfaWQiOjEyMDd9&s=75a5d784cdfc8f5ced8a6fe26c6d921e&auto=format&fit=crop&w=500&q=60",
        "https://images.unsplash.com/photo-1485627658391-1365e4e0dbfe?ixlib=rb-0.3.5&ixid=eyJhcHBfaWQiOjEyMDd9&s=976b0db5c3c2b9932bb20e72f98f9b61&auto=format&fit=crop&w=500&q=60",
        "https://images.unsplash.com/photo-1502550900787-e956c314a221?ixlib=rb-0.3.5&ixid=eyJhcHBfaWQiOjEyMDd9&s=e90f191de3a03c2002ac82c009490e07&auto=format&fit=crop&w=500&q=60",
        "https://images.unsplash.com/photo-1466692476868-aef1dfb1e735?ixlib=rb-0.3.5&ixid=eyJhcHBfaWQiOjEyMDd9&s=9e3cd6ce6496c9c05cbf1b6cda8be0f9&auto=format&fit=crop&w=500&q=60",
        "https://images.unsplash.com/photo-1509885903707-b68568db61ed?ixlib=rb-0.3.5&ixid=eyJhcHBfaWQiOjEyMDd9&s=5f11503655b51165836c2dcefa51a040&auto=format&fit=crop&w=500&q=60",
        "https://images.unsplash.com/photo-1518707399486-6d702a84ff00?ixlib=rb-0.3.5&ixid=eyJhcHBfaWQiOjEyMDd9&s=b5bb16fa7eaed1a1ed47614488f7588d&auto=format&fit=crop&w=500&q=60",
        "https://images.unsplash.com/photo-1519408299519-b7a0274f7d67?ixlib=rb-0.3.5&ixid=eyJhcHBfaWQiOjEyMDd9&s=d4891b98f4554cc55eab1e4a923cbdb1&auto=format&fit=crop&w=500&q=60",
        "https://images.unsplash.com/photo-1506706435692-290e0c5b4505?ixlib=rb-0.3.5&ixid=eyJhcHBfaWQiOjEyMDd9&s=0bb464bb1ceea5155bc079c4f50bc36a&auto=format&fit=crop&w=500&q=60",
        "https://images.unsplash.com/photo-1512355144108-e94a235b10af?ixlib=rb-0.3.5&ixid=eyJhcHBfaWQiOjEyMDd9&s=c622d56d975113a08c71c912618b5f83&auto=format&fit=crop&w=500&q=60"
    ];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            Module = await JSRuntime.LoadUtility();

            scrollTimer = new System.Threading.Timer(async _ =>
            await ScrollEnd(), null, Timeout.Infinite, Timeout.Infinite);

            await LoadImages(true);
        }
    }

    private static System.Threading.Timer? scrollTimer;

    private static void OnScroll()
    {
        scrollTimer?.Change(500, Timeout.Infinite);
    }

    private async Task ScrollEnd()
    {
        if (!_onload)
        {
            //_onload = true;
            //var clientHeight = await Module.GetElementProperties<decimal>(id, "clientHeight");
            //var scrollHeight = await Module.GetElementProperties<decimal>(id, "scrollHeight");
            //var scrollTop = await Module.GetElementProperties<decimal>(id, "scrollTop");
            //var isScrolledToBottom = clientHeight + scrollTop + 100 > scrollHeight;

            //if (isScrolledToBottom)
            //{
            //    _showload = true;
            //    await LoadImages(false);

            //    _showload = false;
            //    await InvokeAsync(StateHasChanged);
            //}
            //_onload = false;
        }
    }

    /// <summary>
    /// 加载图片
    /// </summary>
    /// <param name="firstRender"></param>
    /// <returns></returns>
    private async Task LoadImages(bool firstRender)
    {
        var num = firstRender ? _imageList.Count : 6;
        for (int i = 0; i < num; i++)
        {
            await Task.Delay(200);
            ImageList.Add(_imageList[random.Next(0, _imageList.Count)]);
            await InvokeAsync(StateHasChanged);
        }
    }
}
