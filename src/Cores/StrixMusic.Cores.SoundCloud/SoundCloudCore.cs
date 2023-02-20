using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.ComponentModel;
using SoundCloud.Api;
using StrixMusic.Cores.SoundCloud.Models;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Cores.SoundCloud;

public class SoundCloudCore : ICore
{
    public SoundCloudCore(string instanceId, string token)
        : this(instanceId, SoundCloudClient.CreateAuthorized(token))
    {
    }

    public SoundCloudCore(string instanceId, ISoundCloudClient client)
    {
        Client = client;

        InstanceId = instanceId;
        Id = nameof(SoundCloudCore);
        SourceCore = this;

        DisplayName = "SoundCloud";
        InstanceDescriptor = string.Empty;
        Devices = new List<ICoreDevice>();
        Library = new SoundCloudLibrary(this);
    }

    internal ISoundCloudClient Client { get; }

    public string Id { get; private set; }

    public string InstanceId { get; private set; }

    public string InstanceDescriptor { get; private set; }

    public string DisplayName { get; private set; }

    public ICoreImage? Logo { get; private set; }

    public MediaPlayerType PlaybackType { get; private set; }

    public ICoreUser? User { get; private set; }

    public IReadOnlyList<ICoreDevice> Devices { get; private set; }

    public ICoreLibrary Library { get; private set; }

    public ICorePlayableCollectionGroup? Pins { get; private set; }

    public ICoreSearch? Search { get; private set; }

    public ICoreRecentlyPlayed? RecentlyPlayed { get; private set; }

    public ICoreDiscoverables? Discoverables { get; private set; }

    public bool IsInitialized { get; private set; }

    public ICore SourceCore { get; private set; }

    public event EventHandler<ICoreImage?>? LogoChanged;
    public event CollectionChangedEventHandler<ICoreDevice>? DevicesChanged;
    public event EventHandler<string>? DisplayNameChanged;
    public event EventHandler<string>? InstanceDescriptorChanged;

    public ValueTask DisposeAsync()
    {
        return default;
    }

    public Task<ICoreModel?> GetContextByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<ICoreModel?>(null);
    }

    public Task<IMediaSourceConfig?> GetMediaSourceAsync(ICoreTrack track, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IMediaSourceConfig?>(null);
    }

    public async Task InitAsync(CancellationToken cancellationToken = default)
    {
        await ((SoundCloudLibrary)Library).InitAsync();

        IsInitialized = true;
    }
}
