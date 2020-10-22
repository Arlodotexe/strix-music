using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.Collections;
using OwlCore.Helpers;
using StrixMusic.Sdk.Core.Data;

namespace StrixMusic.Sdk.Core.ViewModels
{
    /// <summary>
    /// A wrapper for <see cref="IArtistCollection"/> that contains props and methods for a ViewModel.
    /// </summary>
    public class PlaylistCollectionViewModel : ObservableObject, IPlaylistCollectionViewModel
    {
        private readonly IPlaylistCollection _collection;

        /// <summary>
        /// Creates a new instance of <see cref="PlaylistCollectionViewModel"/>.
        /// </summary>
        /// <param name="collection">The <see cref="IPlaylistCollection"/> to wrap around.</param>
        public PlaylistCollectionViewModel(IPlaylistCollection collection)
        {
            _collection = collection;

            Playlists = Threading.InvokeOnUI(() => new SynchronizedObservableCollection<IPlaylistCollectionItem>());
            PopulateMorePlaylistsCommand = new AsyncRelayCommand<int>(PopulateMorePlaylistsAsync);
        }

        /// <inheritdoc />
        public ICore SourceCore => _collection.SourceCore;

        /// <inheritdoc />
        public int TotalPlaylistItemsCount => _collection.TotalPlaylistItemsCount;

        /// <inheritdoc />
        public Task AddPlaylistItemAsync(IPlaylistCollectionItem playlist, int index) => _collection.AddPlaylistItemAsync(playlist, index);

        /// <inheritdoc />
        public Task RemovePlaylistItemAsync(int index) => _collection.RemovePlaylistItemAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddPlaylistItemSupported(int index) => _collection.IsAddPlaylistItemSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemovePlaylistItemSupported(int index) => _collection.IsRemovePlaylistItemSupported(index);

        /// <inheritdoc />
        public IAsyncEnumerable<IPlaylistCollectionItem> GetPlaylistItemsAsync(int limit, int offset) => _collection.GetPlaylistItemsAsync(limit, offset);

        /// <inheritdoc />
        public SynchronizedObservableCollection<IPlaylistCollectionItem> Playlists { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMorePlaylistsCommand { get; }

        /// <inheritdoc />
        public async Task PopulateMorePlaylistsAsync(int limit)
        {
            await foreach (var item in _collection.GetPlaylistItemsAsync(limit, Playlists.Count))
            {
                switch (item)
                {
                    case IPlaylist playlist:
                        Playlists.Add(new PlaylistViewModel(playlist));
                        break;
                    case IPlaylistCollection collection:
                        Playlists.Add(new PlaylistCollectionViewModel(collection));
                        break;
                }
            }
        }
    }
}