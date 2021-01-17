using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractStorage;
using OwlCore.Collections;
using OwlCore.Extensions;
using StrixMusic.Core.LocalFiles.Backing.Services;
using StrixMusic.Core.LocalFiles.Models;
using StrixMusic.Core.LocalFiles.Services;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Services.Settings;

namespace StrixMusic.Core.LocalFiles
{
    /// <inheritdoc />
    public class LocalFileCore : ICore
    {
        private static int CoreCount = 0;
        private ICoreLibrary? _coreLibrary;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFileCore"/> class.
        /// </summary>
        /// <param name="instanceId"></param>
        public LocalFileCore(string instanceId)
        {
            //TODO: The constructor warnings will be fixed once models are added to initialize the interfaces.
            InstanceId = instanceId;

            Devices = new SynchronizedObservableCollection<ICoreDevice>();
            RecentlyPlayed = new LocalFilesCoreRecentlyPlayed(this);
            Discoverables = new LocalFilesCoreDiscoverables(this);
            User = new LocalFilesCoreUser(this);
            CoreConfig = new LocalFileCoreConfig(this);
            _coreLibrary = new LocalFilesCoreLibrary(this);
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
        public ICoreLibrary? Library => _coreLibrary;

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
            await coreConfig.ScanFileMetadata();

            ChangeCoreState(CoreState.Loaded);
            CoreCount++;
            if (CoreCount == LocalFileCoreManager.Instances?.Count)
                await LocalFileCoreManager.InitializeDataForAllCores();
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
