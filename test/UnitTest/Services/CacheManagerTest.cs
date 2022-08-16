﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

namespace UnitTest.Services;

public class CacheManagerTest : BootstrapBlazorTestBase
{
    [Fact]
    public void GetStartTime_Ok()
    {
        Cache.SetStartTime();

        Assert.True(DateTime.Now > Cache.GetStartTime());
    }

    [Fact]
    public async Task GetOrCreateAsync_Ok()
    {
        var key = new object();
        var val = 0;
        var actual = await GetOrCreateAsync(key);
        Assert.Equal(1, actual);

        actual = await GetOrCreateAsync(key);
        Assert.Equal(1, actual);

        Task<int> GetOrCreateAsync(object key) => Cache.GetOrCreateAsync<int>(key, entry =>
        {
            val++;
            return Task.FromResult(val);
        });
    }

    [Fact]
    public void GetOrCreate_Ok()
    {
        var key = new object();
        var val = 0;
        var actual = GetOrCreate(key);
        Assert.Equal(1, actual);

        actual = GetOrCreate(key);
        Assert.Equal(1, actual);

        int GetOrCreate(object key) => Cache.GetOrCreate<int>(key, entry =>
        {
            val++;
            return val;
        });
    }
}
