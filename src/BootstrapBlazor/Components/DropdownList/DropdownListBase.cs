﻿using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BootstrapBlazor.Components
{
    /// <summary>
    /// DropdownListBase 组件基类
    /// </summary>
    public abstract class DropdownListBase<TModel, TValue> : BootstrapInputBase<TValue>
    {
        /// <summary>
        /// 获得 样式集合
        /// </summary>
        protected new string? ClassName => CssBuilder.Default("form-select dropdown")
            .AddClass("is-disabled", IsDisabled)
            .AddClassFromAttributes(AdditionalAttributes)
            .Build();

        /// <summary>
        /// 获得 样式集合
        /// </summary>
        protected string? InputClassName => CssBuilder.Default("form-control form-select-input")
            .AddClass($"border-{Color.ToDescriptionString()}", Color != Color.None)
            .Build();

        /// <summary>
        /// 获得 样式集合
        /// </summary>
        protected string? ArrowClassName => CssBuilder.Default("form-select-append")
            .AddClass($"text-{Color.ToDescriptionString()}", Color != Color.None)
            .Build();

        /// <summary>
        /// 设置当前项是否 Active 方法
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected virtual string? ActiveItem(SelectedItem item) => CssBuilder.Default("dropdown-item")
            .AddClass("active", () => item.Value == CurrentValueAsString)
            .Build();

        /// <summary>
        /// 
        /// </summary>
        protected string Text => GetText(CurrentItem);

        /// <summary>
        /// 获得/设置 Select 内部 Input 组件 Id
        /// </summary>
        protected string? InputId => string.IsNullOrEmpty(Id) ? null : $"{Id}_input";

#nullable disable
        /// <summary>
        /// 设置当前项是否 Active 方法
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected virtual string ActiveItem(TModel item) => CssBuilder.Default("dropdown-item")
            .AddClass("active", () => item.Equals(CurrentItem))
            .Build();

        /// <summary>
        /// 
        /// </summary>
        protected TModel CurrentItem { get; set; }
#nullable restore

        /// <summary>
        /// 获得 PlaceHolder 属性
        /// </summary>
        protected string? PlaceHolder
        {
            get
            {
                string? placeHolder = "请选择 ...";
                if (AdditionalAttributes != null && AdditionalAttributes.TryGetValue("placeholder", out var ph) && !string.IsNullOrEmpty(Convert.ToString(ph)))
                {
                    placeHolder = ph.ToString();
                }
                return placeHolder;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public string DefaultText { get; set; } = "请选择 ...";

        /// <summary>
        /// 获得/设置 按钮颜色
        /// </summary>
        [Parameter]
        public Color Color { get; set; } = Color.None;

        /// <summary>
        /// 获得/设置 绑定数据集
        /// </summary>
        [Parameter]
        public IEnumerable<TModel>? Data { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public string TextField { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public string ValueField { get; set; } = "";

        /// <summary>
        /// SelectedItemChanged 方法
        /// </summary>
        [Parameter]
        public EventCallback<TValue> OnSelectedItemChanged { get; set; }

        /// <summary>
        /// SetParametersAsync 方法
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            parameters.SetParameterProperties(this);

            var context = EditContext ?? CascadedEditContext;
            if (context != null && ValueExpression != null)
            {
                var model = context.Model;
                if (model != null)
                {
                    var p = Expression.Property(Expression.Constant(model), typeof(TModel).GetProperty(ValueField));
                    var tDelegate = typeof(Func<>).MakeGenericType(typeof(TValue));
                    ValueExpression = Expression.Lambda(tDelegate, p) as Expression<Func<TValue>>;
                }
            }

            await base.SetParametersAsync(ParameterView.Empty);
        }

        /// <summary>
        /// OnInitialized 方法
        /// </summary>
        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            // 双向绑定其他组件更改了数据源值时
            if (Data != null && CurrentItem != null && CurrentItem.ToString() != CurrentValueAsString)
            {
                CurrentItem = Data.FirstOrDefault(i => GetValue(i) == CurrentValueAsString);
            }

            // 设置数据集合后 SelectedItem 设置默认值
            if (CurrentItem == null || !(Data?.Any(i => GetValue(i) == CurrentItem.ToString() && GetText(i) == Text) ?? false))
            {
                if (Data != null)
                {
                    var item = Data.FirstOrDefault();
                    if (item == null) item = Data.FirstOrDefault(i => GetValue(i) == CurrentValueAsString) ?? Data.FirstOrDefault();
                    if (item != null)
                    {
                        CurrentItem = item;
                        if (Value != null && CurrentValueAsString != GetValue(CurrentItem))
                        {
                            item = Data.FirstOrDefault(i => GetValue(i) == CurrentValueAsString);
                            if (item != null) CurrentItem = item;
                        }
                        CurrentValueAsString = GetValue(CurrentItem);
                    }
                }
            }

            if (CurrentItem != null && OnSelectedItemChanged.HasDelegate) OnSelectedItemChanged.InvokeAsync(Value);
        }

        /// <summary>
        /// 下拉框选项点击时调用此方法
        /// </summary>
        protected void OnItemClick(TModel item)
        {
            CurrentItem = item;

            // ValueChanged
            CurrentValueAsString = GetValue(item);

            if (OnSelectedItemChanged.HasDelegate) OnSelectedItemChanged.InvokeAsync(Value);
        }

#nullable disable
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected string GetValue(TModel model) => GetFieldValue(model, ValueField);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected string GetText(TModel model) => GetFieldValue(model, TextField);
#nullable restore

        private string GetFieldValue(TModel model, string fieldName)
        {
            var ret = "";
            if (model != null)
            {
                if (typeof(TModel).IsValueType || typeof(TModel) == typeof(string))
                {
                    ret = model.ToString();
                }
                else
                {
                    var invoker = GetPropertyValueLambdaCache.GetOrAdd((typeof(TModel), fieldName), key => model.GetPropertyValueLambda<TModel, object>(key.FieldName).Compile());
                    var v = invoker.Invoke(model);
                    ret = v?.ToString() ?? "";
                }
            }
            return ret;
        }

        private static ConcurrentDictionary<(Type ModelType, string FieldName), Func<TModel, object>> GetPropertyValueLambdaCache { get; set; } = new ConcurrentDictionary<(Type, string), Func<TModel, object>>();
    }
}
