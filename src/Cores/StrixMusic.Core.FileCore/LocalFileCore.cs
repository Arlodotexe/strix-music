using Microsoft.Extensions.DependencyInjection;
using OwlCore.Collections;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrixMusic.Core.FileCore
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
        }

        /// <inheritdoc/>
        public ICoreConfig CoreConfig { get; }

        /// <inheritdoc />
        public ICore SourceCore => this;

        /// <inheritdoc/>
        public CoreState CoreState { get; internal set; } = CoreState.Unloaded;

        /// <inheritdoc/>
        public string Name => "FileCore";

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

        /// <inheritdoc/>
        public Task InitAsync(IServiceCollection services)
        {
            throw new NotImplementedException();
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
