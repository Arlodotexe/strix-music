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
using StrixMusic.Sdk.Services.Notifications;
using StrixMusic.Sdk.Services.Settings;

namespace StrixMusic.Core.LocalFiles
{
    /// <inheritdoc />
    public class LocalFilesCore : ICore
    {
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
            CoreConfig = new LocalFilesCoreConfig(this);
            Library = new LocalFilesCoreLibrary(this);
        }

        /// <inheritdoc/>
        public string InstanceId { get; }

        /// <inheritdoc/>
        public ICoreConfig CoreConfig { get; }

        /// <inheritdoc />
        public ICore SourceCore => this;

        /// <inheritdoc/>
        public CoreState CoreState { get; internal set; } = CoreState.Unloaded;

        /// <inheritdoc />
        public string InstanceDescriptor { get; private set; } = string.Empty;

        /// <inheritdoc/>
        public ICoreUser? User { get; }

        /// <inheritdoc/>
        public IReadOnlyList<ICoreDevice> Devices { get; }

        /// <inheritdoc/>
        public ICoreLibrary Library { get; }

        /// <inheritdoc />
        public ICoreSearch? Search { get; }

        /// <inheritdoc/>
        public ICoreRecentlyPlayed? RecentlyPlayed { get; }

        /// <inheritdoc/>
        public ICoreDiscoverables? Discoverables { get; }

        /// <inheritdoc/>
        public ICorePlayableCollectionGroup? Pins { get; }

        /// <inheritdoc/>
        public event EventHandler<CoreState>? CoreStateChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreDevice>? DevicesChanged;

        /// <inheritdoc />
        public event EventHandler<string>? InstanceDescriptorChanged;

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
            // Dispose any resources not known to the SDK.
            // Do not dispose Library, Devices, etc. manually. The SDK will dispose these for you.
            return default;
        }

        /// <inheritdoc/>
        public async Task InitAsync(IServiceCollection services)
        {
            Guard.IsNotNull(services, nameof(services));

            ChangeCoreState(CoreState.Loading);

            if (!(CoreConfig is LocalFilesCoreConfig coreConfig))
                return;

            await coreConfig.SetupConfigurationServices(services);

            var configuredFolder = await coreConfig.GetConfiguredFolder();
            if (configuredFolder is null)
            {
                _ = PickAndSaveFolder();
                ChangeCoreState(CoreState.NeedsSetup);
                return;
            }

            InstanceDescriptor = configuredFolder.Path;
            InstanceDescriptorChanged?.Invoke(this, InstanceDescriptor);

            await coreConfig.SetupServices(services);
            await Library.Cast<LocalFilesCoreLibrary>().InitAsync();

            Guard.IsNotNull(CoreConfig.Services, nameof(CoreConfig.Services));
            ChangeCoreState(CoreState.Loaded);
        }

        private async Task PickAndSaveFolder()
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

            var artist = await fileMetadataManager.Artists.GetArtistById(id);
            if (artist != null)
                return InstanceCache.Artists.GetOrCreate(id, SourceCore, artist);

            var album = await fileMetadataManager.Albums.GetAlbumById(id);
            if (album != null)
                return InstanceCache.Albums.GetOrCreate(id, SourceCore, album);

            var track = await fileMetadataManager.Tracks.GetTrackById(id);
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
