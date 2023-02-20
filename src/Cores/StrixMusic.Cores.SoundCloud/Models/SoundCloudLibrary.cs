using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using SoundCloud.Api;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Cores.SoundCloud.Models;

public class SoundCloudLibrary : SoundCloudPlayableCollectionGroupBase, ICoreLibrary
{
    private readonly ISoundCloudClient _client;

    public SoundCloudLibrary(SoundCloudCore sourceCore)
        : base(sourceCore)
    {
        _client = sourceCore.Client;
    }

    public override string Id { get; } = "library";

    public override string Name { get; } = "Library";

    public override string? Description { get; } = null;

    public async override IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var tracks = await _client.Me.GetTracksAsync(limit);
        foreach (var item in tracks)
            yield return new SoundCloudTrack(SourceCore, item);
    }
}
