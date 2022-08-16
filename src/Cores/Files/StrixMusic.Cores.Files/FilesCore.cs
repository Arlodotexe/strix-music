using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using OwlCore.AbstractStorage;
using OwlCore.AbstractUI.Models;
using OwlCore.Events;
using StrixMusic.Cores.Files.Models;
using StrixMusic.Cores.Files.Services;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.FileMetadata;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Cores.Files
{
    /// <summary>
    /// A common base class for all cores that handle scanning any kind of file system for audio files.
    /// </summary>
    public abstract class FilesCore : ICore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilesCore"/> class.
        /// </summary>
        /// <param name="instanceId">A unique identifier for this core instance.</param>
        protected FilesCore(string instanceId, Progress<FileScanState>? fileScanProgress)
        {
            FileScanProgress = fileScanProgress;
            InstanceId = instanceId;
            Devices = new List<ICoreDevice>();
            Library = new FilesCoreLibrary(this);
        }

        /// <inheritdoc />
        public abstract string Id { get; }

        /// <inheritdoc />
        public abstract string DisplayName { get; }

        /// <inheritdoc />
        public abstract ICoreImage? Logo { get; }

        /// <inheritdoc/>
        public string InstanceId { get; }

        /// <inheritdoc />
        public virtual string InstanceDescriptor { get; set; } = string.Empty;

        /// <inheritdoc />
        public abstract AbstractUICollection AbstractConfigPanel { get; }

        /// <inheritdoc />
        public virtual MediaPlayerType PlaybackType => MediaPlayerType.Standard;

        /// <inheritdoc />
        public ICore SourceCore => this;

        /// <summary>
        /// Manages scanning and caching of all music metadata from files in a folder.
        /// </summary>
        public IFileMetadataManager? FileMetadataManager { get; set; }

        /// <summary>
        /// Reports progress for any file scans that take place.
        /// </summary>
        public Progress<FileScanState>? FileScanProgress { get; set; }

        /// <summary>
        /// The wait behavior of the metadata scanner when InitAsync is called in a file-based <see cref="ICore"/>.
        /// </summary>
        public ScannerWaitBehavior ScannerWaitBehavior { get; set; }

        /// <inheritdoc/>
        public virtual CoreState CoreState { get; protected set; }

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
        public abstract event EventHandler? AbstractConfigPanelChanged;

        /// <inheritdoc />
        public abstract event EventHandler<string>? InstanceDescriptorChanged;

        /// <inheritdoc />
        public event EventHandler<ICoreImage?>? LogoChanged;

        /// <inheritdoc />
        public event EventHandler<string>? DisplayNameChanged;

        /// <inheritdoc/>
        public abstract Task InitAsync(CancellationToken cancellationToken = default);

        /// <inheritdoc/>
        public bool IsInitialized { get; protected set; }

        /// <inheritdoc/>
        public virtual ValueTask DisposeAsync()
        {
            // Dispose any resources not known to the SDK.
            // Do not dispose Library, Devices, etc. manually. The SDK will dispose these for you.
            return default;
        }

        /// <inheritdoc/>
        public async Task<ICoreModel?> GetContextByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            Guard.IsNotNull(FileMetadataManager, nameof(FileMetadataManager));

            var artist = await FileMetadataManager.AlbumArtists.GetByIdAsync(id);
            if (artist != null)
                return InstanceCache.Artists.GetOrCreate(id, SourceCore, artist);

            var album = await FileMetadataManager.Albums.GetByIdAsync(id);
            if (album != null)
                return InstanceCache.Albums.GetOrCreate(id, SourceCore, album);

            var track = await FileMetadataManager.Tracks.GetByIdAsync(id);
            if (track != null)
                return new FilesCoreTrack(SourceCore, track);

            return null;
        }

        /// <inheritdoc/>
        /// <remarks>
        /// You may override this and return a different MediaSourceConfig if needed, such as a Stream instead of a file path.
        /// </remarks>
        public virtual Task<IMediaSourceConfig?> GetMediaSourceAsync(ICoreTrack track, CancellationToken cancellationToken = default)
        {
            if (!(track is FilesCoreTrack t))
                return Task.FromResult<IMediaSourceConfig?>(null);

            Guard.IsNotNullOrWhiteSpace(t.LocalTrackPath, nameof(t.LocalTrackPath));

            var mediaSource = new MediaSourceConfig(track, track.Id, new Uri(t.LocalTrackPath));
            return Task.FromResult<IMediaSourceConfig?>(mediaSource);
        }
    }
}
