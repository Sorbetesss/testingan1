﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Runtime.CompilerServices;

namespace UnitTest.Components;

public class SelectObjectTest : BootstrapBlazorTestBase
{
    [Fact]
    public async Task Value_Ok()
    {
        Product? v = null;
        string? url = null;
        var items = Enumerable.Range(1, 8).Select(i => new Product()
        {
            ImageUrl = $"./images/Pic{i}.jpg",
            Description = $"Pic{i}.jpg"
        });
        var cut = Context.RenderComponent<SelectObject<Product>>(pb =>
        {
            pb.Add(a => a.Value, v);
            pb.Add(a => a.OnValueChanged, p =>
            {
                v = p;
                return Task.CompletedTask;
            });
            pb.Add(a => a.GetTextCallback, p => p?.ImageUrl);
            pb.Add(a => a.ChildContent, context => pb =>
            {
                pb.OpenComponent<ListView<Product>>(0);
                pb.AddAttribute(1, "Items", items);
                pb.AddAttribute(2, "BodyTemplate", new RenderFragment<Product>(p => b => b.AddContent(0, p.ImageUrl)));
                pb.AddAttribute(3, "OnListViewItemClick", new Func<Product, Task>(async p =>
                {
                    context.SetValue(p);
                    await context.CloseAsync();

                    url = p.ImageUrl;
                }));
                pb.CloseComponent();
            });
        });

        var item = cut.Find(".listview-item");
        await cut.InvokeAsync(() => item.Click());
        Assert.NotNull(v);
        Assert.Equal(url, v.ImageUrl);
    }

    class Product
    {
        public string ImageUrl { get; set; } = "";

        public string Description { get; set; } = "";
    }
}
