﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

namespace BootstrapBlazor.Components;

/// <summary>
/// 百度文字识别服务
/// </summary>
public interface IBaiduOcr
{
    /// <summary>
    /// 增值税发票识别
    /// </summary>
    /// <param name="image"></param>
    /// <param name="token"></param>
    Task<BaiduOcrResult<InvoiceEntity>> CheckVatInvoiceAsync(byte[] image, CancellationToken token = default);

    /// <summary>
    /// 增值税发票识别
    /// </summary>
    /// <param name="image"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<string> CheckVatInvoiceJsonAsync(byte[] image, CancellationToken token = default);
}
