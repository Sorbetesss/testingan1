﻿using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;

namespace Microsoft.AspNetCore.Components.Forms
{
    /// <summary>
    /// FieldIdentifier 扩展操作类
    /// </summary>
    public static class FieldIdentifierExtensions
    {
        private static ConcurrentDictionary<(Type ModelType, string FieldName), string> DisplayNameCache { get; } = new ConcurrentDictionary<(Type, string), string>();

        private static ConcurrentDictionary<(Type ModelType, string FieldName), PropertyInfo> PropertyInfoCache { get; } = new ConcurrentDictionary<(Type, string), PropertyInfo>();

        /// <summary>
        /// 获取显示名称方法
        /// </summary>
        /// <param name="fieldIdentifier"></param>
        /// <returns></returns>
        public static string GetDisplayName(this FieldIdentifier fieldIdentifier) => GetDisplayName(fieldIdentifier.Model, fieldIdentifier.FieldName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static string GetDisplayName(this object model, string fieldName)
        {
            var cacheKey = (Type: model.GetType(), FieldName: fieldName);

            if (!DisplayNameCache.TryGetValue(cacheKey, out var dn))
            {
                if (TryGetValidatableProperty(cacheKey.Type, cacheKey.FieldName, out var propertyInfo))
                {
                    var displayNameAttribute = propertyInfo.GetCustomAttribute<DisplayNameAttribute>();
                    if (displayNameAttribute != null)
                    {
                        dn = displayNameAttribute.DisplayName;

                        // add display name into cache
                        DisplayNameCache.GetOrAdd(cacheKey, key => dn);
                    }
                }
            }
            return dn ?? cacheKey.FieldName;
        }

        private static bool TryGetValidatableProperty(Type modelType, string fieldName, out PropertyInfo propertyInfo)
        {
            var cacheKey = (ModelType: modelType, FieldName: fieldName);
            if (!PropertyInfoCache.TryGetValue(cacheKey, out propertyInfo))
            {
                // Validator.TryValidateProperty 只能对 Public 属性生效
                propertyInfo = cacheKey.ModelType.GetProperty(cacheKey.FieldName);

                if (propertyInfo != null) PropertyInfoCache[cacheKey] = propertyInfo;
            }
            return propertyInfo != null;
        }
    }
}
