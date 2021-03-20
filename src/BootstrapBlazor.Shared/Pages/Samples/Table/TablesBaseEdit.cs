﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using BootstrapBlazor.Components;
using BootstrapBlazor.Shared.Pages.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BootstrapBlazor.Shared.Pages
{
    /// <summary>
    ///
    /// </summary>
    public abstract class TablesBaseEdit : TablesBaseQuery
    {
        private static readonly object _objectLock = new();

        /// <summary>
        ///
        /// </summary>
        protected List<Foo> EditItems { get; set; } = GenerateItems();

        /// <summary>
        ///
        /// </summary>
        protected IEnumerable<SelectedItem>? Educations { get; set; }

        /// <summary>
        /// OnInitialized 方法
        /// </summary>
        protected override void OnInitialized()
        {
            base.OnInitialized();

            Educations = typeof(EnumEducation).ToSelectList();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        protected Task<QueryData<Foo>> OnEditQueryAsync(QueryPageOptions options) => FooQueryAsync(EditItems, options);

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        protected static Task<Foo> OnAddAsync()
        {
            return Task.FromResult(new Foo() { DateTime = DateTime.Now });
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected Task<bool> OnSaveAsync(Foo item)
        {
            // 增加数据演示代码
            if (item.Id == 0)
            {
                // 演示代码，生产中请根据实际情况考虑是否加锁操作
                lock (_objectLock)
                {
                    item.Id = EditItems.Max(i => i.Id) + 1;
                    EditItems.Add(item);
                }
            }
            else
            {
                var oldItem = EditItems.FirstOrDefault(i => i.Id == item.Id);
                if (oldItem != null)
                {
                    oldItem.Name = item.Name;
                    oldItem.Address = item.Address;
                    oldItem.DateTime = item.DateTime;
                    oldItem.Count = item.Count;
                    oldItem.Complete = item.Complete;
                    oldItem.Education = item.Education;
                }
            }
            return Task.FromResult(true);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        protected Task<bool> OnDeleteAsync(IEnumerable<Foo> items)
        {
            items.ToList().ForEach(i => EditItems.Remove(i));
            return Task.FromResult(true);
        }
    }
}
