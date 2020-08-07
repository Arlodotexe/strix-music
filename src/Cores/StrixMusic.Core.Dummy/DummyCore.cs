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
        public async Task<IPlayableCollectionGroup> GetLibrary()
        {
            SerializedLibrary library = Deserializer.DeserializeLibrary();
            var lib = new Library(library.Tracks!, library.Albums!, library.Artists!, this);
            return lib;
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
