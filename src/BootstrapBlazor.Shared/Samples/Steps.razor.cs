﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

namespace BootstrapBlazor.Shared.Samples;

/// <summary>
/// Steps
/// </summary>
public sealed partial class Steps
{
    [NotNull]
    private ConsoleLogger? Logger { get; set; }

    [NotNull]
    private List<StepItem>? Items { get; set; }

    private BootstrapBlazor.Components.Steps? _steps;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override void OnInitialized()
    {
        Items = new()
        {
            new StepItem()
            {
                Title = Localizer["StepItemI1Text"],
                Template = BootstrapDynamicComponent.CreateComponent<Counter>().Render(),
                IsActive = true
            },
            new StepItem()
            {
                Title = Localizer["StepItemI2Text"],
                Template = BootstrapDynamicComponent.CreateComponent<FetchData>().Render()
            },
            new StepItem()
            {
                Title = Localizer["StepItemI3Text"],
                Template = BootstrapDynamicComponent.CreateComponent<Counter>().Render()
            }
        };
    }

    private void PrevStep()
    {
        _steps?.Prev();
    }

    private void NextStep()
    {
        _steps?.Next();
    }

    private void ResetStep()
    {
        Items.ForEach(i =>
        {
            i.Status = StepStatus.Wait;
        });
    }

    private Task OnStatusChanged(StepStatus status)
    {
        Logger.Log($"Steps Status: {status}");
        return Task.CompletedTask;
    }

    private IEnumerable<AttributeItem> GetAttributes() => new AttributeItem[]
    {
        new()
        {
            Name = "Items",
            Description = Localizer["StepsItems"],
            Type = "IEnumerable<StepItem>",
            ValueList = " — ",
            DefaultValue = " — "
        },
        new()
        {
            Name = "IsVertical",
            Description = Localizer["StepsIsVertical"],
            Type = "bool",
            ValueList = "true|false",
            DefaultValue = "false"
        },
        new()
        {
            Name = "IsCenter",
            Description = Localizer["StepsIsCenter"],
            Type = "bool",
            ValueList = "true|false",
            DefaultValue = "false"
        },
        new()
        {
            Name = "Status",
            Description = Localizer["StepsStatus"],
            Type = "StepStatus",
            ValueList = "Wait|Process|Finish|Error|Success",
            DefaultValue = "Wait"
        }
    };

    private IEnumerable<AttributeItem> GetStepItemAttributes() => new AttributeItem[]
    {
        new()
        {
            Name = "IsCenter",
            Description = Localizer["StepsAttrIsCenter"],
            Type = "bool",
            ValueList = "true|false",
            DefaultValue = "false"
        },
        new()
        {
            Name = "IsIcon",
            Description = Localizer["StepsAttrIsIcon"],
            Type = "bool",
            ValueList = "true|false",
            DefaultValue = "false"
        },
        new()
        {
            Name = "IsLast",
            Description = Localizer["StepsAttrIsLast"],
            Type = "bool",
            ValueList = "true|false",
            DefaultValue = "false"
        },
        new()
        {
            Name = "StepIndex",
            Description = Localizer["StepsAttrStepIndex"],
            Type = "int",
            ValueList = " — ",
            DefaultValue = "0"
        },
        new()
        {
            Name = "Space",
            Description = Localizer["StepsAttrSpace"],
            Type = "string",
            ValueList = " — ",
            DefaultValue = "—"
        },
        new()
        {
            Name = "Title",
            Description = Localizer["StepsAttrTitle"],
            Type = "string",
            ValueList = " — ",
            DefaultValue = " — "
        },
        new()
        {
            Name = "Icon",
            Description = Localizer["StepsAttrIcon"],
            Type = "string",
            ValueList = " — ",
            DefaultValue = " — "
        },
        new()
        {
            Name = "Description",
            Description = Localizer["StepsAttrDescription"],
            Type = "string",
            ValueList = " — ",
            DefaultValue = " — "
        },
        new()
        {
            Name = "Status",
            Description = Localizer["StepsAttrStatus"],
            Type = "StepStatus",
            ValueList = "Wait|Process|Finish|Error|Success",
            DefaultValue = "Wait"
        },
        new()
        {
            Name = "Template",
            Description = Localizer["StepsAttrTemplate"],
            Type = "RenderFragment",
            ValueList = " — ",
            DefaultValue = " — "
        }
    };

    private IEnumerable<EventItem> GetEvents() => new List<EventItem>()
    {
        new()
        {
            Name = "OnStatusChanged",
            Description = Localizer["StepsEventOnStatusChanged"],
            Type ="Func<StepStatus, Task>"
        }
    };
}
