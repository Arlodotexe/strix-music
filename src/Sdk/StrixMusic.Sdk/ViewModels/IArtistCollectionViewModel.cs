using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.Provisos;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.ViewModels.Helpers.Sorting;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// An interfaced ViewModel for <see cref="IArtistCollection" />. This is needed so because multiple view models implement <see cref="IArtistCollection"/>, and the UI needs to create controls that handle only the ViewModels properties for an <see cref="IArtistCollection"/>.
    /// </summary>
    public interface IArtistCollectionViewModel : IArtistCollection, IPlayableCollectionViewModel, IImageCollectionViewModel, IAsyncInit
    {
        /// <summary>
        /// The artist items in this collection.
        /// </summary>
        public ObservableCollection<IArtistCollectionItem> Artists { get; }

        /// <summary>
        /// Keeps the default artists collection while sorting.
        /// </summary>
        public ObservableCollection<IArtistCollectionItem> UnsortedArtists { get; }

        /// <summary>
        /// Sorts the artist collection by <see cref="ArtistSorting"/>.
        /// </summary>
        /// <param name="artistSorting">The <see cref="ArtistSorting"/> according to which the order should be changed.</param>
        public void SortArtistCollection(ArtistSorting artistSorting);

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

        /// <summary>
        /// <inheritdoc cref="IArtistCollectionBase.PlayArtistCollectionAsync"/>
        /// </summary>
        public IAsyncRelayCommand PlayArtistCollectionAsyncCommand { get; }

        /// <summary>
        /// Plays a single artist from this artist collection.
        /// </summary>
        public IAsyncRelayCommand<IArtistCollectionItem> PlayArtistAsyncCommand { get; }

        /// <summary>
        /// <inheritdoc cref="IArtistCollectionBase.PauseArtistCollectionAsync"/>
        /// </summary>
        public IAsyncRelayCommand PauseArtistCollectionAsyncCommand { get; }

        /// <summary>
        /// Sorts artist collection by <see cref="ArtistSorting"/>.
        /// </summary>
        public RelayCommand<ArtistSorting> SortArtistCollectionCommand { get; }
    }
}