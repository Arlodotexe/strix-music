using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.Events;
using StrixMusic.Cores.Files.Models;
using StrixMusic.Cores.Files.Services;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Services.FileMetadataManager;

namespace StrixMusic.Cores.Files
{
    /// <inheritdoc />
    public abstract class FilesCore : ICore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilesCore"/> class.
        /// </summary>
        /// <param name="instanceId">A unique identifier for this core instance.</param>
        protected FilesCore(string instanceId)
        {
            InstanceId = instanceId;

            Devices = new List<ICoreDevice>();
            Library = new FilesCoreLibrary(this);
        }

        /// <inheritdoc/>
        public string InstanceId { get; }

        /// <inheritdoc/>
        public abstract ICoreConfig CoreConfig { get; protected set; }

        /// <inheritdoc />
        public ICore SourceCore => this;

        /// <inheritdoc/>
        public virtual CoreState CoreState { get; protected set; }

        /// <inheritdoc />
        public virtual string InstanceDescriptor { get; set; } = string.Empty;

        /// <inheritdoc/>
        public ICoreUser? User => null;

        /// <inheritdoc/>
        public IReadOnlyList<ICoreDevice> Devices { get; }

        /// <inheritdoc/>
        public ICoreLibrary Library { get; protected set; }

        /// <inheritdoc />
        public ICoreSearch? Search => null;

        /// <inheritdoc/>
        public ICoreRecentlyPlayed? RecentlyPlayed => null;

        /// <inheritdoc/>
        public ICoreDiscoverables? Discoverables => null;

        /// <inheritdoc/>
        public ICorePlayableCollectionGroup? Pins => null;

        /// <inheritdoc/>
        public abstract event EventHandler<CoreState>? CoreStateChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreDevice>? DevicesChanged;

        /// <inheritdoc />
        public abstract event EventHandler<string>? InstanceDescriptorChanged;

        /// <inheritdoc/>
        public abstract Task InitAsync(IServiceCollection services);

        /// <inheritdoc/>
        public virtual ValueTask DisposeAsync()
        {
            // Dispose any resources not known to the SDK.
            // Do not dispose Library, Devices, etc. manually. The SDK will dispose these for you.
            return default;
        }

        /// <inheritdoc/>
        public async Task<ICoreMember?> GetContextById(string id)
        {
            var fileMetadataManager = this.GetService<FileMetadataManager>();

            var artist = await fileMetadataManager.Artists.GetArtistById(id);
            if (artist != null)
                return InstanceCache.Artists.GetOrCreate(id, SourceCore, artist);

            var album = await fileMetadataManager.Albums.GetAlbumById(id);
            if (album != null)
                return InstanceCache.Albums.GetOrCreate(id, SourceCore, album);

            var track = await fileMetadataManager.Tracks.GetTrackById(id);
            if (track != null)
                return new FilesCoreTrack(SourceCore, track);

            return null;
        }

        /// <inheritdoc/>
        public Task<IMediaSourceConfig?> GetMediaSource(ICoreTrack track)
        {
            if (!(track is FilesCoreTrack t))
                return Task.FromResult<IMediaSourceConfig?>(null);

            Guard.IsNotNull(t.LocalTrackPath, nameof(t.LocalTrackPath));

            var mediaSource = new MediaSourceConfig(track, track.Id, t.LocalTrackPath);
            return Task.FromResult<IMediaSourceConfig?>(mediaSource);
        }
    }
}
