﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using System.Text.Json;
using System.Text.Json.Serialization;

namespace BootstrapBlazor.Components;

/// <summary>
/// DockViewComponent 转化器
/// </summary>
class DockViewComponentConverter : JsonConverter<List<IDockViewComponentBase>>
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="typeToConvert"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public override List<IDockViewComponentBase>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="value"></param>
    /// <param name="options"></param>
    public override void Write(Utf8JsonWriter writer, List<IDockViewComponentBase> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (var item in value)
        {
            if (item is IDockViewContent content)
            {
                writer.WriteRawValue(JsonSerializer.Serialize(content, options));
            }
            else if (item is IDockViewComponent contentItem)
            {
                if (contentItem.Visible)
                {
                    writer.WriteRawValue(JsonSerializer.Serialize(contentItem, options));
                }
            }
        }
        writer.WriteEndArray();
    }
}
