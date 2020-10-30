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
    /// A wrapper for <see cref="ICoreArtistCollection"/> that contains props and methods for a ViewModel.
    /// </summary>
    public class ArtistCollectionViewModel : ObservableObject, IArtistCollectionViewModel
    {
        private readonly ICoreArtistCollection _collection;

        /// <summary>
        /// Creates a new instance of <see cref="ArtistCollectionViewModel"/>.
        /// </summary>
        /// <param name="collection">The <see cref="ICoreArtistCollection"/> to wrap around.</param>
        public ArtistCollectionViewModel(ICoreArtistCollection collection)
        {
            _collection = collection;
            PopulateMoreArtistsCommand = new AsyncRelayCommand<int>(PopulateMoreArtistsAsync);

            Artists = Threading.InvokeOnUI(() => new SynchronizedObservableCollection<IArtistCollectionItem>());
        }

        /// <inheritdoc />
        public ICore SourceCore => _collection.SourceCore;

        /// <inheritdoc />
        public int TotalArtistItemsCount => _collection.TotalArtistItemsCount;

        /// <inheritdoc />
        public Task AddArtistItemAsync(IArtistCollectionItem artist, int index) =>
            _collection.AddArtistItemAsync(artist, index);

        /// <inheritdoc />
        public Task RemoveArtistAsync(int index) => _collection.RemoveArtistAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddArtistSupported(int index) => _collection.IsAddArtistSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveArtistSupported(int index) => _collection.IsRemoveArtistSupported(index);

        /// <inheritdoc />
        public IAsyncEnumerable<IArtistCollectionItem> GetArtistsAsync(int limit, int offset) =>
            _collection.GetArtistsAsync(limit, offset);

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreArtistsCommand { get; }

        /// <inheritdoc />
        public SynchronizedObservableCollection<IArtistCollectionItem> Artists { get; }

        /// <inheritdoc />
        public async Task PopulateMoreArtistsAsync(int limit)
        {
            await foreach (var item in _collection.GetArtistsAsync(limit, Artists.Count))
            {
                switch (item)
                {
                    case ICoreArtist artist:
                        Artists.Add(new ArtistViewModel(artist));
                        break;
                    case ICoreArtistCollection collection:
                        Artists.Add(new ArtistCollectionViewModel(collection));
                        break;
                }
            }
        }
    }
}