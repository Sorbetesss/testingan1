﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

namespace BootstrapBlazor.Components;

/// <summary>
/// JSModule extensions class
/// </summary>
public static class JSModuleExtensions
{
    /// <summary>
    /// IJSRuntime 扩展方法 动态加载脚本 脚本目录为 modules
    /// </summary>
    /// <param name="jsRuntime"></param>
    /// <param name="fileName"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    public static async Task<JSModule> LoadModule(this IJSRuntime jsRuntime, string fileName, string? version = null)
    {
        if (!string.IsNullOrEmpty(version))
        {
            fileName = $"{fileName}?v={version}";
        }
        var jSObjectReference = await jsRuntime.InvokeAsync<IJSObjectReference>(identifier: "import", fileName);
        return new JSModule(jSObjectReference);
    }

    /// <summary>
    /// 获得指定类型的加载 Module 名称
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string GetTypeModuleName(this Type type)
    {
        var name = type.Name;
        if (type.IsGenericType)
        {
            var index = type.Name.IndexOf('`');
            name = type.Name[..index];
        }
        return name;
    }
}
