using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hqub.MusicBrainz.API;
using Microsoft.Extensions.DependencyInjection;
using StrixMusic.Core.MusicBrainz.Models;
using StrixMusic.Sdk;
using StrixMusic.Sdk.Enums;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Core.MusicBrainz
{
    /// <summary>
    /// Mock Core
    /// </summary>
    public class MusicBrainzCore : ICore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MusicBrainzCore"/> class.
        /// </summary>
        /// <param name="instanceId"></param>
        public MusicBrainzCore(string instanceId)
        {
            InstanceId = instanceId;
            CoreConfig = new MusicBrainzCoreConfig(this);

            Library = new MusicBrainzLibrary(this);
        }

        /// <inheritdoc/>
        public ICoreConfig CoreConfig { get; }

        /// <inheritdoc/>
        public CoreState CoreState => throw new NotImplementedException();

        /// <inheritdoc/>
        public string Name => throw new NotImplementedException();

        /// <inheritdoc/>
        public IUser User => throw new NotImplementedException();

        /// <inheritdoc/>
        public IReadOnlyList<IDevice> Devices => throw new NotImplementedException();

        /// <inheritdoc/>
        public ILibrary Library { get; }

        /// <inheritdoc/>
        public IRecentlyPlayed RecentlyPlayed => throw new NotImplementedException();

        /// <inheritdoc/>
        public IPlayableCollectionGroup Discoverables => throw new NotImplementedException();

        /// <inheritdoc/>
        public string InstanceId { get; }

        /// <inheritdoc/>
        public event EventHandler<CoreState>? CoreStateChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IDevice>>? DevicesChanged;

        /// <inheritdoc/>
        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<IAsyncEnumerable<string>> GetSearchAutoCompleteAsync(string query)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<ISearchResults> GetSearchResultsAsync(string query)
        {
            return Task.FromResult(new MusicBrainzSearchResults(this) as ISearchResults);
        }

        /// <inheritdoc/>
        public Task InitAsync()
        {
            var service = CoreConfig.Services.GetService<MusicBrainzClient>();

            return Task.CompletedTask;
        }
    }
}
