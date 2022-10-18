﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

namespace BootstrapBlazor.Shared.Samples;

/// <summary>
/// 
/// </summary>
public sealed partial class Timers
{
    [NotNull]
    private BlockLogger? Trace { get; set; }

    private Task OnTimeout()
    {
        Trace.Log("timer time up");
        return Task.CompletedTask;
    }

    private Task OnCancel()
    {
        Trace?.Log("timer canceled");
        return Task.CompletedTask;
    }

    private static IEnumerable<AttributeItem> GetAttributes()
    {
        return new AttributeItem[]
        {
            new AttributeItem()
            {
                Name = "Width",
                Description = "Component width",
                Type = "int",
                ValueList = " — ",
                DefaultValue = "300"
            },
            new AttributeItem()
            {
                Name = "StrokeWidth",
                Description = "Progress bar width",
                Type = "int",
                ValueList = " — ",
                DefaultValue = "6"
            },
            new AttributeItem()
            {
                Name = "IsVibrate",
                Description = "Device vibrates when countdown ends",
                Type = "bool",
                ValueList = "true/false",
                DefaultValue = "true"
            },
            new AttributeItem()
            {
                Name = "Value",
                Description = "Countdown time",
                Type = "Timespan",
                ValueList = " — ",
                DefaultValue = " — "
            },
            new AttributeItem()
            {
                Name = "Color",
                Description = "Progress bar color",
                Type = "Color",
                ValueList = "Primary / Secondary / Success / Danger / Warning / Info / Dark",
                DefaultValue = "Primary"
            }
        };
    }
}
