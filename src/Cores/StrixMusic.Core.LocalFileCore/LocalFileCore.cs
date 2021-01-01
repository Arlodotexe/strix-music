using Microsoft.Extensions.DependencyInjection;
using OwlCore.Collections;
using StrixMusic.Core.LocalFiles.Models;
using StrixMusic.Core.LocalFiles.Models;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        /// <inheritdoc/>
        private Task<ICoreSearchResults> GetSearchResultsAsync(string query)
        {
            throw new NotImplementedException();
        }

        private bool _configured = false;

        /// <inheritdoc/>
        public async Task InitAsync(IServiceCollection services)
        {
            ChangeCoreState(CoreState.Loading);

            if (!(CoreConfig is LocalFileCoreConfig coreConfig))
                return;

            // This was for testing purposes, and is now disabled.
            if (!_configured)
            {
                await coreConfig.SetupConfigurationServices(services);
                await coreConfig.SetupFileCoreFolder();
                _configured = true;

                ChangeCoreState(CoreState.Loaded);
                return;
            }
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

        /// <summary>
        /// Configures the minimum required services for core configuration in a safe manner and is guaranteed not to throw.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task SetupConfigurationServices(IServiceCollection services)
        {
            // var provider = services.BuildServiceProvider();

            // _fileSystemService = provider.GetRequiredService<IFileSystemService>();
            // return _fileSystemService.InitAsync();
            return Task.CompletedTask;
        }
    }
}
