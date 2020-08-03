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
        public event EventHandler<IDevice> DeviceAdded;

        /// <inheritdoc/>
        public event EventHandler<IDevice> DeviceRemoved;

        /// <inheritdoc/>
        public event EventHandler<IPlayableCollectionGroup> LibraryChanged;

        /// <inheritdoc/>
        public event EventHandler<IPlayableCollectionGroup> RecentlyPlayedChanged;

        /// <inheritdoc/>
        public event EventHandler<IPlayableCollectionGroup> DiscoverableAdded;

        /// <inheritdoc/>
        public event EventHandler<IPlayableCollectionGroup> DiscoverableRemoved;

        /// <inheritdoc/>
        public Task<IList<IDevice>> GetDevices()
        {
            return null;
        }

        /// <inheritdoc/>
        public Task<IList<IPlayableCollectionGroup>> GetDiscoverables()
        {
            return null;
        }

        /// <inheritdoc/>
        public Task<IPlayableCollectionGroup> GetLibrary()
        {
            return null;
        }

        /// <inheritdoc/>
        public Task<IPlayableCollectionGroup> GetRecentlyPlayed()
        {
            return null;
        }

        /// <inheritdoc/>
        public Task<IEnumerable<string>> GetSearchAutoComplete(string query)
        {
            return null;
        }

        /// <inheritdoc/>
        public Task<ISearchResults> GetSearchResults(string query)
        {
            return null;
        }
    }
}
