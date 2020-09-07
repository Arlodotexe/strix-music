using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using StrixMusic.Core.Files.Models;
using StrixMusic.Core.Files.Services;
using StrixMusic.Sdk.Interfaces;

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
        public FileCore(string instanceId)
        {
            InstanceId = instanceId;

            CoreConfig = new FileCoreConfig(this);
            User = new FileUser(this);
        }

        /// <inheritdoc/>
        public ICoreConfig CoreConfig { get; }

        /// <inheritdoc/>
        public string Name => nameof(FileCore);

        /// <inheritdoc/>
        public IUser User { get; }

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
        public string InstanceId { get; }

        /// <inheritdoc/>
        public event EventHandler<IDevice>? DeviceAdded;

        /// <inheritdoc/>
        public event EventHandler<IDevice>? DeviceRemoved;

        /// <inheritdoc/>
        public event EventHandler<IPlayableCollectionGroup>? LibraryChanged;

        /// <inheritdoc/>
        public event EventHandler<IPlayableCollectionGroup>? RecentlyPlayedChanged;

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
        public Task<IRecentlyPlayed> GetRecentlyPlayedAsync()
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
            return Task.CompletedTask;
        }
    }
}
