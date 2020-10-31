using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.Collections;
using StrixMusic.Sdk.Core.Data;

namespace StrixMusic.Sdk.Core.ViewModels
{
    /// <summary>
    /// An observable <see cref="ICoreArtistCollection"/>.
    /// </summary>
    public interface IArtistCollectionViewModel : ICoreArtistCollection
    {
        /// <summary>
        /// The artist items in this collection.
        /// </summary>
        public SynchronizedObservableCollection<ICoreArtistCollectionItem> Artists { get; }

        /// <summary>
        /// Populates the next set of artists into the collection.
        /// </summary>
        /// <param name="limit">The number of items to load.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task PopulateMoreArtistsAsync(int limit);

        /// <summary>
        /// <inheritdoc cref="PopulateMoreArtistsAsync"/>
        /// </summary>
        public IAsyncRelayCommand<int> PopulateMoreArtistsCommand { get; }
    }
}