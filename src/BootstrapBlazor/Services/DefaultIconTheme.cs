﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

namespace BootstrapBlazor.Components;

internal class DefaultIconTheme : IIconTheme
{
    private Dictionary<ComponentIcons, string> Icons { get; }

    public DefaultIconTheme()
    {
        Icons = new Dictionary<ComponentIcons, string>()
        {
            { ComponentIcons.AnchorLinkIcon, "fa-solid fa-link" },
            { ComponentIcons.AvatarIcon, "fa-solid fa-user" },

            { ComponentIcons.ButtonLoadingIcon, "fa-solid fa-fw fa-spin fa-spinner" },

            { ComponentIcons.CaptchaRefreshIcon, "fa-solid fa-arrows-rotate" },
            { ComponentIcons.CaptchaBarIcon, "fa-solid fa-arrow-right" },
            { ComponentIcons.CameraPlayIcon, "fa-solid fa-circle-play" },
            { ComponentIcons.CameraStopIcon, "fa-solid fa-circle-stop" },
            { ComponentIcons.CameraPhotoIcon, "fa-solid fa-camera-retro" },
            { ComponentIcons.CardCollapseIcon, "fa-solid fa-circle-chevron-right" },
            { ComponentIcons.CarouselPreviousIcon, "fa-solid fa-angle-left" },
            { ComponentIcons.CarouselNextIcon, "fa-solid fa-angle-right" },
            { ComponentIcons.CascaderIcon, "fa-solid fa-angle-up" },
            { ComponentIcons.CascaderSubMenuIcon, "fa-solid fa-angle-down" },
            { ComponentIcons.ConsoleClearButtonIcon, "fa-solid fa-xmark" },

            { ComponentIcons.DatePickBodyPreviousYearIcon, "fa-solid fa-angles-left" },
            { ComponentIcons.DatePickBodyPreviousMonthIcon, "fa-solid fa-angle-left" },
            { ComponentIcons.DatePickBodyNextMonthIcon, "fa-solid fa-angle-right" },
            { ComponentIcons.DatePickBodyNextYearIcon, "fa-solid fa-angles-right" },

            { ComponentIcons.DateTimePickerIcon, "fa-regular fa-calendar-days" },

            { ComponentIcons.TimePickerCellUpIcon, "fa-solid fa-angle-up" },
            { ComponentIcons.TimePickerCellDownIcon, "fa-solid fa-angle-down" },

            { ComponentIcons.DateTimeRangeIcon, "fa-regular fa-calendar-days" },
            { ComponentIcons.DateTimeRangeClearIcon, "fa-solid fa-circle-xmark" },

            { ComponentIcons.LoadingIcon, "fa-solid fa-fw fa-spin fa-spinner" },

            { ComponentIcons.PopConfirmButtonConfirmIcon, "fa-solid fa-circle-exclamation text-info" },
            { ComponentIcons.PopConfirmButtonContentIcon, "fa-solid fa-exclamation-circle text-info" },

            { ComponentIcons.TableSortIconAsc, "fa-solid fa-sort-up" },
            { ComponentIcons.TableSortDesc, "fa-solid fa-sort-down" },
            { ComponentIcons.TableSortIcon, "fa-solid fa-sort" },
            { ComponentIcons.TableFilterIcon, "fa-solid fa-filter" },
            { ComponentIcons.TableExportButtonIcon, "fa-solid fa-download" },

            { ComponentIcons.SearchClearButtonIcon, "fa-regular fa-trash-can" },
            { ComponentIcons.SearchButtonIcon, "fa-fw fa-solid fa-magnifying-glass" },
            { ComponentIcons.SearchButtonLoadingIcon, "fa-solid fa-fw fa-spin fa-spinner" }
        };
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Dictionary<ComponentIcons, string> GetIcons() => Icons;
}
