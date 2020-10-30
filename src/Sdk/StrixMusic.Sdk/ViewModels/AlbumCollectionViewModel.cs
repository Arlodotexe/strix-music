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
    /// A ViewModel for <see cref="ICoreAlbumCollection"/>.
    /// </summary>
    public class AlbumCollectionViewModel : ObservableObject, IAlbumCollectionViewModel
    {
        private readonly ICoreAlbumCollection _collection;

        /// <summary>
        /// Creates a new instance of <see cref="AlbumCollectionViewModel"/>.
        /// </summary>
        /// <param name="collection">The <see cref="ICoreAlbumCollection"/> to wrap around.</param>
        public AlbumCollectionViewModel(ICoreAlbumCollection collection)
        {
            _collection = collection;
            Albums = Threading.InvokeOnUI(() => new SynchronizedObservableCollection<ICoreAlbumCollectionItem>());

            PopulateMoreAlbumsCommand = new AsyncRelayCommand<int>(PopulateMoreAlbumsAsync);
        }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreAlbumsCommand { get; }

        /// <inheritdoc />
        public async Task PopulateMoreAlbumsAsync(int limit)
        {
            await foreach (var item in _collection.GetAlbumItemsAsync(limit, Albums.Count))
            {
                switch (item)
                {
                    case ICoreAlbum album:
                        Albums.Add(new AlbumViewModel(album));
                        break;
                    case ICoreAlbumCollection collection:
                        Albums.Add(new AlbumCollectionViewModel(collection));
                        break;
                }
            }
        }

        /// <inheritdoc />
        public SynchronizedObservableCollection<ICoreAlbumCollectionItem> Albums { get; set; }

        /// <inheritdoc />
        public ICore SourceCore => _collection.SourceCore;

        /// <inheritdoc />
        public int TotalAlbumItemsCount => _collection.TotalAlbumItemsCount;

        /// <inheritdoc />
        public Task AddAlbumItemAsync(ICoreAlbumCollectionItem album, int index) => _collection.AddAlbumItemAsync(album, index);

        /// <inheritdoc />
        public Task RemoveAlbumItemAsync(int index) => _collection.RemoveAlbumItemAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddAlbumItemSupported(int index) => _collection.IsAddAlbumItemSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveAlbumItemSupported(int index) => _collection.IsRemoveAlbumItemSupported(index);

        /// <inheritdoc />
        public IAsyncEnumerable<ICoreAlbumCollectionItem> GetAlbumItemsAsync(int limit, int offset) => _collection.GetAlbumItemsAsync(limit, offset);
    }
}