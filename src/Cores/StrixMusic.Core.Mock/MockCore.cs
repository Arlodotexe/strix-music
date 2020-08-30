using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.Core.Mock.Deserialization;
using StrixMusic.Core.Mock.Models;
using StrixMusic.CoreInterfaces;
using StrixMusic.CoreInterfaces.Interfaces;
using StrixMusic.CoreInterfaces.Interfaces.CoreConfig;

namespace StrixMusic.Core.Mock
{
    /// <summary>
    /// Mock Core
    /// </summary>
    public class MockCore : ICore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MockCore"/> class.
        /// </summary>
        /// <param name="instanceId"></param>
        public MockCore(string instanceId)
        {
            InstanceId = instanceId;
        }

        /// <inheritdoc/>
        public SerializedLibrary _serializedLibaray { get; set; }

        /// <inheritdoc/>
        public ICoreConfig CoreConfig { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public CoreState CoreState => throw new NotImplementedException();

        /// <inheritdoc/>
        public string Name => throw new NotImplementedException();

        /// <inheritdoc/>
        public IUser User => throw new NotImplementedException();

        /// <inheritdoc/>
        public IReadOnlyList<IDevice> Devices => throw new NotImplementedException();

        /// <inheritdoc/>
        public ILibrary Library => throw new NotImplementedException();

        /// <inheritdoc/>
        public IRecentlyPlayed RecentlyPlayed => throw new NotImplementedException();

        /// <inheritdoc/>
        public IPlayableCollectionGroup Discoverables => throw new NotImplementedException();

        /// <inheritdoc/>
        public string InstanceId { get; }

        /// <inheritdoc/>
        public event EventHandler<CoreState> CoreStateChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IDevice>> DevicesChanged;

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
            ISearchResults searchResults = new MockSearchResults();
            return (Task<ISearchResults>)searchResults;
        }

        /// <inheritdoc/>
        public async Task InitAsync()
        {
            _serializedLibaray = await Deserializer.DeserializeLibrary();
        }
    }
}
