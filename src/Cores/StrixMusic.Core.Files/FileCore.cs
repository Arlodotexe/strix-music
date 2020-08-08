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
        /// Initializes a new instance of the <see cref="FileCore"/> class.
        /// </summary>
        public FileCore()
        {
        }

        /// <inheritdoc/>
        public ICoreConfig CoreConfig { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public string Name => nameof(FileCore);

        /// <inheritdoc/>
        public IUser User { get => new FileUser(this); set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public CoreState CoreState => throw new NotImplementedException();

        /// <inheritdoc/>
        public event EventHandler<IDevice>? DeviceAdded;

        /// <inheritdoc/>
        public event EventHandler<IDevice>? DeviceRemoved;

        /// <inheritdoc/>
        public event EventHandler<IPlayableCollectionGroup>? LibraryChanged;

        /// <inheritdoc/>
        public event EventHandler<IPlayableCollectionGroup>? RecentlyPlayedChanged;

        /// <inheritdoc/>
        public IAsyncEnumerable<IDevice> GetDevicesAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<IPlayableCollectionGroup> GetDiscoverablesAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<IPlayableCollectionGroup> GetLibraryAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<IPlayableCollectionGroup> GetRecentlyPlayedAsync()
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
