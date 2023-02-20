using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Cores.SoundCloud.Models;

public sealed class SoundCloudImage : ICoreImage
{
    private readonly Uri _uri;

    public SoundCloudImage(ICore sourceCore, Uri uri)
    {
        SourceCore = sourceCore;
        _uri = uri;
    }

    public string? MimeType => null;

    public double? Height => null;

    public double? Width => null;

    public ICore SourceCore { get; }

    public async Task<Stream> OpenStreamAsync()
    {
        // TODO: Don't make a new HttpClient every time
        HttpClient client = new();
        return await client.GetStreamAsync(_uri);
    }
}
