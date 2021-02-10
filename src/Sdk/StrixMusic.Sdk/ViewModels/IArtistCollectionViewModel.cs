using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.Collections;
using StrixMusic.Sdk.Data;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// An interfaced ViewModel for <see cref="IArtistCollection" />. This is needed so because multiple view models implement <see cref="IArtistCollection"/>, and the UI needs to create controls that handle only the ViewModels properties for an <see cref="IArtistCollection"/>.
    /// </summary>
    public interface IArtistCollectionViewModel : IArtistCollection, IPlayableCollectionViewModel, IImageCollectionViewModel
    {
        /// <summary>
        /// The artist items in this collection.
        /// </summary>
        public ObservableCollection<IArtistCollectionItem> Artists { get; }

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