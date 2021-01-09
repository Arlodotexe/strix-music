using Microsoft.Extensions.DependencyInjection;
using OwlCore.Collections;
using StrixMusic.Core.LocalFiles.Backing.Services;
using StrixMusic.Core.LocalFiles.Models;
using StrixMusic.Core.LocalFiles.Models;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OwlCore.AbstractStorage;
using OwlCore.Extensions;
using StrixMusic.Core.LocalFiles.Services;
using StrixMusic.Sdk.Services.Settings;

namespace StrixMusic.Core.LocalFiles
{
    /// <inheritdoc />
    public class LocalFileCore : ICore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFileCore"/> class.
        /// </summary>
        /// <param name="instanceId"></param>
        public LocalFileCore(string instanceId)
        {
            //TODO: The constructor warnings will be fixed once models are added to initialize the interfaces.
            InstanceId = instanceId;

            Library = new LocalFilesCoreLibrary(this);
            Devices = new SynchronizedObservableCollection<ICoreDevice>();
            RecentlyPlayed = new LocalFilesCoreRecentlyPlayed(this);
            Discoverables = new LocalFilesCoreDiscoverables(this);
            User = new LocalFilesCoreUser(this);
            CoreConfig = new LocalFileCoreConfig(this);
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
        public ICoreUser User { get; }

        /// <inheritdoc/>
        public SynchronizedObservableCollection<ICoreDevice> Devices { get; }

        /// <inheritdoc/>
        public ICoreLibrary Library { get; private set; }

        /// <inheritdoc/>
        public ICoreRecentlyPlayed RecentlyPlayed { get; }

        /// <inheritdoc/>
        public ICoreDiscoverables Discoverables { get; }

        /// <inheritdoc/>
        public string InstanceId { get; }

        /// <inheritdoc/>
        public ICorePlayableCollectionGroup? Pins { get; }

        /// <inheritdoc/>
        public event EventHandler<CoreState>? CoreStateChanged;

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

        private TrackService _trackService;

        /// <inheritdoc/>
        public async Task InitAsync(IServiceCollection services)
        {
            ChangeCoreState(CoreState.Loading);

            if (!(CoreConfig is LocalFileCoreConfig coreConfig))
                return;

            await coreConfig.SetupConfigurationServices(services);
            var configuredFolder = await coreConfig.GetConfiguredFolder();

            if (configuredFolder is null)
            {
                PickAndSetupFolder().FireAndForget();

                ChangeCoreState(CoreState.ConfigRequested);

                return;
            }

            await coreConfig.ConfigureServices(services);

            // todo: move library scanning somewhere else
            _trackService = this.GetService<TrackService>();
            await _trackService.InitAsync();
            await _trackService.CreateOrUpdateTrackMetadata();
            var metaData = await _trackService.GetTrackMetadata(0, 3);

            ChangeCoreState(CoreState.Loaded);
        }

        private async Task PickAndSetupFolder()
        {
            var fileSystem = this.GetService<IFileSystemService>();
            var pickedFolder = await fileSystem.PickFolder();

            // If they don't pick a folder, unload the core.
            if (pickedFolder is null)
            {
                // todo: show notification with "A folder must be picked"
                ChangeCoreState(CoreState.Unloaded);
                return;
            }

            await this.GetService<ISettingsService>().SetValue<string?>(nameof(LocalFilesCoreSettingsKeys.FolderPath), pickedFolder.Path);

            ChangeCoreState(CoreState.Configured);
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<ICoreMember> GetContextById(string id)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<IMediaSourceConfig?> GetMediaSource(ICoreTrack track)
        {
            throw new NotSupportedException();
        }
    }
}
