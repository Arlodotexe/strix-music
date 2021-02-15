using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractStorage;
using OwlCore.Collections;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Core.LocalFiles.Models;
using StrixMusic.Core.LocalFiles.Services;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Services.FileMetadataManager;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;
using StrixMusic.Sdk.Services.Notifications;
using StrixMusic.Sdk.Services.Settings;

namespace StrixMusic.Core.LocalFiles
{
    /// <inheritdoc />
    public class LocalFilesCore : ICore
    {
        private static int _coreCount;
        private readonly ICoreLibrary _coreLibrary;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFilesCore"/> class.
        /// </summary>
        /// <param name="instanceId"></param>
        public LocalFilesCore(string instanceId)
        {
            InstanceId = instanceId;

            Devices = new SynchronizedObservableCollection<ICoreDevice>();
            RecentlyPlayed = new LocalFilesCoreRecentlyPlayed(this);
            Discoverables = new LocalFilesCoreDiscoverables(this);
            CoreConfig = new LocalFileCoreConfig(this);
            _coreLibrary = new LocalFilesCoreLibrary(this);

            LocalFileCoreManager.Instances?.Add(this);
        }

        /// <inheritdoc/>
        public ICoreConfig CoreConfig { get; }

        /// <inheritdoc />
        public ICore SourceCore => this;

        /// <inheritdoc/>
        public CoreState CoreState { get; internal set; } = CoreState.Unloaded;

        /// <inheritdoc/>
        public string Name => "LocalFileCore";

        /// <inheritdoc/>
        public ICoreUser? User { get; }

        /// <inheritdoc/>
        public IReadOnlyList<ICoreDevice> Devices { get; }

        /// <inheritdoc/>
        public ICoreLibrary Library => _coreLibrary;

        /// <inheritdoc />
        public ICoreSearch? Search { get; }

        /// <inheritdoc/>
        public ICoreRecentlyPlayed? RecentlyPlayed { get; }

        /// <inheritdoc/>
        public ICoreDiscoverables? Discoverables { get; }

        /// <inheritdoc/>
        public string InstanceId { get; }

        /// <inheritdoc/>
        public ICorePlayableCollectionGroup? Pins { get; }

        /// <inheritdoc/>
        public event EventHandler<CoreState>? CoreStateChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreDevice>? DevicesChanged;

        /// <summary>
        /// Change the <see cref="CoreState"/>.
        /// </summary>
        /// <param name="state">The new state.</param>
        internal void ChangeCoreState(CoreState state)
        {
            CoreState = state;
            CoreStateChanged?.Invoke(this, state);
        }

        /// <inheritdoc/>
        public ValueTask DisposeAsync()
        {
            return default;
        }

        /// <inheritdoc/>
        public async Task InitAsync(IServiceCollection services)
        {
            Guard.IsNotNull(services, nameof(services));

            ChangeCoreState(CoreState.Loading);

            if (!(CoreConfig is LocalFileCoreConfig coreConfig))
                return;

            await coreConfig.SetupConfigurationServices(services);
            var configuredFolder = await coreConfig.GetConfiguredFolder();

            if (configuredFolder is null)
            {
                PickAndSetupFolder().FireAndForget();

                ChangeCoreState(CoreState.Configuring);

                return;
            }

            await coreConfig.ConfigureServices(services);

            ChangeCoreState(CoreState.Loaded);

            coreConfig.ScanFileMetadata().FireAndForget();

            _coreCount++;

            if (_coreCount == LocalFileCoreManager.Instances?.Count)
                LocalFileCoreManager.InitializeDataForAllCores().FireAndForget();
        }

        private async Task PickAndSetupFolder()
        {
            var fileSystem = this.GetService<IFileSystemService>();
            var pickedFolder = await fileSystem.PickFolder();

            // If they don't pick a folder, unload the core.
            if (pickedFolder is null)
            {
                this.GetService<INotificationService>().RaiseNotification("No folder selected", "Unloading file core.");
                ChangeCoreState(CoreState.Unloaded);
                return;
            }

            await this.GetService<ISettingsService>().SetValue<string?>(nameof(LocalFilesCoreSettingsKeys.FolderPath), pickedFolder.Path);

            ChangeCoreState(CoreState.Configured);
        }

        /// <inheritdoc/>
        public async Task<ICoreMember?> GetContextById(string id)
        {
            var fileMetadataManager = this.GetService<FileMetadataManager>();

            var artist = await fileMetadataManager.Artists.GetArtistMetadataById(id);

            if (artist != null)
            {
                if (artist.ImagePath != null)
                    return new LocalFilesCoreArtist(SourceCore, artist, artist.TrackIds?.Count ?? 0, new LocalFilesCoreImage(SourceCore, artist.ImagePath));

                return new LocalFilesCoreArtist(SourceCore, artist, artist.TrackIds?.Count ?? 0, null);
            }

            var album = await fileMetadataManager.Albums.GetAlbumMetadataById(id);

            if (album != null)
            {
                TrackMetadata? trackWithImage = null;

                if (album.TrackIds != null)
                {
                    foreach (var item in album.TrackIds)
                    {
                        var relatedTrack = await fileMetadataManager.Tracks.GetTrackMetadataById(item);

                        if (relatedTrack == null)
                            continue;

                        if (relatedTrack.ImagePath != null)
                            trackWithImage = relatedTrack;
                    }
                }

                if (trackWithImage == null)
                    return new LocalFilesCoreAlbum(SourceCore, album, album.TrackIds?.Count ?? 0, trackWithImage?.ImagePath != null ? new LocalFilesCoreImage(SourceCore, trackWithImage.ImagePath) : null);

                return new LocalFilesCoreAlbum(SourceCore, album, album.TrackIds?.Count ?? 0, null);
            }

            var track = await fileMetadataManager.Tracks.GetTrackMetadataById(id);

            if (track != null)
                return new LocalFilesCoreTrack(SourceCore, track);

            return null;
        }

        /// <inheritdoc/>
        public Task<IMediaSourceConfig?> GetMediaSource(ICoreTrack track)
        {
            if (!(track is LocalFilesCoreTrack t))
                return Task.FromResult<IMediaSourceConfig?>(null);

            Guard.IsNotNull(t.LocalTrackPath, nameof(t.LocalTrackPath));

            var mediaSource = new MediaSourceConfig(track, track.Id, t.LocalTrackPath);
            return Task.FromResult<IMediaSourceConfig?>(mediaSource);
        }
    }
}
