﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using Microsoft.JSInterop;

namespace BootstrapBlazor.Shared.Samples;

/// <summary>
/// Toasts
/// </summary>
public sealed partial class Toasts
{
    [NotNull]
    private ToastContainer? ToastContainer { get; set; }

    [NotNull]
    private ToastOption? Options1 { get; set; }

    [NotNull]
    private ToastOption? Options2 { get; set; }

    [NotNull]
    private ToastOption? Options3 { get; set; }

    [NotNull]
    private ToastOption? Options4 { get; set; }

    [CascadingParameter]
    [NotNull]
    private BootstrapBlazorRoot? Root { get; set; }

    /// <summary>
    /// OnInitialized
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        Options1 = new ToastOption { Title = "Save data", IsAutoHide = false, Content = "Save data successfully, automatically close after 4 seconds" };
        Options2 = new ToastOption { Category = ToastCategory.Error, Title = "Save data", IsAutoHide = false, Content = "Save data successfully, automatically close after 4 seconds" };
        Options3 = new ToastOption { Category = ToastCategory.Information, Title = "Prompt information", IsAutoHide = false, Content = "Information prompt pop-up window, automatically closes after 4 seconds" };
        Options4 = new ToastOption { Category = ToastCategory.Warning, Title = "Warning message", IsAutoHide = false, Content = "Information prompt pop-up window, automatically closes after 4 seconds" };

        ToastContainer = Root.ToastContainer;
    }

    private async Task OnPlacementClick(Placement placement)
    {
        ToastContainer.SetPlacement(placement);
        await ToastService.Show(new ToastOption()
        {
            Category = ToastCategory.Information,
            Title = "Notification",
            Content = "<b>Toast</b> The component has changed position, it will automatically shut down after 4 seconds"
        });
    }

    private async Task OnSuccessClick()
    {
        ToastContainer.SetPlacement(Placement.BottomEnd);
        await ToastService.Show(new ToastOption()
        {
            Category = ToastCategory.Success,
            Title = "Successfully saved",
            Content = "Save data successfully, automatically close after 4 seconds"
        });
    }

    private async Task OnErrorClick()
    {
        ToastContainer.SetPlacement(Placement.BottomEnd);
        await ToastService.Show(new ToastOption()
        {
            Category = ToastCategory.Error,
            Title = "Failed to save",
            Content = "Failed to save data, automatically closes after 4 seconds"
        });
    }

    private async Task OnInfoClick()
    {
        ToastContainer.SetPlacement(Placement.BottomEnd);
        await ToastService.Show(new ToastOption()
        {
            Category = ToastCategory.Information,
            Title = "Notification",
            Content = "The system adds new components, it will automatically shut down after 4 seconds"
        });
    }

    private async Task OnWarningClick()
    {
        ToastContainer.SetPlacement(Placement.BottomEnd);
        await ToastService.Show(new ToastOption()
        {
            Category = ToastCategory.Warning,
            Title = "Warning",
            Content = "If the system finds abnormality, please deal with it in time, and it will automatically shut down after 4 seconds"
        });
    }

    private async Task OnNotAutoHideClick()
    {
        ToastContainer.SetPlacement(Placement.BottomEnd);
        await ToastService.Show(new ToastOption()
        {
            Category = ToastCategory.Warning,
            IsAutoHide = false,
            Title = "Notification",
            Content = "I will not close automatically, please click the close button in the upper right corner"
        });
    }

    private async Task OnShowHeaderClick()
    {
        ToastContainer.SetPlacement(Placement.BottomEnd);
        await ToastService.Show(new ToastOption()
        {
            Category = ToastCategory.Warning,
            ShowHeader = false,
            Content = "The system adds new components, it will automatically shut down after 4 seconds"
        });
    }

    private async Task OnHeaderTemplateClick()
    {
        ToastContainer.SetPlacement(Placement.BottomEnd);
        await ToastService.Show(new ToastOption()
        {
            Category = ToastCategory.Information,
            HeaderTemplate = RenderHeader,
            Content = "The system adds new components, it will automatically shut down after 4 seconds"
        });
    }

    private IEnumerable<AttributeItem> GetAttributes() => new AttributeItem[]
    {
        new AttributeItem() {
            Name = "Category",
            Description = Localizer["AttrCategory"],
            Type = "ToastCategory",
            ValueList = "Success/Information/Error/Warning",
            DefaultValue = "Success"
        },
        new AttributeItem() {
            Name = "Title",
            Description = Localizer["AttrTitle"],
            Type = "string",
            ValueList = "—",
            DefaultValue = ""
        },
        new AttributeItem() {
            Name = "Cotent",
            Description = Localizer["AttrCotent"],
            Type = "string",
            ValueList = "—",
            DefaultValue = ""
        },
        new AttributeItem() {
            Name = "Delay",
            Description = Localizer["AttrDelay"],
            Type = "int",
            ValueList = "—",
            DefaultValue = "4000"
        },
        new AttributeItem() {
            Name = "IsAutoHide",
            Description = Localizer["AttrIsAutoHide"],
            Type = "boolean",
            ValueList = "",
            DefaultValue = "true"
        },
        new AttributeItem() {
            Name = "IsHtml",
            Description = Localizer["AttrIsHtml"],
            Type = "boolean",
            ValueList = "",
            DefaultValue = "false"
        },
        new AttributeItem() {
            Name = "Placement",
            Description = Localizer["AttrPlacement"],
            Type = "Placement",
            ValueList = "Auto / Top / Left / Bottom / Right",
            DefaultValue = "Auto"
        }
    };
}
