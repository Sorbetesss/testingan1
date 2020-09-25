﻿using BootstrapBlazor.Components;
using BootstrapBlazor.Shared.Common;
using BootstrapBlazor.Shared.Pages.Components;
using System;
using System.Collections.Generic;

namespace BootstrapBlazor.Shared.Pages
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class EditorForms
    {
        private Dummy Model { get; set; } = new Dummy()
        {
            Name = "张三",
            Age = 23,
            BirthDay = new DateTime(1997, 12, 05),
            Education = EnumEducation.Middel
        };

        private Dummy ValidateModel { get; set; } = new Dummy()
        {
            Name = "张三",
            Age = 23,
            BirthDay = new DateTime(1997, 12, 05),
            Education = EnumEducation.Middel
        };

        private IEnumerable<SelectedItem> Hobbys = new List<SelectedItem>()
        {
            new SelectedItem("游泳", "游泳"),
            new SelectedItem("登山", "登山"),
            new SelectedItem("打球", "打球"),
            new SelectedItem("下棋", "下棋")
        };

        private IEnumerable<AttributeItem> GetAttributes() => new AttributeItem[]
        {
            // TODO: 移动到数据库中
            new AttributeItem() {
                Name = "Model",
                Description = "当前绑定数据模型",
                Type = "TModel",
                ValueList = " — ",
                DefaultValue = " — "
            },
            new AttributeItem() {
                Name = "FieldItems",
                Description = "绑定列模板",
                Type = "RenderFragment<TModel>",
                ValueList = " — ",
                DefaultValue = " — "
            },
            new AttributeItem() {
                Name = "Buttons",
                Description = "按钮模板",
                Type = "RenderFragment",
                ValueList = " — ",
                DefaultValue = " — "
            },
            new AttributeItem() {
                Name = "ShowLabel",
                Description = "是否显示 Label",
                Type = "bool?",
                ValueList = "null/true/false",
                DefaultValue = "null"
            }
        };

        private IEnumerable<AttributeItem> GetEditorItemAttributes() => new AttributeItem[]
        {
            // TODO: 移动到数据库中
            new AttributeItem() {
                Name = "Field",
                Description = "当前绑定数据值",
                Type = "TValue",
                ValueList = " — ",
                DefaultValue = " — "
            },
            new AttributeItem() {
                Name = "FieldType",
                Description = "绑定列数据类型",
                Type = "Type",
                ValueList = " — ",
                DefaultValue = " — "
            },
            new AttributeItem() {
                Name = "Editable",
                Description = "是否允许编辑",
                Type = "bool",
                ValueList = "true/false",
                DefaultValue = "true"
            },
            new AttributeItem() {
                Name = "Readonly",
                Description = "是否只读",
                Type = "bool",
                ValueList = "true/false",
                DefaultValue = "false"
            },
            new AttributeItem() {
                Name = "Text",
                Description = "编辑列前置标签名",
                Type = "string",
                ValueList = " — ",
                DefaultValue = " — "
            },
            new AttributeItem() {
                Name = "EditTemplate",
                Description = "列编辑模板",
                Type = "RenderFragment<object>",
                ValueList = " — ",
                DefaultValue = " — "
            }
        };
    }
}
