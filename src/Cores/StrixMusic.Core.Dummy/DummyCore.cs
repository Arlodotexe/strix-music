using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.Core.Dummy.Deserialization;
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
        public event EventHandler<IPlayableCollectionGroup>? LibraryChanged;

        /// <inheritdoc/>
        public event EventHandler<IPlayableCollectionGroup>? RecentlyPlayedChanged;

        /// <inheritdoc/>
        public event EventHandler<IPlayableCollectionGroup>? DiscoverableAdded;

        /// <inheritdoc/>
        public event EventHandler<IPlayableCollectionGroup>? DiscoverableRemoved;

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
        public Task<ISearchResults> GetSearchResults(string query)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task Init()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        Task<ILibrary> ICore.GetLibraryAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        Task<IRecentlyPlayed> ICore.GetRecentlyPlayedAsync()
        {
            throw new NotImplementedException();
        }
    }
}
