﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using BootstrapBlazor.Localization.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace BootstrapBlazor.Server
{
    /// <summary>
    ///
    /// </summary>
    public class Startup
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="env"></param>
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Enviroment = env;
        }

        /// <summary>
        ///
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// 获得 当前运行时环境
        /// </summary>
        public IWebHostEnvironment Enviroment { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="services"></param>
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddResponseCompression();

            services.AddControllers();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddCultureStorage();

            services.AddBlazorBackgroundTask();

            // 增加 BootstrapBlazor 组件
            services.AddBootstrapBlazor(options =>
            {
                // 统一设置 Toast 组件自动消失时间
                options.ToastDelay = 4000;
            });

            // 增加 Table Excel 导出服务
            services.AddBootstrapBlazorTableExcelExport();

            // 增加 Table 数据服务操作类
            services.AddTableDemoDataService();

            // 增加 PetaPoco ORM 数据服务操作类
            // 需要时打开下面代码
            //services.AddPetaPoco(option =>
            //{
            //    // 配置数据信息
            //    // 使用 SQLite 数据以及从配置文件中获取数据库连接字符串
            //    // 需要引用 Microsoft.Data.Sqlite 包，操作 SQLite 数据库
            //    // 需要引用 PetaPoco.Extensions 包，PetaPoco 包扩展批量插入与删除
            //    option.UsingProvider<SQLiteDatabaseProvider>()
            //          .UsingConnectionString(Configuration.GetConnectionString("bb"));
            //});

            // 增加 FreeSql ORM 数据服务操作类
            // 需要时打开下面代码
            services.AddFreeSql(option => {
                option.UseConnectionString(FreeSql.DataType.Sqlite, Configuration.GetConnectionString("bb"))
                //开发环境:自动同步实体
                .UseAutoSyncStructure(true)
                .UseNoneCommandParameter(true)
                //调试sql语句输出
                .UseMonitorCommand(cmd => System.Console.WriteLine(cmd.CommandText))
                ;
            });

            // 增加 EFCore ORM 数据服务操作类
            // 需要时打开下面代码
            //services.AddEntityFrameworkCore<Shared.Pages.BindItemDbContext>(option =>
            //{
            //    // 需要引用 Microsoft.EntityFrameworkCore.Sqlite 包，操作 SQLite 数据库
            //    option.UseSqlite(Configuration.GetConnectionString("bb"));
            //});

            // 增加多语言支持配置信息
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = Configuration.GetSupportCultures().ToList();
                options.DefaultRequestCulture = new RequestCulture(CultureInfo.CurrentCulture);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // 启用本地化
            app.UseRequestLocalization(app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>()!.Value);

            app.UseForwardedHeaders(new ForwardedHeadersOptions() { ForwardedHeaders = ForwardedHeaders.All });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseResponseCompression();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors(builder => builder.WithOrigins(Configuration["AllowOrigins"].Split(',', StringSplitOptions.RemoveEmptyEntries)).AllowAnyHeader().AllowAnyMethod().AllowCredentials());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
