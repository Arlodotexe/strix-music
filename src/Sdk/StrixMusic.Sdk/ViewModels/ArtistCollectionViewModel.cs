using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.Collections;
using OwlCore.Helpers;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions.SdkMember;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// A wrapper for <see cref="ICoreArtistCollection"/> that contains props and methods for a ViewModel.
    /// </summary>
    public class ArtistCollectionViewModel : ObservableObject, IArtistCollectionViewModel
    {
        private readonly IArtistCollection _collection;

        /// <summary>
        /// Creates a new instance of <see cref="ArtistCollectionViewModel"/>.
        /// </summary>
        /// <param name="collection">The <see cref="IArtistCollection"/> to wrap around.</param>
        public ArtistCollectionViewModel(IArtistCollection collection)
        {
            _collection = collection ?? throw new ArgumentNullException(nameof(collection));

            PopulateMoreArtistsCommand = new AsyncRelayCommand<int>(PopulateMoreArtistsAsync);

            SourceCores = collection.GetSourceCores<ICoreArtistCollection>().Select(MainViewModel.GetLoadedCore).ToList();
            Artists = Threading.InvokeOnUI(() => new SynchronizedObservableCollection<IArtistCollectionItem>());
        }

        /// <inheritdoc cref="ISdkMember{T}.SourceCores" />
        public IReadOnlyList<ICore> SourceCores { get; }

        /// <summary>
        /// The sources that were merged to form this member.
        /// </summary>
        public IReadOnlyList<ICoreArtistCollection> Sources => this.GetSources<ICoreArtistCollection>();

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> ISdkMember<ICoreImageCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollection> ISdkMember<ICoreArtistCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollectionItem> ISdkMember<ICoreArtistCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        public int TotalArtistItemsCount => _collection.TotalArtistItemsCount;

        /// <inheritdoc />
        public SynchronizedObservableCollection<IImage> Images => _collection.Images;

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
        public Task<IReadOnlyList<IArtistCollectionItem>> GetArtistItemsAsync(int limit, int offset) =>
            _collection.GetArtistItemsAsync(limit, offset);

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreArtistsCommand { get; }

        /// <inheritdoc />
        public SynchronizedObservableCollection<IArtistCollectionItem> Artists { get; }

        /// <inheritdoc />
        public async Task PopulateMoreArtistsAsync(int limit)
        {
            foreach (var item in await _collection.GetArtistItemsAsync(limit, Artists.Count))
            {
                switch (item)
                {
                    case IArtist artist:
                        Artists.Add(new ArtistViewModel(artist));
                        break;
                    case IArtistCollection collection:
                        Artists.Add(new ArtistCollectionViewModel(collection));
                        break;
                }
            }
        }

        /// <inheritdoc />
        public Task<bool> IsAddImageSupported(int index) => _collection.IsAddImageSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageSupported(int index) => _collection.IsRemoveImageSupported(index);
    }
}