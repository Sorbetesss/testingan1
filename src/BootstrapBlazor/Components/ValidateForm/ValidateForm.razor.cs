﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using BootstrapBlazor.Localization.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace BootstrapBlazor.Components
{
    /// <summary>
    /// ValidateForm 组件类
    /// </summary>
    public sealed partial class ValidateForm
    {
        /// <summary>
        /// A callback that will be invoked when the form is submitted.
        /// If using this parameter, you are responsible for triggering any validation
        /// manually, e.g., by calling <see cref="EditContext.Validate"/>.
        /// </summary>
        [Parameter]
        [NotNull]
        public Func<EditContext, Task>? OnSubmit { get; set; }

        /// <summary>
        /// A callback that will be invoked when the form is submitted and the
        /// <see cref="EditContext"/> is determined to be valid.
        /// </summary>
        [Parameter]
        [NotNull]
        public Func<EditContext, Task>? OnValidSubmit { get; set; }

        /// <summary>
        /// A callback that will be invoked when the form is submitted and the
        /// <see cref="EditContext"/> is determined to be invalid.
        /// </summary>
        [Parameter]
        [NotNull]
        public Func<EditContext, Task>? OnInvalidSubmit { get; set; }

        /// <summary>
        /// 获得/设置 是否验证所有字段 默认 false
        /// </summary>
        [Parameter]
        public bool ValidateAllProperties { get; set; }

        /// <summary>
        /// Specifies the top-level model object for the form. An edit context will
        /// be constructed for this model. If using this parameter, do not also supply
        /// a value for <see cref="EditContext"/>.
        /// </summary>
        [Parameter]
        public object? Model { get; set; }

        /// <summary>
        /// Specifies the content to be rendered inside this
        /// </summary>
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        [Inject]
        [NotNull]
        private IServiceProvider? ServiceProvider { get; set; }

        /// <summary>
        /// 验证组件缓存
        /// </summary>
        private ConcurrentDictionary<FieldIdentifier, IValidateComponent> ValidatorCache { get; } = new();

        /// <summary>
        /// 添加数据验证组件到 EditForm 中
        /// </summary>
        /// <param name="key"></param>
        /// <param name="comp"></param>
        internal void AddValidator(in FieldIdentifier key, IValidateComponent comp) => ValidatorCache.AddOrUpdate(key, k => comp, (k, c) => c = comp);

        /// <summary>
        /// 设置指定字段错误信息
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="errorMessage">错误描述信息，可为空，为空时查找资源文件</param>
        public void SetError<TModel>(Expression<Func<TModel, object?>> expression, string errorMessage)
        {
            if (expression.Body is MemberExpression exp)
            {
                var fieldName = exp.Member.Name;
                var modelType = exp.Expression?.Type;
                if (modelType != null)
                {
                    var validator = ValidatorCache.FirstOrDefault(c => c.Key.Model.GetType() == modelType && c.Key.FieldName == fieldName).Value;
                    if (validator != null)
                    {
                        var results = new List<ValidationResult>
                        {
                            new ValidationResult(errorMessage, new string[] { fieldName })
                        };
                        validator.ToggleMessage(results, true);
                    }
                }
            }
        }

        /// <summary>
        /// 通过指定的模型类型获取当前表单中的绑定属性
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<(FieldIdentifier, PropertyInfo PropertyInfo)> GetBindModelProperties() => ValidatorCache.Keys.Select(key => (key, key.Model.GetType().GetProperty(key.FieldName)!));

        private static bool IsPublic(PropertyInfo p) => p.GetMethod != null && p.SetMethod != null && p.GetMethod.IsPublic && p.SetMethod.IsPublic;

        /// <summary>
        /// EditModel 数据模型验证方法
        /// </summary>
        /// <param name="context"></param>
        /// <param name="results"></param>
        internal void ValidateObject(ValidationContext context, List<ValidationResult> results)
        {
            if (ValidateAllProperties)
            {
                ValidateProperty(context, results);
            }
            else
            {
                // 遍历所有可验证组件进行数据验证
                foreach (var key in ValidatorCache.Keys)
                {
                    // 设置其关联属性字段
                    var propertyValue = LambdaExtensions.GetPropertyValue(key.Model, key.FieldName);

                    // 验证 DataAnnotations
                    var pi = key.Model.GetType().GetProperty(key.FieldName);
                    if (pi != null)
                    {
                        var validator = ValidatorCache[key];
                        var messages = new List<ValidationResult>();
                        var propertyValidateContext = new ValidationContext(key.Model)
                        {
                            MemberName = key.FieldName,
                            DisplayName = key.GetDisplayName()
                        };
                        ValidateDataAnnotations(propertyValue, propertyValidateContext, messages, pi);

                        if (messages.Count == 0)
                        {
                            // 自定义验证组件
                            validator.ValidateProperty(propertyValue, propertyValidateContext, messages);
                        }

                        // 客户端提示
                        validator.ToggleMessage(messages, false);
                        results.AddRange(messages);
                    }
                }
            }
        }

        /// <summary>
        /// 通过表单内绑定的字段验证方法
        /// </summary>
        /// <param name="context"></param>
        /// <param name="results"></param>
        /// <param name="fieldIdentifier"></param>
        internal void ValidateField(ValidationContext context, List<ValidationResult> results, in FieldIdentifier fieldIdentifier)
        {
            if (ValidatorCache.TryGetValue(fieldIdentifier, out var validator))
            {
                var propertyValue = LambdaExtensions.GetPropertyValue(fieldIdentifier.Model, fieldIdentifier.FieldName);
                var pi = fieldIdentifier.Model.GetType().GetProperty(fieldIdentifier.FieldName)!;

                // 验证 DataAnnotations
                ValidateDataAnnotations(propertyValue, context, results, pi);

                if (results.Count == 0)
                {
                    // 自定义验证组件
                    validator.ValidateProperty(propertyValue, context, results);
                }

                // 客户端提示
                validator.ToggleMessage(results, true);
            }
        }

        /// <summary>
        /// 通过属性设置的 DataAnnotation 进行数据验证
        /// </summary>
        /// <param name="value"></param>
        /// <param name="context"></param>
        /// <param name="results"></param>
        /// <param name="propertyInfo"></param>
        private void ValidateDataAnnotations(object? value, ValidationContext context, ICollection<ValidationResult> results, PropertyInfo propertyInfo)
        {
            var rules = propertyInfo.GetCustomAttributes(true).Where(i => i.GetType().BaseType == typeof(ValidationAttribute)).Cast<ValidationAttribute>();
            var displayName = context.DisplayName;
            var memberName = propertyInfo.Name;
            var attributeSpan = "Attribute".AsSpan();
            foreach (var rule in rules)
            {
                if (!rule.IsValid(value))
                {
                    // 查找 resx 资源文件中的 ErrorMessage
                    var ruleNameSpan = rule.GetType().Name.AsSpan();
                    var index = ruleNameSpan.IndexOf(attributeSpan, StringComparison.OrdinalIgnoreCase);
                    var ruleName = rule.GetType().Name.AsSpan().Slice(0, index);
                    var isResx = false;
                    if (!string.IsNullOrEmpty(rule.ErrorMessage))
                    {
                        var resxType = ServiceProvider.GetRequiredService<IOptions<JsonLocalizationOptions>>().Value.ResourceManagerStringLocalizerType;
                        if (resxType != null && JsonStringLocalizerFactory.TryGetLocalizerString(resxType, rule.ErrorMessage, out var resx))
                        {
                            rule.ErrorMessage = resx;
                            isResx = true;
                        }
                    }
                    if (!isResx)
                    {
                        if (JsonStringLocalizerFactory.TryGetLocalizerString(rule.GetType(), nameof(rule.ErrorMessage), out var msg))
                        {
                            rule.ErrorMessage = msg;
                        }

                        if (JsonStringLocalizerFactory.TryGetLocalizerString(context.ObjectType, $"{memberName}.{ruleName.ToString()}", out msg))
                        {
                            rule.ErrorMessage = msg;
                        }
                    }

                    var errorMessage = rule.FormatErrorMessage(displayName ?? memberName);
                    results.Add(new ValidationResult(errorMessage, new string[] { memberName }));
                }
            }
        }

        /// <summary>
        /// 验证整个模型时验证属性方法
        /// </summary>
        /// <param name="context"></param>
        /// <param name="results"></param>
        private void ValidateProperty(ValidationContext context, List<ValidationResult> results)
        {
            var properties = context.ObjectType.GetRuntimeProperties().Where(p => IsPublic(p) && !p.GetIndexParameters().Any());
            foreach (var pi in properties)
            {
                // 设置其关联属性字段
                var propertyValue = LambdaExtensions.GetPropertyValue(context.ObjectInstance, pi.Name);

                // 检查当前值是否为 Class
                if (propertyValue != null && propertyValue is not string && propertyValue.GetType().IsClass)
                {
                    var fieldContext = new ValidationContext(propertyValue)
                    {
                        MemberName = pi.Name
                    };
                    ValidateProperty(fieldContext, results);
                }
                else
                {
                    // 验证 DataAnnotations
                    var fieldIdentifier = new FieldIdentifier(context.ObjectInstance, pi.Name);
                    var messages = new List<ValidationResult>();
                    context.DisplayName = fieldIdentifier.GetDisplayName();
                    ValidateDataAnnotations(propertyValue, context, messages, pi);

                    if (ValidatorCache.TryGetValue(fieldIdentifier, out var validator))
                    {
                        if (messages.Count == 0)
                        {
                            // 自定义验证组件
                            validator.ValidateProperty(propertyValue, context, messages);
                        }

                        // 客户端提示
                        validator.ToggleMessage(messages, true);
                    }
                    results.AddRange(messages);
                }
            }
        }
    }
}
