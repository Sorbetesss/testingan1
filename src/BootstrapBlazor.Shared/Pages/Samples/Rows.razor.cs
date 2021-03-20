﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using BootstrapBlazor.Shared.Common;
using BootstrapBlazor.Shared.Pages.Components;
using System;
using System.Collections.Generic;

namespace BootstrapBlazor.Shared.Pages
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class Rows
    {
        private Foo Model { get; } = new Foo()
        {
            Name = "张三",
            Count = 23,
            Address = "测试地址",
            DateTime = new DateTime(1997, 12, 05),
            Education = EnumEducation.Middel
        };

        private List<EnumEducation> Educations = new List<EnumEducation> { EnumEducation.Middel, EnumEducation.Primary };
        private IEnumerable<AttributeItem> GetAttributes() => new AttributeItem[]
        {
            new AttributeItem() {
                Name = "ItemsPerRow",
                Description = "设置一行显示几个控件",
                Type = "enum",
                ValueList = " One,Two,Three,Four,Six,Twelve ",
                DefaultValue = " One "
            },
            new AttributeItem() {
                Name = "RowType",
                Description = "设置排版格式，子Row如果不指定，会使用父Row的设置",
                Type = "enum?",
                ValueList = "Normal, FormInline,FormRow",
                DefaultValue = "null"
            },
            new AttributeItem() {
                Name = "ColSpan",
                Description = "设置子Row跨父Row列数",
                Type = "int?",
                ValueList = "-",
                DefaultValue = "null"
            },
            new AttributeItem() {
                Name = "MaxCount",
                Description = "设置行内最多显示的控件数",
                Type = "int?",
                ValueList = "-",
                DefaultValue = "null"
            }


        };
    }
}
