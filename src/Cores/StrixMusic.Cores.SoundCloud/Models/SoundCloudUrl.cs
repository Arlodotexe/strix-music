using System;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Cores.SoundCloud.Models;

public sealed class SoundCloudUrl : ICoreUrl
{
    public SoundCloudUrl(ICore sourceCore, string label, Uri url, UrlType type)
    {
        SourceCore = sourceCore;
        Label = label;
        Url = url;
        Type = type;
    }

    public string Label { get; }

    public Uri Url { get; }

    public UrlType Type { get; }

    public ICore SourceCore { get; }
}
