﻿using BootstrapBlazor.Components.EditorForm;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace BootstrapBlazor.Components
{
    /// <summary>
    /// 编辑表单基类
    /// </summary>
    public abstract class EditorFormBase<TModel> : BootstrapComponentBase
    {
        /// <summary>
        /// 获得/设置 列模板
        /// </summary>
        [Parameter]
        public RenderFragment<TModel>? FieldItems { get; set; }

        /// <summary>
        /// 获得/设置 按钮模板
        /// </summary>
        [Parameter]
        public RenderFragment? Buttons { get; set; }

#nullable disable
        /// <summary>
        /// 获得/设置 绑定模型
        /// </summary>
        [Parameter]
        public TModel Model { get; set; }
#nullable restore

        /// <summary>
        /// 获得/设置 是否显示前置标签 默认为 true 显示标签
        /// </summary>
        [Parameter]
        public bool? ShowLabel { get; set; } = true;

        /// <summary>
        /// 获得/设置 级联上下文 EditContext 实例 内置于 EditForm 或者 ValidateForm 时有值
        /// </summary>
        [CascadingParameter]
        private EditContext? CascadedEditContext { get; set; }

        /// <summary>
        /// 获得/设置 级联上下文绑定字段信息集合
        /// </summary>
        [CascadingParameter]
        private IEnumerable<IEditorItem> CascadeEditorItems { get; set; } = Enumerable.Empty<IEditorItem>();

        /// <summary>
        /// 获得/设置 配置编辑项目集合
        /// </summary>
        protected List<IEditorItem> EditorItems { get; set; } = new List<IEditorItem>();

        /// <summary>
        /// 获得/设置 渲染的编辑项集合
        /// </summary>
        protected List<IEditorItem> FormItems { get; set; } = new List<IEditorItem>();

        /// <summary>
        /// OnInitialized 方法
        /// </summary>
        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (CascadedEditContext != null)
            {
                if (CascadedEditContext.Model.GetType() != typeof(TModel)) throw new InvalidOperationException($"验证表单与 {nameof(EditorForm<TModel>)} 绑定模型不一致");

                Model = (TModel)CascadedEditContext.Model;
            }

            if (Model == null) throw new ArgumentNullException(nameof(Model));
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool FirstRender { get; set; } = true;

        /// <summary>
        /// OnAfterRenderAsync 方法
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                FirstRender = false;

                if (CascadeEditorItems.Any())
                {
                    // 通过级联参数渲染组件
                    FormItems.AddRange(CascadeEditorItems);
                }
                else
                {
                    // 如果 EditorItems 有值表示 用户自定义列

                    // 获取绑定模型所有属性
                    var items = Model.GetType().GetProperties().Select(p => new InternalEditorItem<TModel>(Model, p)).ToList();

                    // 通过设定的 FieldItems 模板获取项进行渲染
                    foreach (var el in EditorItems)
                    {
                        var item = items.FirstOrDefault(i => i.GetFieldName() == el.GetFieldName());

                        if (item != null)
                        {
                            // 过滤掉不编辑的列
                            if (!el.Editable) items.Remove(item);
                            else
                            {
                                // 设置只读属性与列模板
                                item.Readonly = el.Readonly;
                                item.EditTemplate = el.EditTemplate;
                            }
                        }
                    }
                    FormItems.AddRange(items.OrderBy(i => GetOrder(i.FieldName)));
                }
                StateHasChanged();
            }
        }

        private int GetOrder(string fieldName) => Model.GetType().GetProperty(fieldName)?.GetCustomAttribute<EditorOrderAttribute>()?.Order ?? 0;

        #region AutoEdit
        /// <summary>
        /// 
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        protected RenderFragment AutoGenerateTemplate(IEditorItem col) => builder =>
        {
            var fieldType = col.FieldType;
            if (fieldType != null && Model != null)
            {
                // GetDisplayName
                var displayName = col.GetDisplayName();
                var fieldName = col.GetFieldName();

                // FieldValue
                var valueInvoker = GetPropertyValueLambdaCache.GetOrAdd((typeof(TModel), fieldName), key => Model.GetPropertyValueLambda<TModel, object?>(key.FieldName).Compile());
                var fieldValue = valueInvoker.Invoke(Model);

                // ValueChanged
                var valueChangedInvoker = CreateLambda(fieldType).Compile();
                var fieldValueChanged = valueChangedInvoker(Model, fieldName);

                // ValueExpression
                var body = Expression.Property(Expression.Constant(Model), typeof(TModel), fieldName);
                var tDelegate = typeof(Func<>).MakeGenericType(fieldType);
                var valueExpression = Expression.Lambda(tDelegate, body);

                var index = 0;
                var componentType = GenerateComponent(fieldType);
                builder.OpenComponent(index++, componentType);
                builder.AddAttribute(index++, "DisplayText", displayName);
                builder.AddAttribute(index++, "Value", fieldValue);
                builder.AddAttribute(index++, "ValueChanged", fieldValueChanged);
                builder.AddAttribute(index++, "ValueExpression", valueExpression);
                builder.AddAttribute(index++, "IsDisabled", col.Readonly);
                builder.AddMultipleAttributes(index++, CreateMultipleAttributes(fieldType));
                builder.CloseComponent();
            }
        };

        private IEnumerable<KeyValuePair<string, object>> CreateMultipleAttributes(Type fieldType)
        {
            var ret = new List<KeyValuePair<string, object>>();
            var type = Nullable.GetUnderlyingType(fieldType) ?? fieldType;
            if (type.IsEnum)
            {
                // 枚举类型
                // 通过字符串转化为枚举类实例
                var items = type.ToSelectList();
                if (items != null) ret.Add(new KeyValuePair<string, object>("Items", items));
            }
            else
            {
                switch (type.Name)
                {
                    case nameof(String):
                        ret.Add(new KeyValuePair<string, object>("placeholder", "请输入 ..."));
                        break;
                    default:
                        break;
                }
            }

            if (ShowLabel.HasValue && ShowLabel.Value)
            {
                ret.Add(new KeyValuePair<string, object>("ShowLabel", true));
            }
            return ret;
        }

        private Type GenerateComponent(Type fieldType)
        {
            Type? ret = null;
            var type = (Nullable.GetUnderlyingType(fieldType) ?? fieldType);
            if (type.IsEnum)
            {
                ret = typeof(Select<>).MakeGenericType(fieldType);
            }
            else
            {
                switch (type.Name)
                {
                    case nameof(Boolean):
                        ret = typeof(Checkbox<>).MakeGenericType(fieldType);
                        break;
                    case nameof(DateTime):
                        ret = typeof(DateTimePicker<>).MakeGenericType(fieldType);
                        break;
                    case nameof(Int32):
                    case nameof(Double):
                    case nameof(Decimal):
                        ret = typeof(BootstrapInput<>).MakeGenericType(fieldType);
                        break;
                    case nameof(String):
                        ret = typeof(BootstrapInput<>).MakeGenericType(typeof(string));
                        break;
                }
            }
            return ret ?? typeof(BootstrapInput<>).MakeGenericType(fieldType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="model"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        protected EventCallback<TType> CreateCallback<TType>(TModel model, string fieldName)
        {
            return EventCallback.Factory.Create<TType>(this, t =>
            {
                if (model != null)
                {
                    var invoker = SetPropertyValueLambdaCache.GetOrAdd((typeof(TModel), fieldName), key => model.SetPropertyValueLambda<TModel, object?>(key.FieldName).Compile());
                    invoker.Invoke(model, t);
                }
            });
        }

        private Expression<Func<TModel, string, object>> CreateLambda(Type fieldType)
        {
            var exp_p1 = Expression.Parameter(typeof(TModel));
            var exp_p2 = Expression.Parameter(typeof(string));
            var method = GetType().GetMethod("CreateCallback", BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(fieldType);
            var body = Expression.Call(Expression.Constant(this), method, exp_p1, exp_p2);

            return Expression.Lambda<Func<TModel, string, object>>(Expression.Convert(body, typeof(object)), exp_p1, exp_p2);
        }

        private static ConcurrentDictionary<(Type ModelType, string FieldName), Func<TModel, object?>> GetPropertyValueLambdaCache { get; } = new ConcurrentDictionary<(Type, string), Func<TModel, object?>>();

        private static ConcurrentDictionary<(Type ModelType, string FieldName), Action<TModel, object?>> SetPropertyValueLambdaCache { get; } = new ConcurrentDictionary<(Type, string), Action<TModel, object?>>();
        #endregion
    }
}
