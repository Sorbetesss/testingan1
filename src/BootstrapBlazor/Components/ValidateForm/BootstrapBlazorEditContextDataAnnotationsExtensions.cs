﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using BootstrapBlazor.Localization.Json;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace BootstrapBlazor.Components
{
    /// <summary>
    /// EditContextDataAnotation 扩展操作类
    /// </summary>
    internal static class BootstrapBlazorEditContextDataAnnotationsExtensions
    {
        private static readonly ConcurrentDictionary<(Type ModelType, string FieldName), Func<object, object>> PropertyValueInvokerCache = new();

        /// <summary>
        /// 添加数据合规检查
        /// </summary>
        /// <param name="editContext">The <see cref="EditContext"/>.</param>
        /// <param name="editForm"></param>
        public static EditContext AddEditContextDataAnnotationsValidation(this EditContext editContext, ValidateForm editForm)
        {
            if (editContext == null)
            {
                throw new ArgumentNullException(nameof(editContext));
            }

            var messages = new ValidationMessageStore(editContext);

            editContext.OnValidationRequested +=
                (sender, eventArgs) => ValidateModel(sender as EditContext, messages, editForm);

            editContext.OnFieldChanged +=
                (sender, eventArgs) => ValidateField(editContext, messages, eventArgs.FieldIdentifier, editForm);

            return editContext;
        }

        private static void ValidateModel(EditContext? editContext, ValidationMessageStore messages, ValidateForm editForm)
        {
            if (editContext != null)
            {
                var validationContext = new ValidationContext(editContext.Model);
                var validationResults = new List<ValidationResult>();

                TryValidateObject(editContext.Model, validationContext, validationResults, editForm);
                editForm.ValidateObject(editContext.Model, validationContext, validationResults);

                messages.Clear();

                foreach (var validationResult in validationResults.Where(v => !string.IsNullOrEmpty(v.ErrorMessage)))
                {
                    if (!validationResult.MemberNames.Any())
                    {
                        messages.Add(new FieldIdentifier(editContext.Model, fieldName: string.Empty), validationResult.ErrorMessage!);
                        continue;
                    }

                    foreach (var memberName in validationResult.MemberNames)
                    {
                        messages.Add(editContext.Field(memberName), validationResult.ErrorMessage!);
                    }
                }
                editContext.NotifyValidationStateChanged();
            }
        }

        private static void ValidateField(EditContext editContext, ValidationMessageStore messages, in FieldIdentifier fieldIdentifier, ValidateForm editForm)
        {
            // 获取验证消息
            var results = new List<ValidationResult>();
            var validationContext = new ValidationContext(fieldIdentifier.Model)
            {
                MemberName = fieldIdentifier.FieldName,
                DisplayName = fieldIdentifier.GetDisplayName()
            };

            var propertyValue = fieldIdentifier.GetPropertyValue();
            TryValidateProperty(propertyValue, validationContext, results);
            editForm.ValidateProperty(propertyValue, validationContext, results);

            messages.Clear(fieldIdentifier);
            messages.Add(fieldIdentifier, results.Where(v => !string.IsNullOrEmpty(v.ErrorMessage)).Select(result => result.ErrorMessage!));

            editContext.NotifyValidationStateChanged();
        }

        /// <summary>
        /// 获取 FieldIdentifier 属性值
        /// </summary>
        /// <param name="fieldIdentifier"></param>
        /// <returns></returns>
        internal static object GetPropertyValue(this in FieldIdentifier fieldIdentifier)
        {
            var cacheKey = (fieldIdentifier.Model.GetType(), fieldIdentifier.FieldName);
            var model = fieldIdentifier.Model;
            var invoker = PropertyValueInvokerCache.GetOrAdd(cacheKey, key => LambdaExtensions.GetPropertyValueLambda<object, object>(model, key.FieldName).Compile());

            return invoker.Invoke(model);
        }

        private static void TryValidateObject(object model, ValidationContext context, ICollection<ValidationResult> results, ValidateForm validateForm)
        {
            var modelType = model.GetType();
            var validateProperties = validateForm.ValidateAllProperties
                ? modelType.GetRuntimeProperties().Where(p => p.IsPublic() && !p.GetIndexParameters().Any())
                : validateForm.GetPropertiesByModelType(model.GetType()).Select(p => model.GetType().GetProperty(p));
            foreach (var p in validateProperties)
            {
                if (p != null)
                {
                    var fieldIdentifier = new FieldIdentifier(model, fieldName: p.Name);
                    var propertyValue = fieldIdentifier.GetPropertyValue();
                    TryValidateProperty(propertyValue, context, results, p);
                }
            }
        }

        private static void TryValidateProperty(object value, ValidationContext context, ICollection<ValidationResult> results, PropertyInfo? propertyInfo = null)
        {
            var modelType = context.ObjectType;
            if (propertyInfo == null)
            {
                propertyInfo = modelType.GetProperty(context.MemberName!);
            }

            if (propertyInfo != null)
            {
                var rules = propertyInfo.GetCustomAttributes(true).Where(i => i.GetType().BaseType == typeof(ValidationAttribute)).Cast<ValidationAttribute>();
                var displayName = new FieldIdentifier(context.ObjectInstance, propertyInfo.Name).GetDisplayName();
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
                            var resxType = ServiceProviderHelper.ServiceProvider.GetRequiredService<IOptions<JsonLocalizationOptions>>().Value.ResourceManagerStringLocalizerType;
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
        }

        private static bool IsPublic(this PropertyInfo p) => p.GetMethod != null && p.SetMethod != null && p.GetMethod.IsPublic && p.SetMethod.IsPublic;
    }
}
