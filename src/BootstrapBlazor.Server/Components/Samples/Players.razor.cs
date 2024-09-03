﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

namespace BootstrapBlazor.Server.Components.Samples;

/// <summary>
/// Player 组件示例
/// </summary>
public partial class Players
{
    private string _url = "https://bitdash-a.akamaihd.net/content/sintel/hls/playlist.m3u8";

    private List<SelectedItem> Items { get; } = [];

    /// <summary>
    /// OnInitialized 方法
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        Items.Add(new SelectedItem("https://rtvelivestream.akamaized.net/rtvesec/la1/la1_main.m3u8", "TestVideo1"));
        Items.Add(new SelectedItem("https://d2zihajmogu5jn.cloudfront.net/bipbop-advanced/bipbop_16x9_variant.m3u8", "TestVideo2"));
        Items.Add(new SelectedItem("https://test-streams.mux.dev/x36xhzz/x36xhzz.m3u8", "TestVideo3"));
        Items.Add(new SelectedItem("https://res.cloudinary.com/dannykeane/video/upload/sp_full_hd/q_80:qmax_90,ac_none/v1/dk-memoji-dark.m3u8", "TestVideo4"));
        Items.Add(new SelectedItem("https://devstreaming-cdn.apple.com/videos/streaming/examples/img_bipbop_adv_example_fmp4/master.m3u8", "TestVideo5"));
        Items.Add(new SelectedItem("https://moctobpltc-i.akamaihd.net/hls/live/571329/eight/playlist.m3u8", "TestVideo6"));
        Items.Add(new SelectedItem("https://cph-p2p-msl.akamaized.net/hls/live/2000341/test/master.m3u8", "TestVideo7"));
        Items.Add(new SelectedItem("https://demo.unified-streaming.com/k8s/features/stable/video/tears-of-steel/tears-of-steel.mp4/.m3u8", "TestVideo8"));
        Items.Add(new SelectedItem("https://diceyk6a7voy4.cloudfront.net/e78752a1-2e83-43fa-85ae-3d508be29366/hls/fitfest-sample-1_Ott_Hls_Ts_Avc_Aac_16x9_1280x720p_30Hz_6.0Mbps_qvbr.m3u8", "TestVideo9"));
    }

    private static Task<PlayerOption> InitNormalPlayer()
    {
        var options = new PlayerOption();
        options.Source.Poster = "https://cdn.plyr.io/static/demo/View_From_A_Blue_Moon_Trailer-HD.jpg";
        options.Source.Sources.Add(new PlayerSources { Url = "https://cdn.plyr.io/static/demo/View_From_A_Blue_Moon_Trailer-720p.mp4", Type = "video/mp4" });
        options.Source.Sources.Add(new PlayerSources { Url = "https://cdn.plyr.io/static/demo/View_From_A_Blue_Moon_Trailer-1080p.mp4", Type = "video/mp4" });
        options.Source.Sources.Add(new PlayerSources { Url = "https://cdn.plyr.io/static/demo/View_From_A_Blue_Moon_Trailer-1440p.mp4", Type = "video/mp4" });

        options.Source.Tracks.Add(new PlayerTracks { Src = "https://cdn.plyr.io/static/demo/video/View_From_A_Blue_Moon_Trailer-HD.en.vtt", SrcLang = "en", Label = "English", Default = true });
        options.Source.Tracks.Add(new PlayerTracks { Src = "https://cdn.plyr.io/static/demo/View_From_A_Blue_Moon_Trailer-HD.fr.vtt", SrcLang = "fr", Label = "Français" });

        options.Thumbnail.Enabled = true;
        options.Thumbnail.PicNum = 184;
        options.Thumbnail.Urls.Add("https://cdn.plyr.io/static/demo/thumbs/100p-00001.jpg");
        options.Thumbnail.Urls.Add("https://cdn.plyr.io/static/demo/thumbs/100p-00002.jpg");
        options.Thumbnail.Urls.Add("https://cdn.plyr.io/static/demo/thumbs/100p-00003.jpg");
        options.Thumbnail.Urls.Add("https://cdn.plyr.io/static/demo/thumbs/100p-00004.jpg");

        options.Makers.Enabled = true;
        options.Makers.Points.Add(new PlayerPoint() { Time = 10, Label = "First Marker" });
        options.Makers.Points.Add(new PlayerPoint() { Time = 50, Label = "Second Marker" });

        return Task.FromResult(options);
    }

    private Task<PlayerOption> InitHlsPlayer()
    {
        var options = new PlayerOption
        {
            IsHls = true,
            Poster = "https://bitdash-a.akamaihd.net/content/sintel/poster.png"
        };
        options.Source.Sources.Add(new PlayerSources { Url = _url, Type = "application/x-mpegURL" });

        options.Makers.Enabled = true;
        options.Makers.Points.Add(new PlayerPoint() { Time = 60, Label = "First Marker" });
        options.Makers.Points.Add(new PlayerPoint() { Time = 300, Label = "Second Marker" });

        return Task.FromResult(options);
    }

    private Task ChangeUrl(SelectedItem e)
    {
        //_options.Source.Sources.Add(new PlayerSources { Url = _url, Type = _type });
        StateHasChanged();
        return Task.CompletedTask;
    }
}