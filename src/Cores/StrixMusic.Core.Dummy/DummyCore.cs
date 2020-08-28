using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.CoreInterfaces;
using StrixMusic.CoreInterfaces.Interfaces;
using StrixMusic.CoreInterfaces.Interfaces.CoreConfig;

namespace StrixMusic.Core.Dummy
{
    /// <inheritdoc/>
    public class DummyCore : ICore
    {
        /// <inheritdoc/>
        public string Name => "Dummy Core";

        /// <inheritdoc/>
        public IUser User => throw new NotImplementedException();

        /// <inheritdoc/>
        public ICoreConfig CoreConfig { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public CoreState CoreState => throw new NotImplementedException();

        /// <inheritdoc/>
        public IReadOnlyList<IDevice> Devices => throw new NotImplementedException();

        /// <inheritdoc/>
        public ILibrary Library => throw new NotImplementedException();

        /// <inheritdoc/>
        public IRecentlyPlayed RecentlyPlayed => throw new NotImplementedException();

        /// <inheritdoc/>
        public IPlayableCollectionGroup Discoverables => throw new NotImplementedException();

        /// <inheritdoc/>
        public event EventHandler<IPlayableCollectionGroup>? LibraryChanged;

        /// <inheritdoc/>
        public event EventHandler<IPlayableCollectionGroup>? RecentlyPlayedChanged;

        /// <inheritdoc/>
        public event EventHandler<IPlayableCollectionGroup>? DiscoverableAdded;

        /// <inheritdoc/>
        public event EventHandler<IPlayableCollectionGroup>? DiscoverableRemoved;

        /// <inheritdoc/>
        public event EventHandler<CoreState>? CoreStateChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IDevice>>? DevicesChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IAsyncEnumerable<string>>>? SearchAutoCompleteChanged;

        /// <inheritdoc/>
        public event EventHandler<ISearchResults>? SearchResultsChanged;

        /// <inheritdoc/>
        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<IDevice> GetDevicesAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<IPlayableCollectionGroup>? GetDiscoverablesAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<string> GetSearchAutoComplete(string query)
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
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task InitAsync()
        {
            throw new NotImplementedException();
        }
    }
}
