using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.CoreInterfaces.Interfaces;

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
        public event EventHandler<IDevice>? DeviceAdded;

        /// <inheritdoc/>
        public event EventHandler<IDevice>? DeviceRemoved;

        /// <inheritdoc/>
        public event EventHandler<IPlayableCollectionGroup>? LibraryChanged;

        /// <inheritdoc/>
        public event EventHandler<IPlayableCollectionGroup>? RecentlyPlayedChanged;

        /// <inheritdoc/>
        public event EventHandler<IPlayableCollectionGroup>? DiscoverableAdded;

        /// <inheritdoc/>
        public event EventHandler<IPlayableCollectionGroup>? DiscoverableRemoved;

        /// <inheritdoc/>
        public Task<IList<IDevice>> GetDevices()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<IList<IPlayableCollectionGroup>?> GetDiscoverables()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<IPlayableCollectionGroup> GetLibrary()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<IPlayableCollectionGroup> GetRecentlyPlayed()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<IEnumerable<string>> GetSearchAutoComplete(string query)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<ISearchResults> GetSearchResults(string query)
        {
            throw new NotImplementedException();
        }
    }
}
