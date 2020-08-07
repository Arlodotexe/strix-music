using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.Core.Files.Models;
using StrixMusic.CoreInterfaces.Interfaces;
using StrixMusic.CoreInterfaces.Interfaces.CoreConfig;

namespace StrixMusic.Core.Files
{

    /// <summary>
    /// A Core that handles local files.
    /// </summary>
    public class FileCore : ICore
    {
        /// <summary>
        /// Configures the <see cref="FileCore"/>
        /// </summary>
        public ICoreConfig CoreConfig { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public string Name => nameof(FileCore);

        /// <inheritdoc/>
        public IUser User { get => new FileUser(); set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public event EventHandler<IDevice> DeviceAdded;

        /// <inheritdoc/>
        public event EventHandler<IDevice> DeviceRemoved;

        /// <inheritdoc/>
        public event EventHandler<IPlayableCollectionGroup> LibraryChanged;

        /// <inheritdoc/>
        public event EventHandler<IPlayableCollectionGroup> RecentlyPlayedChanged;

        /// <inheritdoc/>
        public Task<IList<IDevice>> GetDevices()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<IList<IPlayableCollectionGroup>> GetDiscoverables()
        {
            var emptyResult = (IList<IPlayableCollectionGroup>)new List<IPlayableCollectionGroup>();

            return Task.FromResult(emptyResult);
        }

        /// <inheritdoc
        public Task<IPlayableCollectionGroup> GetLibrary()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<IPlayableCollectionGroup> GetRecentlyPlayedAsync()
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
